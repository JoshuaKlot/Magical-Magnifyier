using System.Collections;
using UnityEngine;

public class Intercatable : MonoBehaviour
{
    [SerializeField] public bool Shrinkable;
    [SerializeField] public bool Pushable;
    [SerializeField] private float weightMultiplier;
    [SerializeField] private float upperLimit;
    [SerializeField] private float lowerLimit;
    [SerializeField] private float thresholdMass;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundDistance = 0.1f;
    [SerializeField] private float defaultPushDistance = 1f; // Distance the box will move when pushed
    [SerializeField] private float pushSpeed = 0.03f; // Duration of the smooth push
    private int pushDistance;
    private bool isGrounded;
    private Rigidbody rb;
    private bool isLit;
    private const float detectionThreshold = 0.1f; // Adjust as necessary
    private Coroutine resetKinematicCoroutine;
    private Vector3 targetVelocity;
    public Vector3 pushDirection;
    public Vector3 startPosition;
    private bool isPushing;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        pushDistance = (int)(defaultPushDistance - (transform.localScale.x * weightMultiplier));
        if (pushDistance < 0)
        {
            pushDistance = 0;
        }

        CheckIfGrounded();

        if (isPushing)
        {
            // Calculate the distance moved since the push started
            float distanceMoved = Vector3.Distance(startPosition, transform.position);

            // Stop the object if it has moved the desired distance or overshot it
            if (distanceMoved >= pushDistance)
            {
                StopMovement();
            }
        }
    }

    private void FixedUpdate()
    {
        if (targetVelocity != Vector3.zero && isPushing)
        {
            rb.velocity = targetVelocity;
        }

        if (isGrounded)
        {
            rb.isKinematic = true;
        }
        else
        {
            rb.isKinematic = false;
        }
    }

    private void MaintainGroundContact()
    {
        if (isGrounded)
        {
            // Lock the Y position to prevent falling through the ground
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            // Optional: Slightly adjust the object's position if it's not perfectly on the ground
            transform.position = new Vector3(transform.position.x, Mathf.Max(transform.position.y, groundDistance), transform.position.z);
        }
        else
        {
            rb.isKinematic = false; // Ensure physics is active if not grounded
        }
    }


    public void Grow()
    {
        if (transform.localScale.x < upperLimit)
        {
            rb.isKinematic = true;

            Vector3 scale = new Vector3(transform.localScale.x + 0.01f,
                                        transform.localScale.y + 0.01f,
                                        transform.localScale.z + 0.01f);
            transform.localScale = scale;

            MaintainGroundContact(); // Ensure the object stays on the ground

            StartKinematicResetCoroutine();
        }
    }

    public void Shrink()
    {
        if (transform.localScale.x > lowerLimit)
        {
            rb.isKinematic = true;

            Vector3 scale = new Vector3(transform.localScale.x - 0.01f,
                                        transform.localScale.y - 0.01f,
                                        transform.localScale.z - 0.01f);
            transform.localScale = scale;

            MaintainGroundContact(); // Ensure the object stays on the ground

            StartKinematicResetCoroutine();
        }
    }

    private void StartKinematicResetCoroutine()
    {
        if (resetKinematicCoroutine != null)
        {
            StopCoroutine(resetKinematicCoroutine);
        }

        resetKinematicCoroutine = StartCoroutine(ResetKinematic());
    }

    private IEnumerator ResetKinematic()
    {
        yield return new WaitForSeconds(0.03f);
        resetKinematicCoroutine = null;
    }

    public Vector3 GetPushDirection(Vector3 colliderPosition)
    {
        Vector3 direction = colliderPosition - transform.position;
        direction.y = 0; // Ignore vertical component

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            return direction.x > 0 ? Vector3.left : Vector3.right;
        }
        else
        {
            return direction.z > 0 ? Vector3.back : Vector3.forward;
        }
    }
    private void CheckIfGrounded()
    {
        float radius = transform.localScale.x / 2;
        float halfHeight = transform.localScale.y / 2;

        // Use a capsule cast for better ground detection
        isGrounded = Physics.CapsuleCast(transform.position - Vector3.up * halfHeight,
                                         transform.position + Vector3.up * halfHeight,
                                         radius, Vector3.down, groundDistance, groundLayer);

        Debug.DrawRay(transform.position, Vector3.down * (halfHeight + groundDistance), isGrounded ? Color.green : Color.red);
    }


    public void OnCollisionEnter(Collision col)
    {
        
        if (col.gameObject.CompareTag("Interactable") && Pushable)
        {
            Debug.Log("Interactable touch detected.");
            Intercatable otherInteractable = col.gameObject.GetComponent<Intercatable>();
            if (otherInteractable != null && otherInteractable.Pushable)
            {
                Debug.Log("Pushing another interactable object.");
                otherInteractable.pushDirection = GetPushDirection(transform.position);
                otherInteractable.startPosition = transform.position;
                otherInteractable.MoveBoxInDirection(otherInteractable.pushDirection);
            }
            StopMovement();
        }
    }
    public void SimulateCollision(GameObject colliderObject)
    {
        if (colliderObject.CompareTag("Player") && Pushable)
        {
            pushDirection = GetPushDirection(colliderObject.transform.position);
            startPosition = transform.position;
            MoveBoxInDirection(pushDirection);
        }
        else if (colliderObject.CompareTag("Interactable") && Pushable)
        {
            Intercatable otherInteractable = colliderObject.GetComponent<Intercatable>();
            if (otherInteractable != null && otherInteractable.Pushable)
            {
                otherInteractable.pushDirection = GetPushDirection(colliderObject.transform.position);
                otherInteractable.startPosition = transform.position;
                otherInteractable.MoveBoxInDirection(otherInteractable.pushDirection);
            }
            StopMovement();
        }
    }




    private void MoveBoxInDirection(Vector3 pushDirection)
    {
        rb.isKinematic = false;
        targetVelocity = pushDirection.normalized * (pushDistance / pushSpeed);
        isPushing = true;
    }

    private void StopMovement()
    {
        targetVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        isPushing = false;
        StartKinematicResetCoroutine();
    }
}

