using System;

using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] private Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] private float period = 2f;

    private float movementFactor; // 0 for not moved, 1 for fully moved.
    private Vector3 startingPosition;

    // Start is called before the first frame update
    private void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        // set movement factor
        float cycles = (period <= Mathf.Epsilon) ? 0f : Time.time / period; // grows continually from 0

        const float tau = Mathf.PI * 2; // about 6.28 
        float rawSinWave = Mathf.Sin(cycles * tau);
        
        movementFactor = rawSinWave / 2f + 0.5f;
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPosition + offset;
    }
}