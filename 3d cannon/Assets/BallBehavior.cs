using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    //World
    public GameObject worldManager;
    WorldBehavior worldManagerScript;
    //Forces
    public Vector3 forces = Vector3.zero;
    public Vector3 speed = Vector3.zero;

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
        speed *= 0.99f;
        forces = Vector3.zero;
        //Hitting ground
        if (transform.position.y+speed.y*Time.deltaTime < transform.localScale.y / 2)
        {
            speed -= speed.y * Vector3.up;
            transform.position = new Vector3(transform.position.x, transform.localScale.y / 2, transform.position.z);
            speed *= 1 - Time.deltaTime;
        }
        //Colliding
        foreach(Transform child in gameObject.transform.parent)
        {
            Vector3 deltaVector = child.transform.position - transform.position - speed * Time.deltaTime;
            if (deltaVector.magnitude < 1)
            {
                Vector3 normalForce = new Vector3(deltaVector.normalized.x * speed.x, deltaVector.normalized.y * speed.y, deltaVector.normalized.z * speed.z);
                speed -= normalForce;
                child.GetComponent<BallBehavior>().speed += normalForce;
            }
        }
        //Apply speed
        transform.position += speed * Time.deltaTime;
    }
}