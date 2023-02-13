using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HS_CameraPlatformManager : MonoBehaviour
{
    [Header("Flythrough Stuff")]
    public GameObject FlythroughCameraParent;

    [Header("VR Stuff")]
    public GameObject VRCameraParent;
    public GameObject firstPersonCamera;
    //public GameObject iOSShadowPlane;


    private void Awake()
    {
        if (firstPersonCamera.GetComponent<SteamVR_Camera>().enabled)
        {
            //activate VR cam
            FlythroughCameraParent.SetActive(false);
            VRCameraParent.SetActive(true);
        }
        else
        {
            //activate flythrough cam
            FlythroughCameraParent.SetActive(true);
            VRCameraParent.SetActive(false);
        }
    }
}
