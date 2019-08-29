﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    //World
    public GameObject worldManager;
    WorldBehavior worldManagerScript;
    //Forces
    Vector3 forces = Vector3.zero;
    Vector3 speed = Vector3.zero;

    private void Start()
    {
        //Setup variables
        worldManagerScript = worldManager.GetComponent<WorldBehavior>();
    }

    private void Update()
    {
        //Apply world forces
        foreach (Vector3 vector3 in worldManagerScript.worldForces)
        {
            forces += vector3 * Time.deltaTime;
        }
        //Apply forces
        speed += forces;
        forces = Vector3.zero;
        //Not below ground
        if (transform.position.y+speed.y*Time.deltaTime < transform.localScale.y / 2)
        {
            speed -= speed.y * Vector3.up;
            transform.position = new Vector3(transform.position.x, transform.localScale.y / 2, transform.position.z);
        }
        //Apply speed
        transform.position += speed * Time.deltaTime;
    }
}