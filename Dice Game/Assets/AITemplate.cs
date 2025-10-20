using System;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Your AI will need to access the GameManager script on the object in the scene.
// You will have access to the following methods:
// void RollDice(): This method with roll the dice for a couple seconds.
// bool IsRolling(): This method will return a bool that tells you if the dice are currently rolling.
// void SetComboActive(int index, bool state): This method will set the interactable state of a combo button at a specified index. It will only do this if the combo has not yet been selected.
// void SelectCombo(int index): This will try to select the combo by index. You can use the enum DiceCombos and cast it to an int. eg. (int)GameManager.DiceCombos.LargeStraight
// void KeepDie(int index): This will toggle the keep button at the index.
// void GetDiceValues(ref int[] values): This will point the array given to the diceValues in the GameManager.
// bool IsComboSelected(int index): This will return if the combo has been selected 


public class AITemplate : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    [SerializeField] AIStates currentState = AIStates.RollDice1;
    [SerializeField] Button aiButton;
    [SerializeField] int[] diceValues = new int[5];
    [SerializeField] int[] diceCount = new int[6];

    [SerializeField] int onePair, twoPair, threeKind, fourKind, fullHouse, smallStraight, largeStraight, numInStraight, currentRun;

    enum AIStates
    {
        RollDice1,
        EvaluateDice1,
        KeepDice,
        RollDice2,
        EvaluateDice2,
        SelectCombo
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.IsRolling() && aiButton.interactable == false)
        {
            CheckCombos();
            aiButton.interactable = true;
        }
    }

    public void TakeAIStep()
    {
        switch (currentState)
        {
            case AIStates.RollDice1:
                Roll();
                currentState = AIStates.EvaluateDice1;
                break;

            case AIStates.EvaluateDice1:
                CheckCombos();
                currentState = AIStates.KeepDice;
                break;

            case AIStates.KeepDice:
                KeepBestDice();
                currentState = AIStates.RollDice2;
                break;

            case AIStates.RollDice2:
                Roll();
                currentState = AIStates.EvaluateDice2;
                break;

            case AIStates.EvaluateDice2:
                CheckCombos();
                currentState = AIStates.SelectCombo;
                break;

            case AIStates.SelectCombo:
                SelectBestCombo();
                currentState = AIStates.RollDice1;
                break;

        }
    }
    void Roll()
    {
        gameManager.RollDice();
        aiButton.interactable = false;
    }

    void CheckCombos()
    {
        // Initialize combo variables
        onePair = -1;
        twoPair = -1;
        threeKind = -1;
        fourKind = -1;
        fullHouse = -1;
        smallStraight = -1;
        largeStraight = -1;
        numInStraight = 0;
        currentRun = 0;

        // Get dice values
        gameManager.GetDiceValues(ref diceValues);

        // Reset and count dice
        Array.Clear(diceCount, 0, diceCount.Length);
        foreach (int value in diceValues)
        {
            if (value >= 0 && value < diceCount.Length)
                diceCount[value]++;
        }

        // Track pairs and multiples
        int pairCount = 0;
        bool hasThree = false, hasFour = false;

        for (int i = 0; i < diceCount.Length; i++)
        {
            if (diceCount[i] == 2)
            {
                pairCount++;
                if (onePair == -1) onePair = i;
                else twoPair = i;
            }

            if (diceCount[i] == 3)
            {
                threeKind = i;
                hasThree = true;
            }

            if (diceCount[i] == 4)
            {
                fourKind = i;
                hasFour = true;
            }
        }

        // Activate combos based on findings
        if (pairCount >= 2)
            gameManager.SetComboActive((int)GameManager.DiceCombos.TwoPair, true);

        if (hasThree)
            gameManager.SetComboActive((int)GameManager.DiceCombos.ThreeKind, true);

        if (hasFour)
            gameManager.SetComboActive((int)GameManager.DiceCombos.FourKind, true);

        if (hasThree && pairCount >= 1) //Look for full house
        {
            fullHouse = threeKind;
            gameManager.SetComboActive((int)GameManager.DiceCombos.FullHouse, true);
        }

        //Look for straights
        currentRun = 0;
        for (int i = 0; i < diceCount.Length; i++)
        {
            currentRun = (diceCount[i] > 0) ? currentRun + 1 : 0;

            if (currentRun >= 4)
            {
                smallStraight = 0;
                gameManager.SetComboActive((int)GameManager.DiceCombos.SmallStraight, true);
            }

            if (currentRun == 5)
            {
                largeStraight = 0;
                gameManager.SetComboActive((int)GameManager.DiceCombos.LargeStraight, true);
            }
        }
    }

    void KeepBestDice()
    {

        // Priority: 4/3 of a kind > potential full house > straight potential > pair
        for (int i = 5; i >= 0; i--)
        {
            if (diceCount[i] >= 3)
            {
                KeepDiceWithValue(i + 1);
                return;
            }
        }

        if (HasStraightPotential())
        {
            KeepStraightDice();
            return;
        }

        for (int i = 5; i >= 0; i--)
        {
            if (diceCount[i] == 2)
            {
                KeepDiceWithValue(i + 1);
                return;
            }
        }

        // Default: keep highest single die
        int maxValue = 0;
        for (int i = 5; i >= 0; i--)
        {
            if (diceCount[i] > 0)
            {
                maxValue = i + 1;
                break;
            }
        }
        KeepDiceWithValue(maxValue);
    }
    void KeepDiceWithValue(int value)
    {
        for (int i = 0; i < diceValues.Length; i++)
        {
            if (diceValues[i] == value)
                gameManager.KeepDie(i);
        }
    }

    bool HasStraightPotential()
    {
        int run = 0;
        for (int i = 0; i < diceCount.Length; i++)
        {
            run = (diceCount[i] > 0) ? run + 1 : 0;
            if (run >= 3) return true;
        }
        return false;
    }

    void KeepStraightDice()
    {
        for (int i = 0; i < diceValues.Length; i++)
        {
            if (IsPartOfStraight(diceValues[i]))
                gameManager.KeepDie(i);
        }
    }

    bool IsPartOfStraight(int value)
    {
        int index = value - 1;
        return (index >= 0 && index < diceCount.Length && diceCount[index] > 0);
    }




    void SelectBestCombo()
   {
        int[] comboPriority = {
            (int)GameManager.DiceCombos.LargeStraight,
            (int)GameManager.DiceCombos.FullHouse,
            (int)GameManager.DiceCombos.FourKind,
            (int)GameManager.DiceCombos.SmallStraight,
            (int)GameManager.DiceCombos.ThreeKind,
            (int)GameManager.DiceCombos.TwoPair
        };

        foreach (int combo in comboPriority)
        {
            if (!gameManager.IsComboSelected(combo))
            {
                gameManager.SelectCombo(combo);
                return;
            }
        }

        // Fallback: pick any unused combo
        for (int i = 0; i < Enum.GetValues(typeof(GameManager.DiceCombos)).Length; i++)
        {
            if (!gameManager.IsComboSelected(i))
            {
                gameManager.SelectCombo(i);
                return;
            }
        }
    }
}