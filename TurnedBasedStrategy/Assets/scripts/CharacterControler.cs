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
    public AIBrain brain;

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

    public LineRenderer ShootLine;
    public float ShotRameinTime = .5f;
    private float ShotRemainCounter;

    public GameObject Defense;
    public bool isDefending;

    // Start is called before the first frame update
    void Start()
    {
        MoveTarget = transform.position;
        NavAgent.speed = MoveSpeed;
        CurrentHealth = MaxHealth;
        UpdateHealthDisplay();

        ShootLine.transform.position = Vector3.zero;

        ShootLine.transform.rotation = Quaternion.identity;

        ShootLine.transform.SetParent(null);
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
        if(ShotRemainCounter > 0)
        {
            ShotRemainCounter -= Time.deltaTime;

            if(ShotRemainCounter<= 0)
            {
                ShootLine.gameObject.SetActive(false);
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
        if(isDefending == true)
        {
            damageToTake *= .5f;
        }
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

        targetPoint.y = Random.Range(targetPoint.y, ShootTargets[currentShootTarget].transform.position.y + .25f);

        Vector3 targetOffset = new Vector3(Random.Range(-ShotMissedRange.x, ShotMissedRange.x),
            Random.Range(-ShotMissedRange.y, ShotMissedRange.y),
            Random.Range(-ShotMissedRange.z, ShotMissedRange.z));

        targetOffset = targetOffset * (Vector3.Distance(ShootTargets[currentShootTarget].transform.position, ShootPoint.position) / ShootRange);

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

            ShootLine.SetPosition(0, ShootPoint.position);
            ShootLine.SetPosition(1, hit.point);
        }
        else
        {
            Debug.Log(name + " Missed Target " + ShootTargets[currentShootTarget].name);
            PlayerInputMenu.instance.ShowErrorText("Shot Missed!");

            ShootLine.SetPosition(0, ShootPoint.position);
            ShootLine.SetPosition(1, ShootPoint.position + (ShootDirection * ShootRange));
        }

        ShootLine.gameObject.SetActive(true);
        ShotRemainCounter = ShotRameinTime;
    }

    public float CheckShotChance()
    {
        float shotChance = 0f;

        RaycastHit hit;

        Vector3 targetpoint = new Vector3(ShootTargets[currentShootTarget].transform.position.x, ShootTargets[currentShootTarget].transform.position.y, ShootTargets[currentShootTarget].transform.position.z);

        Vector3 ShootDirection = (targetpoint - ShootPoint.position).normalized;

        Debug.DrawRay(ShootPoint.position, ShootDirection * ShootRange, Color.red, 1f);
        if (Physics.Raycast(ShootPoint.position, ShootDirection, out hit, ShootRange))
        {
            if (hit.collider.gameObject == ShootTargets[currentShootTarget].gameObject)
            {
                shotChance += 50f;
            }
        }

        targetpoint.y = ShootTargets[currentShootTarget].transform.position.y + .25f;
        ShootDirection = (targetpoint - ShootPoint.position).normalized;

        Debug.DrawRay(ShootPoint.position, ShootDirection * ShootRange, Color.red, 1f);
        if (Physics.Raycast(ShootPoint.position, ShootDirection, out hit, ShootRange))
        {
            if (hit.collider.gameObject == ShootTargets[currentShootTarget].gameObject)
            {
                shotChance += 50f;
            }
        }

        shotChance = shotChance * .95f;
        shotChance *= 1f - (Vector3.Distance(ShootTargets[currentShootTarget].transform.position, ShootPoint.position) / ShootRange);
        return shotChance;
    }

    public void LookAtTarget(Transform Target)
    {
        transform.LookAt(new Vector3(Target.position.x, transform.position.y, Target.position.z), Vector3.up);
    }

    public void SetDefending(bool shouldDefend)
    {
        isDefending = shouldDefend;
        Defense.SetActive(isDefending);
    }
}
