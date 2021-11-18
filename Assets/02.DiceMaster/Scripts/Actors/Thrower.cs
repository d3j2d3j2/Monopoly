// Copyright Michele Pirovano 2014-2015
using UnityEngine;

namespace DiceMaster
{
    /// <summary>
    /// Attach this to any Dice to throw it in a given direction.
    /// Can be triggered, or can be configured to trigger at start.
    /// </summary>
    public class Thrower : MonoBehaviour
    {
        // Parameters
        public float throwStrength = 5;
        public bool triggerOnStart = true;
        public bool autoDestroy = true;

        public Vector3 direction = Vector3.up;
        public float randomDirectionOffset = 0.1f;

        // Internal references
        private Rigidbody targetRB;

        void Awake()
        {
            targetRB = GetComponent<Rigidbody>();
        }

        void Start()
        {
            if (triggerOnStart) Trigger();
        }
        
        void OnDrawGizmos()
        {
            if (!enabled) return;

            // A green line with a small sphere will tell you that a thrower is active
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + direction * throwStrength);
            Gizmos.DrawSphere(transform.position + direction * throwStrength, 0.1f);
        }

        /// <summary>
        /// Apply the throw force.
        /// </summary>
        /// <param name="newTargetRB">The rigidbody that is applied the force. By default this will use the RigidBody attached to the GameObject</param>
        public void Trigger(Rigidbody newTargetRB = null)
        {
            if (newTargetRB != null)
                this.targetRB = newTargetRB;

            var actualDir = direction;

            // Some randomization is useful to avoid always getting the same behaviour
            actualDir += new Vector3(Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)) * randomDirectionOffset;

            // Add the force
            if (targetRB) targetRB.AddForce(actualDir * throwStrength, ForceMode.Impulse);

            if (autoDestroy)
                Destroy(this);
        }

    }
}
