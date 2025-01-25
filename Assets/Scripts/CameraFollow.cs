using CollabXR;
using UnityEngine;

public class CameraFollow : SingletonBehavior<CameraFollow>
{
    public Transform toFollow;
    public float rectWidth = 0.8f;
    Camera cam;
    float widthRatio;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.rect = new Rect(0, 0, rectWidth, 1);
        widthRatio = ((float)Screen.width / (float)Screen.height)*rectWidth;
        Debug.Log(widthRatio);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPos = Vector2.Lerp(transform.position, toFollow.position, Time.deltaTime);
        if (newPos.y >= transform.position.y)
        {
            transform.position = new Vector3(0, newPos.y, transform.position.z);
        }
    }

    public float GetLowerBound()
    {
        return transform.position.y - cam.orthographicSize;
    }

    public float GetLeftBound()
    {
        return transform.position.x - cam.orthographicSize * widthRatio;
    }

    public float GetRightBound()
    {
        return transform.position.x + cam.orthographicSize * widthRatio;
    }
}
