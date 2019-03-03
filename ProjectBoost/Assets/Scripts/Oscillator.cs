using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour {
    [SerializeField] Vector3 movement;
    [Range(0, 1)][SerializeField] float factor;
    [SerializeField] float period = 2f;
    float cycle;
    Vector3 startPosition;
	// Use this for initialization
	void Start () {
        startPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {      
        cycle = period == 0 ? 0 : Time.time / period; // grows contineually from 0
        const float tau = Mathf.PI * 2;
        float sineWave = Mathf.Sin(tau * cycle);

        factor = sineWave / 2f + 0.5f;
        Vector3 offset = movement * factor;
        transform.position = startPosition + offset;
	}
}
