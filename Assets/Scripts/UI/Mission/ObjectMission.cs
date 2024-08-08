using UnityEngine;
using TMPro;

public class ObjectMission : MonoBehaviour, MissionFunc
{
    [SerializeField]
    private TextMeshProUGUI[]       missionText_UI;

    private const int               roomNum = 3;        // �� ��ȣ
    private const int               missionCount = 3;   // ���� �̼� ����

    private Mission                 objectMission;      // Mission Ŭ����

    /// <summary>
    /// �̼��� �ʱ�ȭ(���� �̼� ���� ��ŭ �̼��� �ʱ�ȭ)
    /// </summary>
    private void Awake()
    {        Init(missionCount);
        for (int i = 0; i < missionCount; i++)
        {
            missionText_UI[i].text = objectMission.Detail[i];
        }
    }

    /// <summary>
    /// �̼� Ŭ������ ���� �� �ʱ�ȭ
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
    /// �̼��� �Ϸ�Ǿ��� �� ������ �Լ�(�ش� ��ȣ�� �̼� �Ϸ� ó��, UIó��)
    /// </summary>
    public void MissionCompletedAction(int missionNum)
    {
        objectMission.Complete(missionNum);
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