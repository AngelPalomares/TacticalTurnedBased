using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    private void Awake()
    {
        instance = this;
        moveTarget = transform.position;

    }

    public float MoveSpeed, ManualMoveSpeed;

    private Vector3 moveTarget;

    private Vector2 MoveInput;

    private float TargetRotation;
    public float RotateSpeed;

    public int CurrentAngle;

    public Transform thecam;

    public float FireCamViewAngle = 30f;
    private float TargetCamViewAngle;
    private bool isFireView;

    // Start is called before the first frame update
    void Start()
    {
        TargetCamViewAngle = 45f;
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

        if (isFireView == false)
        {
            TargetRotation = (90f * CurrentAngle) + 45f;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, TargetRotation, 0f), RotateSpeed * Time.deltaTime);

        thecam.localRotation = Quaternion.Slerp(thecam.localRotation, Quaternion.Euler(TargetCamViewAngle, 0f, 0f), RotateSpeed * Time.deltaTime);
    }

    public void SetMoveTarget(Vector3 newTarget)
    {
        moveTarget = newTarget;

        TargetCamViewAngle = 45f;
        isFireView = false;
    }

    public void SetFireView()
    {
        moveTarget = GameManager.instance.ActivePlayer.transform.position;

        TargetRotation = GameManager.instance.ActivePlayer.transform.rotation.eulerAngles.y;

        TargetCamViewAngle = FireCamViewAngle;

        isFireView = true;
    }
}
