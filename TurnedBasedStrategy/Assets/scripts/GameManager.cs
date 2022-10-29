using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }
    //Active players
    public CharacterControler ActivePlayer;

    //creates list for all the characters
    public List<CharacterControler> allchars = new List<CharacterControler>();
    //creates a seperate list for the Player team as well as the enemy team
    public List<CharacterControler> Playertean = new List<CharacterControler>(), enemyteam = new List<CharacterControler>();
    //current character that is active
    private int CurrentChar;
    //Points that are used to determine the amount of actions you can do
    public int TotalTurnPoints = 2;
    [HideInInspector]
    public int TurnPointsRemaining;

    public int CurrentActionCost = 1;

    public GameObject TargetDisplay;

    // Start is called before the first frame update
    void Start()
    {
        //creates a temp list for randomization
        List<CharacterControler> templist = new List<CharacterControler>();
        templist.AddRange(FindObjectsOfType<CharacterControler>());

        int iterations = templist.Count + 50;
        //while loop to decide who goes first
        while(templist.Count > 0 && iterations > 0)
        {
            int RandomPick = Random.Range(0, templist.Count);
            allchars.Add(templist[RandomPick]);

            templist.RemoveAt(RandomPick);
            iterations--;
        }

        //Gets the character controller and determines if they are a player team or enemy team and puts them on the respected list
        foreach(CharacterControler cc in allchars)
        {
            if(cc.isEnemy == false)
            {
                Playertean.Add(cc);
            }
            else
            {
                enemyteam.Add(cc);
            }
        }
        allchars.Clear();

        if(Random.value >= .5f)
        {
            allchars.AddRange(Playertean);
            allchars.AddRange(enemyteam);
        }
        else
        {
            allchars.AddRange(enemyteam);
            allchars.AddRange(Playertean);
        }


        ActivePlayer = allchars[0];
        CameraController.instance.SetMoveTarget(ActivePlayer.transform.position);

        CurrentChar = -1;
        EndTurn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //ends the turn
    public void FinishedMovement()
    {
        SpendTurnPoints();
    }

    public void SpendTurnPoints()
    {
        TurnPointsRemaining -= CurrentActionCost;
        if(TurnPointsRemaining <= 0)
        {
            EndTurn();
        }else
        {
            if (ActivePlayer.isEnemy == false)
            {
                //MoveGrid.instance.ShowPointsInRange(ActivePlayer.moveRange, ActivePlayer.transform.position);

                PlayerInputMenu.instance.ShowInputMenu();
            }
            else
            {
                PlayerInputMenu.instance.HideMenus();
            }
        }
        PlayerInputMenu.instance.UpdateTurnPointText(TurnPointsRemaining);
    }
    //ends the turm and changes to the other player
    public void EndTurn()
    {
        CurrentChar++;
        if (CurrentChar >= allchars.Count)
        {
            CurrentChar = 0;
        }
        ActivePlayer = allchars[CurrentChar];

        CameraController.instance.SetMoveTarget(ActivePlayer.transform.position);

        TurnPointsRemaining = TotalTurnPoints;
        if(ActivePlayer.isEnemy == false)
        {
            //MoveGrid.instance.ShowPointsInRange(ActivePlayer.moveRange, ActivePlayer.transform.position);

            PlayerInputMenu.instance.ShowInputMenu();
            PlayerInputMenu.instance.TurnPointText.gameObject.SetActive(true);
        }
        else
        {
            PlayerInputMenu.instance.HideMenus();
            PlayerInputMenu.instance.TurnPointText.gameObject.SetActive(false);
            StartCoroutine(AISkipCo());
        }

        CurrentActionCost = 1;

        PlayerInputMenu.instance.UpdateTurnPointText(TurnPointsRemaining);
    }

    public IEnumerator AISkipCo()
    {
        yield return new WaitForSeconds(1f);
        EndTurn();
    }
        
}
