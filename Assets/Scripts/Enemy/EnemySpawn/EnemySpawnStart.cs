using UnityEngine;

public class EnemySpawnStart : InteractionObject
{
    public GameObject       enemyMemoryPoolObject;

    /// <summary>
    /// Mission�� ���õ� ����
    /// </summary>
    private const int       enemyMissionNumber = 0;
    private bool            attackSpawnObjectMission = false;

    /// <summary>
    /// �� ������Ʈ�� �������� �ָ� Enemy���� ����
    /// </summary>
    public override void TakeDamage(int damage)
    {
        enemyMemoryPoolObject.GetComponent<EnemyMemoryPool>().SpawnStart();
        // ���� 1ȸ ���� ���� �� Attack Enemy spawn object mission Ŭ����
        if (attackSpawnObjectMission == false)
        {
            Mission_AttackSpawnObject();
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// AttackSpawnObject �̼��� Ŭ�̾� �� �� ȣ�� �� �Լ�
    /// </summary>
    private void Mission_AttackSpawnObject()
    {
        // Ȱ��ȭ�� �̼��� �ְ� �� �̼��� Enemy �̼��� ��
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) &&
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.Enemy);
        // �Ϸ�� �̼� ó��(�Ϸ� ó��, UI ����)
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.Enemy, enemyMissionNumber);
            attackSpawnObjectMission = true;
        }
    }
}
