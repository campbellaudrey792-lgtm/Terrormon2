using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // NEW input system

[RequireComponent(typeof(PlayerInput))]
public class PlayerScript_NewInput : MonoBehaviour
{
    [Header("Tuning")]
    public float moveSpeed = 5f;
    public float stepSize = 1f;

    [Header("State")]
    public bool isMoving;

    private PlayerInput playerInput;
    private InputAction moveAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        // Make sure your action is literally named "Move" in the asset
        moveAction = playerInput.actions["Move"];
    }

    private void OnEnable() => moveAction.Enable();
    private void OnDisable() => moveAction.Disable();

    private void Update()
    {
        if (isMoving) return;

        Vector2 input = moveAction.ReadValue<Vector2>();

        // Convert analog to grid step (-1/0/1)
        input.x = Mathf.Sign(input.x);
        input.y = Mathf.Sign(input.y);

        // Optional: block diagonals
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) input.y = 0f;
        else input.x = 0f;

        if (input != Vector2.zero)
        {
            var targetPos = transform.position;
            targetPos.x += input.x * stepSize;
            targetPos.y += input.y * stepSize;

            StartCoroutine(Move(targetPos));
        }
    }

    private IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
    }
}