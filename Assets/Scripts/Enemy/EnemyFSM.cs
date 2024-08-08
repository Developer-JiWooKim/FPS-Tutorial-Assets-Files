using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { None = -1, Idle = 0, Wander, Pursuit, Attack, }

public class EnemyFSM : MonoBehaviour
{
    /// <summary>
    /// Pursuit 상태에서 사용하는 변수들
    /// </summary>
    [Header("Pursuit")]
    [SerializeField]
    private float           targetRecognitionRange = 4f;        // 인식 범위(이 범위 안에 들어오면 "Pursuit(추적)"상태로 변경)
    [SerializeField]
    private float           PursuitLimitRange = 6f;             // 추적 범위(이 범위 바깥으로 나가면 "Wander(배회)"상태로 변경)

    /// <summary>
    /// Attack 상태에서 사용하는 변수, 프리팹
    /// </summary>
    [Header("Attack")]
    [SerializeField]
    private GameObject      projectilePrefab;                   // 발사체 프리팹
    [SerializeField]
    private Transform       projectileSpawnPoint;               // 발사체 생성 위치
    [SerializeField]
    private float           attackRange = 2.5f;                 // 공격 범위(이 범위 안에 들어오면 "Attack(공격)" 상태로 변경) 
    [SerializeField]
    private float           attackRate = 1f;                    // 공격 주기

    /// <summary>
    /// 미션과 관련된 변수
    /// </summary>
    private const int       enemyMissionNumber = 1;             // Enemy를 처치하는 미션 번호
    private bool            enemyKillMission = false;           // Enemy Kill Mission 클리어 여부

    /// <summary>
    /// Enemy 상태 변수
    /// </summary>
    private EnemyState      enemyState = EnemyState.None;

    private float           lastAttackTime = 0f;                // 공격 주기 계산용 변수

    private Status          status;                             // 이동속도 등의 정보
    private NavMeshAgent    navMeshAgent;                       // 이동 제어를 위한 NavMeshAgent
    private Transform       target;                             // Enemy의 공격 대상(Player 등..)
    private EnemyMemoryPool enemyMemoryPool;                    // Enemy Memory Pool(Enemy 오브젝트 비활성화에 사용)

    /// <summary>
    /// Enemy의 초기 상태를 설정
    /// </summary>
    public void Setup(Transform _target, EnemyMemoryPool _enemyMemoryPool)
    {
        // 컴포넌트들을 얻어오거나, 타겟을 성정
        status = GetComponent<Status>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        target = _target;
        enemyMemoryPool = _enemyMemoryPool;

        // Enemy의 체력바(UI)를 설정
        status.SetUpHP();
        GetComponentInChildren<EnemyHealthBar>().SetupHPBar();

        // NavMeshAgent 컴포넌트에서 회전을 업데이트하지 않도록 설정
        navMeshAgent.updateRotation = false;
    }

    /// <summary>
    /// Enemy가 활성화될 때 Enemy의 상태를 "대기(Idle)"로 설정
    /// </summary>
    private void OnEnable()
    {
        ChangeState(EnemyState.Idle);
    }

    /// <summary>
    /// Enemy가 비활성화될 때 현재 재생 중인 상태를 종료하고, 상태를 "None"으로 설정
    /// </summary>
    private void OnDisable()
    {
        
        StopCoroutine(enemyState.ToString());
        enemyState = EnemyState.None;
        // Enemy Kill Mission을 클리어 하지 않았으면 Mission_EnemyKill() 함수 호출
        if (enemyKillMission == false)
        {
            Mission_EnemyKill();
        }
        
    }

    /// <summary>
    /// Enemy Kill Mission 완료 시 호출될 함수
    /// </summary>
    private void Mission_EnemyKill()
    {
        if (MissionSystem.missionSystem == null)
        {
            return;
        }

        // 활성화된 미션이 있고 그 미션이 Enemy Room의 미션일 때
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) &&
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.Enemy);
        // 완료된 미션 처리(완료 처리, UI 변경)
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.Enemy, enemyMissionNumber); 
            enemyKillMission = true;
        }
    }

    /// <summary>
    /// 상태 변경 시 호출되는 함수
    /// </summary>
    public void ChangeState(EnemyState newState)
    {
        // 현재 재생중인 상태와 바꾸려고 하는 상태가 같으면 바꿀 필요가 x
        if (enemyState == newState)
        {
            return;
        }

        // 이전에 재생중이던 상태 종료
        StopCoroutine(enemyState.ToString());
        
        // 현재 적의 상태를 변경된 상태(newState)로 설정
        enemyState = newState;

        // 새로운 상태 재생
        StartCoroutine(enemyState.ToString());
    }

    /// <summary>
    /// Idle(대기) 상태일 때 하는 행동
    /// </summary>
    private IEnumerator Idle()
    {
        // 타켓과의 거리에 따라 행동 선택(배회, 추적, 원거리 공격)
        StartCoroutine("AutoChangeFromIdleToWander");
        while(true)
        {
            CalculateDistanceToTargetAndSelectState();
            yield return null;
        }
    }

    /// <summary>
    /// Idle 상태에서 n초 후 Wander(배회) 상태로 자동으로 변경해주는 함수
    /// </summary>
    private IEnumerator AutoChangeFromIdleToWander()
    {
        // 1 ~ 4초 시간 대기
        int changeTime = Random.Range(1, 5);
        yield return new WaitForSeconds(changeTime);

        // 상태를 "Wander(배회)"로 변경
        ChangeState(EnemyState.Wander);
    }

    /// <summary>
    /// Wander(배회) 상태일 때 하는 행동
    /// </summary>
    private IEnumerator Wander()
    {
        float currentTime = 0;
        float maxTime = 10;

        // 이동 속도 설정
        navMeshAgent.speed = status.WalkSpeed;

        // 목표 위치 설정
        navMeshAgent.SetDestination(CalculateWanderPosition());

        // 목표 위치로 회전
        Vector3 to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);

        // 목표위치에 근접하게 도달하거나 너무 오랜시간 동안 배회하기 상태에 머물러 있으면
        // 상태를 "Idle(대기)"로 변경
        while (true)
        {
            currentTime += Time.deltaTime;
            to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
            from = new Vector3(transform.position.x, 0, transform.position.z);

            if ((to - from).sqrMagnitude < 0.01f || currentTime >= maxTime)
            {
                ChangeState(EnemyState.Idle);
            }

            // 타켓과의 거리에 따라 행동 선택(배회, 추적, 원거리 공격)
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    /// <summary>
    /// Pursuit(추격) 상태일 때 하는 행동
    /// </summary>
    private IEnumerator Pursuit()
    {
        while(true)
        {
            // 이동속도 설정(배회할 때 걷는 속도로 이동, 추적할 때는 뛰는 속도로 이동)
            navMeshAgent.speed = status.RunSpeed;

            // 목표위치를 대상의 위치로 설정
            navMeshAgent.SetDestination(target.position);

            // 목표 방향으로 계속 주시
            LookRotationToTarget();

            // 타겟과의 거리에 따라 행동 선택(배회, 추격, 원거리 공격)
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    /// <summary>
    /// Attack(공격) 상태일 때 하는 행동
    /// </summary>
    private IEnumerator Attack()
    {
        // 공격 시 이동을 멈춤
        navMeshAgent.ResetPath();

        while (true)
        {
            // 목표 방향으로 계속 주시
            LookRotationToTarget();

            // 타겟과의 거리에 따라 행동 선택(배회, 추격, 원거리 공격)
            CalculateDistanceToTargetAndSelectState();

            if (Time.time - lastAttackTime > attackRate)
            {
                // 공격 주기가 되어야 공격할 수 있도록 하기 위해 현재 시간을 저장
                lastAttackTime = Time.time;

                // 발사체 생성
                GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                clone.GetComponent<EnemyProjectile>().Setup(target.position);
            }
            yield return null;
        }
    }

    /// <summary>
    /// Wander(배회)할때 목표 위치 계산 함수
    /// </summary>
    /// <returns> targetPosition(목표 위치) </returns>
    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10f; // 현재 위치를 원점으로 하는 원의 반지름
        int wanderJitter = 0; // 선택된 각도(wanderJitterMin ~ wanderJitterMax)
        int wanderJitterMin = 0; // 최소 각도
        int wanderJitterMax = 360; // 최대 각도

        // 현재 Enemy가 있는 월드의 중심위치와 크기(구역을 벗어난 행동을 하지 않도록)
        Vector3 rangePosition = new Vector3(26f, 0.01f, -32f);
        Vector3 rangeScale = new Vector3(30f, 0.1f, 35f);

        // 자신의 위치를 중심으로 반지름(wanderRadius) 거리, 선택된 각도(wanderJitter)에 위치한 좌표를 목표지점으로 설정
        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);
    
        // 생성된 목표위치가 자신의 이동구역을 벗어나지 않게 조절
        targetPosition.x = Mathf.Clamp(targetPosition.x, rangePosition.x - (rangeScale.x / 2f), rangePosition.x + (rangeScale.x / 2f));
        targetPosition.y = 0.0f;
        targetPosition.z = Mathf.Clamp(targetPosition.z, rangePosition.z - (rangeScale.z / 2f), rangePosition.z + (rangeScale.z / 2f));

        return targetPosition;
    }

    /// <summary>
    /// 타겟과의 거리를 구한 후 거리에 따라 상태를 변경해주는 함수
    /// </summary>
    private void CalculateDistanceToTargetAndSelectState()
    {
        if (target == null)
        {
            return;
        }

        // 플레이어(target) 와 적의 거리 계산 후 거리에 따라 행동 선택
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= attackRange )
        {
            ChangeState(EnemyState.Attack);
        }
        else if (distance <= targetRecognitionRange)
        {
            ChangeState(EnemyState.Pursuit);
        }
        else if (distance >= PursuitLimitRange)
        {
            ChangeState(EnemyState.Wander);
        }
    }

    /// <summary>
    /// 타겟을 향해 회전시켜주는 함수
    /// </summary>
    private void LookRotationToTarget()
    {
        // 목표 위치
        Vector3 to = new Vector3(target.position.x, 0, target.position.z);

        // 내 위치
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

        // 서서히 돌기
        Quaternion rotation = Quaternion.LookRotation(to - from);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);

        // 바로 돌기
        //transform.rotation = Quaternion.LookRotation(to - from);
    }

    /// <summary>
    /// 반지름과 각도를 받아 위치를 계산하는 함수
    /// </summary>
    private Vector3 SetAngle(float radius, int angle)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;

        return position;
    }

    /// <summary>
    /// 데미지를 입었을 때 실행 될 함수
    /// </summary>
    public void TakeDamage(int damage)
    {
        bool isDie = status.DecreaseHP(damage);
        // Enemy 체력이 0이하가 되어 죽으면 비활성화
        if (isDie == true)
        {
            enemyMemoryPool.DeactivateEnemy(gameObject);
        }

        // Enemy의 체력바 UI를 업데이트(받은 데미지 만큼 체력 감소)
        GetComponentInChildren<EnemyHealthBar>().UpdateHPBar(status.MaxHP, status.CurrentHP);
    }

    /// <summary>
    /// 에디터에서 보여질 Gizmo 
    /// 경로, 인식, 추적, 공격 범위들을 설정된 색깔로 그려서 눈에 보이게  
    /// </summary>
    private void OnDrawGizmos()
    {
        // 배회 상태일 때 이동할 경로 표시(Black)
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);

        // 목표 인식 범위(Red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        // 추적 범위(Green)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, PursuitLimitRange);

        // 공격 범위
        Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
