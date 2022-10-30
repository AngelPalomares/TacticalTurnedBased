using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInputMenu : MonoBehaviour
{
    public static PlayerInputMenu instance;
    
    public GameObject InputMenu,MoveMenu, MeleeButton,FiringMenu;

    public TMP_Text TurnPointText,ErrorText;

    public float errorDisplayTime = 2f;
    //[HideInInspector]
    public float errorCounter;

    public TMP_Text HitChanceText;

    private void Awake()
    {
        instance = this;
    }
    public void HideMenus()
    {
        InputMenu.SetActive(false);
        MoveMenu.SetActive(false);
        MeleeButton.SetActive(false);
        FiringMenu.SetActive(false);
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

    public void ShowFiringMenu()
    {
        HideMenus();
        FiringMenu.SetActive(true);

        UpdateHitChance();
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

    public void HideFiringMenu()
    {
        HideMenus();
        ShowInputMenu();

        GameManager.instance.TargetDisplay.SetActive(false);

        CameraController.instance.SetMoveTarget(GameManager.instance.ActivePlayer.transform.position);
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
            GameManager.instance.ActivePlayer.LookAtTarget(GameManager.instance.ActivePlayer.meleeTargets[GameManager.instance.ActivePlayer.CurrentMeleeTarget].transform);
        }
        else
        {
            ShowErrorText("No Enemies near melee range");
        }
    }


    public void MeleeHit()
    {
        GameManager.instance.ActivePlayer.DoMelee();
        GameManager.instance.CurrentActionCost = 1;

        HideMenus();
        //GameManager.instance.SpendTurnPoints();

        GameManager.instance.TargetDisplay.SetActive(false);
        StartCoroutine(WaitToEndActionCo(1f));
    }

    public IEnumerator WaitToEndActionCo(float TimeToWait)
    {
        yield return new WaitForSeconds(TimeToWait);

        GameManager.instance.SpendTurnPoints();
        CameraController.instance.SetMoveTarget(GameManager.instance.ActivePlayer.transform.position);
    }

    public void NextMeleeTarget()
    {
        GameManager.instance.ActivePlayer.CurrentMeleeTarget++;
        if(GameManager.instance.ActivePlayer.CurrentMeleeTarget >= GameManager.instance.ActivePlayer.meleeTargets.Count)
        {
            GameManager.instance.ActivePlayer.CurrentMeleeTarget = 0;
        }
        GameManager.instance.TargetDisplay.transform.position = GameManager.instance.ActivePlayer.meleeTargets[GameManager.instance.ActivePlayer.CurrentMeleeTarget].transform.position;

        GameManager.instance.ActivePlayer.LookAtTarget(GameManager.instance.ActivePlayer.meleeTargets[GameManager.instance.ActivePlayer.CurrentMeleeTarget].transform);
    }

    public void ShowErrorText(string MessageToShow)
    {
        ErrorText.text = MessageToShow;
        ErrorText.gameObject.SetActive(true);


        errorCounter = errorDisplayTime;
    }

    private void Update()
    {
        if(errorCounter > 0)
        {
            errorCounter -= Time.deltaTime;

            if(errorCounter <= 0)
            {
                ErrorText.gameObject.SetActive(false);
            }

        }
    }

    public void CheckShoot()
    {
        GameManager.instance.ActivePlayer.GetShootTarget();

        if(GameManager.instance.ActivePlayer.ShootTargets.Count > 0)
        {
            ShowFiringMenu();
            GameManager.instance.TargetDisplay.SetActive(true);
            GameManager.instance.TargetDisplay.transform.position = GameManager.instance.ActivePlayer.ShootTargets[GameManager.instance.ActivePlayer.currentShootTarget].transform.position;

            GameManager.instance.ActivePlayer.LookAtTarget(GameManager.instance.ActivePlayer.ShootTargets[GameManager.instance.ActivePlayer.currentShootTarget].transform);

            CameraController.instance.SetFireView();
        }
        else
        {
            ShowErrorText("No Enemies in Firing Range");
        }
    }

    public void NextShootTarget()
    {
        GameManager.instance.ActivePlayer.currentShootTarget++;
        if (GameManager.instance.ActivePlayer.currentShootTarget >= GameManager.instance.ActivePlayer.ShootTargets.Count)
        {
            GameManager.instance.ActivePlayer.currentShootTarget = 0;
        }
        GameManager.instance.TargetDisplay.transform.position = GameManager.instance.ActivePlayer.ShootTargets[GameManager.instance.ActivePlayer.currentShootTarget].transform.position;

        UpdateHitChance();
        GameManager.instance.ActivePlayer.LookAtTarget(GameManager.instance.ActivePlayer.ShootTargets[GameManager.instance.ActivePlayer.currentShootTarget].transform);
        CameraController.instance.SetFireView();
    }

    public void FireShot()
    {
        GameManager.instance.ActivePlayer.FireShot();

        GameManager.instance.CurrentActionCost = 1;
        HideMenus();

        GameManager.instance.TargetDisplay.SetActive(false);
        StartCoroutine(WaitToEndActionCo(1f));
    }

    public void UpdateHitChance()
    {
        float hitchance = Random.Range(50f, 95f);

        HitChanceText.text = "Chance To Hit: " + GameManager.instance.ActivePlayer.CheckShotChance().ToString("F1") + "%";
    }
}
