using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePoint : MonoBehaviour
{
    private void OnMouseDown()
    {
        //FindObjectOfType<CharacterControler>().MoveToPoint(transform.position);
        GameManager.instance.ActivePlayer.MoveToPoint(transform.position);

        MoveGrid.instance.HideMovePoints();
    }
}
