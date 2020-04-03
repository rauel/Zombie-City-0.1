﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grass_animation : MonoBehaviour
{
    bool windAnimationIsOn;
    int windIndex;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        System.Random random = new System.Random((int)(transform.position.x * 3 + transform.position.y * 5));

        windIndex = random.Next(0, 9);

    }

    void Update()
    {
        if (GraphicsController.windAnimationIsOn[windIndex]
            && anim.speed != 1)
        {
            anim.speed = 1;
        }
        else if (!GraphicsController.windAnimationIsOn[windIndex]
            && anim.speed != 0)
        {
            anim.speed = 0;
        }
    }
}
