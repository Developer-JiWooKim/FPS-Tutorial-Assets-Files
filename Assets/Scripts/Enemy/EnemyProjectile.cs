using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private MovementTransform movementTransform;

    private float       projectileDistance = 30f;       // �߻�ü �ִ� �߻� �Ÿ�
    private int         damage = 5;                     // �߻�ü ���ݷ�

    /// <summary>
    /// �߻�ü �ʱ�ȭ
    /// </summary>
    public void Setup(Vector3 position)
    {
        movementTransform = GetComponent<MovementTransform>();
        StartCoroutine("OnMove", position);
    }

    /// <summary>
    /// ��ǥ �������� ��� �����̴� �Լ�, �ִ� �Ÿ��� �ʰ��ϸ� �߻�ü�� �������
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
    /// Enemy�� �߻�ü�� Player���� ����� �� ����� Ʈ���� �Լ�
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
