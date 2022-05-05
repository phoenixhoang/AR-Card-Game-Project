using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Create New Item")]

public class Item : ScriptableObject
{
    // Set attributes for item name, heal points and number of that specific item
    public string itemName;
    public int healPoints;
    public int numberOfItems;
}
