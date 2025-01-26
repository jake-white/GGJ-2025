using CollabXR;
using TMPro;
using UnityEngine;

public enum GameState { Playing, Ended };
public class GameManager : SingletonBehavior<GameManager>
{
    public TextMeshProUGUI currentHeightBox, bestHeightBox;
    public Animator canvasAnimator;
    float currentHeight, bestHeight;
    public Transform head, leftPaw, rightPaw;
    public float meterRatio;
    float lastSignOfLife;
    bool hasLife = true;
    public GameState state = GameState.Playing;
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

    void Update()
    {
        float headHeight = head.transform.position.y * meterRatio;
        currentHeight = headHeight > currentHeight ? headHeight : currentHeight;
        SetStats();
        if(state == GameState.Playing)
        {
            CheckForDeath();
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
        Debug.Log("Ninth Life Activated");
        hasLife = false;
        CatManager.Instance.RocketUpwards();
    }

    void EndGame()
    {
        state = GameState.Ended;
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
    }

    public void RestartGame()
    {
        if(state == GameState.Ended)
        {
            state = GameState.Playing;
            canvasAnimator.Play("RestartGame");
        }
    }
}
