using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBehavior : MonoBehaviour {
    public float mouseSensitivity = 0.1f;
    public float moveSpeed = 5;
    public GameObject ballObject;
    public float shootForce = 10;
    public GameObject ballManager;
    public GameObject cameraRoot;
    [Range(0,1)]
    public float cannonBallSize = 1;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
	void Update () {
        //Transform size of ball
        cannonBallSize += Input.mouseScrollDelta.y / 10;
        //Rotate
        transform.Rotate(new Vector3(Input.GetAxis("Horizontal") * mouseSensitivity, Input.GetAxis("Vertical") * mouseSensitivity / 2, 0));

        if (transform.eulerAngles.x < 280) transform.Rotate(Vector3.right * (280 - transform.eulerAngles.x));
        if (transform.eulerAngles.x > 345) transform.Rotate(Vector3.right * (345 - transform.eulerAngles.x));

        transform.Rotate(Vector3.forward * -transform.eulerAngles.z);

        cameraRoot.transform.Rotate(Vector3.up * (transform.eulerAngles.y - cameraRoot.transform.eulerAngles.y));
        //Shoot ball
        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = Instantiate(ballObject);
            obj.transform.position = transform.position + transform.up * transform.localScale.y * 2;
            obj.transform.localScale = Vector3.one * cannonBallSize;
            obj.GetComponent<BallBehavior>().forces += shootForce * transform.up;
            obj.transform.parent = ballManager.transform;
        }
    }
}
