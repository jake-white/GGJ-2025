using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int points;
    Blind parentBlind;
    private void Awake()
    {
        parentBlind = GetComponentInParent<Blind>();
    }
    public void CollectMe()
    {
        gameObject.SetActive(false);
    }
}
