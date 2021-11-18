// Copyright Michele Pirovano 2014-2015
using UnityEngine;

namespace DiceMaster
{
    /// <summary>
    /// Simulates dice throws, when we do not need actual dice to be rolled.
    /// </summary>
    public class Simulation : MonoBehaviour
    {
        /// <summary>
        /// Simulates a single dice throw.
        /// </summary>
        /// <param name="numberOfFaces">The number of faces of the thrown dice.</param>
        /// <returns>The value returned (0,numberOfFaces]</returns>
        public static int SimulateThrow(int numberOfFaces)
        {
            return Random.Range(0, numberOfFaces);
        }

        /// <summary>
        /// Simulates a number of dice throws.
        /// </summary>
        /// <param name="numberOfFaces">Number of faces for each dice.</param>
        /// <param name="numberOfDice">Number of dice to throw.</param>
        /// <returns>An array with the returned values.</returns>
        public static int[] SimulateThrows(int numberOfFaces, int numberOfDice)
        {
            int[] numbers = new int[numberOfDice];
            for (int i = 0; i < numberOfDice; i++)
                numbers[i] = SimulateThrow(numberOfFaces);
            return numbers;
        }
    }
}
