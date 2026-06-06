using UnityEngine;

[System.Serializable]
public class MathOperation
{
    public string Id;
    public int Op1;  // First operand (RNG A)
    public int Op2;  // Second operand (RNG A)
    public string Op; // Operation type character (+ or -)
    public int Res;  // Computed correct answer
}

public class OperationGenerator : MonoBehaviour
{
    [Header("Spawner Reference Connection")]
    public OrbSpawner orbSpawner;

    [Header("Wave Management Settings")]
    public int totalOrbsForWave = 10;
    public string waveOperator = "+";

    public int level = 1;
    public static int waveNumber;


    void Start()
    {
        if (orbSpawner != null)
        {
            // Simply trigger the spawner to lay out the orbs. 
            // The spawner will handle requesting questions for each instance!
            orbSpawner.SpawnWaveOrbs(totalOrbsForWave);
        }
        else
        {
            Debug.LogError("Error: The OrbSpawner slot is empty on your OperationGenerator component!");
        }
    }

    // Easy mode handles single-digit values from 1 to 9 (per GDD page 12)
    public MathOperation GenerateQuestion()
    {
        MathOperation newOp = new MathOperation();
        newOp.Id = System.Guid.NewGuid().ToString();

        // Handle mixed operator logic for Wave 3 specifications
        string finalOp = "+";

        if(level == 1)
        {
            if(waveNumber == 1)
            {
                finalOp = "+";
            } 
            else if (waveNumber == 2)
            {
                finalOp = "-";
            } 
            else if (waveNumber == 3)
            {
                finalOp = Random.Range(0, 2) == 0 ? "+" : "-";
            }
        } 
        else if (level == 2)
        {
            if(waveNumber == 1)
            {
                finalOp = "*";
            } 
            else if (waveNumber == 2)
            {
                finalOp = "/";
            } 
            else if (waveNumber == 3)
            {
                finalOp = Random.Range(0, 2) == 0 ? "*" : "/";
            }
        } 
        else if (level == 3)
        {
            if(waveNumber == 1)
            {
                finalOp = Random.Range(0, 2) == 0 ? "+" : "-";
            } 
            else if (waveNumber == 2)
            {
                finalOp = Random.Range(0, 2) == 0 ? "*" : "/";
            } 
            else if (waveNumber == 3)
            {
                int rand = Random.Range(0, 4);
                if (rand == 0) finalOp = "+";
                else if (rand == 1) finalOp = "-";
                else if (rand == 2) finalOp = "*";
                else finalOp = "/";
            }
        }

        newOp.Op = finalOp;

        // Handle division independently to strictly enforce single-digit rules
        if (newOp.Op == "/")
        {
            int op1, op2, res;
            
            // Loop until we find a combination where Op1 is also a single digit (1-9)
            do
            {
                op2 = Random.Range(1, 10); // Divisor (1-9)
                res = Random.Range(1, 10); // Result (1-9)
                op1 = res * op2;           // Dividend
            } 
            while (op1 > 9); // Reject if Op1 spills into double digits (e.g. 4 * 3 = 12)

            newOp.Op1 = op1;
            newOp.Op2 = op2;
            newOp.Res = res;
        }
        // Default single-digit generation for +, -, and *
        else
        {
            newOp.Op1 = Random.Range(1, 10);
            newOp.Op2 = Random.Range(1, 10);

            if (newOp.Op == "+")
            {
                newOp.Res = newOp.Op1 + newOp.Op2;
            }
            else if (newOp.Op == "-")
            {
                if (newOp.Op1 < newOp.Op2)
                {
                    int temp = newOp.Op1;
                    newOp.Op1 = newOp.Op2;
                    newOp.Op2 = temp;
                }
                newOp.Res = newOp.Op1 - newOp.Op2;
            } 
            else if(newOp.Op == "*")
            {
                newOp.Res = newOp.Op1 * newOp.Op2;
            }
        }

        return newOp;
    }

    /// <summary>
    /// Updates the arithmetic symbol generator based on the active progression wave.
    /// </summary>
    public void SetupWaveOperations(int wave)
    {
        waveNumber = wave;
        if(level == 3)
        {
            waveOperator = "mix";
        
        }

        //// HANDSHAKE: Automatically instruct the spawner to spawn the fresh batch of Wave Orbs!
        //if (orbSpawner != null)
        //{
        //    orbSpawner.SpawnWaveOrbs(totalOrbsForWave);
        //}
    }
}