

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Robots_VR_ControllerButtons : MonoBehaviour
{
    #region PUBLIC VARIABLES
    public float laserLength = 3.0f;

    public GameObject sceneManager;


    #endregion

    #region PRIVATE VARIABLES

    #endregion

    #region AWAKE AND START FUNCTIONS
    private void Awake()
    {

     
    }

    void Start()
    {
        SteamVR_TrackedController trackedController = GetComponent<SteamVR_TrackedController>();
        trackedController.TriggerClicked += new ClickedEventHandler(DoClick);
        trackedController.TriggerUnclicked += new ClickedEventHandler(UnClick);
        trackedController.PadClicked += new ClickedEventHandler(PadClicked);
        trackedController.MenuButtonClicked += new ClickedEventHandler(MenuClick);
        trackedController.Gripped += new ClickedEventHandler(Gripped);

    }
    #endregion

    private void Update()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        RaycastHit hit;
        Ray ray = new Ray(origin, direction);
        if (Physics.Raycast(ray, out hit))
        {
            string myTag = hit.transform.tag;
            GameObject touchObject = hit.transform.gameObject;

            if (Vector3.Distance(origin, hit.point) < laserLength)
            {
                
            }
            else
            {
                RayMissed();
            }
        }
        else
        {//this means the raycast missed
            RayMissed();
        }
    }
    void RayMissed()
    {

    }
    void DoClick(object sender, ClickedEventArgs e)
    {
        Debug.Log("hey i clicked the trigger button");
        sceneManager.GetComponent<InClassScript>().MySpecialButton();
        //sceneManager.GetComponent<Robots_SendInstructionToMaster>().SendToMaster();
        //this.gameObject.GetComponent<Robots_DrawLine>().LineStart();

    }
    void UnClick(object sender, ClickedEventArgs e)
    {
        Debug.Log("hey i UN clicked the trigger button");

        //this.gameObject.GetComponent<Robots_DrawLine>().LineStop();


    }
    void PadClicked(object sender, ClickedEventArgs e)
    {


        Debug.Log("hey i clicked the pad button");


        float PadLimitHigh = 0.7f;
        float PadLimitLow = 0.3f;
        if (e.padX < -(PadLimitHigh) && e.padY < PadLimitLow)
        { //Left

        }
        else if (e.padX > PadLimitHigh && e.padY < PadLimitLow)
        { //Right


        }








        //
        //here i need to have the world space target go to the next point in my list
        //


        //this.gameObject.GetComponent<Robots_BringTargetToMe>().ComeToMe();


    }

    void Gripped(object sender, ClickedEventArgs e)
    {
        //sceneManager.GetComponent<Robots_BringTargetToMe>().SendAllPointsToMachina();


        Debug.Log("hey i clicked the grip button");

    }

    void MenuClick(object sender, ClickedEventArgs e)
    {
        Debug.Log("hey i clicked the menu button");

    }
}