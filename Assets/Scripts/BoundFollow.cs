using UnityEngine;

public class BoundFollow : MonoBehaviour
{
    public bool isLeft;
    // Update is called once per frame
    void Update()
    {
        float width = transform.localScale.x / 2;
        float sideBound = isLeft ? CameraFollow.Instance.GetLeftBound() - width : CameraFollow.Instance.GetRightBound() + width;
        transform.position = new Vector3(sideBound, CameraFollow.Instance.GetLowerBound(), transform.position.z);
    }
}
