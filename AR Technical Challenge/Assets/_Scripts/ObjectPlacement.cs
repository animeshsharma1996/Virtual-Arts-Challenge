using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;
using UnityEngine.XR.ARSubsystems;

public class ObjectPlacement : MonoBehaviour
{
    [SerializeField] private GameObject placementIndicator;
    [SerializeField] private GameObject objectToPlace;

    private ARRaycastManager raycastManager;
    private Pose placementPose;
    private bool isPlacementValid = false;

    private void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
    }

    private void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        UpdateObjectPlacement();
    }

    private void UpdateObjectPlacement()
    {
        if(isPlacementValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        }
    }

    private void UpdatePlacementIndicator()
    {
        if(isPlacementValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position,placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        Vector3 screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f,0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        isPlacementValid = hits.Count > 0;
        if(isPlacementValid)
        {
            placementPose = hits[0].pose;

            Vector3 cameraForward = Camera.current.transform.forward;
            Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}
