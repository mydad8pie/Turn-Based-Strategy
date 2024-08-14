using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using TMPro;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public Button NextTurnButton;

    public TMP_Text turnText;

    // Number of players
    public int numberOfRealPlayers = 0; // number of extra players

    public int numberOfComputerPlayers = 1;

    private int totalPlayers;

    // Current player index
    public int currentPlayerIndex = 0;

    // Indicates if the player has completed their turn
    public bool playerHasCompletedTurn = false;

    public int turnCounter = 1; // starts at one since game starts on turn one

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        totalPlayers = numberOfRealPlayers + numberOfComputerPlayers;
        currentPlayerIndex = 0;
        // Start the turn routine to manage the turns
        StartCoroutine(TurnRoutine());
        NextTurnButton.onClick.AddListener(EndPlayerTurn);

        UpdateTurnText();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Tab) && currentPlayerIndex == 0 && !playerHasCompletedTurn && !PauseManager.Instance.IsPaused)
        {
            EndPlayerTurn();
        }
    }

    IEnumerator TurnRoutine()
    {
        while (true) // Loop forever to keep the game running
        {
            UpdateTurnText();

            if (currentPlayerIndex < numberOfRealPlayers) // Check if the current player is a human player
            {
                
                // Start the player turn
                StartPlayerTurn(currentPlayerIndex);

                // Wait until the player has completed their turn
                yield return new WaitUntil(() => playerHasCompletedTurn);
                // Reset the flag for the next turn
                playerHasCompletedTurn = false;
                
            }
            else
            {
                int computerPlayerIndex = currentPlayerIndex - numberOfRealPlayers + 1;
                
                StartComputerTurn(computerPlayerIndex);

                // Simulate time for computer turn
                yield return new WaitForSeconds(1f);
            }

            // Move to the next player
            currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;

            if (currentPlayerIndex == 0)
            {
                turnCounter++;
            }

            yield return null;
        }
    }

    void UpdateTurnText()
    {
        turnText.text = "Turn " + turnCounter;
    }

    // Function to start the player turn
    void StartPlayerTurn(int playerIndex)
    {
        Debug.Log("Starting turn "+ turnCounter + " for player " + currentPlayerIndex);

        UnitMovement[] playerUnits = FindObjectsOfType<UnitMovement>();

        foreach(UnitMovement unit in playerUnits){
            if (unit.IsPlayerControlled())
            {
                unit.ResetMoveRange();
            }
        }
        
    }

    // Function to start the computer turn
    void StartComputerTurn(int computerPlayerIndex)
    {
        Debug.Log("Starting turn " + turnCounter + " for computer" + computerPlayerIndex);
        ExecuteComputerActions(computerPlayerIndex);
    }

    // Function to execute the computer actions
    void ExecuteComputerActions(int computerPlayerIndex)
    {
        new WaitForSeconds(6f);
        Debug.Log("Computer " + computerPlayerIndex + " has completed turn "+ turnCounter);
    }

    // Function to end the player turn
    public void EndPlayerTurn()
    {
        if (currentPlayerIndex ==0 && !PauseManager.Instance.IsPaused)
        {
            Debug.Log("Player " + currentPlayerIndex + " turn " + turnCounter + " has ended");
            playerHasCompletedTurn = true;
        }
        else if (PauseManager.Instance.IsPaused)
        {
            Debug.Log("Game is paused");
        }
        else
        {
            Debug.Log("It is not your turn");
        }
    }
}
