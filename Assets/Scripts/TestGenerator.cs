// using UnityEngine;

// public class TestGenerator : MonoBehaviour
// {
//     // Reference to the main generator script
//     private OperationGenerator opGenerator;

//     void Start()
//     {
//         // Find the generator attached to the same GameObject
//         opGenerator = GetComponent<OperationGenerator>();

//         if (opGenerator != null)
//         {
//             RunLevelOneTests();
//         }
//         else
//         {
//             Debug.LogError("OperationGenerator script not found! Make sure both scripts are on the same GameObject.");
//         }
//     }

//     void RunLevelOneTests()
//     {
//         Debug.Log("--- STARTING LEVEL 1 (ADD/SUB) TESTS ---");

//         // Simulate Wave 1: 10 Addition Orbs
//         Debug.Log("--- WAVE 1: ADDITION ---");
//         for (int i = 0; i < 10; i++)
//         {
//             PrintOrb(opGenerator.GenerateQuestion(), i + 1);
//         }

//         // Simulate Wave 2: 10 Subtraction Orbs
//         Debug.Log("--- WAVE 2: SUBTRACTION ---");
//         for (int i = 0; i < 10; i++)
//         {
//             PrintOrb(opGenerator.GenerateQuestion(), i + 1);
//         }
//     }

//     // A helper function to format the output nicely without referencing missing fields
//     void PrintOrb(MathOperation orb, int number)
//     {
//         if (orb == null) return;

//         // Clean GDD-compliant logs printing your operands and evaluated answers!
//         Debug.Log($"Orb {number} | ID: {orb.Id.Substring(0, 5)}... | " +
//                   $"Question: {orb.Op1} {orb.Op} {orb.Op2} = {orb.Res}");
//     }
// }