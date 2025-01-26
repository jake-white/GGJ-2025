using System.Collections.Generic;
using UnityEngine;
public enum BlindState { Intact, Breaking, Broken }
public class Blind : MonoBehaviour
{
    public float width, height;
    public float hp = 100, maxhp = 100;
    public GameObject intact, broken;
    public GameObject brokenLeft, brokenRight, breakIconParent;
    public GameObject fly;
    public float damageSpeed = 0.1f;
    public BlindState state;
    bool unbreakable = false;
    List<GameObject> breakIcons;
    private void Awake()
    {
        breakIcons = new List<GameObject>();
    }
    void Update()
    {
        if(transform.position.y < CameraFollow.Instance.GetLowerBound() - height - 1)
        {
            BlindsManager.Instance.RandomizeBlind(this, false);
        }
    }

    public void Initialize(float width, float height, float y, float z, bool unbreakable, bool shouldHaveFly)
    {
        this.width = width;
        this.height = height;
        this.hp = this.maxhp;
        this.unbreakable = unbreakable;
        fly.SetActive(shouldHaveFly);
        fly.transform.localPosition = new Vector3(Random.Range((-width / 2) + 2, (width / 2) - 2), 0, 0);
        intact.transform.localScale = new Vector3(width, height, 1);
        brokenLeft.transform.localScale = new Vector3(width, height, 1);
        brokenRight.transform.localScale = new Vector3(width, height, 1);
        transform.position = new Vector3(0, y, z);
        foreach (GameObject icon in breakIcons)
        {
            GameObject.Destroy(icon);
        }
        breakIcons = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            GameObject newIcon = Instantiate(BlindsManager.Instance.GetRandomBreakIcon(), breakIconParent.transform);
            newIcon.transform.localPosition = new Vector3(Random.Range((-width / 2)+2, (width / 2)-2), 0, 0);
            newIcon.SetActive(false);
            breakIcons.Add(newIcon);
        }
        state = BlindState.Intact;
        UpdateState();
    }

    public bool DamageDoesItBreak(float amt)
    {
        if (!unbreakable)
        {
            hp -= amt * damageSpeed;
            breakIcons[0].SetActive(hp < 80);
            breakIcons[1].SetActive(hp < 50);
            breakIcons[2].SetActive(hp < 25);
            if (hp <= 0)
            {
                Break();
                return true;
            }
        }
        return false;
    }

    public void Break()
    {
        if (!unbreakable)
        {
            this.hp = 0;
            state = BlindState.Broken;
            brokenLeft.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-20, 0));
            brokenRight.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 20));
            UpdateState();
        }
    }

    void UpdateState()
    {
        intact.SetActive(state == BlindState.Intact || state == BlindState.Breaking);
        broken.SetActive(state == BlindState.Broken);
    }
}
