using System.Collections;
using UnityEngine;

public class WeaponKnifeCollider : MonoBehaviour
{
    [SerializeField]
    private ImpactMemoryPool        impactMemoryPool;
    [SerializeField]
    private Transform               knifeTransform;

    private new Collider            collider;           // 공격 시 활성화되어 공격 대상을 판별할 콜라이더
    private int                     damage;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        collider.enabled = false;
    }

    /// <summary>
    /// 공격 시 애니메이션에 맞춰 콜라이더를 활성화
    /// </summary>
    public void StartCollider(int _damage)
    {
        damage = _damage;
        collider.enabled = true;

        StartCoroutine("DisablebyTime", 0.1f);
    }
    
    /// <summary>
    /// 재생 시간 후 콜라이더를 다시 비활성화 시키는 코루틴 함수
    /// </summary>
    private IEnumerator DisablebyTime(float time)
    {
        yield return new WaitForSeconds(time);

        collider.enabled = false;
    }

    /// <summary>
    /// 활성화된 콜라이더에 닿았을 때 처리
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
