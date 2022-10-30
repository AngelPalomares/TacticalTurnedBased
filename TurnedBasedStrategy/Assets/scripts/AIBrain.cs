using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    public CharacterControler CharCon;

    public float WaitBeforeAttacking = 1f, WaitAfterActing = 1f;

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
            List<float> hitchances = new List<float>();
            for (int i = 0; i < CharCon.ShootTargets.Count; i++)
            {
                CharCon.currentShootTarget = i;
                CharCon.LookAtTarget(CharCon.ShootTargets[i].transform);
                hitchances.Add(CharCon.CheckShotChance());
            }
            float HighestChance = 0f;
            for(int i = 0; i < hitchances.Count; i++)
            {
                if(hitchances[i] > HighestChance)
                {
                    HighestChance = hitchances[i];
                    CharCon.currentShootTarget = i;
                }else if(hitchances[i] == HighestChance)
                {
                    if (Random.value > .5f)
                    {
                        CharCon.currentShootTarget = i;
                    }
                }
            }

            if(HighestChance > 0f)
            {
                CharCon.LookAtTarget(CharCon.ShootTargets[CharCon.currentShootTarget].transform);
                CameraController.instance.SetFireView();
                actiontaken = true;

                StartCoroutine(WaitToShoot());
            }
        }


            if (actiontaken == false)
            {
                //skip turn
                GameManager.instance.EndTurn();
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
