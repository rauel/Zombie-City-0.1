using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = new Vector2(Map.xPosition, Map.yPosition + 5 );
    }
}
