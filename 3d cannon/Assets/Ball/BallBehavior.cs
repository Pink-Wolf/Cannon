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
    [HideInInspector]
    public Vector3 forces = Vector3.zero;
    [HideInInspector]
    public Vector3 speed = Vector3.zero;
    [HideInInspector]
    public Vector2 rotation = Vector2.zero;
    //Object
    public float mass = Mathf.PI * 4 / 3;

    public void OnCreate()
    {
        //Setup variables
        worldManagerScript = worldManager.GetComponent<WorldBehavior>();
        //Create texture
        Texture2D texture = new Texture2D(8, 8);
        Color[] color = new Color[texture.width * texture.height];
        for (int i = 0; i < color.Length; i++)
        {
            color[i] = Color.Lerp(Color.black, Color.white, Random.Range(0f, 0.5f));
        }
        texture.SetPixels(color);
        texture.Apply();
        GetComponent<MeshRenderer>().material.mainTexture = texture;
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
        //Not under ground
        float xz, Xz, xZ, XZ, x, z, y;
        if ((transform.position.x >= 0) && (transform.position.x <= worldManagerScript.worldSize.x - 1) &&
            (transform.position.z >= 0) && (transform.position.z <= worldManagerScript.worldSize.y - 1))
        {
            xz = worldManagerScript.groundHeight[Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z)];
            Xz = worldManagerScript.groundHeight[Mathf.CeilToInt(transform.position.x), Mathf.FloorToInt(transform.position.z)];
            xZ = worldManagerScript.groundHeight[Mathf.FloorToInt(transform.position.x), Mathf.CeilToInt(transform.position.z)];
            XZ = worldManagerScript.groundHeight[Mathf.CeilToInt(transform.position.x), Mathf.CeilToInt(transform.position.z)];
            x = transform.position.x % 1;
            z = transform.position.z % 1;
            y =
                Mathf.Lerp(
                    Mathf.Lerp(
                        xz,
                        Xz,
                        x),
                    Mathf.Lerp(
                        xZ,
                        XZ,
                        x),
                    z);
            transform.position = new Vector3(transform.position.x, Mathf.Max(y, transform.position.y), transform.position.z);
        }
        //Hitting ground
        Vector3 sumNormal = Vector3.zero;
        Vector3 newPos = transform.position + speed * Time.deltaTime;
        for (int ix = Mathf.FloorToInt(newPos.x-transform.localScale.x*0.5f); ix < Mathf.CeilToInt(transform.position.x + transform.localScale.x * 0.5f); ix++)
        {
            for (int iz = Mathf.FloorToInt(newPos.z - transform.localScale.z * 0.5f); iz < Mathf.CeilToInt(transform.position.z + transform.localScale.z * 0.5f); iz++)
            {
                if ((ix > -1) && (ix < worldManagerScript.worldSize.x - 1) && (iz > -1) && (iz < worldManagerScript.worldSize.y - 1))
                {
                    Vector3 closestTerrainPoint = new Vector3(
                        Mathf.Max(ix, Mathf.Min(ix + 1, newPos.x)),
                        0,
                        Mathf.Max(iz, Mathf.Min(iz + 1, newPos.z)));


                    xz = worldManagerScript.groundHeight[ix, iz];
                    Xz = worldManagerScript.groundHeight[ix + 1, iz];
                    xZ = worldManagerScript.groundHeight[ix, iz + 1];
                    XZ = worldManagerScript.groundHeight[ix + 1, iz + 1];
                    x = closestTerrainPoint.x - ix;
                    z = closestTerrainPoint.z - iz;
                    closestTerrainPoint.y =
                        Mathf.Lerp(
                            Mathf.Lerp(
                                xz,
                                Xz,
                                x),
                            Mathf.Lerp(
                                xZ,
                                XZ,
                                x),
                            z);

                    Vector3 deltaVector = newPos - closestTerrainPoint;
                    if (deltaVector.magnitude <= transform.localScale.x / 2)
                    {
                        //send normal vector force against
                        Vector3 normalxz = new Vector3(
                            xz-Xz,
                            1,
                            2 * xz - xZ - Xz
                            ).normalized;
                        Vector3 normalXz = new Vector3(
                            (xz-Xz),
                            1,
                            2 * Xz - XZ - xz
                            ).normalized;
                        Vector3 normalxZ = new Vector3(
                            xZ-XZ,
                            1,
                            (XZ+xz-2*xZ)
                            ).normalized;
                        Vector3 normalXZ = new Vector3(
                            (xZ-XZ),
                            1,
                            (xZ+Xz-2*XZ)
                            ).normalized;

                        Vector3 normal = new Vector3(
                            Mathf.Lerp(
                                Mathf.Lerp(
                                    normalxz.x,
                                    normalXz.x,
                                    x),
                                Mathf.Lerp(
                                    normalxZ.x,
                                    normalXZ.x,
                                    x),
                                z),
                            Mathf.Lerp(
                                Mathf.Lerp(
                                    normalxz.y,
                                    normalXz.y,
                                    x),
                                Mathf.Lerp(
                                    normalxZ.y,
                                    normalXZ.y,
                                    x),
                                z),
                            Mathf.Lerp(
                                Mathf.Lerp(
                                    normalxz.z,
                                    normalXz.z,
                                    x),
                                Mathf.Lerp(
                                    normalxZ.z,
                                    normalXZ.z,
                                    x),
                                z)
                            ).normalized;
                        sumNormal += normal;
                    }
                }
            }
        }
        speed += sumNormal.normalized * speed.magnitude;
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
        //Apply speed and rotation
        transform.position += speed * Time.deltaTime;
        transform.Rotate(new Vector3(speed.z, 0, -speed.x) * 90 * Time.deltaTime, Space.World);
        //Check if so low, we can just delete it
        if (transform.position.y < -16) Destroy(gameObject);
    }
}