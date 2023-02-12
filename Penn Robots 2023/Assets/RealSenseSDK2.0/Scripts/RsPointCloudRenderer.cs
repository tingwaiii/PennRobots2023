using System;
using UnityEngine;
using Intel.RealSense;
using UnityEngine.Rendering;
using UnityEngine.Assertions;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RsPointCloudRenderer : MonoBehaviour
{
    public RsFrameProvider Source;
    private Mesh meshDynamic;
    private Texture2D uvmap;

    [NonSerialized]
    private Vector3[] vertices;

    FrameQueue q;

    private int myWidth;
    private int myHeight; 

    //JEFF ADDING THIS TO SAVE MESH
    public void SaveMesh()
    {
        GameObject g = new GameObject();
        GameObject p = new GameObject();
        p.transform.position = gameObject.transform.parent.position;
        p.transform.rotation = gameObject.transform.parent.rotation;

        g.transform.parent = p.transform;

        g.transform.localPosition = Vector3.zero;
        g.transform.localRotation = Quaternion.identity;
        g.transform.localScale = new Vector3(1, 1, 1);

        MeshFilter mf = g.AddComponent<MeshFilter>();
        MeshRenderer mr = g.AddComponent<MeshRenderer>();

        mr.material = CopyMaterial(gameObject.GetComponent<MeshRenderer>().material);//here i need to make an instance of the material as well and texture

        //need to duplicate 'mesh' so that it isnt a reference of the currently updating one
        mf.mesh = CopyMesh(meshDynamic); 
    }

    public Material CopyMaterial(Material originalMaterial)
    {
        Material copiedMaterial = new Material(originalMaterial.shader);

        Texture2D textureMain = Instantiate(originalMaterial.GetTexture("_MainTex")) as Texture2D; //clone the material hitObject.renderer.material.mainTexture = texture; //set the material equal to the clone
        Texture2D textureUV = Instantiate(originalMaterial.GetTexture("_UVMap")) as Texture2D; //clone the material hitObject.renderer.material.mainTexture = texture; //set the material equal to the clone

        copiedMaterial.SetTexture("_MainTex", textureMain);
        copiedMaterial.SetTexture("_UVMap", textureUV);
        copiedMaterial.SetFloat("_PointSize", originalMaterial.GetFloat("_PointSize"));
        copiedMaterial.SetColor("_Color", originalMaterial.GetColor("_Color"));
        copiedMaterial.SetFloat("_UseDistance", originalMaterial.GetFloat("_UseDistance"));

        return copiedMaterial;
    }
    public Mesh CopyMesh(Mesh originalMesh)
    {
        Mesh copiedMesh = new Mesh()
        {
            indexFormat = IndexFormat.UInt32,
        };

        Vector3[] myVerts = originalMesh.vertices;

        var indices = new int[myVerts.Length];
        for (int i = 0; i < myVerts.Length; i++)
        {
            indices[i] = i;
        }

        copiedMesh.vertices = myVerts;
        copiedMesh.SetIndices(indices, MeshTopology.Points, 0, false);
        copiedMesh.bounds = originalMesh.bounds;
        copiedMesh.uv = originalMesh.uv;
        copiedMesh.normals = originalMesh.normals;
        copiedMesh.colors = originalMesh.colors;
        copiedMesh.tangents = originalMesh.tangents;

        return copiedMesh;
    }
    //JEFF ADDING THIS

    void Start()
    {
        Source.OnStart += OnStartStreaming;
        Source.OnStop += Dispose;
    }

    private void OnStartStreaming(PipelineProfile obj)
    {
        q = new FrameQueue(1);

        using (var depth = obj.Streams.FirstOrDefault(s => s.Stream == Stream.Depth && s.Format == Format.Z16).As<VideoStreamProfile>())
            ResetMesh(depth.Width, depth.Height);

        Source.OnNewSample += OnNewSample;
    }

    private void ResetMesh(int width, int height)
    {
        myWidth = width;
        myHeight = height;


        Assert.IsTrue(SystemInfo.SupportsTextureFormat(TextureFormat.RGFloat));
        uvmap = new Texture2D(width, height, TextureFormat.RGFloat, false, true)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point,
        };
        GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_UVMap", uvmap);

        if (meshDynamic != null)
            meshDynamic.Clear();
        else
            meshDynamic = new Mesh()
            {
                indexFormat = IndexFormat.UInt32,
            };

        vertices = new Vector3[width * height];

        var indices = new int[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
            indices[i] = i;

        meshDynamic.MarkDynamic();
        meshDynamic.vertices = vertices;

        var uvs = new Vector2[width * height];
        Array.Clear(uvs, 0, uvs.Length);
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                uvs[i + j * width].x = i / (float)width;
                uvs[i + j * width].y = j / (float)height;
            }
        }

        meshDynamic.uv = uvs;

        meshDynamic.SetIndices(indices, MeshTopology.Points, 0, false);
        meshDynamic.bounds = new Bounds(Vector3.zero, Vector3.one * 10f);

        GetComponent<MeshFilter>().sharedMesh = meshDynamic;
    }


    void OnDestroy()
    {
        if (q != null)
        {
            q.Dispose();
            q = null;
        }

        if (meshDynamic != null)
            Destroy(null);
    }

    private void Dispose()
    {
        Source.OnNewSample -= OnNewSample;

        if (q != null)
        {
            q.Dispose();
            q = null;
        }
    }

    private void OnNewSample(Frame frame)
    {
        if (q == null)
            return;
        try
        {
            if (frame.IsComposite)
            {
                using (var fs = frame.As<FrameSet>())
                using (var points = fs.FirstOrDefault<Points>(Stream.Depth, Format.Xyz32f))
                {
                    if (points != null)
                    {
                        q.Enqueue(points);
                    }
                }
                return;
            }

            if (frame.Is(Extension.Points))
            {
                q.Enqueue(frame);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }


    protected void LateUpdate()
    {
        if (q != null)
        {
            Points points;
            if (q.PollForFrame<Points>(out points))
                using (points)
                {
                    if (points.Count != meshDynamic.vertexCount)
                    {
                        using (var p = points.GetProfile<VideoStreamProfile>())
                            ResetMesh(p.Width, p.Height);
                    }

                    if (points.TextureData != IntPtr.Zero)
                    {
                        uvmap.LoadRawTextureData(points.TextureData, points.Count * sizeof(float) * 2);
                        uvmap.Apply();
                    }

                    if (points.VertexData != IntPtr.Zero)
                    {
                        points.CopyVertices(vertices);

                        meshDynamic.vertices = vertices;
                        meshDynamic.UploadMeshData(false);
                    }
                }
        }
    }
}
