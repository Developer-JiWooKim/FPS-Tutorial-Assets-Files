using UnityEngine;
using TMPro;

public class ObjectMission : MonoBehaviour, MissionFunc
{
    [SerializeField]
    private TextMeshProUGUI[]       missionText_UI;

    private const int               roomNum = 3;        // 방 번호
    private const int               missionCount = 3;   // 가진 미션 개수

    private Mission                 objectMission;      // Mission 클래스

    /// <summary>
    /// 미션을 초기화(가진 미션 개수 만큼 미션을 초기화)
    /// </summary>
    private void Awake()
    {        Init(missionCount);
        for (int i = 0; i < missionCount; i++)
        {
            missionText_UI[i].text = objectMission.Detail[i];
        }
    }

    /// <summary>
    /// 미션 클래스를 생성 후 초기화
    /// </summary>
    private void Init(int missionCount)
    {
        objectMission = new Mission(missionCount);

        objectMission.RoomNum = roomNum;
        objectMission.Detail[0] = "Interacting with Items";
        objectMission.Detail[1] = "Attack Targets";
        objectMission.Detail[2] = "Attack Explosion Barrel";
    }

    /// <summary>
    /// 미션이 완료되었을 때 실행할 함수(해당 번호의 미션 완료 처리, UI처리)
    /// </summary>
    public void MissionCompletedAction(int missionNum)
    {
        objectMission.Complete(missionNum);
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