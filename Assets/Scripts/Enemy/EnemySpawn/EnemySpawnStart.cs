using UnityEngine;

public class EnemySpawnStart : InteractionObject
{
    public GameObject       enemyMemoryPoolObject;

    /// <summary>
    /// Mission과 관련된 변수
    /// </summary>
    private const int       enemyMissionNumber = 0;
    private bool            attackSpawnObjectMission = false;

    /// <summary>
    /// 이 오브젝트에 데미지를 주면 Enemy들을 스폰
    /// </summary>
    public override void TakeDamage(int damage)
    {
        enemyMemoryPoolObject.GetComponent<EnemyMemoryPool>().SpawnStart();
        // 최초 1회 공격 받을 시 Attack Enemy spawn object mission 클리어
        if (attackSpawnObjectMission == false)
        {
            Mission_AttackSpawnObject();
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// AttackSpawnObject 미션이 클이어 될 때 호출 될 함수
    /// </summary>
    private void Mission_AttackSpawnObject()
    {
        // 활성화된 미션이 있고 그 미션이 Enemy 미션일 때
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) &&
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.Enemy);
        // 완료된 미션 처리(완료 처리, UI 변경)
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.Enemy, enemyMissionNumber);
            attackSpawnObjectMission = true;
        }
    }
}
