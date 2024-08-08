using UnityEngine;
using TMPro;

public class EnemyMission : MonoBehaviour, MissionFunc
{
    [SerializeField]
    private TextMeshProUGUI[]       missionText_UI;

    private const int               roomNum = 2;        // 방 번호
    private const int               missionCount = 2;   // 가진 미션 개수

    private Mission                 enemyMission;       // Mission 클래스 


    /// <summary>
    /// 미션을 초기화(가진 미션 개수 만큼 미션을 초기화)
    /// </summary>
    private void Awake()
    {
        Init(missionCount);
        for (int i = 0; i < missionCount; i++)
        {
            missionText_UI[i].text = enemyMission.Detail[i];
        }
    }

    /// <summary>
    /// 미션 클래스를 생성 후 초기화
    /// </summary>
    private void Init(int missionCount)
    {
        enemyMission = new Mission(missionCount);

        enemyMission.RoomNum = roomNum;
        enemyMission.Detail[0] = "Attack the Enemy Spawn Object";
        enemyMission.Detail[1] = "Kill the Enemies";
    }

    /// <summary>
    /// 미션이 완료되었을 때 실행할 함수(해당 번호의 미션 완료 처리, UI처리)
    /// </summary>
    public void MissionCompletedAction(int missionNum)
    {
        enemyMission.Complete(missionNum);
        missionText_UI[missionNum].fontStyle = FontStyles.Strikethrough;
    }

    /// <summary>
    /// 방 번호를 반환하는 함수
    /// </summary>
    public int GetRoomNum()
    {
        return roomNum;
    }
}