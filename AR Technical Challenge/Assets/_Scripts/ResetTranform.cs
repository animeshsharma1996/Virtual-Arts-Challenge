using UnityEngine;

public class ResetTranform : MonoBehaviour
{
    private Vector3 defaultPosition = new Vector3(0,0,0);

    private void Start()
    {
        defaultPosition = transform.localPosition;
    }

    public void ReinstatePositions()
    {
        transform.localPosition = defaultPosition;
    }
}
