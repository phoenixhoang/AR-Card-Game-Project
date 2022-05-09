using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Create enum for different game states
public enum GameState { START, PLAYERTURN, ENEMYTURN, WIN, LOSE}

public class Manager : MonoBehaviour
{
    // Enum Game State
    public GameState gameState;

    // UI Game Objects for the UI display
    public GameObject pokemonOptions, statusBars, battleMenu, 
        itemMenu, attackMenu, winMessage, loseMessage, ranAwayMessage;

    // Prefab Game Objects for Pikachu and Eevee
    public GameObject pikachuPrefab, eeveePrefab;

    // Create a player and enemy status bar
    public StatusBar playerStatusBar, enemyStatusBar;

    // Pokemon Game Object for retrieving component info
    Pokemon playerPokemon, pikachuPokemon;
    Pokemon enemyPokemon, eeveePokemon;

    // Array of attack and item options
    public Button [] attackOptions, itemOptions;

    // UI Text for pokemon names and the win or lose texts
    public TextMeshProUGUI firstPokemonNameText, secondPokemonNameText;
    public TextMeshProUGUI WinText, LoseText;

    // Float for player target health and enemy target health for HP bar animation
    float targetHealthPlayer, targetHealthEnemy, lerp;

    // Check if Pikachu and Eevee are tracked
    public bool pikachuTracked, eeveeTracked;

    // Different audio clips for different game states
    public AudioClip Menu, Battle, Win;

    // Audio source to play the track
    public AudioSource Track;

    public bool updateEnabled;

    // Getter and setter if Pikachu is tracked
    public bool PikachuTracked 
    { 
        get{ return pikachuTracked; } 
        set{ pikachuTracked = value; } 
    }

    // Getter and setter if Eevee is tracked
    public bool EeveeTracked 
    { 
        get{ return eeveeTracked; } 
        set{ eeveeTracked = value; }     
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set the game state to start and play the menu soundtrack
        gameState = GameState.START;
        Track = this.GetComponent<AudioSource>();
        Track.clip = Menu;
        Track.Play();

        updateEnabled = true;

        // Set up Pokemon prefab
        SetupPokemon();
    }

    // Update is called once per frame
    void Update()
    {
        // Only update when update is enabled
        if (updateEnabled == true)
        {
            // Check if player has chosen their Pokémon
            if (playerPokemon != null && enemyPokemon != null)
            {
                /* If the player pokémon's health is less then the target, 
                slowly update the player pokémon's health for animation implementation
                */
                if(targetHealthPlayer < playerPokemon.currentHP)
                {
                    // Increase the timer
                    lerp++;

                    // When the time is reached, adjust the health
                    if(lerp > 30)
                    {
                        playerPokemon.currentHP -= 1;
                        playerStatusBar.SetHP(playerPokemon.currentHP);
                        lerp = 0;
                    }
                }

                /* If the enemy pokémon's health is less then the target, 
                slowly update the enemy pokémon's health for animation implementation
                */
                if (targetHealthEnemy < enemyPokemon.currentHP)
                {
                    // Increase the timer
                    lerp++;

                    // When the time is reached, adjust the health
                    if (lerp > 30)
                    {
                        enemyPokemon.currentHP -= 1;
                        enemyStatusBar.SetHP(enemyPokemon.currentHP);
                        lerp = 0;
                        
                        // If the health is correct after slowly reducing it, start enemy's turn
                        if (targetHealthEnemy == enemyPokemon.currentHP)
                        {
                            StartCoroutine(enemyTurn());
                        }
                    }
                }
            }
        }
    }

    public void SetupPokemon()
    {
        // Get the Pikachu and Eevee pokemon prefab game object
        pikachuPokemon = pikachuPrefab.GetComponent<Pokemon>();
        eeveePokemon = eeveePrefab.GetComponent<Pokemon>();

        // Set pokémon name text
        firstPokemonNameText.text = pikachuPokemon.pokemonName;
        secondPokemonNameText.text = eeveePokemon.pokemonName;
    }

    void ChooseFirstPokemon()
    {
        // Set game state to player turn
        gameState = GameState.PLAYERTURN;

        // If the first pokémon was chosen, set the status bar and player pokémon to pikachu
        playerStatusBar.SetStatusBar(pikachuPokemon);
        playerPokemon = pikachuPokemon;
        targetHealthPlayer = pikachuPokemon.currentHP;

        // Set the status bar and enemy pokémon to eevee
        enemyStatusBar.SetStatusBar(eeveePokemon);
        enemyPokemon = eeveePokemon;
        targetHealthEnemy = eeveePokemon.currentHP;

        // Start the soundtrack coroutine
        StartCoroutine(WaitTimer());
        Track.clip = Battle;
        Track.Play();

        // Set the attack and item buttons then show the battle menu
        setAttackButtons();
        setItemButtons();
        ShowBattleMenu();
    }

    void ChooseSecondPokemon()
    {
        // Set game state to player turn
        gameState = GameState.PLAYERTURN;

        // If the second pokémon was chosen, set the status bar and player pokémon to eevee
        playerStatusBar.SetStatusBar(eeveePokemon);
        playerPokemon = eeveePokemon;
        targetHealthPlayer = eeveePokemon.currentHP;

        // Set the status bar and enemy pokémon to pikachu
        enemyStatusBar.SetStatusBar(pikachuPokemon);
        enemyPokemon = pikachuPokemon;
        targetHealthEnemy = pikachuPokemon.currentHP;

        // Start the soundtrack coroutine
        StartCoroutine(WaitTimer());
        Track.clip = Battle;
        Track.Play();

        // Set the attack and item buttons then show the battle menu
        setAttackButtons();
        setItemButtons();
        ShowBattleMenu();
    }

    IEnumerator WaitTimer()
    {
        // Wait for 3 seconds
        yield return new WaitForSeconds(3);
    }

    void setAttackButtons()
    {
        // Setting attack buttons by looping to check how many moves the pokémon has
        for (int i = 0; i < 4; i++)
        {
            // Check if the pokémon has more attack moves than current i in loop
            if (i < playerPokemon.AttackMoves.Count)
            {   
                // If true, add the attack option's move name
                attackOptions [i].GetComponentInChildren<TextMeshProUGUI>().text = 
                    playerPokemon.AttackMoves[i].moveName;
            }
            else
            {
                // Else remove the attack option button
                attackOptions [i].gameObject.SetActive(false);
            }
        }
    }

    public void attack(int attackChoice)
    {
        // If player's turn, player can attack the enemy
        if (gameState == GameState.PLAYERTURN)
        {
            // The amount of hp the enemy loses depends on the player's attack choice
            targetHealthEnemy = enemyPokemon.currentHP - playerPokemon.AttackMoves[attackChoice].movePower;
        
            // If the enemy pokémon's health is less than or equal to 0, the player wins the game
            if (targetHealthEnemy <= 0)
            {
                gameState = GameState.WIN;
                playerWin();
            }
            // Else game state is enemy's turn and hide the battle menu
            else
            {
                gameState = GameState.ENEMYTURN;
                HideBattleMenu();
            }
        }
    }

    IEnumerator enemyTurn()
    {
        // Random enemy attack is chosen
        int enemyRandomAtk = Random.Range(0, enemyPokemon.AttackMoves.Count - 1);

        // Enemy waits 2 seconds before attacking
        yield return new WaitForSeconds(2);

        // The player loses hp depending on the enemy's attack
        targetHealthPlayer = playerPokemon.currentHP - enemyPokemon.AttackMoves[enemyRandomAtk].movePower;

        // Wait for 2 seconds to show health bar animation before continuing
        yield return new WaitForSeconds(2);

        // If player pokémon's health is less than or equal to 0, the player loses the game 
        if (targetHealthPlayer <= 0)
        {
            gameState = GameState.LOSE;
            playerLose();
        }
        // Else game state is player's turn and show the battle menu again
        else
        {
            gameState = GameState.PLAYERTURN;
            ShowBattleMenu();
        }
    }

    void setItemButtons()
    {
        // Setting item buttons by looping to check how many moves the pokémon has
        for (int i = 0; i < 4; i++)
        {
            // Check if there are more items than current i in loop
            if (i < playerPokemon.Items.Count)
            {
                 // If true, add the item option's item name
                itemOptions [i].GetComponentInChildren<TextMeshProUGUI>().text = 
                    playerPokemon.Items[i].itemName;
            }

            else
            {
                // Else remove the item option button
                itemOptions [i].gameObject.SetActive(false);
            }
        }
    }

    public void item(int itemChoice)
    {
        // If the gamestate is player's turn
        if (gameState == GameState.PLAYERTURN)
        {
            // Heal the pokémon's hp depending on the item choice
            playerPokemon.currentHP += playerPokemon.Items[itemChoice].healPoints;

            // Decrease the amount of that item by 1
            playerPokemon.Items[itemChoice].numberOfItems -= 1;

            // If the player pokémon's currentHP is more than their max hp
            if (playerPokemon.currentHP > playerPokemon.maxHP)
            {
                // Set player pokémon's hp to max hp
                playerPokemon.currentHP = playerPokemon.maxHP;
            }

            // Set the new HP in the status bar
            playerStatusBar.SetHP(playerPokemon.currentHP);

            // If the player has run out of an item
            if (playerPokemon.Items[itemChoice].numberOfItems == 0)
            {
                // Remove the button of that specific item choice
                itemOptions [itemChoice].gameObject.SetActive(false);
            }

            // Set the game state to enemy's turn
            gameState = GameState.ENEMYTURN;

            // Hide the player's battle menu and start the enemy's turn
            HideBattleMenu();
            StartCoroutine(enemyTurn());
        }
    }

    // Player win soundtrack and shows the win message
    public void playerWin()
    {
        WinText.text = ("Congratulations!\nYou have won the battle against\n" 
            + enemyPokemon.pokemonName + "!");

        Track.clip = Win;
        Track.Play();
        ShowWinMessage();
        updateEnabled = false;
    }

    // Player lose stops the music and shows the lose message
    public void playerLose()
    {
        LoseText.text = ("Oh no!\nYou have lost the battle against\n" 
            + enemyPokemon.pokemonName 
            + "!\nTry thinking of a new strategy for next time!");

        Track.Stop();
        ShowLoseMessage();
        updateEnabled = false;
    }

    // Show options of which Pokémons the player can choose from
    public void ShowPokemonOptions()
    {
        ShowBlankUI();
        pokemonOptions.SetActive(true);
    }

    // Show the battle menu with the status bars
    public void ShowBattleMenu()
    {
        ShowBlankUI();
        statusBars.SetActive(true);
        battleMenu.SetActive(true);
    }

    // Hide the battle menu but show the status bars
    public void HideBattleMenu()
    {
        ShowBlankUI();
        statusBars.SetActive(true);
    }

    // Show the attack moves the player can choose from with the status bar
    public void ShowAttackMenu()
    {
        ShowBlankUI();
        statusBars.SetActive(true);
        attackMenu.SetActive(true);
    }

    // Show the items the player can choose from with the status bar
    public void ShowItemMenu()
    {
        ShowBlankUI();
        statusBars.SetActive(true);
        itemMenu.SetActive(true);
    }

    // Show a win message
    public void ShowWinMessage()
    {
        ShowBlankUI();
        winMessage.SetActive(true);
    }

    // Show a lose message
    public void ShowLoseMessage()
    {
        ShowBlankUI();
        loseMessage.SetActive(true);
    }

    // Show a ran away message
    public void ShowRanAwayMessage()
    {
        ShowBlankUI();
        ranAwayMessage.SetActive(true);
    }

    // Set all UI activity settings to false
    public void ShowBlankUI()
    {
        pokemonOptions.SetActive(false);
        statusBars.SetActive(false);
        battleMenu.SetActive(false);
        attackMenu.SetActive(false);
        itemMenu.SetActive(false);
        winMessage.SetActive(false);
        loseMessage.SetActive(false);
        ranAwayMessage.SetActive(false);
    }
}
