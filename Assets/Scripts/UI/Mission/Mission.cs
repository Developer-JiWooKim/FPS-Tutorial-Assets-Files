/// <summary>
/// �̼� ������ Ŭ����
/// </summary>
[System.Serializable]
public class Mission : MissionFunc
{
    public int RoomNum { get; set; }
    public int[] MissionNum { get; set; }
    public string[] Detail{ get; set; }
    public bool[] Completed { get; set; }
    public Mission(int count)
    {
        Setup(count);
    }

    /// <summary>
    /// default init stuff
    /// </summary>
    public void Setup(int missionCount)
    {
        
        MissionNum = new int[missionCount];
        Detail = new string[missionCount];
        Completed = new bool[missionCount];

        for (int i = 0; i < missionCount; i++)
        {
            MissionNum[i] = i;
            Completed[i] = false;
            Detail[i] = null;
        }
    }

    /// <summary>
    /// �̼� ��ȣ�� �޾� �Ϸ� ó��
    /// </summary>
    public void Complete(int _missionNum)
    {
        if (MissionNum[_missionNum] != _missionNum)
        {
            return;
        }
        Completed[_missionNum] = true;
    }

    /// <summary>
    /// �� ��ȣ�� ��ȯ�ϴ� �Լ�
    /// </summary>
    public int GetRoomNum()
    {
        return RoomNum;
    }

    /// <summary>
    /// �̼��� �Ϸ�Ǿ��� �� ������ �Լ�
    /// </summary>
    public void MissionCompletedAction(int missionNum)
    {
    }
}
