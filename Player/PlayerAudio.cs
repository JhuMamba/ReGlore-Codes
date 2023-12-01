using JhutenFPP.Manager;
using JhutenFPP.PlayerControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioSource weaponAudioSource;
    [SerializeField] private AudioSource hitAudioSource;
    public AudioClip[] footstepSounds;
    public AudioClip[] weaponSounds;
    public AudioClip[] hitSounds;
    public float footstepInterval = 0.5f;
    public float footstepIntervalRun = 0.3f;

    private Rigidbody rb;
    private InputControl _inputControl;
    private float lastFootstepTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _inputControl = GetComponent<InputControl>();
        lastFootstepTime = Time.time;
    }

    void Update()
    {
        if (rb.velocity.magnitude < 0.2f) return;
        // Check if the player is moving.
        if (!_inputControl.Run && Time.time - lastFootstepTime > footstepInterval)
        {
            // Play a random footstep sound.
            PlayFootStepSFX();

            // Update the last footstep time.
            lastFootstepTime = Time.time;
        }
        else if (_inputControl.Run && Time.time - lastFootstepTime > footstepIntervalRun)
        {
            PlayFootStepSFX();
            lastFootstepTime = Time.time;
        }
    }

    private void PlayFootStepSFX()
    {
        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        footstepAudioSource.clip = clip;
        footstepAudioSource.volume = Random.Range(.05f, .08f);
        footstepAudioSource.pitch = Random.Range(.8f, 1.2f);
        footstepAudioSource.Play();
    }
    private void PlaySwordSwingSFX()
    {
        AudioClip clip = weaponSounds[Random.Range(0, weaponSounds.Length)];
        weaponAudioSource.clip = clip;
        weaponAudioSource.volume = Random.Range(.2f, .3f);
        weaponAudioSource.pitch = Random.Range(.5f, .8f);
        weaponAudioSource.Play();
    }
    public void PlayHitSFX()
    {
        AudioClip clip = hitSounds[Random.Range(0, hitSounds.Length)];
        hitAudioSource.volume = Random.Range(.2f, .4f);
        hitAudioSource.pitch = Random.Range(.8f, 1.2f);
        hitAudioSource.PlayOneShot(clip);
    }
}
