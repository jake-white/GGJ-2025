using CollabXR;
using System.Collections.Generic;
using UnityEngine;

public class BlindsManager : SingletonBehavior<BlindsManager>
{
    [Header("Pool Settings")]
    public GameObject baseBlind;
    public List<Blind> blindPool;
    public int poolSize;
    [Header("Blind Settings")]
    public float firstBlindPosition;
    public float minHeight, maxHeight;
    public float minInterval, maxInterval;
    public float blindWidth;
    public List<GameObject> breakingIcons;

    private float lastBlindHeight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override protected void Awake()
    {
        base.Awake();
        blindPool = new List<Blind>();
        lastBlindHeight = firstBlindPosition;
        for(int i = 0; i < poolSize; i++)
        {
            Blind instance = Instantiate(baseBlind, transform).GetComponent<Blind>();
            RandomizeBlind(instance, i < 3);
            blindPool.Add(instance);
        }
    }

    public void RestartLevel()
    {
        lastBlindHeight = firstBlindPosition;
        for (int i = 0; i < poolSize; i++)
        {
            RandomizeBlind(blindPool[i], i < 3);
        }
    }

    public void RandomizeBlind(Blind b, bool shouldBeUnbreakable)
    {
        float height = Random.Range(minHeight, maxHeight);
        bool shouldHaveFly = Random.Range(0.0f, 1.0f) > 0.7f;
        b.Initialize(blindWidth, height, lastBlindHeight, baseBlind.transform.position.z, shouldBeUnbreakable, shouldHaveFly);
        lastBlindHeight += height + RandomInterval();
    }

    public GameObject GetRandomBreakIcon()
    {
        int index = Random.Range(0, breakingIcons.Count);
        return breakingIcons[index];
    }

    float RandomInterval()
    {
        return Random.Range(minInterval, maxInterval);
    }
}
