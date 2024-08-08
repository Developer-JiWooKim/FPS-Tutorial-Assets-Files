using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private MovementTransform movementTransform;

    private float       projectileDistance = 30f;       // 발사체 최대 발사 거리
    private int         damage = 5;                     // 발사체 공격력

    /// <summary>
    /// 발사체 초기화
    /// </summary>
    public void Setup(Vector3 position)
    {
        movementTransform = GetComponent<MovementTransform>();
        StartCoroutine("OnMove", position);
    }

    /// <summary>
    /// 목표 방향으로 계속 움직이는 함수, 최대 거리를 초과하면 발사체를 사라지게
    /// </summary>
    private IEnumerator OnMove(Vector3 targetPosition)
    {
        Vector3 start = transform.position;

        movementTransform.MoveTo((targetPosition - transform.position).normalized);

        while (true)
        {
            if (Vector3.Distance(transform.position, start) >=  projectileDistance)
            {
                Destroy(gameObject);
                yield break;
            }

            yield return null;
        }
    }

    /// <summary>
    /// Enemy의 발사체가 Player에게 닿았을 때 실행될 트리거 함수
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
