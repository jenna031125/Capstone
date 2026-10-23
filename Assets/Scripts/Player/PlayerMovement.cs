using UnityEngine;
using Yarn.Unity;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    public Vector2 _movement;
    private Rigidbody2D _rb;

    public bool canMove = true;
    private DialogueRunner _dialogueRunner;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _dialogueRunner = FindFirstObjectByType<DialogueRunner>();
    }

    public void SetMovement(bool status)
    {
        canMove = status;
        if (!canMove)
        {
            _movement = Vector2.zero; // 입력 벡터 초기화
            if (_rb != null) _rb.linearVelocity = Vector2.zero;
        }
    }

    void Update()
    {
        // 1. 대화 진행 여부 감지 및 canMove 자동 복원
        if (_dialogueRunner != null)
        {
            if (_dialogueRunner.IsDialogueRunning)
            {
                canMove = false;
            }
            else if (!canMove && !_dialogueRunner.IsDialogueRunning)
            {
                SetMovement(true);
            }
        }

        // 2. 이동 불가능 상태(대화 중)일 때 움직임 벡터 완전 리셋
        if (!canMove)
        {
            _movement = Vector2.zero;
            if (_rb != null) _rb.linearVelocity = Vector2.zero;
        }
        else
        {
            // 이동 가능할 때 입력값 받아오기
            _movement.x = InputManager.Movement.x;
            _movement.y = InputManager.Movement.y;

            if (_movement.sqrMagnitude < 0.01f)
            {
                _movement.x = Input.GetAxisRaw("Horizontal");
                _movement.y = Input.GetAxisRaw("Vertical");
            }

            if (_rb != null) _rb.linearVelocity = _movement.normalized * _moveSpeed;
        }

        // 3. 애니메이터 상태 업데이트 (이동 불가능할 때는 IsWalking이 false가 됨)
        Animator activeAnimator = GetComponentInChildren<Animator>();

        if (activeAnimator != null)
        {
            activeAnimator.SetFloat("MoveX", _movement.x);
            activeAnimator.SetFloat("MoveY", _movement.y);
            activeAnimator.SetBool("IsWalking", _movement.sqrMagnitude > 0.01f);

            if (_movement != Vector2.zero)
            {
                activeAnimator.SetFloat("LastMoveX", _movement.x);
                activeAnimator.SetFloat("LastMoveY", _movement.y);
            }
        }

        FlipSprite();
    }

    void FlipSprite()
    {
        if (_movement.x > 0)
        {
            // right animation
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        else if (_movement.x < 0)
        {
            // left animation
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }
}