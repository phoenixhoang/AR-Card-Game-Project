using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusBar : MonoBehaviour
{
    // Set attributes for pokémon's name and hp bar
    public TextMeshProUGUI nameText;
    public Slider hpSlider;

    // Constructor for status bar
    public void SetStatusBar(Pokemon pokemon)
    {
        nameText.text = pokemon.pokemonName;
        hpSlider.maxValue = pokemon.maxHP;
        hpSlider.value = pokemon.currentHP;
    }

    // Setter for hp bar
    public void SetHP(int hp)
    {
        hpSlider.value = hp;
    }

    // Getter for HP bar
    public int GetHP()
    {
        return (int) hpSlider.value;
    }
}
