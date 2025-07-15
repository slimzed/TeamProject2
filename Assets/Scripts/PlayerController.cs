using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            Debug.Log("high attack");
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("mid attack");
        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("low attack");
        }
    }
}
