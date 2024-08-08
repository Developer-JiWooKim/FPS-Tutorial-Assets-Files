using UnityEngine;

public class MovementTransform : MonoBehaviour
{
    [SerializeField]
    private float       moveSpeed = 0.0f;

    [SerializeField]
    private Vector3     moveDirection = Vector3.zero;


    /// <summary>
    /// 이동 방향이 설정되면 알아서 이동
    /// </summary>
    private void Update()
    {
        transform.position += moveDirection * Time.deltaTime * moveSpeed;
    }

    /// <summary>
    /// 외부에서 매개변수로 이동방향을 설정
    /// </summary>
    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }
}
