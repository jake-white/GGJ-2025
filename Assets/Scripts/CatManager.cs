using CollabXR;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PawState { Free, Grabbing, Grabbed };
public class CatManager : SingletonBehavior<CatManager>
{
    public InputActionAsset asset;
    public string leftMoveName, rightMoveName;
    public Paw leftPaw, rightPaw;
    public Transform head;

    private InputAction leftMoveAction, leftGrabAction, rightMoveAction, rightGrabAction;
    public Rigidbody2D body;
    Vector3 bodyStart, leftPawStart, rightPawStart;

    protected override void Awake()
    {
        base.Awake();
        leftMoveAction = asset.FindAction(leftMoveName);
        rightMoveAction = asset.FindAction(rightMoveName);
        bodyStart = body.transform.position;
        leftPawStart = leftPaw.transform.position;
        rightPawStart = rightPaw.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        leftPaw.Move(leftMoveAction.ReadValue<Vector2>());
        rightPaw.Move(rightMoveAction.ReadValue<Vector2>());
        body.gravityScale = PawsGrabbed() > 0 ? 1 : 2;
    }

    public int PawsGrabbed()
    {
        int amt = 0;
        if(leftPaw.state == PawState.Grabbed)
        {
            amt++;
        }
        if (rightPaw.state == PawState.Grabbed)
        {
            amt++;
        }
        return amt;
    }

    public Vector2 GetBodyVelocity()
    {
        return body.linearVelocity;
    }

    public void RocketUpwards()
    {
        float yToKeep = body.transform.position.y;
        Debug.Log("yToKeep="+yToKeep);
        body.transform.position = new Vector3(0, yToKeep, 0);
        body.linearVelocity = new Vector2(0, 50);
        leftPaw.RocketUpwards(new Vector3(-5, yToKeep + 8));
        rightPaw.RocketUpwards(new Vector3(5, yToKeep + 8));
    }

    public void ResetCat()
    {
        body.transform.position = bodyStart;
        leftPaw.transform.position = leftPawStart;
        leftPaw.UnGrab(true);
        leftPaw.body.linearVelocity = Vector2.zero;
        rightPaw.transform.position = rightPawStart;
        rightPaw.UnGrab(true);
        rightPaw.body.linearVelocity = Vector2.zero;
    }
}