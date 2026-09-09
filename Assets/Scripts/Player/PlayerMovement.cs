using UnityEngine;
using Yarn.Unity;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    private Vector2 _movement;
    private Rigidbody2D _rb;

    // We keep this public so you can see it working in the Inspector
    public bool canMove = true;
    private DialogueRunner _dialogueRunner;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        // Automatically finds your "Dialogue System" prefab's runner
        _dialogueRunner = FindFirstObjectByType<DialogueRunner>();
    }

    public void SetMovement(bool status)
    {
        canMove = status;
        if (!canMove)
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }

    void Update()
    {
        // AUTOMATION: If the runner isn't running, ensure the player is unfrozen
        if (_dialogueRunner != null && !_dialogueRunner.IsDialogueRunning && !canMove)
        {
            SetMovement(true);
        }

        if (canMove)
        {
            _movement.x = InputManager.Movement.x;
            _movement.y = InputManager.Movement.y;

            if (_movement.sqrMagnitude < 0.01f)
            {
                _movement.x = Input.GetAxisRaw("Horizontal");
                _movement.y = Input.GetAxisRaw("Vertical");
            }

            _rb.linearVelocity = _movement.normalized * _moveSpeed;
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }

        FlipSprite();
    }

    void FlipSprite()
    {
        if (_movement.x > 0)
        {
            //play the right animaion
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        else if (_movement.x < 0)
        {
            // play the left animation
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            //play the idle animation.
        }
    }
}