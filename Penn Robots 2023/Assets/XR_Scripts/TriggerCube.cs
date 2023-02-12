using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCube : MonoBehaviour
{
    public GameObject robotTarget;
    public GameObject sceneManager;

    private void OnTriggerEnter(Collider other)
    {
        //this.gameobject<getcomponent>().collider.ontriggerenter

        Debug.Log("something hit met! it was called: " + other.name);

        robotTarget.transform.position = other.transform.position;
        sceneManager.GetComponent<Robots_SendInstructionToMaster>().SendToMaster();
        //move robot target to me
        //call send to machina
    }


}
