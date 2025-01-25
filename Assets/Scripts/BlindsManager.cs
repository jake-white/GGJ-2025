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
            RandomizeBlind(instance);
            blindPool.Add(instance);
        }
    }

    public void RandomizeBlind(Blind b)
    {
        float height = Random.Range(minHeight, maxHeight);
        b.Initialize(height, lastBlindHeight, baseBlind.transform.position.z);
        lastBlindHeight += height + RandomInterval();
    }

    float RandomInterval()
    {
        return Random.Range(minInterval, maxInterval);
    }
}
