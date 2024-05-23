using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

    // a public varable to store the number of players
    public int numberOfPlayers = 2;

    // a public variable to store the current player index
    private int currentPlayerIndex = 0;

    // a public variable to indicate if the player has completed their turn
    private bool playerHasCompletedTurn = false;


    void Start()
    {
        currentPlayerIndex = 0;
        //start the turn routine to manage the turns
        StartCoroutine(TurnRoutine());
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Tab) && currentPlayerIndex == 0 && !playerHasCompletedTurn)
        {
            EndPlayerTurn();
        }
    }

    IEnumerator TurnRoutine()
    {
        while (true) // loop forever to keep the game running
        {
            if(currentPlayerIndex == 0)// check if the current player is a human player
            {
                // Start the player turn
                StartPlayerTurn(currentPlayerIndex);

                // Wait until the player has completed their turn
                yield return new WaitUntil(() => playerHasCompletedTurn);
                // rest the flag for the nex turn
                playerHasCompletedTurn = false;
            }
            else
            {
                // Start the computer turn
                StartComputerTurn(currentPlayerIndex);

                // Simulate time for comuter turn
                yield return new WaitForSeconds(6f);
            }

            // move to the next player
            currentPlayerIndex = (currentPlayerIndex + 1) % numberOfPlayers;
        }

    }

    // a function to start the player turn
    void StartPlayerTurn(int playerIndex)
    {
        Debug.Log("Player " + playerIndex + " turn has started");
    }

    // a function to start the computer turn
    void StartComputerTurn(int playerIndex)
    {
        Debug.Log("Computer " + playerIndex + " turn has started");

        ExecuteComputerActions(playerIndex);
    }

    // a function to execute the computer actions
    void ExecuteComputerActions(int playerIndex){

        Debug.Log("Computer " + playerIndex + " Has completed their turn");
    }

    // a function to end the player turn
    public void EndPlayerTurn()
    {

        Debug.Log("Player " + currentPlayerIndex + " turn has ended");
        playerHasCompletedTurn = true;
    }


}
