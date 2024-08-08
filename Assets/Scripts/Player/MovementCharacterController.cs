using UnityEngine;

/// <summary>
/// 자동으로 컴포넌트를 추가하기 위해
/// </summary>
[RequireComponent(typeof(CharacterController))] 
public class MovementCharacterController : MonoBehaviour
{
    [SerializeField]
    private float                   moveSpeed;
    private Vector3                 moveForce;

    [SerializeField]
    private float                   jumpForce;              // 점프할 때 가할 물리 값

    [SerializeField]
    private float                   gravity;

    private CharacterController     characterController;

    /// <summary>
    /// MoveSpeed를 읽는 용도의 프로퍼티(음수 제한)
    /// </summary>
    public float MoveSpeed
    {
        set => moveSpeed = Mathf.Max(0, value); // 음수가 적용되지 않게 Mathf.Max()함수 사용
        get => moveSpeed;
    }

    /// <summary>
    /// 캐릭터 컴포넌트 컴포넌트를 얻어와 변수에 저장
    /// </summary>
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        CharacterMove();
    }

    /// <summary>
    /// 캐릭터 이동 함수
    /// </summary>
    private void CharacterMove()
    {
        // 땅과 떨어져있으면(허공에 있으면) gravity 만큼 y축 이동속도 감소
        if (!characterController.isGrounded)
        {
            moveForce.y += gravity * Time.deltaTime;
        }

        characterController.Move(moveForce * Time.deltaTime); // moveForce만큼 캐릭터 이동
    }

    /// <summary>
    /// 캐릭터를 이동할 방향으로 회전시키는 함수
    /// </summary>
    public void MoveTo(Vector3 direction)
    {
        // 이동 방향 = 캐릭터의 회전 값 * 방향 값
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);

        // moveForce = 이동 방향(y값 = 0) * 속도
        moveForce = new Vector3(direction.x * moveSpeed, moveForce.y, direction.z * moveSpeed);
    }

    /// <summary>
    /// 캐릭터가 점프 시 y축에 jumpForce물리값을 가함
    /// </summary>
    public void Jump()
    {
        if (characterController.isGrounded)
        {
            moveForce.y = jumpForce;
        }
    }

}
