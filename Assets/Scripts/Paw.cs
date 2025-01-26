using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class Paw : MonoBehaviour
{
    public Transform armAttach;
    public LineRenderer lr;
    public GameObject claws;
    public PawState state;
    public Rigidbody2D body;
    public Paw otherPaw;
    List<Blind> blindsGrabbable;
    Blind grabbedBlind;
    DistanceJoint2D joint;
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        blindsGrabbable = new List<Blind>();
        joint = GetComponent<DistanceJoint2D>();
    }

    void Update()
    {
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, armAttach.position);
        if (!GameManager.Instance.BlindsAreUnbreaking())
        {
            if (grabbedBlind != null)
            {
                bool broken = grabbedBlind.DamageDoesItBreak(Time.deltaTime);
                if (broken)
                {
                    UnGrab(true);
                }
            }
        }
    }

    public void Move(Vector2 v)
    {
        bool shouldApplyUpwardsForce = CatManager.Instance.PawsGrabbed() > 0 ||
            Vector2.Distance(transform.position, CatManager.Instance.head.position) < joint.distance - 1;
        if (!shouldApplyUpwardsForce)
        {
            v.y = v.y > 0 ? 0 : v.y;
            if(transform.position.y > BlindsManager.Instance.firstBlindPosition + 10)
            {
                body.gravityScale = 1;
            }
        }
        else
        {
            body.gravityScale = 0;
        }
        body.AddForce(v*GameManager.Instance.GetForceMod());
    }

    void Collect(Collectible c)
    {
        GameManager.Instance.AddFlies(1);
        c.CollectMe();
    }

    public void TryGrab(CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (state == PawState.Free)
            {
                state = PawState.Grabbing;
                if (blindsGrabbable.Count > 0)
                {
                    Blind likelyBlind = blindsGrabbable[blindsGrabbable.Count - 1];  // grabs the most recently added blind
                    if (likelyBlind.state != BlindState.Broken)
                    {
                        if (otherPaw.grabbedBlind == null || otherPaw.grabbedBlind != likelyBlind)
                        {
                            Grab(likelyBlind);
                        }
                        else // trying to grab the same blind!
                        {
                            likelyBlind.Break();
                            otherPaw.UnGrab(true);
                        }
                    }
                }
            }
        }
        else if (ctx.canceled)
        {
            if (state == PawState.Grabbed)
            {
                UnGrab(false);
            }
            state = PawState.Free;
        }
        claws.SetActive(state == PawState.Free);
    }

    public void Grab(Blind b)
    {
        body.bodyType = RigidbodyType2D.Kinematic;
        body.linearVelocity = Vector2.zero;
        state = PawState.Grabbed;
        grabbedBlind = b;
    }

    public void UnGrab(bool forcedByBreak)
    {
        body.bodyType = RigidbodyType2D.Dynamic;
        grabbedBlind = null;
        if(!forcedByBreak)
        {
            body.linearVelocity = CatManager.Instance.GetBodyVelocity();
        }
        else
        {
            //CatManager.Instance.PlayMeow(2);
        }
        state = PawState.Free;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Blind blind = collider.GetComponentInParent<Blind>();
        Collectible collectible = collider.GetComponentInParent<Collectible>();
        if(blind != null)
        {
            blindsGrabbable.Add(blind);
        }

        if(collectible != null)
        {
            Collect(collectible);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        Blind blind = collider.GetComponentInParent<Blind>();
        if (blind != null && blindsGrabbable.Contains(blind))
        {
            blindsGrabbable.Remove(blind);
        }
    }

    public void RocketUpwards(Vector2 startingPos)
    {
        transform.position = startingPos;
        Debug.Log("setting paw to " + startingPos);
        body.linearVelocity = new Vector2(0, 50);
    }
}
