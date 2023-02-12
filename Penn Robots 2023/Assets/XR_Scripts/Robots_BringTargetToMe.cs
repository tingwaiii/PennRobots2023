using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Robots_BringTargetToMe : MonoBehaviour
{
    public GameObject RobotTarget;

    public GameObject controllerL;
    public GameObject controllerR;
    public GameObject ARCamera;
    public GameObject WebCamera;
    private GameObject myController;

    public void ComeToMe()
    {

        if (controllerL.activeSelf)
        {
            myController = controllerL;
        }
        else if (controllerR.activeSelf)
        {
            myController = controllerR;
        }
        else if (ARCamera.activeSelf)
        {
            myController = ARCamera;
        }
        else if (WebCamera.activeSelf)
        {
            myController = WebCamera;
        }

        Debug.Log("my controller is: " + myController.name);

        RobotTarget.transform.position = myController.transform.position;
        RobotTarget.transform.rotation = myController.transform.rotation;

        RobotTarget.transform.RotateAround(RobotTarget.transform.position, RobotTarget.transform.right, -90);
    }
}
