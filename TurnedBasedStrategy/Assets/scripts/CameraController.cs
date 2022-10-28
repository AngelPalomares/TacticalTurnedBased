using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    private void Awake()
    {
        instance = this;
    }

    public float MoveSpeed, ManualMoveSpeed;

    private Vector3 moveTarget;

    private Vector2 MoveInput;

    private float TargetRotation;
    public float RotateSpeed;

    public int CurrentAngle;
    // Start is called before the first frame update
    void Start()
    {
        moveTarget = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveTarget != transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, MoveSpeed * Time.deltaTime);
        }

        MoveInput.x = Input.GetAxis("Horizontal");
        MoveInput.y = Input.GetAxis("Vertical");
        MoveInput.Normalize();

        if(MoveInput != Vector2.zero)
        {
            transform.position += ((transform.forward * (MoveInput.y * ManualMoveSpeed)) + (transform.right * (MoveInput.x * ManualMoveSpeed))) *Time.deltaTime;
            moveTarget = transform.position;
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            SetMoveTarget(GameManager.instance.ActivePlayer.transform.position);
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            CurrentAngle--;
            if(CurrentAngle <=0)
            {
                CurrentAngle = 3;
            }
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            CurrentAngle++;
            if(CurrentAngle >= 4)
            {
                CurrentAngle = 0;
            }
        }

        TargetRotation = (90f * CurrentAngle) + 45f;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, TargetRotation, 0f), RotateSpeed * Time.deltaTime);
    }

    public void SetMoveTarget(Vector3 newTarget)
    {
        moveTarget = newTarget;
    }
}
