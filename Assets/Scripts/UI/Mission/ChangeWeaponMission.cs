using UnityEngine;
using TMPro;

public class ChangeWeaponMission : MonoBehaviour, MissionFunc
{
    [SerializeField]
    private TextMeshProUGUI[]   missionText_UI;

    private const int           roomNum = 1;            // �� ��ȣ
    private const int           missionCount = 3;       // ���� �̼� ����

    private Mission             changeWeaponMission;    // Mission Ŭ����


    /// <summary>
    /// �̼��� �ʱ�ȭ(���� �̼� ���� ��ŭ �̼��� �ʱ�ȭ)
    /// </summary>
    private void Awake()
    {
        Init(missionCount);
        for (int i = 0; i < missionCount; i++)
        {
            missionText_UI[i].text = changeWeaponMission.Detail[i];
        }
    }

    /// <summary>
    /// �̼� Ŭ������ ���� �� �ʱ�ȭ
    /// </summary>
    private void Init(int missionCount)
    {
        changeWeaponMission = new Mission(missionCount);

        changeWeaponMission.RoomNum = roomNum;
        changeWeaponMission.Detail[0] = "Try switch another weapon";
        changeWeaponMission.Detail[1] = "Try running aim mode";
        changeWeaponMission.Detail[2] = "Try different attack motions of your Knife ";
    }

    /// <summary>
    /// �̼��� �Ϸ�Ǿ��� �� ������ �Լ�(�ش� ��ȣ�� �̼� �Ϸ� ó��, UIó��)
    /// </summary>
    public void MissionCompletedAction(int missionNum)
    {
        changeWeaponMission.Complete(missionNum);
        missionText_UI[missionNum].fontStyle = FontStyles.Strikethrough;
    }

    /// <summary>
    /// �� ��ȣ�� ��ȯ�ϴ� �Լ�
    /// </summary>
    public int GetRoomNum()
    {
        return roomNum;
    }
}
