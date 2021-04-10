using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class ObjectPlacement : MonoBehaviour
{
    [SerializeField] private GameObject placementIndicator = null;
    [SerializeField] private GameObject objectToPlace = null;

    [SerializeField] private Button nextButton = null;
    [SerializeField] private Button setRotationButton = null;
    [SerializeField] private Button recalibrateButton = null;
    [SerializeField] private TMP_Text objectPlacedText = null;
    [SerializeField] private TMP_Text objectRotatedText = null;
    [SerializeField] private TMP_Text calibrationCompleteText = null;
    [SerializeField] private TMP_Text gameSceneText = null;

    private ARRaycastManager raycastManager;
    private Pose placementPose;
    private ARObjectOrientation rotateAR;
    private bool isPlacementValid = false;
    private bool objectPlaced = false;
    private bool objectRotated = false;
    private GameObject placedObject;

    private void Start()
    {
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(SetUpGameScene);

        setRotationButton.onClick.RemoveAllListeners();
        setRotationButton.onClick.AddListener(SetARObjectRotation);

        recalibrateButton.onClick.RemoveAllListeners();
        recalibrateButton.onClick.AddListener(Recalibrate);

        objectPlaced = false;
        objectRotated = false;

        nextButton.gameObject.SetActive(false);
        setRotationButton.gameObject.SetActive(false);
        objectPlacedText.gameObject.SetActive(false);
        objectRotatedText.gameObject.SetActive(false);
        calibrationCompleteText.gameObject.SetActive(false);

        placedObject = Instantiate(objectToPlace);
        rotateAR = placedObject.GetComponentInChildren<ARObjectOrientation>();
        placedObject.SetActive(false);

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
        if (isPlacementValid && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (!objectPlaced)
                {
                    placedObject.SetActive(true);
                    placedObject.transform.position = placementPose.position;
                    objectPlacedText.gameObject.SetActive(true);
                    objectPlaced = true;
                    placementIndicator.SetActive(true);
                }
            }

            if (objectPlaced && !objectRotated)
            {
                rotateAR.SetRotationButton(setRotationButton);
                rotateAR.enabled = true;
            }
        }
    }

    private void SetUpGameScene()
    {
        nextButton.gameObject.SetActive(false);
        setRotationButton.gameObject.SetActive(false);
        objectPlacedText.gameObject.SetActive(false);
        objectRotatedText.gameObject.SetActive(false);
        calibrationCompleteText.gameObject.SetActive(false);

        recalibrateButton.gameObject.SetActive(true);
        gameSceneText.gameObject.SetActive(true);
        rotateAR.SetHasCalibrated(true);
    }


    private void SetARObjectRotation()
    {
        objectRotatedText.gameObject.SetActive(true);
        calibrationCompleteText.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        objectRotated = true;
    }

    private void Recalibrate()
    {
        rotateAR.SetHasCalibrated(false);
        rotateAR.ResetTransformsComponents();

        objectPlaced = false;
        objectRotated = false;

        recalibrateButton.gameObject.SetActive(false);
        gameSceneText.gameObject.SetActive(false);
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
        Vector3 screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f,0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneEstimated);

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
