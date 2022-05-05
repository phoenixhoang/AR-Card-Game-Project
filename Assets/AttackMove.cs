using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Create New Move")]

public class AttackMove : ScriptableObject
{
    // Set attributes for attack move name and move power
    public string moveName;
    public int movePower;
}
