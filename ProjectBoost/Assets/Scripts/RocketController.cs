using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketController : MonoBehaviour {
    [SerializeField] float force;
    [SerializeField] float rotationSpeed;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip exploded;
    [SerializeField] AudioClip finished;

    [SerializeField] ParticleSystem thrustParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem winParticles;

    enum State {Trancsending, Dying, Alive};
    enum ThrustState {Land, Thrusting }
    State state = State.Alive;
    ThrustState thrustState = ThrustState.Land;
    Rigidbody rb;
    AudioSource audioSource;
    bool disableCollision = false;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
	}

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive) {
            ChangeState();
            Rotate();
        }
        if (Debug.isDebugBuild)
        {
            DebugInput();
        }
	}

    private void OnCollisionEnter(Collision other)
    {
        if(state != State.Alive || disableCollision == true)
        {
            return;
        }
        if (other.gameObject.tag == "Friendly")
        {
            print("Friendly");
        }
        else if(other.gameObject.tag == "Finish")
        {
            StartFinishProcess();
        }
        else
        {
            StartDeathProcess();
        }
    }

    void DebugInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            LoadNextScene();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            disableCollision = !disableCollision;
        }
    }

    void StartFinishProcess()
    {
        audioSource.Stop();
        state = State.Trancsending;
        Invoke("LoadNextScene", 1.5f);
        audioSource.PlayOneShot(finished);
        winParticles.Play();
    } 

    void StartDeathProcess()
    {
        audioSource.Stop();
        thrustParticles.Stop();
        audioSource.PlayOneShot(exploded);
        deathParticles.Play();
        state = State.Dying;
        Invoke("LoadFirstScene", 1.0f);
    }

    void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        SceneManager.LoadScene(nextSceneIndex == 3? 0 : nextSceneIndex);
    }
    void LoadFirstScene()
    {
        SceneManager.LoadScene(0);
    }
    void Rotate()
    {
        rb.freezeRotation = true;
        if (Input.GetKey(KeyCode.A))
        {
            RotateLeft();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RotateRight();
        }
        rb.freezeRotation = false;
    }

    void RotateRight()
    {
        transform.Rotate(-Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    void RotateLeft()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    void ChangeState()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {           
            thrustState = ThrustState.Thrusting;            
        }     

        if(thrustState == ThrustState.Thrusting)
        {
            Thrust();
        }
        else if(thrustState == ThrustState.Land)
        {
            audioSource.Stop();
            thrustParticles.Stop();
        }
    }

    void Thrust()
    {
        rb.AddRelativeForce(Vector3.up * force * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
            thrustParticles.Play();
        }
    }
}
