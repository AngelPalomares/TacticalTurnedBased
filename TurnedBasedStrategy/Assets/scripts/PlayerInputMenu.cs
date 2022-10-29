using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInputMenu : MonoBehaviour
{
    public static PlayerInputMenu instance;
    
    public GameObject InputMenu,MoveMenu, MeleeButton;

    public TMP_Text TurnPointText;

    private void Awake()
    {
        instance = this;
    }
    public void HideMenus()
    {
        InputMenu.SetActive(false);
        MoveMenu.SetActive(false);
        MeleeButton.SetActive(false);
    }

    public void ShowInputMenu()
    {
        InputMenu.SetActive(true);
    }

    public void ShowMoveMenu()
    {
        HideMenus();
        MoveMenu.SetActive(true);
        ShowMove();
    }

    public void ShowMove()
    {
        if(GameManager.instance.TurnPointsRemaining >=1)
        {
            MoveGrid.instance.ShowPointsInRange(GameManager.instance.ActivePlayer.moveRange, GameManager.instance.ActivePlayer.transform.position);
            GameManager.instance.CurrentActionCost = 1;
        }
    }

    public void ShowRun()
    {
        if (GameManager.instance.TurnPointsRemaining >= 2)
        {
            MoveGrid.instance.ShowPointsInRange(GameManager.instance.ActivePlayer.RunRange, GameManager.instance.ActivePlayer.transform.position);
            GameManager.instance.CurrentActionCost = 2;
        }
    }

    public void HideMoveMenu()
    {
        HideMenus();
        MoveGrid.instance.HideMovePoints();
        ShowInputMenu();
    }

    public void UpdateTurnPointText(int turnpoints)
    {
        TurnPointText.text = "Turn Points Remaining: " + turnpoints;
    }

    public void SkipTurn()
    {
        GameManager.instance.EndTurn();
    }

    public void OpenMeleeMenu()
    {
        HideMenus();
        MeleeButton.SetActive(true);
        
    }
    public void CloseMeleeMenu()
    {
        HideMenus();
        ShowInputMenu();
        GameManager.instance.TargetDisplay.SetActive(false);
    }

    public void CheckMelee()
    {
        GameManager.instance.ActivePlayer.GetMeleeTargets();
        if(GameManager.instance.ActivePlayer.meleeTargets.Count > 0)
        {
            OpenMeleeMenu();

            GameManager.instance.TargetDisplay.SetActive(true);
            GameManager.instance.TargetDisplay.transform.position = GameManager.instance.ActivePlayer.meleeTargets[GameManager.instance.ActivePlayer.CurrentMeleeTarget].transform.position;
        }
        else
        {
            Debug.Log("No Enemies ner melee range");
        }
    }


    public void MeleeHit()
    {
        GameManager.instance.ActivePlayer.DoMelee();
        GameManager.instance.CurrentActionCost = 1;

        HideMenus();
        //GameManager.instance.SpendTurnPoints();

        StartCoroutine(WaitToEndActionCo(1f));
    }

    public IEnumerator WaitToEndActionCo(float TimeToWait)
    {
        yield return new WaitForSeconds(TimeToWait);

        GameManager.instance.SpendTurnPoints();
    }

    public void NextMeleeTarget()
    {
        GameManager.instance.ActivePlayer.CurrentMeleeTarget++;
        if(GameManager.instance.ActivePlayer.CurrentMeleeTarget >= GameManager.instance.ActivePlayer.meleeTargets.Count)
        {
            GameManager.instance.ActivePlayer.CurrentMeleeTarget = 0;
        }
        GameManager.instance.TargetDisplay.transform.position = GameManager.instance.ActivePlayer.meleeTargets[GameManager.instance.ActivePlayer.CurrentMeleeTarget].transform.position;
    }
}
