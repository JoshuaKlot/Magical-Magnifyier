using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void Update()
    {
        pushDistance = (int)(defaultPushDistance - (transform.localScale.x * weightMultiplier));
            if(pushDistance<0)
        {
            pushDistance = 0;
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
        rb.isKinematic = false;
        resetKinematicCoroutine = null;
    }

    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player") && Pushable)
        {
            Vector3 pushDirection = GetPushDirection(col);
            StartCoroutine(MoveBoxSmoothly(pushDirection));
        }
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

    private IEnumerator MoveBoxSmoothly(Vector3 pushDirection)
    {
        rb.isKinematic = true;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + pushDirection.normalized * pushDistance;
        float elapsedTime = 0f;
        float timeMoving = pushDistance * pushSpeed;
        while (elapsedTime < timeMoving)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / timeMoving);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;

        StartKinematicResetCoroutine();
    }
}
