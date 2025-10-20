using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIStep : MonoBehaviour
{
    [SerializeField] GameObject gameManager;

    TicTacToe gameScript;

    // Variables
    int[] squareValues;
    int winner;


    PlayerTurn playerTurn;
    public List<Button> buttons; // This is a list of the buttons for the players to press.

    void Start()
    {
        gameScript = gameManager.GetComponent<TicTacToe>();
    }
    // public void DoAIStep()
    // {
    //     // Read the board

    //     // Evaluate the info

    //     // Choose an action

    // }
    public void DoAIStep()
    {
        // Get current board and player info
        int[] board = new int[9];
        gameScript.ReadBoard(ref board);
        PlayerTurn currentPlayer = gameScript.GetPlayer();

        // Only act if it's the AI's turn
        if (currentPlayer != PlayerTurn.Player1) return;

        PlayerTurn opponent = PlayerTurn.Player2;
        bool moveMade = false;

        // Try to win
        if (!moveMade) moveMade = TryCompleteLine(board, currentPlayer);

        // Try to block opponent
        if (!moveMade) moveMade = TryCompleteLine(board, opponent);

        // Pick a random move
        if (!moveMade) moveMade = TryRandomMove(board);
    }
    bool TryCompleteLine(int[] board, PlayerTurn target)
    {
        int[,] lines = {
        {0,1,2}, {3,4,5}, {6,7,8},
        {0,3,6}, {1,4,7}, {2,5,8},
        {0,4,8}, {2,4,6}
    };

        int targetVal = (int)target;

        for (int i = 0; i < lines.GetLength(0); i++)
        {
            int a = lines[i, 0], b = lines[i, 1], c = lines[i, 2];
            int sum = board[a] + board[b] + board[c];

            if (sum == targetVal * 2)
            {
                if (board[a] == 0 && gameScript.MakeAIMove(a)) return true;
                if (board[b] == 0 && gameScript.MakeAIMove(b)) return true;
                if (board[c] == 0 && gameScript.MakeAIMove(c)) return true;
            }
        }
        return false;
    }

    bool TryRandomMove(int[] board)
    {
        List<int> emptyIndices = new List<int>();
        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] == 0) emptyIndices.Add(i);
        }

        if (emptyIndices.Count == 0) return false;

        int randomIndex = emptyIndices[UnityEngine.Random.Range(0, emptyIndices.Count)];
        return gameScript.MakeAIMove(randomIndex);
    }

}
