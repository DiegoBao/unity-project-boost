using System;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] private float rcsThrust = 100f;
    [SerializeField] private float mainThrust = 1f;
    [SerializeField] private AudioClip mainEngine;
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private AudioClip landingClip;

    [SerializeField] private ParticleSystem mainEngineParticles;
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] private ParticleSystem landingParticles;

    [SerializeField] private float levelLoadDelay = 2f;
    
    private AudioSource audioSource;
    private Rigidbody rigidBody;
    private bool isThrusting = false;
    private State state = State.Alive;

    private bool isCollisionOn = true;

    // Start is called before the first frame update
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if (Debug.isDebugBuild)
            RespondToDebugInput();
    }

    private void RespondToDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            isCollisionOn = !isCollisionOn;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive && !isCollisionOn) return;

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Fuel":
                print("Fuel");
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(explosionClip);
        mainEngineParticles.Stop();
        explosionParticles.Play();
        Invoke(nameof(LoadFirstLevel), levelLoadDelay);
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(landingClip);
        mainEngineParticles.Stop();
        landingParticles.Play();
        Invoke(nameof(LoadNextLevel), levelLoadDelay);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1); // TODO allow for more than 2 levels 
    }
    
    private void RespondToThrustInput()
    {
        isThrusting = Input.GetKey(KeyCode.Space);
        ApplyThrustSound();
    }

    private void ApplyThrustSound()
    {
        if (isThrusting)
        {
            if (!audioSource.isPlaying)
                audioSource.PlayOneShot(mainEngine);
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void FixedUpdate()
    {
        ApplyThrust();
    }

    private void ApplyThrust()
    {
        if (isThrusting)
        {
            rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime, ForceMode.Acceleration);
            mainEngineParticles.Play();
        }
        else
        {
            mainEngineParticles.Stop();
        }
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; // take manual control of rotation

        if (Input.GetKey(KeyCode.A))
            ApplyRotation(true);
        else if (Input.GetKey(KeyCode.D)) ApplyRotation(false);

        rigidBody.freezeRotation = false;
    }

    private void ApplyRotation(bool left)
    {
        float rotationThisFrame = rcsThrust * Time.deltaTime * (left ? 1 : -1);
        transform.Rotate(Vector3.forward * rotationThisFrame);
    }

    private enum State
    {
        Alive,
        Dying,
        Transcending,
    }
}