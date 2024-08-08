using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { None = -1, Idle = 0, Wander, Pursuit, Attack, }

public class EnemyFSM : MonoBehaviour
{
    /// <summary>
    /// Pursuit ���¿��� ����ϴ� ������
    /// </summary>
    [Header("Pursuit")]
    [SerializeField]
    private float           targetRecognitionRange = 4f;        // �ν� ����(�� ���� �ȿ� ������ "Pursuit(����)"���·� ����)
    [SerializeField]
    private float           PursuitLimitRange = 6f;             // ���� ����(�� ���� �ٱ����� ������ "Wander(��ȸ)"���·� ����)

    /// <summary>
    /// Attack ���¿��� ����ϴ� ����, ������
    /// </summary>
    [Header("Attack")]
    [SerializeField]
    private GameObject      projectilePrefab;                   // �߻�ü ������
    [SerializeField]
    private Transform       projectileSpawnPoint;               // �߻�ü ���� ��ġ
    [SerializeField]
    private float           attackRange = 2.5f;                 // ���� ����(�� ���� �ȿ� ������ "Attack(����)" ���·� ����) 
    [SerializeField]
    private float           attackRate = 1f;                    // ���� �ֱ�

    /// <summary>
    /// �̼ǰ� ���õ� ����
    /// </summary>
    private const int       enemyMissionNumber = 1;             // Enemy�� óġ�ϴ� �̼� ��ȣ
    private bool            enemyKillMission = false;           // Enemy Kill Mission Ŭ���� ����

    /// <summary>
    /// Enemy ���� ����
    /// </summary>
    private EnemyState      enemyState = EnemyState.None;

    private float           lastAttackTime = 0f;                // ���� �ֱ� ���� ����

    private Status          status;                             // �̵��ӵ� ���� ����
    private NavMeshAgent    navMeshAgent;                       // �̵� ��� ���� NavMeshAgent
    private Transform       target;                             // Enemy�� ���� ���(Player ��..)
    private EnemyMemoryPool enemyMemoryPool;                    // Enemy Memory Pool(Enemy ������Ʈ ��Ȱ��ȭ�� ���)

    /// <summary>
    /// Enemy�� �ʱ� ���¸� ����
    /// </summary>
    public void Setup(Transform _target, EnemyMemoryPool _enemyMemoryPool)
    {
        // ������Ʈ���� �����ų�, Ÿ���� ����
        status = GetComponent<Status>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        target = _target;
        enemyMemoryPool = _enemyMemoryPool;

        // Enemy�� ü�¹�(UI)�� ����
        status.SetUpHP();
        GetComponentInChildren<EnemyHealthBar>().SetupHPBar();

        // NavMeshAgent ������Ʈ���� ȸ���� ������Ʈ���� �ʵ��� ����
        navMeshAgent.updateRotation = false;
    }

    /// <summary>
    /// Enemy�� Ȱ��ȭ�� �� Enemy�� ���¸� "���(Idle)"�� ����
    /// </summary>
    private void OnEnable()
    {
        ChangeState(EnemyState.Idle);
    }

    /// <summary>
    /// Enemy�� ��Ȱ��ȭ�� �� ���� ��� ���� ���¸� �����ϰ�, ���¸� "None"���� ����
    /// </summary>
    private void OnDisable()
    {
        
        StopCoroutine(enemyState.ToString());
        enemyState = EnemyState.None;
        // Enemy Kill Mission�� Ŭ���� ���� �ʾ����� Mission_EnemyKill() �Լ� ȣ��
        if (enemyKillMission == false)
        {
            Mission_EnemyKill();
        }
        
    }

    /// <summary>
    /// Enemy Kill Mission �Ϸ� �� ȣ��� �Լ�
    /// </summary>
    private void Mission_EnemyKill()
    {
        if (MissionSystem.missionSystem == null)
        {
            return;
        }

        // Ȱ��ȭ�� �̼��� �ְ� �� �̼��� Enemy Room�� �̼��� ��
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) &&
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.Enemy);
        // �Ϸ�� �̼� ó��(�Ϸ� ó��, UI ����)
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.Enemy, enemyMissionNumber); 
            enemyKillMission = true;
        }
    }

    /// <summary>
    /// ���� ���� �� ȣ��Ǵ� �Լ�
    /// </summary>
    public void ChangeState(EnemyState newState)
    {
        // ���� ������� ���¿� �ٲٷ��� �ϴ� ���°� ������ �ٲ� �ʿ䰡 x
        if (enemyState == newState)
        {
            return;
        }

        // ������ ������̴� ���� ����
        StopCoroutine(enemyState.ToString());
        
        // ���� ���� ���¸� ����� ����(newState)�� ����
        enemyState = newState;

        // ���ο� ���� ���
        StartCoroutine(enemyState.ToString());
    }

    /// <summary>
    /// Idle(���) ������ �� �ϴ� �ൿ
    /// </summary>
    private IEnumerator Idle()
    {
        // Ÿ�ϰ��� �Ÿ��� ���� �ൿ ����(��ȸ, ����, ���Ÿ� ����)
        StartCoroutine("AutoChangeFromIdleToWander");
        while(true)
        {
            CalculateDistanceToTargetAndSelectState();
            yield return null;
        }
    }

    /// <summary>
    /// Idle ���¿��� n�� �� Wander(��ȸ) ���·� �ڵ����� �������ִ� �Լ�
    /// </summary>
    private IEnumerator AutoChangeFromIdleToWander()
    {
        // 1 ~ 4�� �ð� ���
        int changeTime = Random.Range(1, 5);
        yield return new WaitForSeconds(changeTime);

        // ���¸� "Wander(��ȸ)"�� ����
        ChangeState(EnemyState.Wander);
    }

    /// <summary>
    /// Wander(��ȸ) ������ �� �ϴ� �ൿ
    /// </summary>
    private IEnumerator Wander()
    {
        float currentTime = 0;
        float maxTime = 10;

        // �̵� �ӵ� ����
        navMeshAgent.speed = status.WalkSpeed;

        // ��ǥ ��ġ ����
        navMeshAgent.SetDestination(CalculateWanderPosition());

        // ��ǥ ��ġ�� ȸ��
        Vector3 to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);

        // ��ǥ��ġ�� �����ϰ� �����ϰų� �ʹ� �����ð� ���� ��ȸ�ϱ� ���¿� �ӹ��� ������
        // ���¸� "Idle(���)"�� ����
        while (true)
        {
            currentTime += Time.deltaTime;
            to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
            from = new Vector3(transform.position.x, 0, transform.position.z);

            if ((to - from).sqrMagnitude < 0.01f || currentTime >= maxTime)
            {
                ChangeState(EnemyState.Idle);
            }

            // Ÿ�ϰ��� �Ÿ��� ���� �ൿ ����(��ȸ, ����, ���Ÿ� ����)
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    /// <summary>
    /// Pursuit(�߰�) ������ �� �ϴ� �ൿ
    /// </summary>
    private IEnumerator Pursuit()
    {
        while(true)
        {
            // �̵��ӵ� ����(��ȸ�� �� �ȴ� �ӵ��� �̵�, ������ ���� �ٴ� �ӵ��� �̵�)
            navMeshAgent.speed = status.RunSpeed;

            // ��ǥ��ġ�� ����� ��ġ�� ����
            navMeshAgent.SetDestination(target.position);

            // ��ǥ �������� ��� �ֽ�
            LookRotationToTarget();

            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ����(��ȸ, �߰�, ���Ÿ� ����)
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    /// <summary>
    /// Attack(����) ������ �� �ϴ� �ൿ
    /// </summary>
    private IEnumerator Attack()
    {
        // ���� �� �̵��� ����
        navMeshAgent.ResetPath();

        while (true)
        {
            // ��ǥ �������� ��� �ֽ�
            LookRotationToTarget();

            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ����(��ȸ, �߰�, ���Ÿ� ����)
            CalculateDistanceToTargetAndSelectState();

            if (Time.time - lastAttackTime > attackRate)
            {
                // ���� �ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð��� ����
                lastAttackTime = Time.time;

                // �߻�ü ����
                GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                clone.GetComponent<EnemyProjectile>().Setup(target.position);
            }
            yield return null;
        }
    }

    /// <summary>
    /// Wander(��ȸ)�Ҷ� ��ǥ ��ġ ��� �Լ�
    /// </summary>
    /// <returns> targetPosition(��ǥ ��ġ) </returns>
    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10f; // ���� ��ġ�� �������� �ϴ� ���� ������
        int wanderJitter = 0; // ���õ� ����(wanderJitterMin ~ wanderJitterMax)
        int wanderJitterMin = 0; // �ּ� ����
        int wanderJitterMax = 360; // �ִ� ����

        // ���� Enemy�� �ִ� ������ �߽���ġ�� ũ��(������ ��� �ൿ�� ���� �ʵ���)
        Vector3 rangePosition = new Vector3(26f, 0.01f, -32f);
        Vector3 rangeScale = new Vector3(30f, 0.1f, 35f);

        // �ڽ��� ��ġ�� �߽����� ������(wanderRadius) �Ÿ�, ���õ� ����(wanderJitter)�� ��ġ�� ��ǥ�� ��ǥ�������� ����
        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);
    
        // ������ ��ǥ��ġ�� �ڽ��� �̵������� ����� �ʰ� ����
        targetPosition.x = Mathf.Clamp(targetPosition.x, rangePosition.x - (rangeScale.x / 2f), rangePosition.x + (rangeScale.x / 2f));
        targetPosition.y = 0.0f;
        targetPosition.z = Mathf.Clamp(targetPosition.z, rangePosition.z - (rangeScale.z / 2f), rangePosition.z + (rangeScale.z / 2f));

        return targetPosition;
    }

    /// <summary>
    /// Ÿ�ٰ��� �Ÿ��� ���� �� �Ÿ��� ���� ���¸� �������ִ� �Լ�
    /// </summary>
    private void CalculateDistanceToTargetAndSelectState()
    {
        if (target == null)
        {
            return;
        }

        // �÷��̾�(target) �� ���� �Ÿ� ��� �� �Ÿ��� ���� �ൿ ����
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
    /// Ÿ���� ���� ȸ�������ִ� �Լ�
    /// </summary>
    private void LookRotationToTarget()
    {
        // ��ǥ ��ġ
        Vector3 to = new Vector3(target.position.x, 0, target.position.z);

        // �� ��ġ
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

        // ������ ����
        Quaternion rotation = Quaternion.LookRotation(to - from);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);

        // �ٷ� ����
        //transform.rotation = Quaternion.LookRotation(to - from);
    }

    /// <summary>
    /// �������� ������ �޾� ��ġ�� ����ϴ� �Լ�
    /// </summary>
    private Vector3 SetAngle(float radius, int angle)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;

        return position;
    }

    /// <summary>
    /// �������� �Ծ��� �� ���� �� �Լ�
    /// </summary>
    public void TakeDamage(int damage)
    {
        bool isDie = status.DecreaseHP(damage);
        // Enemy ü���� 0���ϰ� �Ǿ� ������ ��Ȱ��ȭ
        if (isDie == true)
        {
            enemyMemoryPool.DeactivateEnemy(gameObject);
        }

        // Enemy�� ü�¹� UI�� ������Ʈ(���� ������ ��ŭ ü�� ����)
        GetComponentInChildren<EnemyHealthBar>().UpdateHPBar(status.MaxHP, status.CurrentHP);
    }

    /// <summary>
    /// �����Ϳ��� ������ Gizmo 
    /// ���, �ν�, ����, ���� �������� ������ ����� �׷��� ���� ���̰�  
    /// </summary>
    private void OnDrawGizmos()
    {
        // ��ȸ ������ �� �̵��� ��� ǥ��(Black)
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);

        // ��ǥ �ν� ����(Red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        // ���� ����(Green)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, PursuitLimitRange);

        // ���� ����
        Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
