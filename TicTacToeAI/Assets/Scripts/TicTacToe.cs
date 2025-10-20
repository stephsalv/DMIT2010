using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour
{
    // UI elements
    [SerializeField] Button[] gameButtons;
    [SerializeField] Button startButton, aiButton;
    [SerializeField] TMP_Text winText;
    [SerializeField] GameObject volumePanel;

    // Variables
    int[] squareValues;

    PlayerTurn playerTurn;

    int winner;

    // Audio
    [SerializeField] AudioSource buttonAudio, voiceAudio;

    [SerializeField] AudioClip buttonPress, startVoice, xwins, owins, draw;

    [SerializeField] AudioMixer mixer;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (var button in gameButtons)
        {
            button.interactable = false;
        }
        aiButton.interactable = false;
        squareValues = new int[9];
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ExitGame();
        }
    }

    // Initialize the game
    public void StartGame()
    {
        for (int i = 0; i < gameButtons.Length; i++)
        {
            gameButtons[i].interactable = true;
            gameButtons[i].GetComponentInChildren<TMP_Text>().text = "";
            gameButtons[i].GetComponentInChildren<TMP_Text>().color = new Color(0.2f, 0.2f, 0.2f);
            squareValues[i] = 0;
        }

        aiButton.interactable = true;
        startButton.interactable = false;

        playerTurn = PlayerTurn.Player1;

        winner = 0;
        winText.text = "";

        voiceAudio.clip = startVoice;
        voiceAudio.Play();
    }

    void EndGame()
    {
        UpdateWinSquares();

        foreach (var button in gameButtons)
        {
            button.interactable = false;
        }

        aiButton.interactable = false;
        startButton.interactable = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // Try to select a square for the current player
    public void SelectSquare(int index)
    {
        if (squareValues[index] == 0)
        {
            squareValues[index] = (int)playerTurn;

            // Update the text of the selected square
            if (playerTurn == PlayerTurn.Player1)
            {
                gameButtons[index].GetComponentInChildren<TMP_Text>().text = "X";
                CheckForWin();
                playerTurn = PlayerTurn.Player2;
            }
            else
            {
                gameButtons[index].GetComponentInChildren<TMP_Text>().text = "O";
                CheckForWin();
                playerTurn = PlayerTurn.Player1;
            }
            gameButtons[index].interactable = false;
            buttonAudio.clip = buttonPress;
            buttonAudio.Play();
        }
    }

    public void ReadBoard(ref int[] values)
    {
        squareValues.CopyTo(values, 0);
    }

    public PlayerTurn GetPlayer()
    {
        return playerTurn;
    }

    void CheckForWin()
    {
        bool tie;

        if (squareValues[0] + squareValues[1] + squareValues[2] == (int)playerTurn * 3 ||
            squareValues[3] + squareValues[4] + squareValues[5] == (int)playerTurn * 3 ||
            squareValues[6] + squareValues[7] + squareValues[8] == (int)playerTurn * 3 ||
            squareValues[0] + squareValues[3] + squareValues[6] == (int)playerTurn * 3 ||
            squareValues[1] + squareValues[4] + squareValues[7] == (int)playerTurn * 3 ||
            squareValues[2] + squareValues[5] + squareValues[8] == (int)playerTurn * 3 ||
            squareValues[0] + squareValues[4] + squareValues[8] == (int)playerTurn * 3 ||
            squareValues[6] + squareValues[4] + squareValues[2] == (int)playerTurn * 3)
        {
            winner = (int)playerTurn;

            if (winner == (int)PlayerTurn.Player1)
            {
                winText.text = "X Wins";
                voiceAudio.clip = xwins;
                voiceAudio.Play();
            }
            else
            {
                winText.text = "O Wins";
                voiceAudio.clip = owins;
                voiceAudio.Play();
            }

            EndGame();
        }

        if (winner == 0)
        {
            tie = true;

            for (int i = 0; i < squareValues.Length; i++)
            {
                if (squareValues[i] == 0)
                {
                    tie = false;
                }
            }
            if (tie)
            {
                winText.text = "Draw";
                voiceAudio.clip = draw;
                voiceAudio.Play();
                EndGame();
            }
        }
    }

    void UpdateWinSquares()
    {
        if (squareValues[0] + squareValues[1] + squareValues[2] == (int)playerTurn * 3)
        {
            gameButtons[0].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            gameButtons[1].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            gameButtons[2].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
        }
        if (squareValues[3] + squareValues[4] + squareValues[5] == (int)playerTurn * 3)
        {
            gameButtons[3].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            gameButtons[4].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            gameButtons[5].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
        }
        if (squareValues[6] + squareValues[7] + squareValues[8] == (int)playerTurn * 3)
        {
            gameButtons[6].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            gameButtons[7].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            gameButtons[8].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
        }
        if (squareValues[0] + squareValues[3] + squareValues[6] == (int)playerTurn * 3)
        {
            gameButtons[0].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            gameButtons[3].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            gameButtons[6].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
        }
        if (squareValues[1] + squareValues[4] + squareValues[7] == (int)playerTurn * 3)
        {
            gameButtons[1].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            gameButtons[4].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            gameButtons[7].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
        }
        if (squareValues[2] + squareValues[5] + squareValues[8] == (int)playerTurn * 3)
        {
            gameButtons[2].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            gameButtons[5].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            gameButtons[8].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
        }
        if (squareValues[0] + squareValues[4] + squareValues[8] == (int)playerTurn * 3)
        {
            gameButtons[0].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            gameButtons[4].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            gameButtons[8].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
        }
        if (squareValues[2] + squareValues[4] + squareValues[6] == (int)playerTurn * 3)
        {
            gameButtons[2].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            gameButtons[4].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            gameButtons[6].GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
        }
    }

    public void ToggleVolumePanel()
    {
        volumePanel.SetActive(!volumePanel.activeSelf);
    }

    public void ChangeMasterVolume(float volume)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 40);
    }

    public void ChangeSFXVolume(float volume)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 40);
    }

    public void ChangeMusicVolume(float volume)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 40);
    }
}

public enum PlayerTurn
{
    Player1 = 1,
    Player2 = 4
}
