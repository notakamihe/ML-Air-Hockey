using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Goal : MonoBehaviour
{
    public Team team;

    Field field;

    private void Start()
    {
        field = GetComponentInParent<Field>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Puck"))
        {
            field.ScoreGoal(team);
        }
    }
}