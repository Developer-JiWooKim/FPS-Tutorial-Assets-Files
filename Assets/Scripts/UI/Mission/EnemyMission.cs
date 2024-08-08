using UnityEngine;
using TMPro;

public class EnemyMission : MonoBehaviour, MissionFunc
{
    [SerializeField]
    private TextMeshProUGUI[]       missionText_UI;

    private const int               roomNum = 2;        // �� ��ȣ
    private const int               missionCount = 2;   // ���� �̼� ����

    private Mission                 enemyMission;       // Mission Ŭ���� 


    /// <summary>
    /// �̼��� �ʱ�ȭ(���� �̼� ���� ��ŭ �̼��� �ʱ�ȭ)
    /// </summary>
    private void Awake()
    {
        Init(missionCount);
        for (int i = 0; i < missionCount; i++)
        {
            missionText_UI[i].text = enemyMission.Detail[i];
        }
    }

    /// <summary>
    /// �̼� Ŭ������ ���� �� �ʱ�ȭ
    /// </summary>
    private void Init(int missionCount)
    {
        enemyMission = new Mission(missionCount);

        enemyMission.RoomNum = roomNum;
        enemyMission.Detail[0] = "Attack the Enemy Spawn Object";
        enemyMission.Detail[1] = "Kill the Enemies";
    }

    /// <summary>
    /// �̼��� �Ϸ�Ǿ��� �� ������ �Լ�(�ش� ��ȣ�� �̼� �Ϸ� ó��, UIó��)
    /// </summary>
    public void MissionCompletedAction(int missionNum)
    {
        enemyMission.Complete(missionNum);
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