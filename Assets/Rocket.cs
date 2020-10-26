using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Rocket : MonoBehaviour
{
    private Rigidbody rigidBody;
    private AudioSource audioSource;
    [SerializeField]
    private float rcsThrust = 100f;

    [SerializeField] private float mainThrust = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Thrust();
        Rotate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                print("Ok");
                break;
            case "Fuel":
                print("Fuel");
                break;
            case "Finished":
                print("Finished");
                break;
            default:
                print("Dead");
                break;
        }
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * mainThrust);
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true; // take manual control of rotation
        
        if (Input.GetKey(KeyCode.A))
        {
            ApplyRotation(true);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            ApplyRotation(false);
        }
        
        rigidBody.freezeRotation = false;
    }

    private void ApplyRotation(bool left)
    {
        var rotationThisFrame = rcsThrust * Time.deltaTime * (left ? 1 : - 1);
        transform.Rotate(Vector3.forward * rotationThisFrame);
    }
}
