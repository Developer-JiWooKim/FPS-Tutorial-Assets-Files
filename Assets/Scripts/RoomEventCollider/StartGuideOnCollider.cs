using System;
using UnityEngine;

/// <summary>
/// 시작하는 방에서는 무기를 활성화 하지 않고 가이드와 미션 UI만 활성화 하므로 해당 클래스를 작성
/// </summary>
public class StartGuideOnCollider : MonoBehaviour
{
    [SerializeField]
    private GuideController     guideController;

    private E_RoomName          roomName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            roomName = (E_RoomName)Enum.Parse(typeof(E_RoomName), this.tag); // 해당 콜라이더의 태그를 가져와 E_RoomName타입의 변수에 저장

            guideController.GuideEvent(roomName); // 해당 방의 가이드 UI를 실행

            // 미션을 활성화, UI를 활성화
            MissionSystem.missionSystem.ActiveMission((int)E_RoomName.Start);
            
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 해당 방의 이름을 저장(Start)
    /// </summary>
    private void Awake()
    {
        roomName = E_RoomName.Start;
    }
}
