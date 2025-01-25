using UnityEngine;

public class Blind : MonoBehaviour
{
    public float height;
    public int hp = 10;
    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < CameraFollow.Instance.GetLowerBound() - height - 1)
        {
            BlindsManager.Instance.RandomizeBlind(this);
            Debug.Log("Rerandomizing");
        }
    }

    public void Initialize(float height, float y, float z)
    {
        this.height = height;
        this.hp = 10;
        transform.localScale = new Vector3(100, height, 1);
        transform.position = new Vector3(0, y, z);
    }
}
