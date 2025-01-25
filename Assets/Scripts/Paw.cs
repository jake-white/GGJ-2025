using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class Paw : MonoBehaviour
{
    public Transform armAttach;
    public LineRenderer lr;
    public GameObject claws;
    public PawState state;
    public float forceMod = 1.0f;
    private Rigidbody2D body;
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
        body.AddForce(v*forceMod);
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
                    Grab(blindsGrabbable[blindsGrabbable.Count - 1]); // grabs the most recently added blind
                }
            }
        }
        else if (ctx.canceled)
        {
            if (state == PawState.Grabbed)
            {
                UnGrab();
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

    public void UnGrab()
    {
        body.bodyType = RigidbodyType2D.Dynamic;
        grabbedBlind = null;
        body.linearVelocity = CatManager.Instance.GetBodyVelocity();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Blind blind = collider.GetComponent<Blind>();
        if(blind != null)
        {
            blindsGrabbable.Add(blind);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        Blind blind = collider.GetComponent<Blind>();
        if (blind != null && blindsGrabbable.Contains(blind))
        {
            blindsGrabbable.Remove(blind);
        }
    }
}
