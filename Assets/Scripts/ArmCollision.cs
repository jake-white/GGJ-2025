using UnityEngine;

public class ArmCollision : MonoBehaviour
{
    public LineRenderer lineRenderer;
    BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        Vector2 pos0 = lineRenderer.GetPosition(0);
        Vector2 pos1 = lineRenderer.GetPosition(1);
        transform.position = Vector2.Lerp(pos0, pos1, 0.5f);
        float angle = Mathf.Rad2Deg*Mathf.Atan2((pos0.y - pos1.y), (pos0.x - pos1.x));
        transform.rotation = Quaternion.Euler(0, 0, angle);
        boxCollider.size = new Vector2(Vector2.Distance(pos0, pos1), 1);
    }
}
