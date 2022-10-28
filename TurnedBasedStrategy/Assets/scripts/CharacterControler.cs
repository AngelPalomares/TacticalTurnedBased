using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterControler : MonoBehaviour
{
    public float MoveSpeed;
    public Vector3 MoveTarget;

    public NavMeshAgent NavAgent;

    private bool isMoving;

    public bool isEnemy;

    // Start is called before the first frame update
    void Start()
    {
        MoveTarget = transform.position;
        NavAgent.speed = MoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving == true)
        {
            //transform.position = Vector3.MoveTowards(transform.position, MoveTarget, MoveSpeed * Time.deltaTime);

            if (GameManager.instance.ActivePlayer == this)
            {
                CameraController.instance.SetMoveTarget(transform.position);
                if (Vector3.Distance(transform.position, MoveTarget) < .2f)
                {
                    isMoving = false;

                    GameManager.instance.FinishedMovement();
                }
            }
        }
    }

    public void MoveToPoint(Vector3 pointtomove)
    {
        MoveTarget = pointtomove;

        NavAgent.SetDestination(MoveTarget);
        isMoving = true;
    }
}
