using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCamera : MonoBehaviour
{
    public float speed = 10.0f;
    public float speedModifier = 1.5f;
    public float sensitivity = 100.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Vector3 startPosition;
    private Vector3 startRotation;
    private bool firstClick = true;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            transform.position = startPosition;
            transform.eulerAngles = startRotation;
        }

        // Little hack to prevent the camera from jittering
        if (Input.GetMouseButtonDown(1))
        {
            firstClick = true;
        }

        if (Input.GetMouseButton(1))
        {
            if (!firstClick)
            {
                yaw += sensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
                pitch -= sensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;
                pitch = Mathf.Clamp(pitch, -90f, 90f); // Prevent the camera from flipping over
                transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
            }
            firstClick = false;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        float speed = this.speed;
        bool first = false; ;
        if (Input.GetAxis("Speed Loss") > 0)
        {
            speed /= speedModifier;
            first = true;
        }

        if (Input.GetAxis("Speed Mod") > 0)
        {
            if (first)
            {
                speed = this.speed / (3 * speedModifier);
            }
            else
            {
                speed *= speedModifier;
            }
        }


        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical) * speed * Time.deltaTime;
        transform.Translate(movement);
    }
}
