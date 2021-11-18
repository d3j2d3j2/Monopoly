// Copyright Michele Pirovano 2014-2015
using UnityEngine;

namespace DiceMaster
{
    /// <summary>
    /// Will play a sound when a rigidbody hits something at a minimum speed
    /// </summary>
    public class OnHitSound : MonoBehaviour
    {

        public AudioSource audioSource;
        private static float hitThreshold = 0.002f;

        private static float minimum_pitch = 1.5f;
        private static float speed_factor = 200;

        void OnCollisionEnter(Collision other)
        {
            float sqrMagnitude = GetComponent<Rigidbody>().velocity.sqrMagnitude;
            if (sqrMagnitude > hitThreshold)
            {
                audioSource.pitch = minimum_pitch + (sqrMagnitude / speed_factor);
                audioSource.volume = 0.7f + (sqrMagnitude / speed_factor);
                audioSource.Play();
            }
        }
    }
}
