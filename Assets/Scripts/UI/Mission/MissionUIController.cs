using UnityEngine;

public class MissionUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject      missionBackground;  // �̼� ��� �̹���

    private bool            isMissionUIOn;      // �̼� UI�� ������ �˻�� ����

    /// <summary>
    /// ���� �ʱ�ȭ �� �̼� ��� ��Ȱ��ȭ
    /// </summary>
    private void Awake()
    {
        isMissionUIOn = false;
        missionBackground.SetActive(false);
    }

    /// <summary>
    /// isMissionUIOn������ ���� �̼� UI�� Ȱ��ȭ / ��Ȱ��ȭ �����ִ� �Լ�
    /// </summary>
    public void OnMissionUI(GameObject activeMission)
    {
        StateChange(); // ���� �̼� UI���¸� ����
        if (isMissionUIOn == false)
        {
            MissionUIOffEvent(activeMission);
            return;
        }
        MissionUIEvent(activeMission);
    }

    /// <summary>
    /// isMissionUIOn ���¸� �ٲٴ� �Լ�
    /// </summary>
    public void StateChange()
    {
        isMissionUIOn = !isMissionUIOn;
    }

    /// <summary>
    /// �̼� UI�� ��Ȱ��ȭ�ϴ� �Լ�
    /// </summary>
    private void MissionUIOffEvent(GameObject activeMission)
    {
        missionBackground.SetActive(false);
        activeMission.SetActive(false);
    }

    /// <summary>
    /// �̼� UI�� Ȱ��ȭ �ϴ� �Լ�
    /// </summary>
    private void MissionUIEvent(GameObject activeMission)
    {
        missionBackground.SetActive(true); // �̼� ����� ON
        activeMission.SetActive(true); // �ش� ������Ʈ�� Ȱ��ȭ
    }
}