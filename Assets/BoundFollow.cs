using UnityEngine;

public class BoundFollow : MonoBehaviour
{
    public bool isLeft;
    // Update is called once per frame
    void Update()
    {
        float sideBound = isLeft ? CameraFollow.Instance.GetLeftBound() - 5 : CameraFollow.Instance.GetRightBound() + 5;
        transform.position = new Vector3(sideBound, CameraFollow.Instance.GetLowerBound(), transform.position.z);
    }
}
