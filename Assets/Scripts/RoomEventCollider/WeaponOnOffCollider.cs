using System;
using UnityEngine;

public class WeaponOnOffCollider : MonoBehaviour
{
    [SerializeField]
    private GuideController     guideController;

    /// <summary>
    /// �÷��̾�� ���� ��(ĳ���Ͱ� �濡 �� �� or ���� ��)
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<WeaponSwitchSystem>().SetIsSwitchWeapon();
            other.GetComponent<PlayerController>().IsWeaponOnOff = !other.GetComponent<PlayerController>().IsWeaponOnOff;

            // �ش� �ݶ��̴��� �±׸� ������ E_RoomNameŸ���� ������ ����
            E_RoomName roomName = (E_RoomName)Enum.Parse(typeof(E_RoomName), this.tag); 
            guideController.GuideEvent(roomName); // �ش� ���� ���̵� UI�� ����

            MissionSystem.missionSystem.ActiveMission((int)roomName); // �ش� ���� �̼� Ȱ��ȭ
        }
    }
}
