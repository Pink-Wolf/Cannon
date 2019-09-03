using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    //World
    public GameObject worldManager;
    WorldBehavior worldManagerScript;
    [HideInInspector]
    public GameObject tankBase;
    [HideInInspector]
    public GameObject tankObj;
    [HideInInspector]
    public GameObject tankHead;
    //Forces
    public Vector3 forces = Vector3.zero;
    public Vector3 speed = Vector3.zero;
    //Object
    public float mass = Mathf.PI * 4 / 3;

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
            forces += vector3 * Time.deltaTime * mass;
        }
        //Apply forces
        speed += forces / mass;
        speed *= 0.99f;
        forces = Vector3.zero;
        //Hitting ground
        if (transform.position.y+speed.y*Time.deltaTime < transform.localScale.y / 2)
        {
            speed -= Mathf.Min(speed.y,0) * Vector3.up;
            transform.position = new Vector3(transform.position.x, transform.localScale.y / 2, transform.position.z);
            speed *= 1 - Time.deltaTime;
        }
        //Colliding
        foreach(Transform child in gameObject.transform.parent)
        {
            if (transform != child)
            {
                float radiusSum = (transform.localScale.x + child.localScale.x) / 2;
                Vector3 deltaVector = child.transform.position - transform.position - speed * Time.deltaTime;
                if (deltaVector.magnitude < radiusSum)
                {
                    speed -= deltaVector.normalized * speed.magnitude / mass;
                    child.GetComponent<BallBehavior>().forces += deltaVector.normalized * speed.magnitude * mass;
                }
            }
        }
        //Colliding with tank head
        Vector3 deltaPos = transform.position - tankHead.transform.position;
        if (deltaPos.magnitude < (tankHead.transform.localScale.x + transform.localScale.x)/2)
        {
            speed += deltaPos.normalized * speed.magnitude;
            speed += tankObj.GetComponent<CannonBehavior>().speed;
        }
        //Colliding with tank base
        Vector3 maxPoint = tankBase.transform.position + tankBase.transform.localScale / 2;
        Vector3 minPoint = tankBase.transform.position - tankBase.transform.localScale / 2;
        Vector3 closestPoint = new Vector3(
            Mathf.Min(maxPoint.x, Mathf.Max(minPoint.x, transform.position.x)),
            Mathf.Min(maxPoint.y, Mathf.Max(minPoint.y, transform.position.y)),
            Mathf.Min(maxPoint.z, Mathf.Max(minPoint.z, transform.position.z))
            );
        deltaPos = transform.position - closestPoint;
        if (deltaPos.magnitude <= transform.localScale.x / 2)
        {
            speed += deltaPos.normalized * speed.magnitude;
            if (Mathf.Abs(deltaPos.y) < transform.localScale.y * 0.45f) speed += tankObj.GetComponent<CannonBehavior>().speed;
        }
        //Apply speed
        transform.position += speed * Time.deltaTime;
    }
}