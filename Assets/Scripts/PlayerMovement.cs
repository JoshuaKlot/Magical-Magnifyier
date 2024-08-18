using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    [SerializeField] private float speed = 12f;
    [SerializeField] private float gravity = -9.81f * 2;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private AudioSource[] footsteps;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float keySensitivity = 90f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    private float yRotation;
    Vector3 velocity;
    bool isGrounded;
    bool isMoving;
    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);
    private bool isPlayingFootsteps = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // GroundCheck
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Resetting the Default Velocity
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        // Check if the player can jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Check if the player is moving
        if (lastPosition != gameObject.transform.position && isGrounded)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        lastPosition = gameObject.transform.position;

        // Play footstep sounds if the player is moving and grounded
        if (isMoving && !isPlayingFootsteps)
        {
            StartCoroutine(PlayFootsteps());
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Water"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private IEnumerator PlayFootsteps()
    {
        isPlayingFootsteps = true;

        while (isMoving)
        {
            for (int i = 0; i < footsteps.Length; i++)
            {
                if (isMoving && isGrounded)
                {
                    footsteps[i].Play();
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        isPlayingFootsteps = false;
    }
}
