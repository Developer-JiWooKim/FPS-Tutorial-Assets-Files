using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ȱ��ȭ�� �̼��� �����ϴ� Ŭ����
/// </summary>
public class MissionSystem : MonoBehaviour
{
    [SerializeField]
    private MissionUIController     missionController;

    [SerializeField]
    private List<GameObject>        missionList;        // �̼� ������Ʈ���� ���� ����Ʈ

    public static MissionSystem     missionSystem;      // �̱��� ���� ���� ���� static ��ü ����

    public GameObject               activeMission;      // Ȱ��ȭ�� �̼� ������Ʈ

    public bool                     isActiveMission;    // �̼� Ȱ��ȭ ���� �Ǵܿ� ����

    /// <summary>
    /// �̱��� ���� ����, ���� �ʱ�ȭ
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
    /// �ܺηκ��� �� ��ȣ�� �Ű������� �޾� ����Ʈ�� �ִ� ���� ��ȣ�� ���� �̼��� Ȱ��ȭ �����ִ� �Լ� 
    /// </summary>
    public void ActiveMission(int _roomNum)
    {
        // MemoryPool���� �̼��� ���� �����Ƿ� MemoryPool �濡 �� ���� �̼��� Ȱ��ȭ ���� �ʰ�
        if (_roomNum >= missionList.Count)
        {
            Debug.Log("Memory Pool room don't have mission");
            return;
        }

        StateChange(); // �̼��� ���������� ����, ���� ������ Ŵ

        // �濡�� ���� �� �̼��� ��
        if (isActiveMission == false)
        {
            missionController.OnMissionUI(activeMission);
            activeMission = null; // Ȱ��ȭ�Ǿ��ִ� ������Ʈ�� ����
            return;
        }

        // �̼� ����Ʈ �߿��� �ݶ��̴��� ���� ���� �̸��� ���� ���� �̸��� ã�� �̼��� Ȱ��ȭ
        for (int i = 0; i < missionList.Count; i++)
        {
            if (missionList[i].GetComponent<MissionFunc>().GetRoomNum() == _roomNum)
            {
                activeMission = missionList[i];
                missionController.OnMissionUI(activeMission); // �ش� �̼� UI�� Ȱ��ȭ
                return;
            }
        }
    }

    /// <summary>
    /// �̼� Ȱ�� ���� ���� ���� ��ȯ
    /// </summary>
    private void StateChange()
    {
        isActiveMission = !isActiveMission;
    }

    /// <summary>
    /// Ȱ��ȭ�� �̼��� �Ϸ�Ǿ��� �� ������ �Լ�
    /// </summary>
    public void MissionComplete(int roomNum, int missionNum)
    {
        // ���� ���� �� ��ȣ�� Ȱ��ȭ�� �̼��� �� ��ȣ�� ��
        bool isCheck = ( roomNum == activeMission.GetComponent<MissionFunc>().GetRoomNum() ); 
        if (isCheck)
        {
            // �ش� �̼��� ���� ���� ���� ��ȣ�� ���� �̼��� Ŭ���� ��Ŵ
            activeMission.GetComponent<MissionFunc>().MissionCompletedAction(missionNum);
        }
    }
}