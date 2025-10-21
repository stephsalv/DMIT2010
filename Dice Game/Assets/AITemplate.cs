using System;
using System.Collections.Generic;
using System.Linq;
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

        gameManager.GetDiceValues(ref diceValues);

        //Reset
        Array.Clear(diceCount, 0, diceCount.Length);
        foreach (int value in diceValues)
        {
            if (value >= 0 && value < diceCount.Length)
            {
                diceCount[value]++;
            }
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
        bool smallStraightSelected = gameManager.IsComboSelected((int)GameManager.DiceCombos.SmallStraight);
        bool largeStraightSelected = gameManager.IsComboSelected((int)GameManager.DiceCombos.LargeStraight);
        bool twoPairSelected = gameManager.IsComboSelected((int)GameManager.DiceCombos.TwoPair);
        bool threeKindSelected = gameManager.IsComboSelected((int)GameManager.DiceCombos.ThreeKind);
        bool fourKindSelected = gameManager.IsComboSelected((int)GameManager.DiceCombos.FourKind);
        bool fullHouseSelected = gameManager.IsComboSelected((int)GameManager.DiceCombos.FullHouse);

        bool twoPairAvailable = (twoPair != -1);
        bool threeKindAvailable = (threeKind != -1);
        bool fourKindAvailable = (fourKind != -1);
        bool fullHouseAvailable = (fullHouse != -1);
        bool smallStraightAvailable = (smallStraight != -1);
        bool largeStraightAvailable = (largeStraight != -1);

        bool skipStraights = smallStraightSelected && largeStraightSelected;

        int[] sortedDistinct = diceValues.Distinct().OrderBy(x => x).ToArray();

        if (!skipStraights)
        {
            List<int> currentRun = new List<int>();
            List<int> longestRun = new List<int>();

            for (int i = 0; i < sortedDistinct.Length; i++)
            {
                if (currentRun.Count == 0 || sortedDistinct[i] == currentRun.Last() + 1)
                {
                    currentRun.Add(sortedDistinct[i]);
                }
                else
                {
                    currentRun.Clear();
                    currentRun.Add(sortedDistinct[i]);
                }

                if (currentRun.Count > longestRun.Count)
                    longestRun = new List<int>(currentRun);
            }
            if (longestRun.Count >= 5 && largeStraightAvailable && !largeStraightSelected)
            {
                KeepStraight(longestRun.Take(5).ToList());
                return;
            }
            if (longestRun.Count >= 4 && smallStraightAvailable && !smallStraightSelected)
            {
                KeepStraight(longestRun.Take(4).ToList());
                return;
            }
            if (longestRun.Count >= 3 && (!smallStraightSelected || !largeStraightSelected))
            {
                KeepStraight(longestRun.Take(3).ToList());
                return;
            }
        }

        var groups = diceValues.GroupBy(x => x)
                       .OrderByDescending(g => g.Count())
                       .ThenByDescending(g => g.Key)
                       .ToList();

        if (groups.Count == 0) return;

        bool skipPairs = fullHouseSelected && fourKindSelected && threeKindSelected && twoPairSelected;

        if (!skipPairs)
        {
            foreach (var group in groups)
            {
                int value = group.Key;
                int count = group.Count();

                // Four of a kind
                if (count == 4 && !fourKindSelected)
                {
                    KeepValue(value);
                    return;
                }

                // Full house
                if (count == 3 && groups.Count >= 2 && groups[1].Count() == 2 && !fullHouseSelected)
                {
                    KeepValue(value);
                    return;
                }

                // Three of a kind
                if (count == 3 && !threeKindSelected)
                {
                    if (!threeKindSelected || !fourKindSelected || !fullHouseSelected)
                    {
                        KeepValue(value);
                        return;
                    }
                }

                // Two pair
                if (count == 2 && groups.Count >= 2 && !twoPairSelected)
                {
                    KeepValue(value);
                    return;
                }

                // Single pair
                if (count == 2)
                {
                    // Keep if can have higher combo
                    if (!fullHouseSelected || !fourKindSelected || !threeKindSelected || !twoPairSelected)
                    {
                        KeepValue(value);
                        return;
                    }
                }
            }
        }
    }
    void KeepValue(int val)
    {
        for (int i = 0; i < diceValues.Length; i++)
        {
            if (diceValues[i] == val)
                gameManager.KeepDie(i);
        }
    }

    void KeepStraight(List<int> run)
    {
        bool[] keptFace = new bool[6];

        for (int i = 0; i < diceValues.Length; i++)
        {
            int face = diceValues[i];
            if (run.Contains(face) && !keptFace[face])
            {
                gameManager.KeepDie(i);
                keptFace[face] = true;
            }
        }
    }

    //void SelectBestCombo()
    //{
    //    int[] comboPriority = {
    //        (int)GameManager.DiceCombos.LargeStraight,
    //        (int)GameManager.DiceCombos.SmallStraight,
    //        (int)GameManager.DiceCombos.FullHouse,
    //        (int)GameManager.DiceCombos.FourKind,
    //        (int)GameManager.DiceCombos.ThreeKind,
    //        (int)GameManager.DiceCombos.TwoPair
    //    };

    //    foreach (int combo in comboPriority)
    //        for (int i = 0; i < diceValues.Length; i++)
    //        {
    //            if (!gameManager.IsComboSelected(combo))
    //            {
    //                gameManager.SelectCombo(combo);
    //                return;
    //            }
    //            if (diceValues[i] == val)
    //                gameManager.KeepDie(i);
    //        }
    //}

    void SelectBestCombo() //new code for John
    {
        if (largeStraight != -1 && !gameManager.IsComboSelected((int)GameManager.DiceCombos.LargeStraight))
        {
            gameManager.SelectCombo((int)GameManager.DiceCombos.LargeStraight);
        }

        else if (smallStraight != -1 && !gameManager.IsComboSelected((int)GameManager.DiceCombos.SmallStraight))
        {
            gameManager.SelectCombo((int)GameManager.DiceCombos.SmallStraight);
        }
        else if (fullHouse != -1 && !gameManager.IsComboSelected((int)GameManager.DiceCombos.FullHouse))
        {
            gameManager.SelectCombo((int)GameManager.DiceCombos.FullHouse);
        }
        else if (fourKind != -1 && !gameManager.IsComboSelected((int)GameManager.DiceCombos.FourKind))
        {
            gameManager.SelectCombo((int)GameManager.DiceCombos.FourKind);
        }
        else if (threeKind != -1 && !gameManager.IsComboSelected((int)GameManager.DiceCombos.ThreeKind))
        {
            gameManager.SelectCombo((int)GameManager.DiceCombos.ThreeKind);
        }
        else if (twoPair != -1 && !gameManager.IsComboSelected((int)GameManager.DiceCombos.TwoPair))
        {
            gameManager.SelectCombo((int)GameManager.DiceCombos.TwoPair);
        }
    }
}