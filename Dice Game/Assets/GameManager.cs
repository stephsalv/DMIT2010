using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject keepPanel, rollButton;

    [SerializeField] GameObject[] dice, keepButtons, comboButtons;

    [SerializeField] Sprite[] diceSprites;

    [SerializeField] int[] diceValues;

    [SerializeField] bool diceRolling;

    [SerializeField] bool[] keepDice, comboSelected;

    float timeStamp, rollTime;

    int rollNumber;

    public enum DiceCombos
    {
        TwoPair,
        ThreeKind,
        FourKind,
        FullHouse,
        SmallStraight,
        LargeStraight
    }

    // Start is called before the first frame update
    void Start()
    {
        diceValues = new int[5];
        keepDice = new bool[5];
        comboSelected = new bool[6];
        rollTime = 2.0f;
        diceRolling = false;
        rollNumber = 0;
        keepPanel.SetActive(false);
        Screen.SetResolution(1280, 800, true);
    }

    // Update is called once per frame
    void Update()
    {
        #region Debug: Alter dice

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }


        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            diceValues[0]++;
            if (diceValues[0] == 6)
            {
                diceValues[0] = 0;
            }
            dice[0].GetComponent<Image>().sprite = diceSprites[diceValues[0]];
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            diceValues[1]++;
            if (diceValues[1] == 6)
            {
                diceValues[1] = 0;
            }
            dice[1].GetComponent<Image>().sprite = diceSprites[diceValues[1]];
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            diceValues[2]++;
            if (diceValues[2] == 6)
            {
                diceValues[2] = 0;
            }
            dice[2].GetComponent<Image>().sprite = diceSprites[diceValues[2]];
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            diceValues[3]++;
            if (diceValues[3] == 6)
            {
                diceValues[3] = 0;
            }
            dice[3].GetComponent<Image>().sprite = diceSprites[diceValues[3]];
        }
        if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            diceValues[4]++;
            if (diceValues[4] == 6)
            {
                diceValues[4] = 0;
            }
            dice[4].GetComponent<Image>().sprite = diceSprites[diceValues[4]];
        }
        if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            diceValues[5]++;
            if (diceValues[5] == 6)
            {
                diceValues[5] = 0;
            }
            dice[5].GetComponent<Image>().sprite = diceSprites[diceValues[5]];
        }
        #endregion

        if (diceRolling)
        {
            if (Time.time > timeStamp + rollTime)
            {
                diceRolling = false;
                rollButton.GetComponent<Button>().interactable = true;
                if (rollNumber == 1)
                {
                    keepPanel.SetActive(true);
                }
                if (rollNumber == 2)
                {
                    rollNumber = 0;
                    for (int i = 0; i < keepDice.Length; i++)
                    {
                        keepDice[i] = false;
                        keepButtons[i].GetComponentInChildren<TMP_Text>().text = "Roll";
                    }
                }
            }
            else
            {
                for (int i = 0; i < diceValues.Length; i++)
                {
                    if (!keepDice[i])
                    {
                        diceValues[i] = UnityEngine.Random.Range(0, 6);
                        dice[i].GetComponent<Image>().sprite = diceSprites[diceValues[i]];
                    }
                }
            }
        }
    }

    public void RollDice()
    {
        if (!diceRolling)
        {
            rollNumber++;
            diceRolling = true;
            timeStamp = Time.time;
            rollButton.GetComponent<Button>().interactable = false;
            keepPanel.SetActive(false);
        }
    }

    public void SetComboActive(int index, bool state)
    {
        if (!comboSelected[index])
        {
            // Set the active state of the combo buttons.
            comboButtons[index].GetComponent<Button>().interactable = state;
        }
    }

    public void SelectCombo(int index)
    {
        // Get the button component.
        Button button = comboButtons[index].GetComponent<Button>();

        // If there is a button component then make in not interactable and set its text to Done.
        if (button != null)
        {
            if (button.interactable)
            {
                button.interactable = false;
                button.GetComponentInChildren<TMP_Text>().text = "Done";
                comboSelected[index] = true;
                rollNumber = 0;
                keepPanel.SetActive(false);
            }
        }
    }

    public void KeepDie(int index)
    {
        if (keepDice[index])
        {
            keepDice[index] = false;
            keepButtons[index].GetComponentInChildren<TMP_Text>().text = "Roll";
        }
        else
        {
            keepDice[index] = true;
            keepButtons[index].GetComponentInChildren<TMP_Text>().text = "Keep";
        }
    }

    public void GetDiceValues(ref int[] values)
    {
        values = diceValues;
    }

    public bool IsComboSelected(int index)
    {
        return comboSelected[index];
    }

    public bool IsRolling()
    {
        return diceRolling;
    }

}
