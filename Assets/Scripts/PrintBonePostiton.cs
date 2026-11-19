using UnityEngine;

public class PrintBonePositions : MonoBehaviour
{
    [SerializeField] private Transform leftUpperLeg;
    [SerializeField] private Transform leftLowerLeg;
    [SerializeField] private Transform leftFoot;

    [SerializeField] private Transform rightUpperLeg;
    [SerializeField] private Transform rightLowerLeg;
    [SerializeField] private Transform rightFoot;

    private void Start()
    {
        PrintPosition("Upperleg.l", leftUpperLeg);
        PrintPosition("Lowerleg.l", leftLowerLeg);
        PrintPosition("Foot.l", leftFoot);

        PrintPosition("Upperleg.r", rightUpperLeg);
        PrintPosition("Lowerleg.r", rightLowerLeg);
        PrintPosition("Foot.r", rightFoot);
    }

    private void PrintPosition(string boneName, Transform bone)
    {
        if (bone == null)
            return;

        Debug.Log(
            boneName + " World Position: " + bone.position
            );
    }
}
