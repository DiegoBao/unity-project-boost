using System;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] private float rcsThrust = 100f;

    [SerializeField] private float mainThrust = 1f;
    private AudioSource audioSource;
    private Rigidbody rigidBody;

    private State state = State.Alive;

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
            Thrust();
            Rotate();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) return;

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Fuel":
                print("Fuel");
                break;
            case "Finish":
                state = State.Transcending;
                Invoke(nameof(LoadNextLevel), 1f); // TODO: parameterise time
                break;
            default:
                state = State.Dying;
                Invoke(nameof(LoadFirstLevel), 1f);
                break;
        }
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1); // TODO allow for more than 2 levels 
    }

    private bool isThrusting = false;
    
    private void Thrust()
    {
        isThrusting = Input.GetKey(KeyCode.Space); 
        if (isThrusting)
        {
            // rigidBody.AddRelativeForce(Vector3.up * mainThurst, ForceMode.Acceleration);
            if (!audioSource.isPlaying) audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void FixedUpdate()
    {
        if (isThrusting)
        {
            rigidBody.AddRelativeForce(Vector3.up * mainThrust, ForceMode.Acceleration);
        }
    }

    private void Rotate()
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