using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChainMathCourse : MonoBehaviour
{
    private const int StageCount = 5;

    [SerializeField] private TMP_Text[] questionLabels;
    [SerializeField] private ChainMathDoor[] doorSlots;
    [SerializeField] private GameObject[] stageBarriers;
    [SerializeField] private Transform resetPoint;

    private readonly ChainMathDoor[,] doors = new ChainMathDoor[StageCount, 3];
    private readonly int[] correctAnswers = new int[StageCount];
    private int currentStage;
    private int currentValue;

    private void Start()
    {
        if (questionLabels.Length != StageCount || stageBarriers == null || stageBarriers.Length != StageCount ||
            System.Array.Exists(stageBarriers, barrier => barrier == null) ||
            doorSlots.Length != StageCount * 3 || doors.GetLength(1) != 3)
        {
            Debug.LogError("Chain Math Course requires five stages and three doors per stage.", this);
            enabled = false;
            return;
        }

        for (int stage = 0; stage < StageCount; stage++)
            for (int slot = 0; slot < 3; slot++)
                doors[stage, slot] = doorSlots[stage * 3 + slot];

        GenerateChain();
    }

    public void SelectDoor(ChainMathDoor door, Transform player)
    {
        if (door == null || player == null || door.StageIndex != currentStage)
            return;

        if (!door.IsCorrect)
        {
            ResetCourse(player);
            return;
        }

        int correctAnswer = correctAnswers[currentStage];
        currentValue = correctAnswer;
        stageBarriers[currentStage].SetActive(false);
        currentStage++;
    }

    private void ResetCourse(Transform player)
    {
        currentStage = 0;
        GenerateChain();
        Teleport(player, resetPoint);
    }

    private void GenerateChain()
    {
        foreach (GameObject barrier in stageBarriers)
            barrier.SetActive(true);
        currentStage = 0;
        currentValue = Random.Range(2, 13);

        for (int stage = 0; stage < StageCount; stage++)
        {
            int left = currentValue;
            GenerateQuestion(left, out string symbol, out int operand, out int correctAnswer);
            correctAnswers[stage] = correctAnswer;
            questionLabels[stage].text = stage == 0
                ? $"<b>{left} {symbol} {operand} = ?</b>"
                : $"<b>{symbol} {operand}</b>";

            int correctSlot = Random.Range(0, 3);
            HashSet<int> choices = new() { correctAnswer };
            for (int slot = 0; slot < 3; slot++)
            {
                int answer = correctAnswer;
                if (slot != correctSlot)
                {
                    do
                    {
                        answer = Mathf.Max(1, correctAnswer + Random.Range(-10, 11));
                    } while (choices.Contains(answer));
                    choices.Add(answer);
                }
                doors[stage, slot].Configure(this, stage, slot == correctSlot, answer);
            }

            currentValue = correctAnswer;
        }

        currentValue = 0;
    }

    private static void GenerateQuestion(int value, out string symbol, out int operand, out int answer)
    {
        int operation = Random.Range(0, 4);
        if (operation == 1 && value <= 2)
            operation = 0;
        if (operation == 2 && value > 50)
            operation = 0;

        if (operation == 3)
        {
            List<int> divisors = new();
            for (int divisor = 2; divisor <= Mathf.Min(12, value); divisor++)
                if (value % divisor == 0)
                    divisors.Add(divisor);
            if (divisors.Count > 0)
            {
                operand = divisors[Random.Range(0, divisors.Count)];
                symbol = "÷";
                answer = value / operand;
                return;
            }
            operation = 0;
        }

        switch (operation)
        {
            case 1:
                operand = Random.Range(1, Mathf.Min(12, value - 1) + 1);
                symbol = "−";
                answer = value - operand;
                break;
            case 2:
                operand = Random.Range(2, 5);
                symbol = "×";
                answer = value * operand;
                break;
            default:
                operand = Random.Range(2, 13);
                symbol = "+";
                answer = value + operand;
                break;
        }
    }

    private static void Teleport(Transform player, Transform destination)
    {
        if (destination == null)
            return;

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;
        player.SetPositionAndRotation(destination.position, destination.rotation);
        if (controller != null)
            controller.enabled = true;
    }
}
