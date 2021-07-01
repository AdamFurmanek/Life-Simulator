using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraNavigation : MonoBehaviour
{
    private GameObject mainCamera, previewCamera;
    private Camera camera;
    private GameObject trackedObjectMain, trackedObjectPreview;
    [SerializeField] private float zoomSensitivity, moveSensitivity, rotationSensitivity;
    private TrackingInfo trackingInfo;

    void Awake()
    {
        mainCamera = transform.Find("MainCamera").gameObject;
        previewCamera = transform.Find("PreviewCamera").gameObject;
        camera = mainCamera.transform.Find("Camera").GetComponent<Camera>();
        trackingInfo = FindObjectOfType<TrackingInfo>();
    }

    void Update()
    {
        Vector3 newPosition = camera.transform.localPosition + Vector3.down * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSensitivity;
        newPosition.y = Mathf.Clamp(newPosition.y, 1, 200);
        camera.transform.localPosition = newPosition;

        if (trackedObjectMain != null)
        {
            mainCamera.transform.position = trackedObjectMain.transform.position;
        }
        if (trackedObjectPreview != null)
        {
            previewCamera.transform.position = trackedObjectPreview.transform.position;
            previewCamera.transform.rotation = trackedObjectPreview.transform.rotation;
        }

        if (Input.GetMouseButton(1))
        {
            trackedObjectMain = null;
            Vector3 desiredMove = new Vector3(-Input.GetAxis("Mouse X"), 0, -Input.GetAxis("Mouse Y")) * Time.deltaTime * moveSensitivity;

            desiredMove = Quaternion.Euler(new Vector3(0f, mainCamera.transform.eulerAngles.y, 0f)) * desiredMove;
            desiredMove = mainCamera.transform.InverseTransformDirection(desiredMove);

            mainCamera.transform.Translate(desiredMove, Space.Self);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                if (hit.transform.CompareTag("Human"))
                {
                    trackingInfo.gameObject.SetActive(true);
                    trackedObjectMain = hit.transform.parent.parent.gameObject;
                    trackedObjectPreview = hit.transform.parent.parent.gameObject;
                }
            }
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 newRotation = mainCamera.transform.localEulerAngles + new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * rotationSensitivity;
            newRotation.x = Mathf.Clamp(newRotation.x, 270, 359);

            mainCamera.transform.localEulerAngles = newRotation;

        }
    }

    public void Center()
    {
        trackedObjectMain = trackedObjectPreview;
        Debug.Log("lala");
    }
}
