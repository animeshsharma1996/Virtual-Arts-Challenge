using UnityEngine;
using UnityEngine.UI;

public class ARObjectOrientation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 0.01f;
    
    private bool hasCalibrated = false;
    private Button setRotationButton;
    private Vector2 startPos;
    private Vector2 direction;

    private void Start()
    {
        hasCalibrated = false;
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
                    transform.Rotate(-  direction.y * rotationSpeed, - direction.x * rotationSpeed, 0f);
                    setRotationButton.gameObject.SetActive(true);
                    break;

            }
        }
    }

    private void ExpandObjectByPinching()
    {

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
