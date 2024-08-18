using System.Collections;

using UnityEngine;

public class Intercatable : MonoBehaviour
{
    [SerializeField] public bool Shrinkable;
    [SerializeField] private bool Pushable;
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
    private Vector3 pushDirection;
    private Vector3 startPosition;
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

    public void Grow()
    {
        if (transform.localScale.x < upperLimit)
        {
            rb.isKinematic = true;

            Vector3 scale = new Vector3(transform.localScale.x + 0.01f,
                                        transform.localScale.y + 0.01f,
                                        transform.localScale.z + 0.01f);
            transform.localScale = scale;

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

    private Vector3 GetPushDirection(Collision col)
    {
        Vector3 direction = col.transform.position - transform.position;
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
        // Use a raycast to check if the object is grounded
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundDistance, groundLayer);
    }

    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player") && Pushable)
        {
            pushDirection = GetPushDirection(col);
            startPosition = transform.position;
            MoveBoxInDirection(pushDirection);
        }

        if (col.gameObject.CompareTag("Interactable") && Pushable)
        {

            Intercatable otherInteractable = col.gameObject.GetComponent<Intercatable>();
            StopMovement();
            Debug.Log("Touch");
            col.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            if (otherInteractable != null && otherInteractable.Pushable)
            {
                otherInteractable.pushDirection = GetPushDirection(col);
                otherInteractable.startPosition = transform.position;
                // Push the other interactable object in the same direction
                otherInteractable.MoveBoxInDirection(otherInteractable.pushDirection);
            }

            // Stop the current object's movement after initiating the push of the other object
            
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

