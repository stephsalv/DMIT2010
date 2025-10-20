using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToeAI : MonoBehaviour
{
    int[] squares; // This is a list of all the values for the squares.
    public List<Button> buttons; // This is a list of the buttons for the players to press.
    public Text playerText; // This is the reference to the text field at the top.
    public Button replayButton; // This is the reference to the replay button.

    enum gameState { Player1Turn = 1, // This value allows you to add a line of squares to see which player won. 1+1+1 means player 1 has a line. 4+4+4 means player 2 has a line.
                     Player2Turn = 4,
                     Player1Win = 2,
                     Player2Win = 3,
                     Draw = 5};

    gameState myGameState;

    int player1AILevel;
    int player2AILevel;

    int rand;
    public int numberOfTurns;


    void Start ()
    {
        squares = new int[9];
        myGameState = gameState.Player1Turn;
        player1AILevel = 0;
        player2AILevel = 0;
        numberOfTurns = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        CheckForWin();

        switch (myGameState)
        {
            case gameState.Player1Turn:
                playerText.text = "Player 1 Turn";
                playerText.color = Color.green;

                if (player1AILevel > 0)
                {
                    AIPlayer();
                }
                break;
            case gameState.Player2Turn:
                playerText.text = "Player 2 Turn";
                playerText.color = Color.yellow;

                if (player2AILevel > 0)
                {
                    AIPlayer();
                }
                break;
            case gameState.Player1Win:
                playerText.text = "Player 1 Wins";
                playerText.color = Color.green;
                break;
            case gameState.Player2Win:
                playerText.text = "Player 2 Wins";
                playerText.color = Color.yellow;
                break;
            case gameState.Draw:
                playerText.text = "Draw";
                playerText.color = Color.blue;
                break;
            default:
                playerText.text = "Error";
                playerText.color = Color.red;
                break;
        }
	}

    public void SelectSquare(int buttonNum)
    {
        // Try to set the value of the square to the integer value of myGameState. If the selection of the square is successful change the text of the button.
        if (SetSelection(buttonNum, (int)myGameState))
        {
            // Player 1 uses 'X' and player 2 uses 'O'
            if (myGameState == gameState.Player1Turn)
            {
                buttons[buttonNum].GetComponentInChildren<Text>().text = "X";
                myGameState = gameState.Player2Turn;
            }
            else
            {
                buttons[buttonNum].GetComponentInChildren<Text>().text = "O";
                myGameState = gameState.Player1Turn;
            }
            // Do not let the button be used again if it was selected.
            buttons[buttonNum].interactable = false;
            numberOfTurns++;
        }
    }

    public bool SetSelection(int index, int value)
    {
        bool valueSet = false;

        // This is just an extra check to make sure the button can only be pressed if not already selected.
        if (squares[index] == 0)
        {
            squares[index] = value;
            valueSet = true;
        }

        return valueSet;
    }

    void ResetSquares()
    {
        // Set all square values to 0, allow all buttons to be selected, clear the text on the buttons.
        for (int i = 0; i < squares.Length; i++)
        {
            squares[i] = 0;
            buttons[i].interactable = true;
            buttons[i].GetComponentInChildren<Text>().text = "";
        }
        // Turn off the replay button.
        replayButton.gameObject.SetActive(false);
        // Set myGameState to player1Turn
        myGameState = gameState.Player1Turn;
        numberOfTurns = 0;

    }

    void DisableSquares()
    {
        // Disable all the buttons.
        for (int i = 0; i < squares.Length; i++)
        {
            buttons[i].interactable = false;
        }
        replayButton.gameObject.SetActive(true);
    }

    public void PlayAgain()
    {
        // Reset the game.
        ResetSquares();
    }

    void CheckForWin()
    {
        bool draw = true;

        // If any line of squares add up to 3 then player 1 has won.
        if (squares[0] + squares[1] + squares[2] == 3 ||
            squares[3] + squares[4] + squares[5] == 3 ||
            squares[6] + squares[7] + squares[8] == 3 ||
            squares[0] + squares[3] + squares[6] == 3 ||
            squares[1] + squares[4] + squares[7] == 3 ||
            squares[2] + squares[5] + squares[8] == 3 ||
            squares[0] + squares[4] + squares[8] == 3 ||
            squares[2] + squares[4] + squares[6] == 3)
        {
            myGameState = gameState.Player1Win;
            DisableSquares();
        }
        // If any line of squares add up to 12 then player 2 has won.
        else if (squares[0] + squares[1] + squares[2] == 12 ||
            squares[3] + squares[4] + squares[5] == 12 ||
            squares[6] + squares[7] + squares[8] == 12 ||
            squares[0] + squares[3] + squares[6] == 12 ||
            squares[1] + squares[4] + squares[7] == 12 ||
            squares[2] + squares[5] + squares[8] == 12 ||
            squares[0] + squares[4] + squares[8] == 12 ||
            squares[2] + squares[4] + squares[6] == 12)
        {
            myGameState = gameState.Player2Win;
            DisableSquares();
        }
        else
        {
            // If no player has won and all the squares have been selected then it is a draw.
            for (int i = 0; i < squares.Length && draw == true; i++)
            {
                if (squares[i] == 0)
                {
                    draw = false;
                }
            }
            if (draw)
            {
                myGameState = gameState.Draw;
                DisableSquares();
            }
        }
    }

    public void SetPlayer1AILevel(int level)
    {
        player1AILevel = level;
    }

    public void SetPlayer2AILevel(int level)
    {
        player2AILevel = level;
    }

    void AIPlayer()
    {
        bool selected = false; // This is set to true when the AI selects a square.
        int playerValue; // This is the value used by player 1. (1)
        int opponentValue; // This is the value used by player 2. (4)

        // Set the player value and opponent value based on who's turn it is.
        if (myGameState == gameState.Player1Turn)
        {
            playerValue = (int)gameState.Player1Turn;
            opponentValue = (int)gameState.Player2Turn;
        }
        else
        {
            playerValue = (int)gameState.Player2Turn;
            opponentValue = (int)gameState.Player1Turn;
        }

        // If the AI level is 2 or higher then try to win.
        if (myGameState == gameState.Player1Turn && player1AILevel > 1 ||
            myGameState == gameState.Player2Turn && player2AILevel > 1 &&
            !selected)
        {
            #region Check for win
            // If the current player has two of the bottom row selected then select the remaining square.
            if (squares[0] + squares[1] + squares[2] == playerValue * 2)
            {
                SelectSquare(0);
                SelectSquare(1);
                SelectSquare(2);
                selected = true;
            }
            // If the current player has two of the middle row selected then select the remaining square.
            else if (squares[3] + squares[4] + squares[5] == playerValue * 2)
            {
                SelectSquare(3);
                SelectSquare(4);
                SelectSquare(5);
                selected = true;
            }
            // If the current player has two of the top row selected then select the remaining square.
            else if (squares[6] + squares[7] + squares[8] == playerValue * 2)
            {
                SelectSquare(6);
                SelectSquare(7);
                SelectSquare(8);
                selected = true;
            }
            // If the current player has two of the left column selected then select the remaining square.
            else if (squares[0] + squares[3] + squares[6] == playerValue * 2)
            {
                SelectSquare(0);
                SelectSquare(3);
                SelectSquare(6);
                selected = true;
            }
            // If the current player has two of the middle column selected then select the remaining square.
            else if (squares[1] + squares[4] + squares[7] == playerValue * 2)
            {
                SelectSquare(1);
                SelectSquare(4);
                SelectSquare(7);
                selected = true;
            }
            // If the current player has two of the right column selected then select the remaining square.
            else if (squares[2] + squares[5] + squares[8] == playerValue * 2)
            {
                SelectSquare(2);
                SelectSquare(5);
                SelectSquare(8);
                selected = true;
            }
            // If the current player has two of the first diagonal selected then select the remaining square.
            else if (squares[0] + squares[4] + squares[8] == playerValue * 2)
            {
                SelectSquare(0);
                SelectSquare(4);
                SelectSquare(8);
                selected = true;
            }
            // If the current player has two of the second diagonal selected then select the remaining square.
            else if (squares[2] + squares[4] + squares[6] == playerValue * 2)
            {
                SelectSquare(2);
                SelectSquare(4);
                SelectSquare(6);
                selected = true;
            }
            #endregion
        }

        // If the AI level is 4 and Player1Turn then try and force a win.
        if (myGameState == gameState.Player1Turn && player1AILevel > 3 &&
            !selected)
        {
            #region Check for numberOfTurns

            switch (numberOfTurns)
            {
                case 0:
                    Debug.Log("first");
                    SelectSquare(0);
                    selected = true;
                    break;
                case 1:
                    break;
                case 2:
                    Debug.Log("second");
                    if (squares[1] == (int)gameState.Player2Turn)
                    {
                        SelectSquare(4);
                    }
                    else if (squares[2] == (int)gameState.Player2Turn)
                    {
                        SelectSquare(8);
                    }
                    else if (squares[3] == (int)gameState.Player2Turn)
                    {
                        SelectSquare(4);
                    }
                    else if (squares[4] == (int)gameState.Player2Turn)
                    {
                        SelectSquare(8);
                    }
                    else if (squares[5] == (int)gameState.Player2Turn)
                    {
                        SelectSquare(4);
                    }
                    else if (squares[6] == (int)gameState.Player2Turn)
                    {
                        SelectSquare(8);
                    }
                    else if (squares[7] == (int)gameState.Player2Turn)
                    {
                        SelectSquare(4);
                    }
                    else if (squares[8] == (int)gameState.Player2Turn)
                    {
                        SelectSquare(6);
                    }
                    selected = true;
                    break;
                case 3:
                    break;
                case 4:
                    Debug.Log("third");
                    if (squares[1] == (int)gameState.Player2Turn && squares[4] == 0)
                    {
                        SelectSquare(6);
                        selected = true;

                    }
                    else if (squares[1] == (int)gameState.Player2Turn && squares[8] == (int)gameState.Player2Turn)
                    {
                        SelectSquare(6);
                        selected = true;
                    }
                    else if (squares[2] == (int)gameState.Player2Turn)
                    {
                        SelectSquare(6);
                        selected = true;

                    }
                    else if (squares[3] == (int)gameState.Player2Turn)
                    {
                        SelectSquare(2);
                        selected = true;

                    }
                    else if (squares[8] == (int)gameState.Player2Turn && squares[4] == 0)
                    {
                        SelectSquare(4);
                        selected = true;
                    }
                    break;
                default:
                    break;
            }

            #endregion
        }

        // If the AI level is 3 or higher then try to block.
        if ((myGameState == gameState.Player1Turn && player1AILevel > 2 ||
            myGameState == gameState.Player2Turn && player2AILevel > 2) &&
            !selected)
        {
            #region Check for block
            // If the opponent player has two of the bottom row selected then select the remaining square.
            if (squares[0] + squares[1] + squares[2] == opponentValue * 2)
            {
                SelectSquare(0);
                SelectSquare(1);
                SelectSquare(2);
                selected = true;
            }
            // If the opponent player has two of the middle row selected then select the remaining square.
            else if (squares[3] + squares[4] + squares[5] == opponentValue * 2)
            {
                SelectSquare(3);
                SelectSquare(4);
                SelectSquare(5);
                selected = true;
            }
            // If the opponent player has two of the top row selected then select the remaining square.
            else if (squares[6] + squares[7] + squares[8] == opponentValue * 2)
            {
                SelectSquare(6);
                SelectSquare(7);
                SelectSquare(8);
                selected = true;
            }
            // If the opponent player has two of the left column selected then select the remaining square.
            else if (squares[0] + squares[3] + squares[6] == opponentValue * 2)
            {
                SelectSquare(0);
                SelectSquare(3);
                SelectSquare(6);
                selected = true;
            }
            // If the opponent player has two of the middle column selected then select the remaining square.
            else if (squares[1] + squares[4] + squares[7] == opponentValue * 2)
            {
                SelectSquare(1);
                SelectSquare(4);
                SelectSquare(7);
                selected = true;
            }
            // If the opponent player has two of the right column selected then select the remaining square.
            else if (squares[2] + squares[5] + squares[8] == opponentValue * 2)
            {
                SelectSquare(2);
                SelectSquare(5);
                SelectSquare(8);
                selected = true;
            }
            // If the opponent player has two of the first diagonal selected then select the remaining square.
            else if (squares[0] + squares[4] + squares[8] == opponentValue * 2)
            {
                SelectSquare(0);
                SelectSquare(4);
                SelectSquare(8);
                selected = true;
            }
            // If the opponent player has two of the second diagonal selected then select the remaining square.
            else if (squares[2] + squares[4] + squares[6] == opponentValue * 2)
            {
                SelectSquare(2);
                SelectSquare(4);
                SelectSquare(6);
                selected = true;
            }
            #endregion
        }

        // If the AI level is 4 and Player2Turn then try to draw.
        if (myGameState == gameState.Player2Turn && player2AILevel > 3 &&
            !selected)
        {
            #region Check for numberOfTurns

            switch (numberOfTurns)
            {
                case 0:
                    break;
                case 1:
                    if (squares[4] == 0)
                    {
                        SelectSquare(4);
                    }
                    else
                    {
                        SelectSquare(0);
                    }
                    selected = true;
                    break;
                case 2:
                    break;
                case 3:
                    if (squares[4] == (int)gameState.Player2Turn) // Player 2 has the center
                    {
                        if (squares[1] + squares[3] + squares[5] + squares[7] == 0) // None of the side squares are selected
                        {
                            SelectSquare(1);
                            selected = true;
                        }
                        else if (squares[0] == (int)gameState.Player1Turn && squares[5] == (int)gameState.Player1Turn)
                        {
                            SelectSquare(2);
                            selected = true;
                        }
                        else if (squares[0] == (int)gameState.Player1Turn && squares[7] == (int)gameState.Player1Turn)
                        {
                            SelectSquare(6);
                            selected = true;
                        }
                        else if (squares[2] == (int)gameState.Player1Turn && squares[3] == (int)gameState.Player1Turn)
                        {
                            SelectSquare(0);
                            selected = true;
                        }
                        else if (squares[2] == (int)gameState.Player1Turn && squares[7] == (int)gameState.Player1Turn)
                        {
                            SelectSquare(8);
                            selected = true;
                        }
                        else if (squares[6] == (int)gameState.Player1Turn && squares[1] == (int)gameState.Player1Turn)
                        {
                            SelectSquare(0);
                            selected = true;
                        }
                        else if (squares[6] == (int)gameState.Player1Turn && squares[5] == (int)gameState.Player1Turn)
                        {
                            SelectSquare(8);
                            selected = true;
                        }
                        else if (squares[8] == (int)gameState.Player1Turn && squares[1] == (int)gameState.Player1Turn)
                        {
                            SelectSquare(2);
                            selected = true;
                        }
                        else if (squares[8] == (int)gameState.Player1Turn && squares[3] == (int)gameState.Player1Turn)
                        {
                            SelectSquare(6);
                            selected = true;
                        }
                    }
                    break;
                case 4:                    
                    break;
                default:
                    break;
            }

            #endregion
        }

        // If no square was selected then choose a random one until one is selected.
        while (!selected)
        {
            rand = Random.Range(0, 9);
            // Can the random button chosen be selected?
            if (buttons[rand].interactable)
            {
                SelectSquare(rand);
                selected = true;
            }
        }
    }
}
