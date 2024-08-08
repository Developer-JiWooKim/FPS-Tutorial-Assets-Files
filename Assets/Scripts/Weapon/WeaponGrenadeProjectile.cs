using UnityEngine;

public class WeaponGrenadeProjectile : MonoBehaviour
{
    [Header("Explosion Barrel")]
    [SerializeField]
    private GameObject      explosionPrefab;            // ���� ����Ʈ ������
    [SerializeField]
    private float           explosionRadius = 10.0f;    // ���� ����
    [SerializeField]    
    private float           explosionForce = 500.0f;
    [SerializeField]
    private float           throwForce = 1000.0f;

    private int             explosionDamage;
    private new Rigidbody   rigidbody;

    /// <summary>
    /// �ʱ�ȭ �Լ�
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

        // ���� ������ �ִ� ��� ������Ʈ�� Collider������ �޾ƿ� ���� ȿ�� ó��
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
