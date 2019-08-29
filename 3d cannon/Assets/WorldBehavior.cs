using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBehavior : MonoBehaviour
{
    public Vector3[] worldForces = new Vector3[] { new Vector3(0, -10, 0) };
    public float spawnTimer = 1;
    float nextSpawnTime = 0;
    public GameObject ballObject;

    public void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            nextSpawnTime = Time.time + spawnTimer;
            GameObject obj = Instantiate(ballObject);
            obj.transform.position = new Vector3(Random.Range(-5, 5), 5, Random.Range(-5, 5));
        }
    }
}