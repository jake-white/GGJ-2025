using CollabXR;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : SingletonBehavior<CameraFollow>
{
    public List<Transform> toFollowUp;
    public float rectWidth = 0.8f;
    public Transform skybox;
    public Color nightColor;
    Camera cam;
    float widthRatio, boundAmt;
    float startingOrtho, targetOrtho;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.rect = new Rect(0, 0, rectWidth, 1);
        widthRatio = ((float)Screen.width / (float)Screen.height)*rectWidth;
        boundAmt = BlindsManager.Instance.blindWidth / 2;
        targetOrtho = (boundAmt / widthRatio)*1.1f;
        startingOrtho = targetOrtho * 2;
        cam.orthographicSize = startingOrtho;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 yToFollow = transform.position;
        foreach(Transform t in toFollowUp)
        {
            yToFollow = t.position.y > yToFollow.y ? t.position : yToFollow;
        }
        float ease = EaseOutQuart(Time.deltaTime);
        Vector2 newPos = Vector2.Lerp(transform.position, yToFollow, ease);
        if (newPos.y >= transform.position.y)
        {
            transform.position = new Vector3(0, newPos.y, transform.position.z);
        }
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetOrtho, Time.deltaTime);
        AdjustSkybox();
    }

    void AdjustSkybox()
    {
        skybox.transform.localPosition = new Vector3(0, -GameManager.Instance.GetHeight(), 50);
    }

    public float GetLowerBound()
    {
        return transform.position.y - cam.orthographicSize;
    }

    public float GetLeftBound()
    {
        return -boundAmt;
    }

    public float GetRightBound()
    {
        return boundAmt;
    }

    public float EaseOutQuart(float t)
    {
        t--;
        return -(t * t * t * t - 1);
    }

    public void SnapToCat()
    {
        transform.position = new Vector3(transform.position.x, toFollowUp[0].position.y, transform.position.z);
    }
}
