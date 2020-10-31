using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "AttackData_", menuName = "PTAZ/Battle/AttackData", order = 2)]
public class AttackData : Action
{
    [Header("Attack")]
    public int Power = 1;
    public int ChargedPower = 2;

    public int Cost = 0;
}
