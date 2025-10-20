using System;
using System.Collections.Generic;
using UnityEngine;

public class AIStep : MonoBehaviour
{
    [SerializeField] GameObject gameManager;

    TicTacToe gameScript;

    void Start()
    {
        gameScript = gameManager.GetComponent<TicTacToe>();
    }
    public void DoAIStep()
    {
        // Read the board

        // Evaluate the info

        // Choose an action
        
    }
}
