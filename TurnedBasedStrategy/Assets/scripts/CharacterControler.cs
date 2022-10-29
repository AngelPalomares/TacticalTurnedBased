using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;

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

    public float MaxHealth = 10f;
    
    public float CurrentHealth;

    public float MeleeDamage = 5f;

    public TMP_Text healthText;
    public Slider HealthSlider;

    //All this is the shooting Mechanincs for the game
    public float ShootRange,ShootDamage;
    [HideInInspector]
    public List<CharacterControler> ShootTargets = new List<CharacterControler>();
    [HideInInspector]
    public int currentShootTarget;

    //shooting point
    public Transform ShootPoint;

    public Vector3 ShotMissedRange;


    // Start is called before the first frame update
    void Start()
    {
        MoveTarget = transform.position;
        NavAgent.speed = MoveSpeed;
        CurrentHealth = MaxHealth;
        UpdateHealthDisplay();
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
        //meleeTargets[CurrentMeleeTarget].gameObject.SetActive(false);
        meleeTargets[CurrentMeleeTarget].TakeDamage(MeleeDamage);
    }

    public void TakeDamage(float damageToTake)
    {
        CurrentHealth -= damageToTake;

        if(CurrentHealth <= 0)
        {
            CurrentHealth = 0;

            NavAgent.enabled = false;

            transform.rotation = Quaternion.Euler(-70f, transform.rotation.eulerAngles.y, 0f);

            GameManager.instance.allchars.Remove(this);
            if(GameManager.instance.Playertean.Contains(this))
            {
                GameManager.instance.Playertean.Remove(this);
            }
            if (GameManager.instance.enemyteam.Contains(this))
            {
                GameManager.instance.enemyteam.Remove(this);
            }
        }

        UpdateHealthDisplay();
    }

    public void UpdateHealthDisplay()
    {
        healthText.text = "HP: " + CurrentHealth + "/" + MaxHealth;

        HealthSlider.maxValue = MaxHealth;
        HealthSlider.value = CurrentHealth;
    }

    public void GetShootTarget()
    {
        ShootTargets.Clear();

        if (isEnemy == false)
        {
            foreach (CharacterControler cc in GameManager.instance.enemyteam)
            {
                if (Vector3.Distance(transform.position, cc.transform.position) < ShootRange)
                {
                    ShootTargets.Add(cc);
                }
            }
        } else
        {
            foreach (CharacterControler cc in GameManager.instance.Playertean)
            {
                if (Vector3.Distance(transform.position, cc.transform.position) < ShootRange)
                {
                    ShootTargets.Add(cc);
                }
            }
        }
        if (currentShootTarget >= ShootTargets.Count)
        {
            currentShootTarget = 0;
        }
    }

    public void FireShot()
    {
        Vector3 targetPoint = new Vector3(ShootTargets[currentShootTarget].transform.position.x, ShootTargets[currentShootTarget].transform.position.y, ShootTargets[currentShootTarget].transform.position.z);

        Vector3 targetOffset = new Vector3(Random.Range(-ShotMissedRange.x, ShotMissedRange.x),
            Random.Range(-ShotMissedRange.y, ShotMissedRange.y),
            Random.Range(-ShotMissedRange.z, ShotMissedRange.z));

        targetOffset = targetOffset * (Vector3.Distance(targetPoint, ShootPoint.position) / ShootRange);

        targetPoint += targetOffset;

        Vector3 ShootDirection = (targetPoint - ShootPoint.position).normalized;

        Debug.DrawRay(ShootPoint.position, ShootDirection * ShootRange, Color.blue, 1f);

        RaycastHit hit;

        if(Physics.Raycast(ShootPoint.position,ShootDirection, out hit, ShootRange))
        {
            if(hit.collider.gameObject == ShootTargets[currentShootTarget].gameObject)
            {
                Debug.Log(name + "Shot Target " + ShootTargets[currentShootTarget].name);
                ShootTargets[currentShootTarget].TakeDamage(ShootDamage);
            }
            else
            {
                Debug.Log(name + " Missed Target " + ShootTargets[currentShootTarget].name);
                PlayerInputMenu.instance.ShowErrorText("Shot Missed!");
            }
        }
        else
        {
            Debug.Log(name + " Missed Target " + ShootTargets[currentShootTarget].name);
            PlayerInputMenu.instance.ShowErrorText("Shot Missed!");
        }
    }
}
