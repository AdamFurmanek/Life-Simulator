using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera camera;
    private GameObject trackedObject;
    public float zoomSensitivity, moveSensitivity, rotationSensitivity;

    private void Start()
    {
        camera = transform.Find("Camera").gameObject.GetComponent<Camera>();
    }

    private void Update()
    {
        Vector3 newPosition = camera.transform.localPosition + Vector3.down * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSensitivity;
        newPosition.y = Mathf.Clamp(newPosition.y, 2, 200);
        camera.transform.localPosition = newPosition;

        if (trackedObject != null)
        {
            transform.position = trackedObject.transform.position;
        }

        if (Input.GetMouseButton(1))
        {
            trackedObject = null;
            Vector3 desiredMove = new Vector3(-Input.GetAxis("Mouse X"), 0, -Input.GetAxis("Mouse Y")) * Time.deltaTime * moveSensitivity;

            desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
            desiredMove = transform.InverseTransformDirection(desiredMove);

            transform.Translate(desiredMove, Space.Self);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                if(hit.transform.CompareTag("Human"))
                    trackedObject = hit.transform.parent.parent.gameObject;
            }
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 newRotation = transform.localEulerAngles + new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * rotationSensitivity;
            newRotation.x = Mathf.Clamp(newRotation.x, 270, 359);

            transform.localEulerAngles = newRotation;

        }
    }

}
