using CollabXR;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { Playing, Ended };
public class GameManager : SingletonBehavior<GameManager>
{
    public TextMeshProUGUI currentHeightBox, bestHeightBox, flyBox;
    public Image lifeIndicator;
    public Animator canvasAnimator;
    float currentHeight, bestHeight;
    int currentFlies;
    public Transform head, leftPaw, rightPaw;
    public float meterRatio;
    public float forceMod;
    float catnipTime, unbreakingTime = 0.0f;
    float lastSignOfLife;
    bool hasLife = false;
    public GameState state = GameState.Playing;
    public ParticleSystem catnipParticles;
    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Start()
    {
        SetStats();
        lastSignOfLife = Time.time;
        state = GameState.Playing;
    }

    public void AddFlies(int f)
    {
        currentFlies += f;
    }

    void Update()
    {
        float headHeight = head.transform.position.y * meterRatio;
        currentHeight = headHeight > currentHeight ? headHeight : currentHeight;
        SetStats();
        if(state == GameState.Playing)
        {
            CheckForDeath();
        }
        if(BlindsAreUnbreaking())
        {
            unbreakingTime -= Time.deltaTime;
        }
        if(catnipTime > 0.0f)
        {
            catnipTime -= Time.deltaTime;
        }
    }

    public float GetHeight()
    {
        return currentHeight;
    }

    public void CheckForDeath()
    {
        float headHeight = head.transform.position.y;
        float leftHeight = leftPaw.transform.position.y;
        float rightHeight = rightPaw.transform.position.y;
        float lowerBound = CameraFollow.Instance.GetLowerBound() - 5;
        if(headHeight < lowerBound && leftHeight < lowerBound && rightHeight < lowerBound)
        {
            if(hasLife)
            {
                Respawn();
            }
            else if(Time.time - lastSignOfLife > 1)
            {
                EndGame();
            }
        }
        else
        {
            lastSignOfLife = Time.time;
        }
    }

    void Respawn()
    {
        hasLife = false;
        CatManager.Instance.RocketUpwards();
    }

    void EndGame()
    {
        state = GameState.Ended;
        currentFlies = 0;
        CatManager.Instance.PlayMeow(1);
        PowerupManager.Instance.ClearNumber();
        BlindsManager.Instance.RestartLevel();
        CatManager.Instance.ResetCat();
        CameraFollow.Instance.SnapToCat();
        if (currentHeight > bestHeight)
        {
            bestHeight = currentHeight;
            SetStats();
            LeaderboardManager.Instance.AddScore(bestHeight);
        }
        currentHeight = 0.0f;
        canvasAnimator.Play("EndGame");
    }

    public void SetStats()
    {
        currentHeightBox.text = currentHeight.ToString("0.#") + "m";
        bestHeightBox.text = "Best: " + bestHeight.ToString("0.#") + "m";
        flyBox.text = currentFlies.ToString();
        lifeIndicator.color = new Color(1.0f, 1.0f, 1.0f, hasLife ? 1.0f : 0.2f);
    }

    public void RestartGame()
    {
        if(state == GameState.Ended)
        {
            state = GameState.Playing;
            canvasAnimator.Play("RestartGame");
        }
    }

    public bool CanAfford(int cost)
    {
        return currentFlies >= cost;
    }

    public void Buy(int cost)
    {
        currentFlies -= cost;
    }

    public void GetLife()
    {
        hasLife = true;
    }

    public void Catnip()
    {
        catnipTime = 30.0f;
        catnipParticles.Stop();
        ParticleSystem.MainModule module = catnipParticles.main;
        module.duration = catnipTime;
        catnipParticles.Play();
    }

    public void UnbreakingBlinds()
    {
        unbreakingTime = 20.0f;
    }

    public bool BlindsAreUnbreaking()
    {
        return unbreakingTime > 0;
    }

    public float GetForceMod()
    {
        float catnipMod = catnipTime > 0.0f ? Mathf.Clamp(catnipTime / 10.0f, 1.0f, 100.0f) : 1.0f;
        return forceMod * catnipMod;
    }
}
