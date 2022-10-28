using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGrid : MonoBehaviour
{
    public MovePoint StartPoint;
    public Vector2Int SpawnRage;

    public LayerMask WhatIsGround,WhatIsObstacle;

    public float ObstacleCheckRange;

    public List<MovePoint> allmovepoints = new List<MovePoint>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateMoveGrid();
        //HideMovePoints();
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
}
