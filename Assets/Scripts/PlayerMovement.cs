using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private InputAction moveAction;

    private void OnEnable()
    {
        moveAction = new InputAction(
            "Move",
            InputActionType.Value,
            expectedControlType: "Vector2"
            );

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        moveAction.Enable();
    }

    private void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector3 movement = new Vector3(input.x, 0, input.y);

        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        moveAction?.Dispose();
    }
}
