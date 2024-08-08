using UnityEngine;

public class WeaponGrenadeProjectile : MonoBehaviour
{
    [Header("Explosion Barrel")]
    [SerializeField]
    private GameObject      explosionPrefab;            // 폭발 이펙트 프리팹
    [SerializeField]
    private float           explosionRadius = 10.0f;    // 폭발 범위
    [SerializeField]    
    private float           explosionForce = 500.0f;
    [SerializeField]
    private float           throwForce = 1000.0f;

    private int             explosionDamage;
    private new Rigidbody   rigidbody;

    /// <summary>
    /// 초기화 함수
    /// </summary>
    public void Setup(int _damage, Vector3 rotation)
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(rotation * throwForce);

        explosionDamage = _damage;
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(explosionPrefab, transform.position, transform.rotation);

        // 폭발 범위에 있는 모든 오브젝트의 Collider정보를 받아와 폭발 효과 처리
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage((int)(explosionDamage * 0.2f));
                continue;
            }

            EnemyFSM enemy = hit.GetComponentInParent<EnemyFSM>();
            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage);
                continue;
            }

            InteractionObject interaction = hit.GetComponent<InteractionObject>();
            if (interaction != null)
            {
                interaction.TakeDamage(explosionDamage);
            }

            Rigidbody _rigidbody = hit.GetComponent<Rigidbody>();
            if (_rigidbody != null)
            {
                _rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            Destroy(gameObject);
        }
    }
}
