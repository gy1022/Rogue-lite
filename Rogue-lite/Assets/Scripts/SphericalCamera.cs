using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 10.0f;
    public float mouseSensitivity = 100.0f;
    public float scrollSensitivity = 2.0f;
    public float minYAngle = 5.0f;
    public float maxYAngle = 85.0f;

    private float currentHorizontalAngle = 0.0f;
    private float currentVerticalAngle = 4.5f;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            HandleInput();
            UpdateCameraPosition();
        }
        
    }

    private void HandleInput()
    {
        currentHorizontalAngle -= Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        currentVerticalAngle += Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minYAngle, maxYAngle);

        distance += -Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
        distance = Mathf.Clamp(distance, 2.0f, 100.0f);
    }

    private void UpdateCameraPosition()
    {
        float verticalAngleRadians = currentVerticalAngle * Mathf.Deg2Rad;
        float horizontalAngleRadians = currentHorizontalAngle * Mathf.Deg2Rad;

        float x = distance * Mathf.Sin(verticalAngleRadians) * Mathf.Cos(horizontalAngleRadians);
        float z = distance * Mathf.Sin(verticalAngleRadians) * Mathf.Sin(horizontalAngleRadians);
        float y = distance * Mathf.Cos(verticalAngleRadians);

        Vector3 newPosition = new Vector3(x, y, z);
        newPosition = target.position + newPosition;

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 6);
        transform.LookAt(target);

    }

}
