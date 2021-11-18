// Copyright Michele Pirovano 2014-2015
using UnityEngine;

namespace DiceMaster
{
    /// <summary>
    /// Attach this to any Dice to make it spin around a given axis.
    /// Can be triggered, or can be configured to trigger at start.
    /// </summary>
    public class Spinner : MonoBehaviour
    {
        // Parameters
        public float spinTorque = 5;
        public bool triggerOnStart = true;
        public bool autoDestroy = true;

        public Vector3 axis = Vector3.up;
        public float randomAxisOffset = 0.1f;

        // Internal references
        private Rigidbody targetRB;

        void Awake()
        {
            targetRB = GetComponent<Rigidbody>();
        }

        void Start()
        {
            //if (triggerOnStart) Trigger();
        }
        
        void OnDrawGizmos()
        {
            if (!enabled) return;

            // A wire sphere and a magenta line will tell you that a spinner is active
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, transform.position + axis* spinTorque);
        }

        /// <summary>
        /// Apply the spin torque.
        /// </summary>
        /// <param name="newTargetRB">The rigidbody that is applied the spin. By default this will use the RigidBody attached to the GameObject</param>
        public void Trigger(Rigidbody newTargetRB = null)
        {
            if (newTargetRB != null) 
                this.targetRB = newTargetRB;

            var actualAxis = axis;

            // Some randomization is useful to avoid always getting the same behaviour
            actualAxis += new Vector3(Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)) * randomAxisOffset;
            actualAxis.Normalize();

            // Add the torque
            if (targetRB) this.targetRB.AddTorque(actualAxis * spinTorque, ForceMode.Impulse);

            if (autoDestroy)
                Destroy(this);
        }

    }
}
