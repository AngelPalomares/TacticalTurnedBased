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

    public float moveRange = 3.5f,RunRange = 8f;

    public float MeleeRange = 1.5f;
    [HideInInspector]
    public List<CharacterControler> meleeTargets = new List<CharacterControler>();

    //[HideInInspector]
    public int CurrentMeleeTarget;



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

    public void GetMeleeTargets()
    {
        meleeTargets.Clear();

        if(isEnemy == false)
        {
            foreach(CharacterControler cc in GameManager.instance.enemyteam)
            {
                if(Vector3.Distance(transform.position, cc.transform.position) < MeleeRange)
                {
                    meleeTargets.Add(cc);
                }
            }
        }
        else
        {
            foreach (CharacterControler cc in GameManager.instance.Playertean)
            {
                if (Vector3.Distance(transform.position, cc.transform.position) < MeleeRange)
                {
                    meleeTargets.Add(cc);
                }
            }
        }
        if(CurrentMeleeTarget >= meleeTargets.Count)
        {
            CurrentMeleeTarget = 0;
        }
    }

    public void DoMelee()
    {
        meleeTargets[CurrentMeleeTarget].gameObject.SetActive(false);
    }
}
