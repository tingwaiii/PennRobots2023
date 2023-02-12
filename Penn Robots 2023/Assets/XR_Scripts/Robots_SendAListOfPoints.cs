using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robots_SendAListOfPoints : MonoBehaviour
{
    public GameObject listOfPointsParent;
    public GameObject robotTarget_WorldSpace;
    public RobotController_Kinematics controllerRobot;

    public void SendListOfPoints()
    {
        for (int i = 0;i<listOfPointsParent.transform.childCount;i++)
        {
            robotTarget_WorldSpace.transform.position = listOfPointsParent.transform.GetChild(i).position;
            robotTarget_WorldSpace.transform.rotation = listOfPointsParent.transform.GetChild(i).rotation;

            controllerRobot.RobotTarget_RobotSpace.position = robotTarget_WorldSpace.transform.position;//set position in world to match the robot target
                                                                                              //RobotTarget_RobotSpace.localPosition = RobotTarget_RobotSpace.localPosition + new Vector3(TCPDifferencePosition.x, TCPDifferencePosition.y, TCPDifferencePosition.z);

            controllerRobot.RobotTarget_RobotSpace.rotation = robotTarget_WorldSpace.transform.rotation;//set position in world to match the robot target

            //send to machina...
            GetComponent<Robots_SendInstructionToMaster>().SendToMaster();
        }

    }
}
