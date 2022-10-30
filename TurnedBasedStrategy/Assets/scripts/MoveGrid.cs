using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGrid : MonoBehaviour
{
    public static MoveGrid instance;
    public MovePoint StartPoint;
    public Vector2Int SpawnRage;

    public LayerMask WhatIsGround,WhatIsObstacle;

    public float ObstacleCheckRange;

    public List<MovePoint> allmovepoints = new List<MovePoint>();

    private void Awake()
    {
        instance = this;
        GenerateMoveGrid();
        HideMovePoints();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateMoveGrid()
    {
        for (int i = -SpawnRage.x; i <= SpawnRage.x; i++)
        {
            for (int j = -SpawnRage.y; j <= SpawnRage.y; j++)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position + new Vector3(i, 10f, j), Vector3.down, out hit, 20f, WhatIsGround))
                {
                    if (Physics.OverlapSphere(hit.point, ObstacleCheckRange, WhatIsObstacle).Length == 0)
                    {
                        MovePoint newPoint = Instantiate(StartPoint, hit.point, transform.rotation);
                        newPoint.transform.SetParent(transform);

                        allmovepoints.Add(newPoint);
                    }
                }
            }
        }
        StartPoint.gameObject.SetActive(false);
    }

    public void HideMovePoints()
    {
        foreach(MovePoint mp in allmovepoints)
        {
            mp.gameObject.SetActive(false);
        }
    }

    public void ShowPointsInRange(float MoveRange, Vector3 CenterPoint)
    {
        HideMovePoints();

        foreach(MovePoint mp in allmovepoints)
        {
            if(Vector3.Distance(CenterPoint,mp.transform.position) <=MoveRange)
            {
                mp.gameObject.SetActive(true);

                foreach(CharacterControler cc in GameManager.instance.allchars)
                {
                    if(Vector3.Distance(cc.transform.position, mp.transform.position) < .5f)
                    {
                        mp.gameObject.SetActive(false);
                    }
                }

            }
        }
    }

    public List<MovePoint>Getmovepointsinrange(float moverange, Vector3 centerpoint)
    {
        List<MovePoint> foundpoints = new List<MovePoint>();


        foreach (MovePoint mp in allmovepoints)
        {
            if (Vector3.Distance(centerpoint, mp.transform.position) <= moverange)
            {
                bool shouldAdd = true;
                //mp.gameObject.SetActive(true);

                foreach (CharacterControler cc in GameManager.instance.allchars)
                {
                    if (Vector3.Distance(cc.transform.position, mp.transform.position) < .5f)
                    {
                        shouldAdd = false;
                    }
                }

                if(shouldAdd == true)
                {
                    foundpoints.Add(mp);
                }

            }
        }

        return foundpoints;
    }
}
