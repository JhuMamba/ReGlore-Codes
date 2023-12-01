using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobAudio : MonoBehaviour
{
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioSource weaponAudioSource;
    [SerializeField] private AudioSource hitAudioSource;
    public AudioClip[] footstepSounds;
    public AudioClip[] weaponSounds;
    public AudioClip[] hitSounds;
    public float footstepInterval = 0.5f; // Adjust the interval between footsteps.

    private Rigidbody rb;
    private float lastFootstepTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastFootstepTime = Time.time;
    }

    void Update()
    {
        // Check if the player is moving.
        if (rb.velocity.magnitude > 0.2f && Time.time - lastFootstepTime > footstepInterval)
        {
            // Play a random footstep sound.
            PlayFootStepSFX();

            // Update the last footstep time.
            lastFootstepTime = Time.time;
        }
    }

    private void PlayFootStepSFX()
    {
        AudioClip clip = footstepSounds[0];
        footstepAudioSource.clip = clip;
        footstepAudioSource.volume = Random.Range(.01f, .03f);
        footstepAudioSource.pitch = Random.Range(.3f, .5f);
        footstepAudioSource.PlayOneShot(clip);
    }
    private void PlayMobSwingSFX()
    {
        AudioClip clip = weaponSounds[Random.Range(0, weaponSounds.Length)];
        weaponAudioSource.clip = clip;
        weaponAudioSource.pitch = Random.Range(.8f, 1.2f);
        weaponAudioSource.Play();
    }
    public void PlayHitSFX()
    {
        AudioClip clip = hitSounds[Random.Range(0, hitSounds.Length)];
        hitAudioSource.volume = Random.Range(.5f, .8f);
        hitAudioSource.pitch = Random.Range(.8f, 1.2f);
        hitAudioSource.PlayOneShot(clip);
    }
}
