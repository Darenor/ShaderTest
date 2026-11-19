using UnityEngine;

public class ProceduralArmSwing : MonoBehaviour
{
    [Header("Arm Bones")]
    [SerializeField] private Transform leftArm;
    [SerializeField] private Transform rightArm;

    [Header("Arm Position")]
    [SerializeField] private float armDownAngle = 90f;

    [Header("Swing Settings")]
    [SerializeField] private float swingAngle = 20f;
    [SerializeField] private float walkFrequency = 2.5f;
    [SerializeField] private float smoothSpeed = 8f;

    private Quaternion leftInitialRotation;
    private Quaternion rightInitialRotation;

    private float phase;
    private float currentSwing;

    private void Start()
    {
        if (leftArm == null || rightArm == null)
        {
            Debug.LogError("ProceduralArmSwing: Missing arm references!");
            enabled = false;
            return;
        }

        leftInitialRotation = leftArm.rotation;
        rightInitialRotation = rightArm.rotation;
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        float movement = Mathf.Clamp01(
            new Vector2(horizontal, vertical).magnitude
            );

        if (movement > 0.01f)
        {
            phase += Time.deltaTime *
                walkFrequency *
                movement *
                Mathf.PI * 2f;
        }

        currentSwing = Mathf.MoveTowards(
            currentSwing,
            movement,
            smoothSpeed * Time.deltaTime
            );
    }

    private void LateUpdate()
    {
        Quaternion leftDown = Quaternion.AngleAxis(
            armDownAngle,
            transform.forward
            );

        Quaternion rightDown = Quaternion.AngleAxis(
            -armDownAngle,
            transform.forward
            );


        float swing = Mathf.Sin(phase) *
            swingAngle *
            currentSwing;

        Quaternion leftSwing = Quaternion.AngleAxis(
            swing,
            transform.right
            );

        Quaternion rightSwing = Quaternion.AngleAxis(
            -swing,
            transform.right
            );

        leftArm.rotation =
            leftSwing * leftDown * leftInitialRotation;

        rightArm.rotation =
            rightSwing * rightDown * rightInitialRotation;
    }
}
