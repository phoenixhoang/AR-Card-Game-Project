using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon : MonoBehaviour
{
    // Set attributes for pokémon name, current hp and max hp
    public string pokemonName;
    public int maxHP, currentHP;

    // Set attributes for attack moves and items
    public List<AttackMove> AttackMoves;
    public List<Item> Items;
}
