using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Map.yPosition += 1;
            Debug.Log("W gedrückt!");
        }
        if (Input.GetKey(KeyCode.A))
        {
            Map.xPosition -= 1;
        }
        if (Input.GetKey(KeyCode.S))
        {

            Map.yPosition -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Map.xPosition += 1;
        }
    }
}
