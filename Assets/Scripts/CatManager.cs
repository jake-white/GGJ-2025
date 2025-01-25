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

    protected override void Awake()
    {
        base.Awake();
        leftMoveAction = asset.FindAction(leftMoveName);
        rightMoveAction = asset.FindAction(rightMoveName);
    }

    // Update is called once per frame
    void Update()
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
}