using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;



public class Editor_GH_JSON_Import : EditorWindow
{
    [MenuItem("UPenn/GH_JSON_Import")]
    static void Apply()
    {
        
        EditorUtility.DisplayDialog("Import Toolpath", "Please select a JSON file exported from a Grasshopper toolpath", "OK");

        string path = EditorUtility.OpenFilePanel("GH_JSON_Import", "", "JSON");


        if (path.Length != 0)
        {
            string fileContent = File.ReadAllText(path);
            GH_JSON_Planes planesInJson = JsonUtility.FromJson<GH_JSON_Planes>(fileContent);


            string grandParentName = "ROBOTTARGETS_PARENT";
            GameObject gP = GameObject.Find(grandParentName);
            if (gP == null)
            {//this creates a new grandparent for all import geometry if there isn't one previously
                gP = new GameObject(grandParentName);
                gP.transform.position = Vector3.zero;
                gP.transform.eulerAngles = new Vector3(-90,0,0);
                gP.transform.localScale = new Vector3(1, 1, 1);
            }


            foreach (GH_JSON_Plane planeInJSON in planesInJson.MyDataTable)
            {
                Debug.Log("Found plane: " + planeInJSON.Plane);

                string cleanString = planeInJSON.Plane.Replace("O(","");
                cleanString = cleanString.Replace(") ", "");
                cleanString = cleanString.Replace("(", "");
                cleanString = cleanString.Replace(")", "");
                cleanString = cleanString.Replace("Z", ",");

                Debug.Log("Clean string: " + cleanString);

                string[] splitString = cleanString.Split(',');

                Vector3 position = new Vector3(float.Parse(splitString[0]), float.Parse(splitString[1]), float.Parse(splitString[2]));
                Vector3 rotation = new Vector3(float.Parse(splitString[3]), float.Parse(splitString[4]), float.Parse(splitString[5]));

                Debug.Log("Parsed Position: " + position);
                Debug.Log("Parsed Rotation: " + rotation);



                GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/XR_Prefabs/Prefabs/AxisPrefab.prefab", typeof(GameObject)) as GameObject;

                GameObject gO = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

                //GameObject gO = new GameObject();
                gO.transform.parent = gP.transform;
                gO.transform.localPosition = position/1000.0f;//these are in MM, need to convert to M
                gO.transform.right = rotation;
                gO.transform.RotateAround(gO.transform.position,gO.transform.forward,180.0f);
            }
        }
    }
}