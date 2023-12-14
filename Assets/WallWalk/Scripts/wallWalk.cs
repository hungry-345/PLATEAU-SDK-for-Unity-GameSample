using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallWalk : MonoBehaviour
{

    private LayerMask wallAble;
    public bool wallChecked = false;
    public float wallRadius = 0.28f;
    
    public void wallCheck()
    {
        Vector3 sperePosition = new Vector3(transform.position.x,transform.position.y,transform.position.z);
        wallChecked = Physics.CheckSphere(sperePosition,wallRadius, wallAble,QueryTriggerInteraction.Ignore);
    }

    public void wallMove()
    {
        if (!wallChecked)
        {

        }
    }
}
