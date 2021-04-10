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
        minDistanceComponent = (transform.position - objectComponents[0].position).magnitude;
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
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    break;

                case TouchPhase.Moved:
                    direction = touch.position - startPos;
                    transform.Rotate(0f, - direction.x * rotationSpeed, 0f);
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

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference =   currentMagnitude - prevMagnitude;
            ExpandObject(difference);
        }
        else
        {
            foreach (Transform childTransform in objectComponents)
            {
                Vector3 alignedVector = childTransform.position - transform.position;
                Vector3 newPosComponent = childTransform.transform.position;
                newPosComponent = Vector3.Lerp(newPosComponent, childTransform.transform.position + Input.GetAxis("Mouse ScrollWheel") * 10.0f * alignedVector, Time.deltaTime*10.0f);

                float distanceComponent = (transform.position - newPosComponent).magnitude;
                if (distanceComponent >= minDistanceComponent && distanceComponent < maxDistanceComponent)
                {
                    childTransform.transform.position = newPosComponent;
                }
            }
        }
    }

    private void ExpandObject(float difference)
    {
        foreach (Transform childTransform in objectComponents)
        {
            Vector3 alignedVector = childTransform.position - transform.position;
            Vector3 newPosComponent = childTransform.transform.position;
            newPosComponent = Vector3.Lerp(newPosComponent, childTransform.transform.position + difference * expansionSpeed * alignedVector, Time.deltaTime );

            float distanceComponent = (transform.position - newPosComponent).magnitude;
            if (distanceComponent >= minDistanceComponent && distanceComponent < maxDistanceComponent)
            {
                childTransform.transform.position = newPosComponent;
            }
        }
    }

    public void ResetTransformsComponents()
    {
        int index = 0;
        foreach (Transform childTransform in objectComponents)
        {
            childTransform.position = defaultComponents[index].position;
            ++index;
        }
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
