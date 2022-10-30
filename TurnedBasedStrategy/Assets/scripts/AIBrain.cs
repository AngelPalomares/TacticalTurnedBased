using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    public CharacterControler CharCon;

    public float WaitBeforeAttacking = 1f, WaitAfterActing = 1f;


    [Range(0f, 100f)]
    public float IgnoreShootChance = 20f, MoveRandomChance = 50f;

    public float MoveChance = 60f, DefendChance = 25f, SkipChance = 15f;

    public void ChooseAction()
    {
        StartCoroutine(ChooseCo());
    }

    public IEnumerator ChooseCo()
    {
        Debug.Log(name + " is choosing an action");
        yield return new WaitForSeconds(WaitBeforeAttacking);

        bool actiontaken = false;

        CharCon.GetMeleeTargets();

        if (CharCon.meleeTargets.Count > 0)
        {
            Debug.Log("is Meleeing");

            CharCon.CurrentMeleeTarget = Random.Range(0, CharCon.meleeTargets.Count);

            GameManager.instance.CurrentActionCost = 1;

            StartCoroutine(WaitToEndAction(WaitAfterActing));

            CharCon.DoMelee();

            actiontaken = true;
        }

        CharCon.GetShootTarget();

        //AI system to determine the shooting mechincs into the game
        if (actiontaken == false && CharCon.ShootTargets.Count > 0)
        {
            if (Random.Range(0f, 100f) > IgnoreShootChance)
            {


                List<float> hitchances = new List<float>();
                for (int i = 0; i < CharCon.ShootTargets.Count; i++)
                {
                    CharCon.currentShootTarget = i;
                    CharCon.LookAtTarget(CharCon.ShootTargets[i].transform);
                    hitchances.Add(CharCon.CheckShotChance());
                }
                float HighestChance = 0f;
                for (int i = 0; i < hitchances.Count; i++)
                {
                    if (hitchances[i] > HighestChance)
                    {
                        HighestChance = hitchances[i];
                        CharCon.currentShootTarget = i;
                    }
                    else if (hitchances[i] == HighestChance)
                    {
                        if (Random.value > .5f)
                        {
                            CharCon.currentShootTarget = i;
                        }
                    }
                }

                if (HighestChance > 0f)
                {
                    CharCon.LookAtTarget(CharCon.ShootTargets[CharCon.currentShootTarget].transform);
                    CameraController.instance.SetFireView();
                    actiontaken = true;

                    StartCoroutine(WaitToShoot());
                }
            }
        }


        if (actiontaken == false)
        {

            float ActionDecision = Random.Range(0f, MoveChance + DefendChance + SkipChance);

            if (ActionDecision < MoveChance)
            {


                float MoveRandom = Random.Range(0, 100f);
                List<MovePoint> potentialmovepoints = new List<MovePoint>();
                int selectedpoints = 0;

                if (MoveRandom > MoveRandomChance)
                {


                    int NearestPlayer = 0;

                    for (int i = 1; i < GameManager.instance.Playertean.Count; i++)
                    {
                        if (Vector3.Distance(transform.position, GameManager.instance.Playertean[NearestPlayer].transform.position)
                                > Vector3.Distance(transform.position, GameManager.instance.Playertean[i].transform.position))
                        {
                            NearestPlayer = i;
                        }
                    }

                    //List<MovePoint> potentialmovepoints;
                    //int selectedpoints = 0;

                    potentialmovepoints = MoveGrid.instance.Getmovepointsinrange(CharCon.moveRange, transform.position);

                    float clossespoint = 1000f;

                    for (int i = 0; i < potentialmovepoints.Count; i++)
                    {
                        if (Vector3.Distance(GameManager.instance.Playertean[NearestPlayer].transform.position, potentialmovepoints[i].transform.position) < clossespoint)
                        {
                            clossespoint = Vector3.Distance(GameManager.instance.Playertean[NearestPlayer].transform.position, potentialmovepoints[i].transform.position);
                            selectedpoints = i;
                        }
                    }


                    GameManager.instance.CurrentActionCost = 1;
                }
                else
                {
                    potentialmovepoints = MoveGrid.instance.Getmovepointsinrange(CharCon.moveRange, transform.position);

                    selectedpoints = Random.Range(0, potentialmovepoints.Count);

                    GameManager.instance.CurrentActionCost = 1;
                }

                CharCon.MoveToPoint(potentialmovepoints[selectedpoints].transform.position);
            }//else if is used to determine whether the player should defend or not
            else if (ActionDecision < MoveChance + DefendChance)
            {
                CharCon.SetDefending(true);

                GameManager.instance.CurrentActionCost = GameManager.instance.TurnPointsRemaining;
                StartCoroutine(WaitToEndAction(WaitAfterActing));
            }
            else
            {
               //skip turn
               GameManager.instance.EndTurn();
                
            }
        }

    }

        IEnumerator WaitToEndAction(float time)
        {
            Debug.Log("Waiting To End Action");
            yield return new WaitForSeconds(time);
            GameManager.instance.SpendTurnPoints();
        }

    IEnumerator WaitToShoot()
    {
        yield return  new WaitForSeconds(.5f);
        CharCon.FireShot();
        GameManager.instance.CurrentActionCost = 1;

        StartCoroutine(WaitToEndAction(WaitAfterActing));
    }
}
