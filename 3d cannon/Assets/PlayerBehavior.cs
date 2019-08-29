using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour {
    public float mouseSensitivity = 0.1f;
    public float moveSpeed = 5;
    public GameObject ballObject;
    public float shootForce = 10;
    

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
	void Update () {
        //Rotate view
        transform.Rotate(new Vector3(Input.GetAxis("Horizontal") * -mouseSensitivity, Input.GetAxis("Vertical") * mouseSensitivity, 0));
        if ((transform.eulerAngles.x > 60) && (transform.eulerAngles.x < 180))
        {
            transform.Rotate(Vector3.right * (60 - Mathf.Abs(transform.eulerAngles.x)));
        }
        if ((transform.eulerAngles.x < 300) && (transform.eulerAngles.x > 180))
        {
            transform.Rotate(Vector3.right * (300 - Mathf.Abs(transform.eulerAngles.x)));
        }
        transform.Rotate(Vector3.forward * -transform.eulerAngles.z);
        //Move around
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        }
        //Shoot ball
        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = Instantiate(ballObject);
            obj.transform.position = transform.position;
            obj.GetComponent<BallBehavior>().forces += shootForce * transform.forward;
        }
    }
}
