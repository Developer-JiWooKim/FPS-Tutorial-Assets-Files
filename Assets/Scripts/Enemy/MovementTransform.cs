using UnityEngine;

public class MovementTransform : MonoBehaviour
{
    [SerializeField]
    private float       moveSpeed = 0.0f;

    [SerializeField]
    private Vector3     moveDirection = Vector3.zero;


    /// <summary>
    /// �̵� ������ �����Ǹ� �˾Ƽ� �̵�
    /// </summary>
    private void Update()
    {
        transform.position += moveDirection * Time.deltaTime * moveSpeed;
    }

    /// <summary>
    /// �ܺο��� �Ű������� �̵������� ����
    /// </summary>
    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }
}
