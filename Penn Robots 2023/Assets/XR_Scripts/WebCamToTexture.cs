using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCamToTexture : MonoBehaviour
{
    private WebCamTexture webCamTexture;
    public GameObject webcamTextureObject;

    // Start is called before the first frame update
    void Start()
    {
        GetCamera();



    }

    public void GetCamera()
    {
        WebCamDevice[] cam_devices = WebCamTexture.devices;
        // for debugging purposes, prints available devices to the console
        for (int i = 0; i < cam_devices.Length; i++)
        {
            print("Webcam available: " + cam_devices[i].name);
        }

        webCamTexture = new WebCamTexture(cam_devices[0].name, 480, 640, 30);



        Renderer renderer = webcamTextureObject.GetComponent<Renderer>();
        renderer.material.mainTexture = webCamTexture;
        webCamTexture.Play();


    }
}