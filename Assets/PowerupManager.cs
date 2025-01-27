using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using CollabXR;

public class PowerupManager : SingletonBehavior<PowerupManager>
{
    public TextMeshProUGUI currentDialBox;
    public List<AudioClip> clips;
    public Animator canvasAnimator;
    public float waitTime;
    public List<Powerup> powerups;
    List<InputAction> actions;
    InputAction tab;
    string currentDial;
    AudioSource source;
    IEnumerator coroutine;
    bool waitingOnPowerup = false;

    protected override void Awake()
    {
        base.Awake();
        source = GetComponent<AudioSource>();
        //this is disgusting but there are only 5 hours left
        actions = new List<InputAction> {
            InputSystem.actions.FindAction("0"),
            InputSystem.actions.FindAction("1"),
            InputSystem.actions.FindAction("2"),
            InputSystem.actions.FindAction("3"),
            InputSystem.actions.FindAction("4"),
            InputSystem.actions.FindAction("5"),
            InputSystem.actions.FindAction("6"),
            InputSystem.actions.FindAction("7"),
            InputSystem.actions.FindAction("8"),
            InputSystem.actions.FindAction("9"),
        };
        tab = InputSystem.actions.FindAction("Tab");
    }

    // Update is called once per frame
    void Update()
    {
        if (!waitingOnPowerup) {
            currentDialBox.text = currentDial;
            int numCheck = 0;
            foreach (InputAction action in actions)
            {
                if (action.WasPerformedThisFrame())
                {
                    InputNumber(numCheck++);
                }
                numCheck++;
            }
            canvasAnimator.SetBool("holdingTab", tab.IsPressed());
        }
    }

    public void InputNumber(int n)
    {
        if (!waitingOnPowerup && GameManager.Instance.state == GameState.Playing)
        {
            source.clip = clips[n];
            source.Play();
            currentDial += n;
            int matchFound = -1;
            bool matchPossible = false;
            for (int i = 0; i < powerups.Count; ++i)
            {
                string s = powerups[i].phoneNumber;
                for (int j = 0; j < currentDial.Length; j++)
                {
                    if (currentDial[j] != s[j])
                    {
                        break;
                    }
                    if (j == currentDial.Length - 1) // last char of dial
                    {
                        matchPossible = true;
                    }
                    if (j == s.Length - 1) // last char of phone number
                    {
                        matchFound = i;
                    }
                }
            }
            if (matchFound > -1)
            {
                CallPowerup(matchFound);
            }
            if (!matchPossible)
            {
                ClearNumber();
            }
        }
    }

    public void CallPowerup(int i)
    {
        Powerup p = powerups[i];
        if (GameManager.Instance.CanAfford(p.cost)) {
            waitingOnPowerup = true;
            GameManager.Instance.Buy(p.cost);
            CatManager.Instance.PlayMeow(0);
            coroutine = WaitForPowerup(p);
            StartCoroutine(coroutine);
        }
        else
        {
            ClearNumber();
            waitingOnPowerup = false;
        }
    }

    IEnumerator WaitForPowerup(Powerup p)
    {
        yield return new WaitForSeconds(waitTime);
        if(p.name == "Ninth Life")
        {
            GameManager.Instance.GetLife();
        }
        if (p.name == "Catnip")
        {
            GameManager.Instance.Catnip();
        }
        if (p.name == "Blinds Of Unbreaking")
        {
            GameManager.Instance.UnbreakingBlinds();
        }
        waitingOnPowerup = false;
        ClearNumber();
    }
    public void ClearNumber()
    {
        currentDial = "";
    }
}

[System.Serializable]
public class Powerup
{
    public int cost;
    public string phoneNumber;
    public string name;
}
