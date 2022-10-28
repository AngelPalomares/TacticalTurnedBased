using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControler : MonoBehaviour
{
    public float MoveSpeed;
    public Vector3 MoveTarget;
    // Start is called before the first frame update
    void Start()
    {
        MoveTarget = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, MoveTarget, MoveSpeed * Time.deltaTime);
    }

    public void MoveToPoint(Vector3 pointtomove)
    {
        MoveTarget = pointtomove;
    }
}
