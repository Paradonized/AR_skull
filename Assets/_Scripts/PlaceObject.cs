using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnchansedTouch = UnityEngine.InputSystem.EnhancedTouch; 

public class PlaceObject : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private GameObject placedObject;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    [SerializeField]
    private GameObject placedPrefab;

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    private void OnEnable()
    {
        EnchansedTouch.TouchSimulation.Enable();
        EnchansedTouch.EnhancedTouchSupport.Enable();
        EnchansedTouch.Touch.onFingerDown += FingerDown;
    }

    private void OnDisable()
    {
        EnchansedTouch.Touch.onFingerDown -= FingerDown;
        EnchansedTouch.EnhancedTouchSupport.Disable();
        EnchansedTouch.TouchSimulation.Disable();
    }

    private void FingerDown(EnchansedTouch.Finger finger)
    {
        if (finger == null) return;
        OnHit(finger);
    }

    private void OnHit(EnchansedTouch.Finger finger)
    {

        if (raycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose pose = hits[0].pose;
            placedObject = Instantiate(placedPrefab, pose.position, pose.rotation);

            Vector3 position = placedObject.transform.position;
            Vector3 cameraPosition = Camera.main.transform.position;

            Vector3 direction = cameraPosition - position;
            Vector3 targetRotationE = Quaternion.LookRotation(direction).eulerAngles;
            targetRotationE = Vector3.Scale(targetRotationE, placedObject.transform.up.normalized);

            Quaternion targetRotation = Quaternion.Euler(targetRotationE);
            placedObject.transform.rotation = targetRotation;
        }
    }

    private void Update()
    {
        if (placedObject != null)
        {
            planeManager.SetTrackablesActive(false);
            planeManager.enabled = false;
        }
    }

    public void ResetBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
 