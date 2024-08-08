using System;
using UnityEngine;

/// <summary>
/// �����ϴ� �濡���� ���⸦ Ȱ��ȭ ���� �ʰ� ���̵�� �̼� UI�� Ȱ��ȭ �ϹǷ� �ش� Ŭ������ �ۼ�
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
            roomName = (E_RoomName)Enum.Parse(typeof(E_RoomName), this.tag); // �ش� �ݶ��̴��� �±׸� ������ E_RoomNameŸ���� ������ ����

            guideController.GuideEvent(roomName); // �ش� ���� ���̵� UI�� ����

            // �̼��� Ȱ��ȭ, UI�� Ȱ��ȭ
            MissionSystem.missionSystem.ActiveMission((int)E_RoomName.Start);
            
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// �ش� ���� �̸��� ����(Start)
    /// </summary>
    private void Awake()
    {
        roomName = E_RoomName.Start;
    }
}
