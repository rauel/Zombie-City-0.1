using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class water_animation : MonoBehaviour {
    
    private Animator anim;
    int offset;
    

	void Start ()
    {
        int randomValue = (int)((transform.position.x * transform.position.y) * (transform.position.x * transform.position.y)) % 307;

        System.Random random = new System.Random((int)(transform.position.x * Map.SEED + transform.position.y * Map.SEED));
        offset = random.Next(0, randomValue);

        anim = GetComponent<Animator>();
        anim.speed = 0;
    }
	

	void Update ()
    {
        if (offset < 100)
        {
            offset++;
        }
        else if (offset == 100)
        {
            anim.speed = 1;

            offset = 999;
        }
	}
}
