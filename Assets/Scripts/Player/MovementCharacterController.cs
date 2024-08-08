using UnityEngine;

/// <summary>
/// �ڵ����� ������Ʈ�� �߰��ϱ� ����
/// </summary>
[RequireComponent(typeof(CharacterController))] 
public class MovementCharacterController : MonoBehaviour
{
    [SerializeField]
    private float                   moveSpeed;
    private Vector3                 moveForce;

    [SerializeField]
    private float                   jumpForce;              // ������ �� ���� ���� ��

    [SerializeField]
    private float                   gravity;

    private CharacterController     characterController;

    /// <summary>
    /// MoveSpeed�� �д� �뵵�� ������Ƽ(���� ����)
    /// </summary>
    public float MoveSpeed
    {
        set => moveSpeed = Mathf.Max(0, value); // ������ ������� �ʰ� Mathf.Max()�Լ� ���
        get => moveSpeed;
    }

    /// <summary>
    /// ĳ���� ������Ʈ ������Ʈ�� ���� ������ ����
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
    /// ĳ���� �̵� �Լ�
    /// </summary>
    private void CharacterMove()
    {
        // ���� ������������(����� ������) gravity ��ŭ y�� �̵��ӵ� ����
        if (!characterController.isGrounded)
        {
            moveForce.y += gravity * Time.deltaTime;
        }

        characterController.Move(moveForce * Time.deltaTime); // moveForce��ŭ ĳ���� �̵�
    }

    /// <summary>
    /// ĳ���͸� �̵��� �������� ȸ����Ű�� �Լ�
    /// </summary>
    public void MoveTo(Vector3 direction)
    {
        // �̵� ���� = ĳ������ ȸ�� �� * ���� ��
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);

        // moveForce = �̵� ����(y�� = 0) * �ӵ�
        moveForce = new Vector3(direction.x * moveSpeed, moveForce.y, direction.z * moveSpeed);
    }

    /// <summary>
    /// ĳ���Ͱ� ���� �� y�࿡ jumpForce�������� ����
    /// </summary>
    public void Jump()
    {
        if (characterController.isGrounded)
        {
            moveForce.y = jumpForce;
        }
    }

}
