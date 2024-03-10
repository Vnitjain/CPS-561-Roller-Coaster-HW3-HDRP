using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyForceAndPlaySoundOnCollision : MonoBehaviour
{
    [SerializeField]
    private float forceMagnitude = 10f;
    [SerializeField]
    private float minimumSpeed = 1f;
    [SerializeField]
    private AudioClip loopSoundClip;

    private AudioSource audioSource;

    private void Start()
    {
        // Get or add an AudioSource component to the same GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // If there's no AudioSource component, add one and set the clip
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = loopSoundClip;
            audioSource.loop = true; // Set to loop the audio
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if the collision involves a Rigidbody component
        Rigidbody otherRigidbody = other.gameObject.GetComponent<Rigidbody>();

        if (otherRigidbody != null)
        {
            // Calculate the current speed of the object
            float currentSpeed = otherRigidbody.velocity.magnitude;

            // Check if the current speed is below the minimum
            if (currentSpeed < minimumSpeed)
            {
                // Apply force continuously as long as the object stays in the trigger
                otherRigidbody.AddForce(forceMagnitude * other.gameObject.transform.forward);

                // Check if the audio source is not playing, and if not, play the sound
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Stop playing the sound when the object exits the trigger zone
        audioSource.Stop();
    }
}
