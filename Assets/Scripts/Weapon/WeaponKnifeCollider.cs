using System.Collections;
using UnityEngine;

public class WeaponKnifeCollider : MonoBehaviour
{
    [SerializeField]
    private ImpactMemoryPool        impactMemoryPool;
    [SerializeField]
    private Transform               knifeTransform;

    private new Collider            collider;           // ���� �� Ȱ��ȭ�Ǿ� ���� ����� �Ǻ��� �ݶ��̴�
    private int                     damage;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        collider.enabled = false;
    }

    /// <summary>
    /// ���� �� �ִϸ��̼ǿ� ���� �ݶ��̴��� Ȱ��ȭ
    /// </summary>
    public void StartCollider(int _damage)
    {
        damage = _damage;
        collider.enabled = true;

        StartCoroutine("DisablebyTime", 0.1f);
    }
    
    /// <summary>
    /// ��� �ð� �� �ݶ��̴��� �ٽ� ��Ȱ��ȭ ��Ű�� �ڷ�ƾ �Լ�
    /// </summary>
    private IEnumerator DisablebyTime(float time)
    {
        yield return new WaitForSeconds(time);

        collider.enabled = false;
    }

    /// <summary>
    /// Ȱ��ȭ�� �ݶ��̴��� ����� �� ó��
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        impactMemoryPool.SpawnImpact(other, knifeTransform);
        if (other.CompareTag("ImpactEnemy"))
        {
            other.GetComponentInParent<EnemyFSM>().TakeDamage(damage);
        }
        else if (other.CompareTag("InteractionObject"))
        {
            other.GetComponent<InteractionObject>().TakeDamage(damage);
        }
    }
}
