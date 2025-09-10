using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewBehaviourScript : MonoBehaviour
{
    public float mouseSensitivity = 4f;
    float cameraVerticalRotation = 0f;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
    // hide cursor and put in centre
    Cursor.lockState = CursorLockMode.Locked;
    startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // TEMPORARY: WASD limited movement within 1m box
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) move += transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= transform.forward;
        if (Input.GetKey(KeyCode.A)) move -= transform.right;
        if (Input.GetKey(KeyCode.D)) move += transform.right;
        if (Input.GetKey(KeyCode.Q)) move += Vector3.up;
        if (Input.GetKey(KeyCode.E)) move -= Vector3.up;
        if (move != Vector3.zero)
        {
            Vector3 intendedPos = transform.position + move.normalized * 0.1f; // move speed
            Vector3 offset = intendedPos - startPosition;
            // Clamp X, Y, Z axes to [-1, 1] from start position
            float clampedX = Mathf.Clamp(offset.x, -1f, 1f);
            float clampedY = Mathf.Clamp(offset.y, -1f, 1f);
            float clampedZ = Mathf.Clamp(offset.z, -1f, 1f);
            intendedPos = new Vector3(startPosition.x + clampedX, startPosition.y + clampedY, startPosition.z + clampedZ);
            transform.position = intendedPos;
        }

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) // TEMPORARY: only rotate when left mouse button is held
        {
            float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // vertical rotation (pitch)
            cameraVerticalRotation -= inputY;
            cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);

            // horizontal rotation (yaw)
            float cameraHorizontalRotation = transform.localEulerAngles.y + inputX;

            transform.localEulerAngles = new Vector3(cameraVerticalRotation, cameraHorizontalRotation, 0f);
        }
    }
}
