using System.Collections;
using UnityEngine;

public class ExplosionBarrel : InteractionObject
{
    [Header("Explosion Barrel")]
    [SerializeField]
    private GameObject      explosionPrefab;                        // ��ź ȿ�� ������
    [SerializeField]
    private float           explosionDelayTime = 0.2f;              // ���� ���� �ð�
    [SerializeField]
    private float           explosionRadius = 10.0f;                // ���� ����
    [SerializeField]
    private float           explosionForce = 1000.0f;               // ���� ����

    /// <summary>
    /// �̼ǰ� ���õ� ����
    /// </summary>
    private const int       objectMissionNumber = 2;
    private bool            attackExplosionBarrelMission = false;

    private bool            isExplode = false;                      // ���� ����

    /// <summary>
    /// �� ������Ʈ�� �ʱ� ��ġ, ȸ�� ��
    /// </summary>
    private Vector3         pos;
    private Quaternion      rot;

    /// <summary>
    /// �ʱ� ��ġ, �ʱ� ȸ�� ���� ������ ����
    /// </summary>
    private void Awake()
    {
        pos = transform.position;
        rot = transform.rotation;
    }

    /// <summary>
    /// ��Ȱ��ȭ �� �� �� ������Ʈ �ʱ�ȭ
    /// </summary>
    private void OnDisable()
    {
        SetUp();
    }

    /// <summary>
    /// �������� �Ծ��� �� ���� �� �Լ�
    /// </summary>
    public override void TakeDamage(int damage)
    {
        currentHP -= damage;
        // ���� �ǰ� 1ȸ �� attackExplosionBarrelMission�� Ŭ���� �Ǿ����� ������ �̼��� Ŭ����
        if (attackExplosionBarrelMission == false)
        {
            Mission_AttackExplosionBarrel();
        }

        // �巳�� ü���� 0���ϰ� �ǰ�, ���� �������� ���� ���¸� explosionDelayTime �� ���� 
        if (currentHP <= 0 && isExplode == false)
        {
            
            StartCoroutine("ExplodeBarrel");
        }
    }

    /// <summary>
    /// AttackExplosionBarrel �̼� Ŭ���� �� ȣ��� �Լ�
    /// </summary>
    private void Mission_AttackExplosionBarrel()
    {
        // Ȱ��ȭ�� �̼��� �ְ� �� �̼��� Object�� �̼��� ��
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) &&
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.Object);
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.Object, objectMissionNumber);
            attackExplosionBarrelMission = true;
        }
    }

    /// <summary>
    /// explosionDelayTime �� �� ���� ȿ�� �ڷ�ƾ
    /// </summary>
    private IEnumerator ExplodeBarrel()
    {
        yield return new WaitForSeconds(explosionDelayTime);

        // ��ó�� Barrel�� ������ �ٽ� ���� Barrel�� ��Ʈ������ �� ��(StackOverFlow ����)
        isExplode = true;

        // Bounds�� �ݶ��̴�, ���� ����Ʈ ����
        Bounds bounds = GetComponent<Collider>().bounds;
        Instantiate(explosionPrefab, new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), transform.rotation);

        // ���� ������ �ִ� ��� ������Ʈ�� Collider ������ ���� ���� ȿ�� ó��
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            // ���� ������ �ε��� OBJ�� Player
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(50);
                continue;
            }

            // ���� ������ �ε��� OBJ�� Enemy
            EnemyFSM enemy = hit.GetComponentInParent<EnemyFSM>();
            if (enemy != null)
            {
                enemy.TakeDamage(300);
                continue;
            }

            // ���� ������ �ε��� OBJ�� InteractionOBJ
            InteractionObject interaction = hit.GetComponent<InteractionObject>();
            if (interaction != null)
            {
                interaction.TakeDamage(300);
            }

            // �߷��� ������ �ִ� ������Ʈ�̸� ���� �޾� �з�������
            Rigidbody rigidbody = hit.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        gameObject.GetComponentInParent<BarrelManager>().ReSpawnBarrel(gameObject); // �� ������Ʈ ���� �� �ٽ� ����
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ���� ���� �ʱ�ȭ, ü�� �ʱ�ȭ, ��ġ �ʱ�ȭ, ���� �ʱ�ȭ
    /// </summary>
    public void SetUp()
    {
        isExplode = false;
        currentHP = maxHP;
        transform.position = pos;
        transform.rotation = rot;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
