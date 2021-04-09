using UnityEngine;
using UnityEngine.UI;

public class RotateARObject : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 0.01f;
    private Button setRotationButton;
    private Vector2 startPos;
    private Vector2 direction;

    public void SetRotationButton(Button button)
    {
        setRotationButton = button;
    }   
    
    private void Update()
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
                    //transform.Rotate(-direction * rotationSpeed);
                    transform.Rotate(-direction.y * rotationSpeed, -direction.x * rotationSpeed, 0f);
                    setRotationButton.gameObject.SetActive(true);
                    break;

            }
        }
    }
}
