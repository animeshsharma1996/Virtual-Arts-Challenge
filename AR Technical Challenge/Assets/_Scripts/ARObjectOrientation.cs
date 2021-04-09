using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ARObjectOrientation : MonoBehaviour
{
    [SerializeField] private List<Transform> objectComponents = new List<Transform>();
    [SerializeField] private float rotationSpeed = 0.01f;
    [SerializeField] private float maxDistanceComponent = 0.2f;
    [SerializeField] private float expansionSpeed = 0.1f;

    private float minDistanceComponent = 0f;
    private bool hasCalibrated = false;
    private Button setRotationButton;
    private Vector2 startPos;
    private Vector2 direction;
    private List<Transform> defaultComponents = new List<Transform>();

    private void Start()
    {
        hasCalibrated = false;
        defaultComponents = objectComponents;
        minDistanceComponent = (transform.position - objectComponents[1].position).magnitude;
    }

    private void Update()
    {
        if (!hasCalibrated)
        {
            RotateObjectBySwipe();
        }
        else
        {
            ExpandObjectByPinching();
        }
    }

    private void RotateObjectBySwipe()
    {
        // Track a single touch as a direction control.
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Handle finger movements based on TouchPhase
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    break;

                case TouchPhase.Moved:
                    direction = touch.position - startPos;
                    transform.Rotate(- direction.y * rotationSpeed, - direction.x * rotationSpeed, 0f);
                    setRotationButton.gameObject.SetActive(true);
                    break;

            }
        }
    }

    private void ExpandObjectByPinching()
    {
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            float prevMagnitude = (touchZero.position - touchZero.deltaPosition).magnitude;
            float currentMagnitude = (touchOne.position - touchOne.deltaPosition).magnitude;

            float difference = currentMagnitude - prevMagnitude;
            ExpandObject(difference);
        }
    }

    private void ExpandObject(float difference)
    {
        foreach (Transform childTransform in objectComponents)
        {
            Vector3 alignedVector = childTransform.position - transform.position;
            Vector3 newPosComponent = Vector3.Lerp(childTransform.transform.position, childTransform.transform.position + difference * expansionSpeed * alignedVector, Time.deltaTime );

            float distanceComponent = (transform.position - newPosComponent).magnitude;
            if (distanceComponent >= minDistanceComponent && distanceComponent < maxDistanceComponent)
            {
                childTransform.transform.position = newPosComponent;
            }
        }
    }

    public void ResetTransformsComponents()
    {
        objectComponents = defaultComponents;
    }

    public void SetRotationButton(Button button)
    {
        setRotationButton = button;
    }

    public void SetHasCalibrated(bool value)
    {
        hasCalibrated = value;
    }
}
