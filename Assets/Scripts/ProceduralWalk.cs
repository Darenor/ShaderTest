
using UnityEngine;

public class ProceduralWalk : MonoBehaviour
{
    [System.Serializable]
    public class FootStep
    {
        public Transform target;
        public Vector3 localOffset;

        [HideInInspector] public bool isStepping;
        [HideInInspector] public Vector3 startPosition;
        [HideInInspector] public Vector3 endPosition;
        [HideInInspector] public float progress;
    }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Foot Settings")]
    [SerializeField] private FootStep leftFoot;
    [SerializeField] private FootStep rightFoot;

    [SerializeField] private float stepDistance = 0.45f;
    [SerializeField] private float stepHeight = 0.25f;
    [SerializeField] private float stepDuration = 0.3f;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float raycastHeight = 1f;
    [SerializeField] private float groundOffset = 0.03f;

    private bool nextLeftFoot = true;

    private void Start()
    {
        leftFoot.localOffset =
            transform.InverseTransformPoint(leftFoot.target.position);

        rightFoot.localOffset =
            transform.InverseTransformPoint(rightFoot.target.position);
    }

    private void Update()
    {
        HandleMovement();

        UpdateFoot(leftFoot);
        UpdateFoot(rightFoot);

        TryStartStep();
    }

    
    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(
            horizontal,
            0f,
            vertical
            ).normalized;

        if (movement.sqrMagnitude < 0.01f)
            return;

        transform.position +=
            movement * moveSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(movement);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
            );

        leftFoot.target.rotation = transform.rotation;
        rightFoot.target.rotation = transform.rotation;
    }


    private void TryStartStep()
    {
        if (leftFoot.isStepping || rightFoot.isStepping)
            return;

        if (nextLeftFoot)
        {
            if (NeedsStep(leftFoot))
            {
                StartStep(leftFoot);
                nextLeftFoot = false;
            }
            else if (NeedsStep(rightFoot))
            {
                StartStep(rightFoot);
                nextLeftFoot = true;
            }
        }
        else
        {
            if (NeedsStep(rightFoot))
            {
                StartStep(rightFoot);
                nextLeftFoot = true;
            }
            else if (NeedsStep(leftFoot))
            {
                StartStep(leftFoot);
                nextLeftFoot = false;
            }
        }
    }

    private bool NeedsStep(FootStep foot)
    {
        Vector3 desiredPosition =
            transform.TransformPoint(foot.localOffset);

        return Vector3.Distance(
            foot.target.position,
            desiredPosition
        ) > stepDistance;
    }

    private void StartStep(FootStep foot)
    {
        foot.isStepping = true;
        foot.progress = 0f;

        foot.startPosition = foot.target.position;

        Vector3 desiredPosition =
            transform.TransformPoint(foot.localOffset);

        Vector3 rayOrigin = desiredPosition +
                            Vector3.up * raycastHeight;

        if (Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out RaycastHit hit,
            raycastHeight + 2f,
            groundMask))
        {
            foot.endPosition =
                hit.point + Vector3.up * groundOffset;
        }
        else
        {
            foot.endPosition = desiredPosition;
        }
    }

    private void UpdateFoot(FootStep foot)
    {
        if (!foot.isStepping)
            return;

        foot.progress += Time.deltaTime / stepDuration;

        float t = Mathf.Clamp01(foot.progress);

        Vector3 position = Vector3.Lerp(
            foot.startPosition,
            foot.endPosition,
            t
        );

        float height = Mathf.Sin(t * Mathf.PI) * stepHeight;

        position += Vector3.up * height;

        foot.target.position = position;

        if (t >= 1f)
        {
            foot.target.position = foot.endPosition;
            foot.isStepping = false;
        }
    }
}
