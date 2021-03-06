using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ARObjectOrientation : MonoBehaviour
{
    [SerializeField] private List<Transform> objectComponents = new List<Transform>();  //List of the components of the objects
    [SerializeField] private List<ResetTranform> resetTranforms = new List<ResetTranform>(); //List of ResetTransform class handling to reset the transform of the components
    [SerializeField] private float rotationSpeed = 0.01f;
    [SerializeField] private float maxDistanceComponent = 0.2f; //Max distance upto which the components can expand
    [SerializeField] private float expansionSpeed = 0.1f;

    //Min distance upto which the components can shrink so that they don't go inside the object
    private float minDistanceComponent = 0f;
    private bool hasCalibrated = false;
    private Button setRotationButton;
    private Vector2 startPos;
    private Vector2 direction;

    private void Start()
    {
        hasCalibrated = false;
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

    //Swiping rotates the object in two axes, horizontal and vertical
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
                    transform.Rotate(direction.y * rotationSpeed, - direction.x * rotationSpeed, 0f);
                    setRotationButton.gameObject.SetActive(true);
                    break;
            }
        }
    }

    //Pinch gesture logic by touch
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

            float difference = currentMagnitude - prevMagnitude;
            ExpandObject(difference);
        }
    }

    //Expand the components of the object based on pinch
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
        foreach (ResetTranform resetTranform in resetTranforms)
        {
            resetTranform.ReinstatePositions();
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
