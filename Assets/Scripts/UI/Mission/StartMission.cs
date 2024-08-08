using UnityEngine;
using TMPro;

public class StartMission : MonoBehaviour, MissionFunc
{
    [SerializeField]
    private TextMeshProUGUI[]   missionText_UI;

    private const int           roomNum = 0;        // �� ��ȣ
    private const int           missionCount = 2;   // ���� �̼� ����

    private Mission             startMission;       // Mission Ŭ����

    /// <summary>
    /// �̼��� �ʱ�ȭ(���� �̼� ���� ��ŭ �̼��� �ʱ�ȭ)
    /// </summary>
    private void Awake()
    {
        Init(missionCount);
        for (int i = 0; i < missionCount; i++)
        {
            missionText_UI[i].text = startMission.Detail[i];
        }
    }

    /// <summary>
    /// �̼� Ŭ������ ���� �� �ʱ�ȭ
    /// </summary>
    private void Init(int missionCount)
    {
        startMission = new Mission(missionCount);

        startMission.RoomNum = roomNum;
        startMission.Detail[0] = "Try Move";
        startMission.Detail[1] = "Try Dash";
    }

    /// <summary>
    /// �̼��� �Ϸ�Ǿ��� �� ������ �Լ�(�ش� ��ȣ�� �̼� �Ϸ� ó��, UIó��)
    /// </summary>
    public void MissionCompletedAction(int missionNum)
    {
        startMission.Complete(missionNum);
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