using UnityEngine;

public class MissionUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject      missionBackground;  // 미션 배경 이미지

    private bool            isMissionUIOn;      // 미션 UI가 켜진지 검사용 변수

    /// <summary>
    /// 변수 초기화 및 미션 배경 비활성화
    /// </summary>
    private void Awake()
    {
        isMissionUIOn = false;
        missionBackground.SetActive(false);
    }

    /// <summary>
    /// isMissionUIOn변수에 따라 미션 UI를 활성화 / 비활성화 시켜주는 함수
    /// </summary>
    public void OnMissionUI(GameObject activeMission)
    {
        StateChange(); // 현재 미션 UI상태를 변경
        if (isMissionUIOn == false)
        {
            MissionUIOffEvent(activeMission);
            return;
        }
        MissionUIEvent(activeMission);
    }

    /// <summary>
    /// isMissionUIOn 상태를 바꾸는 함수
    /// </summary>
    public void StateChange()
    {
        isMissionUIOn = !isMissionUIOn;
    }

    /// <summary>
    /// 미션 UI를 비활성화하는 함수
    /// </summary>
    private void MissionUIOffEvent(GameObject activeMission)
    {
        missionBackground.SetActive(false);
        activeMission.SetActive(false);
    }

    /// <summary>
    /// 미션 UI를 활성화 하는 함수
    /// </summary>
    private void MissionUIEvent(GameObject activeMission)
    {
        missionBackground.SetActive(true); // 미션 배경을 ON
        activeMission.SetActive(true); // 해당 오브젝트를 활성화
    }
}