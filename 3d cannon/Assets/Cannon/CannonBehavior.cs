using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBehavior : MonoBehaviour {
    public float mouseSensitivity = 0.1f;
    public float rotationSpeed = 0.5f;
    public float moveSpeed = 5;
    public GameObject ballObject;
    public float shootForce = 10;
    public GameObject ballManager;
    public GameObject tankObject;
    public GameObject cameraObject;
    public GameObject cannonObject;
    public GameObject cannonPlate;
    public GameObject tankHead;
    public GameObject terrain; WorldBehavior terrainScript;
    public float cannonBallSize = 1;
    public Vector3 speed;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        terrainScript = terrain.GetComponent<WorldBehavior>();

        transform.position = new Vector3(terrainScript.worldSize.x * 0.5f, 0, terrainScript.worldSize.y * 0.5f);
    }
	void Update () {
        //Transform size of ball
        cannonBallSize += Input.mouseScrollDelta.y / 10;
        //Rotate cannon
        float ry = Input.GetAxis("Horizontal") * mouseSensitivity;
        float rx = Input.GetAxis("Vertical") * mouseSensitivity;

        if (ry + cannonObject.transform.eulerAngles.x > 355) ry = 355 - cannonObject.transform.eulerAngles.x;
        if (ry + cannonObject.transform.eulerAngles.x < 280) ry = 280 - cannonObject.transform.eulerAngles.x;

        cannonObject.transform.Rotate(ry * Vector3.right);
        cannonObject.transform.Rotate(rx * Vector3.up, Space.World);
        cameraObject.transform.Rotate((cannonObject.transform.eulerAngles.y - cameraObject.transform.eulerAngles.y) * Vector3.up, Space.World);

        cannonObject.transform.Rotate(Vector3.forward * -cannonObject.transform.eulerAngles.z);


        tankHead.transform.Rotate(rx * Vector3.up, Space.World);
        //Move tank
        speed = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            speed += tankHead.transform.forward * moveSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            speed -= tankHead.transform.forward * moveSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            speed += tankHead.transform.right * moveSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            speed -= tankHead.transform.right * moveSpeed;
        }
        tankObject.transform.position += speed * Time.deltaTime;

        tankObject.transform.position = new Vector3(
            Mathf.Max(cannonPlate.transform.localScale.x * 0.5f, Mathf.Min(terrainScript.worldSize.x - cannonPlate.transform.localScale.x * 0.5f - 1, tankObject.transform.position.x)),
            tankObject.transform.position.y,
            Mathf.Max(cannonPlate.transform.localScale.z * 0.5f, Mathf.Min(terrainScript.worldSize.y - cannonPlate.transform.localScale.z * 0.5f - 1, tankObject.transform.position.z)));
        //Get height
        float xz = terrainScript.groundHeight[Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z)];
        float Xz = terrainScript.groundHeight[Mathf.CeilToInt(transform.position.x), Mathf.FloorToInt(transform.position.z)];
        float xZ = terrainScript.groundHeight[Mathf.FloorToInt(transform.position.x), Mathf.CeilToInt(transform.position.z)];
        float XZ = terrainScript.groundHeight[Mathf.CeilToInt(transform.position.x), Mathf.CeilToInt(transform.position.z)];
        float x = transform.position.x % 1;
        float z = transform.position.z % 1;

        tankObject.transform.position = new Vector3(tankObject.transform.position.x,
            Mathf.Lerp(
                Mathf.Lerp(
                    xz,
                    Xz,
                    x),
                Mathf.Lerp(
                    xZ,
                    XZ,
                    x),
                z)
            , tankObject.transform.position.z);
        //Shoot ball
        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = Instantiate(ballObject);
            obj.transform.position = cannonObject.transform.position + cannonObject.transform.up * transform.localScale.y * 2;
            obj.transform.localScale = Vector3.one * cannonBallSize;
            obj.transform.parent = ballManager.transform;

            BallBehavior objBehavior = obj.GetComponent<BallBehavior>();
            objBehavior.mass *= 8 * cannonBallSize * cannonBallSize * cannonBallSize;
            objBehavior.speed += speed;
            objBehavior.forces += shootForce * cannonObject.transform.up;
            objBehavior.tankBase = cannonPlate;
            objBehavior.tankObj = tankObject;
            objBehavior.tankHead = tankHead;
            objBehavior.worldManager = terrain;
            objBehavior.OnCreate();
        }
    }
}
