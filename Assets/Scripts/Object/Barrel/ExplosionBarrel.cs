using System.Collections;
using UnityEngine;

public class ExplosionBarrel : InteractionObject
{
    [Header("Explosion Barrel")]
    [SerializeField]
    private GameObject      explosionPrefab;                        // 폭탄 효과 프리팹
    [SerializeField]
    private float           explosionDelayTime = 0.2f;              // 폭발 지연 시간
    [SerializeField]
    private float           explosionRadius = 10.0f;                // 폭발 범위
    [SerializeField]
    private float           explosionForce = 1000.0f;               // 폭발 위력

    /// <summary>
    /// 미션과 관련된 변수
    /// </summary>
    private const int       objectMissionNumber = 2;
    private bool            attackExplosionBarrelMission = false;

    private bool            isExplode = false;                      // 폭발 여부

    /// <summary>
    /// 이 오브젝트의 초기 위치, 회전 값
    /// </summary>
    private Vector3         pos;
    private Quaternion      rot;

    /// <summary>
    /// 초기 위치, 초기 회전 값을 변수에 저장
    /// </summary>
    private void Awake()
    {
        pos = transform.position;
        rot = transform.rotation;
    }

    /// <summary>
    /// 비활성화 될 때 이 오브젝트 초기화
    /// </summary>
    private void OnDisable()
    {
        SetUp();
    }

    /// <summary>
    /// 데미지를 입었을 때 실행 될 함수
    /// </summary>
    public override void TakeDamage(int damage)
    {
        currentHP -= damage;
        // 최초 피격 1회 시 attackExplosionBarrelMission이 클리어 되어있지 않으면 미션을 클리어
        if (attackExplosionBarrelMission == false)
        {
            Mission_AttackExplosionBarrel();
        }

        // 드럼통 체력이 0이하가 되고, 아직 폭발하지 않은 상태면 explosionDelayTime 뒤 폭발 
        if (currentHP <= 0 && isExplode == false)
        {
            
            StartCoroutine("ExplodeBarrel");
        }
    }

    /// <summary>
    /// AttackExplosionBarrel 미션 클리어 시 호출될 함수
    /// </summary>
    private void Mission_AttackExplosionBarrel()
    {
        // 활성화된 미션이 있고 그 미션이 Object의 미션일 때
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) &&
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.Object);
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.Object, objectMissionNumber);
            attackExplosionBarrelMission = true;
        }
    }

    /// <summary>
    /// explosionDelayTime 초 후 폭발 효과 코루틴
    /// </summary>
    private IEnumerator ExplodeBarrel()
    {
        yield return new WaitForSeconds(explosionDelayTime);

        // 근처의 Barrel이 터져서 다시 현재 Barrel을 터트리려고 할 때(StackOverFlow 방지)
        isExplode = true;

        // Bounds형 콜라이더, 폭발 이펙트 생성
        Bounds bounds = GetComponent<Collider>().bounds;
        Instantiate(explosionPrefab, new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), transform.rotation);

        // 폭발 범위에 있는 모든 오브젝트의 Collider 정보를 얻어와 폭발 효과 처리
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            // 폭발 범위에 부딪힌 OBJ가 Player
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(50);
                continue;
            }

            // 폭발 범위에 부딪힌 OBJ가 Enemy
            EnemyFSM enemy = hit.GetComponentInParent<EnemyFSM>();
            if (enemy != null)
            {
                enemy.TakeDamage(300);
                continue;
            }

            // 폭발 범위에 부딪힌 OBJ가 InteractionOBJ
            InteractionObject interaction = hit.GetComponent<InteractionObject>();
            if (interaction != null)
            {
                interaction.TakeDamage(300);
            }

            // 중력을 가지고 있는 오브젝트이면 힘을 받아 밀려나도록
            Rigidbody rigidbody = hit.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        gameObject.GetComponentInParent<BarrelManager>().ReSpawnBarrel(gameObject); // 이 오브젝트 폭발 후 다시 스폰
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 폭발 상태 초기화, 체력 초기화, 위치 초기화, 물리 초기화
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
