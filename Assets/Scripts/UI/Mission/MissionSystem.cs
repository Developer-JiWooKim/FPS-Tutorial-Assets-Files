using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 활성화된 미션을 관리하는 클래스
/// </summary>
public class MissionSystem : MonoBehaviour
{
    [SerializeField]
    private MissionUIController     missionController;

    [SerializeField]
    private List<GameObject>        missionList;        // 미션 오브젝트들을 담을 리스트

    public static MissionSystem     missionSystem;      // 싱글톤 패턴 적용 위한 static 객체 생성

    public GameObject               activeMission;      // 활성화된 미션 오브젝트

    public bool                     isActiveMission;    // 미션 활성화 여부 판단용 변수

    /// <summary>
    /// 싱글톤 패턴 적용, 변수 초기화
    /// </summary>
    private void Awake()
    {
        // singleton pattern
        if (missionSystem == null)
        {
            missionSystem = this;
        }
        else if (missionSystem != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        activeMission = null;
        isActiveMission = false;
    }

    /// <summary>
    /// 외부로부터 방 번호를 매개변수로 받아 리스트에 있는 같은 번호를 가진 미션을 활성화 시켜주는 함수 
    /// </summary>
    public void ActiveMission(int _roomNum)
    {
        // MemoryPool에는 미션을 두지 않으므로 MemoryPool 방에 들어갈 때는 미션이 활성화 되지 않게
        if (_roomNum >= missionList.Count)
        {
            Debug.Log("Memory Pool room don't have mission");
            return;
        }

        StateChange(); // 미션이 켜져있으면 끄고, 꺼져 있으면 킴

        // 방에서 나갈 때 미션을 끔
        if (isActiveMission == false)
        {
            missionController.OnMissionUI(activeMission);
            activeMission = null; // 활성화되어있는 오브젝트를 제거
            return;
        }

        // 미션 리스트 중에서 콜라이더로 부터 받은 이름과 같은 방의 이름을 찾아 미션을 활성화
        for (int i = 0; i < missionList.Count; i++)
        {
            if (missionList[i].GetComponent<MissionFunc>().GetRoomNum() == _roomNum)
            {
                activeMission = missionList[i];
                missionController.OnMissionUI(activeMission); // 해당 미션 UI를 활성화
                return;
            }
        }
    }

    /// <summary>
    /// 미션 활성 여부 변수 상태 변환
    /// </summary>
    private void StateChange()
    {
        isActiveMission = !isActiveMission;
    }

    /// <summary>
    /// 활성화된 미션이 완료되었을 때 실행할 함수
    /// </summary>
    public void MissionComplete(int roomNum, int missionNum)
    {
        // 전달 받은 방 번호와 활성화된 미션의 방 번호를 비교
        bool isCheck = ( roomNum == activeMission.GetComponent<MissionFunc>().GetRoomNum() ); 
        if (isCheck)
        {
            // 해당 미션을 가진 방의 같은 번호를 가진 미션을 클리어 시킴
            activeMission.GetComponent<MissionFunc>().MissionCompletedAction(missionNum);
        }
    }
}