// Copyright Michele Pirovano 2014-2015
using UnityEngine;
using UnityEngine.Events;

namespace DiceMaster
{
    /// <summary>
    /// Attach this to any GameObject to make it spawn dice.
    /// This will spawn dice in a grid when triggered.
    /// Will also automatically work with any Thrower or Spinner.
    /// </summary>
    public class Spawner : MonoBehaviour
    {
        // Parameters
        public GameObject dicePrefab;
        public bool triggerOnStart = true;
        public int nSpawn = 1;
        public int span = 5;
        public bool randomizeInitialRotation = true;

        // References
        Thrower thrower;
        Spinner spinner;

        // Events
        [System.Serializable]
        public class SpawnDiceEvent : UnityEvent<Dice> { }
        public SpawnDiceEvent onSpawnDice;

        void Start()
        {
            if (onSpawnDice == null)
                onSpawnDice = new SpawnDiceEvent();

            if (triggerOnStart)
                Trigger();
        }

        void OnDrawGizmos()
        {
            if (!thrower) this.thrower = GetComponent<Thrower>();
            if (!spinner) this.spinner = GetComponent<Spinner>();

            for (int i = 0; i < nSpawn; i++)
            {
                var p = GetSpawnPosition(i);
                Gizmos.color = Color.yellow;
                Gizmos.DrawIcon(p, "DiceIcon.png", true);

                // Also add thrower/spinner stuff too
                Gizmos.color = Color.green;
                if (thrower && thrower.enabled)
                    Gizmos.DrawLine(p, p + thrower.throwStrength * thrower.direction);

                Gizmos.color = Color.magenta;
                if (spinner && spinner.enabled)
                    Gizmos.DrawLine(p, p + spinner.spinTorque * spinner.axis);
            }

        }

        /// <summary>
        /// Gets the spawn position for a given dice in the array
        /// Virtual, so it can be extended with different spawn rules
        /// </summary>
        /// <param name="index">index of the dice to spawn in the array</param>
        /// <returns>The position the dice should be spawned at</returns>
        public virtual Vector3 GetSpawnPosition(int index)
        {
            // Spawn in a grid
            int rowSize = 6;
            return transform.position + Vector3.forward * span * (index / rowSize)
                    + Vector3.right * span * (index % rowSize);
        }

        /// <summary>
        /// Called when we want to spawn new dice
        /// </summary>
        public void Trigger()
        {
            // Check if we have throwers or spinners
            var thrower = GetComponent<Thrower>();
            if (thrower) thrower.autoDestroy = false;

            var spinner = GetComponent<Spinner>();
            if (spinner) spinner.autoDestroy = false;

            // Spawn
            for (int i = 0; i < nSpawn; i++)
            {
                GameObject diceGo = GameObject.Instantiate(dicePrefab, GetSpawnPosition(i), Quaternion.identity) as GameObject;
                if (randomizeInitialRotation)
                    diceGo.transform.rotation = Random.rotationUniform;

                var rb = diceGo.GetComponent<Rigidbody>();

                if (thrower && thrower.enabled)
                    thrower.Trigger(rb);

                if (spinner && spinner.enabled)
                    spinner.Trigger(rb);

                if (onSpawnDice != null)
                    onSpawnDice.Invoke(diceGo.GetComponent<Dice>());
            }
        }
    }
}
