using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InClassScript : MonoBehaviour
{
    public GameObject robotWorldTarget;
    public float howFarToGo;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("!!!!!! Hello, i started my in class script");
    }

    public void MySpecialButton()
    {
        Debug.Log("!!!!!! Hello, i clicked the special function");

        Vector3 currPos = robotWorldTarget.transform.position;

        currPos = new Vector3(currPos.x + howFarToGo, currPos.y, currPos.z);

        robotWorldTarget.transform.position = currPos;
    }
}
