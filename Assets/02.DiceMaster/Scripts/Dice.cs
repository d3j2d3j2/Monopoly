// Copyright Michele Pirovano 2014-2016
using UnityEngine;
using UnityEngine.Events;

namespace DiceMaster
{
    /// <summary>
    /// Handles dice behaviour at runtime.
    /// </summary>
    public class Dice : MonoBehaviour
    {
        [System.Serializable]
        public class ShowNumberEvent : UnityEvent<int> { }

        // Parameters
        public DiceDefinition definition;
        public GameObject[] dynamicFaceGos;

        public bool verbose = false;
        public ShowNumberEvent onShowNumber;

        // State
        private Rigidbody rb;
        private bool triggerReadingFlag = false;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.transform.position = rb.transform.position + new Vector3(0, 10, 0);
            if (onShowNumber == null)
                onShowNumber = new ShowNumberEvent();

            if (DM.Config.overridenMaxAngularVelocity > 0)
                rb.maxAngularVelocity = DM.Config.overridenMaxAngularVelocity;

        }

        void Update()
        {
            if (rb.velocity.sqrMagnitude > (DM.Config.checkSpeedThreshold * DM.Config.checkSpeedThreshold) * 1.1f)
                triggerReadingFlag = true;

            if (triggerReadingFlag && rb.velocity.sqrMagnitude < (DM.Config.checkSpeedThreshold * DM.Config.checkSpeedThreshold)
                && rb.IsSleeping())
            {
                CheckShownNumber();
                triggerReadingFlag = false;
            }
        }

        /// <summary>
        /// Called to check what number is shown
        /// </summary>
        void CheckShownNumber()
        {
            float parallelTolerance = DM.Config.parallelTolerance;
            Vector3 global_show_direction = -Physics.gravity;

            var local_show_direction = transform.InverseTransformDirection(global_show_direction);
            local_show_direction.Normalize();

            if (definition.pipsAtVertices)
                local_show_direction *= -1;

            int shownNumber = DiceConfig.Instance.wrongFaceOutputValue;
            for (int i = 0; i < definition.faces.Length; i++)
            {
                var v = definition.faces[i].direction;
                float parallelValue = Vector3.Dot(local_show_direction, v);
                if (parallelValue > 1 - parallelTolerance)
                {
                    shownNumber = definition.faces[i].value;
                    break;
                }
            }

            if (onShowNumber != null) onShowNumber.Invoke(shownNumber);
            if (verbose) Debug.Log(this.name + " shown number: " + shownNumber);
        }

    }
}
