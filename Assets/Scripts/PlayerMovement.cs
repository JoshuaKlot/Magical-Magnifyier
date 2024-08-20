
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for Image

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    [SerializeField] private float speed = 12f;
    [SerializeField] private float gravity = -9.81f * 2;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private AudioSource[] footsteps;
    [SerializeField] private AudioSource jump;
    [SerializeField] private AudioSource fall;
    [SerializeField] private AudioSource water;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float keySensitivity = 90f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Image fadeImage; // Image component for fading to black
    [SerializeField] private float fadeDuration = 1f; // Duration of the fade effect

    private float yRotation;
    Vector3 velocity;
    bool isGrounded;
    bool isMoving;
    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);
    private bool isPlayingFootsteps = false;
    private bool wasGrounded = true; // To track if the player was grounded in the previous frame

    void Start()
    {
        controller = GetComponent<CharacterController>();
        fadeImage.color = new Color(0, 0, 0, 0); // Make sure the fade image is initially transparent
    }

    void Update()
    {
        // GroundCheck
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Play fall sound when landing
        if (isGrounded && !wasGrounded)
        {
            if(!fall.isPlaying)
                fall.Play();
        }

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
            jump.Play(); // Play jump sound
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

        wasGrounded = isGrounded; // Update the wasGrounded flag at the end of the Update loop
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Water"))
        {
            StartCoroutine(HandleWaterCollision());
        }
    }

    private IEnumerator HandleWaterCollision()
    {
        water.Play(); // Play the water sound effect

        // Fade to black
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(elapsedTime / fadeDuration));
            yield return null;
        }

        // Reload the current scene after the fade effect
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
                    // Randomize the pitch of the footstep sound
                    footsteps[i].pitch = Random.Range(0.8f, 1.2f); // Adjust the range as needed

                    footsteps[i].Play();
                    yield return new WaitForSeconds(0.5f); // You might want to adjust this delay
                }
            }
        }

        isPlayingFootsteps = false;
    }
}
