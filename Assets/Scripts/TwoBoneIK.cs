
using UnityEngine;

public class TwoBoneIK : MonoBehaviour
{
    [Header("Leg Bones")]
    [SerializeField] private Transform upperLeg;
    [SerializeField] private Transform lowerLeg;
    [SerializeField] private Transform foot;

    [Header("IK Targets")]
    [SerializeField] private Transform footTarget;
    [SerializeField] private Transform kneePole;
    
    [Header("Foot Rotation")]
    [SerializeField] private Vector3 footRotationOffset = new Vector3(0f, 180f, 0f);

    private float upperLength;
    private float lowerLength;

    private void Start()
    {
        if (upperLeg == null ||
            lowerLeg == null ||
            foot == null ||
            footTarget == null ||
            kneePole == null)
        {
            Debug.LogError("TwoBoneIK: Missing references!", this);
            enabled = false;
            return;
        }

        upperLength = Vector3.Distance(
            upperLeg.position,
            lowerLeg.position
        );

        lowerLength = Vector3.Distance(
            lowerLeg.position,
            foot.position
        );

        if (upperLength < 0.001f || lowerLength < 0.001f)
        {
            Debug.LogError("TwoBoneIK: Invalid bone lengths!", this);
            enabled = false;
        }
    }

    private void LateUpdate()
    {
        Solve();
    }

    private void Solve()
    {
        Vector3 hipPosition = upperLeg.position;
        Vector3 targetPosition = footTarget.position;

        Vector3 direction = targetPosition - hipPosition;
        float distance = direction.magnitude;

        if (distance < 0.001f)
            return;

        Vector3 directionNormalized = direction.normalized;

        float minDistance = Mathf.Abs(upperLength - lowerLength) + 0.001f;
        float maxDistance = upperLength + lowerLength - 0.001f;

        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        float along = (
            upperLength * upperLength -
            lowerLength * lowerLength +
            distance * distance
        ) / (2f * distance);

        float height = Mathf.Sqrt(
            Mathf.Max(0f, upperLength * upperLength - along * along)
        );

        Vector3 poleDirection = kneePole.position - hipPosition;

        Vector3 perpendicular = Vector3.ProjectOnPlane(
            poleDirection,
            directionNormalized
        ).normalized;

        if (perpendicular.sqrMagnitude < 0.001f)
            return;

        Vector3 desiredKneePosition =
            hipPosition +
            directionNormalized * along +
            perpendicular * height;

        Vector3 currentUpperDirection =
            lowerLeg.position - upperLeg.position;

        Vector3 desiredUpperDirection =
            desiredKneePosition - hipPosition;

        Quaternion upperRotation = Quaternion.FromToRotation(
            currentUpperDirection,
            desiredUpperDirection
        );

        upperLeg.rotation = upperRotation * upperLeg.rotation;

        Vector3 currentKneePosition = lowerLeg.position;

        Vector3 currentLowerDirection =
            foot.position - currentKneePosition;

        Vector3 desiredLowerDirection =
            targetPosition - currentKneePosition;

        Quaternion lowerRotation = Quaternion.FromToRotation(
            currentLowerDirection,
            desiredLowerDirection
        );

        lowerLeg.rotation = lowerRotation * lowerLeg.rotation;
        
        foot.rotation =
            footTarget.rotation *
            Quaternion.Euler(footRotationOffset);
    }
}
