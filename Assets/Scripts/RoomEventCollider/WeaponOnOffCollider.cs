using System;
using UnityEngine;

public class WeaponOnOffCollider : MonoBehaviour
{
    [SerializeField]
    private GuideController     guideController;

    /// <summary>
    /// 플레이어와 접촉 시(캐릭터가 방에 들어갈 때 or 나갈 때)
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<WeaponSwitchSystem>().SetIsSwitchWeapon();
            other.GetComponent<PlayerController>().IsWeaponOnOff = !other.GetComponent<PlayerController>().IsWeaponOnOff;

            // 해당 콜라이더의 태그를 가져와 E_RoomName타입의 변수에 저장
            E_RoomName roomName = (E_RoomName)Enum.Parse(typeof(E_RoomName), this.tag); 
            guideController.GuideEvent(roomName); // 해당 방의 가이드 UI를 실행

            MissionSystem.missionSystem.ActiveMission((int)roomName); // 해당 방의 미션 활성화
        }
    }
}
