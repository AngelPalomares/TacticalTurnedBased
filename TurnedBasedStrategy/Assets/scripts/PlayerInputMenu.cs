using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInputMenu : MonoBehaviour
{
    public static PlayerInputMenu instance;
    
    public GameObject InputMenu,MoveMenu;

    public TMP_Text TurnPointText;

    private void Awake()
    {
        instance = this;
    }
    public void HideMenus()
    {
        InputMenu.SetActive(false);
        MoveMenu.SetActive(false);
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
}
