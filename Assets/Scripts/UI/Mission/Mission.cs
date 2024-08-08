/// <summary>
/// 미션 데이터 클래스
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
    /// 미션 번호를 받아 완료 처리
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
    /// 방 번호를 반환하는 함수
    /// </summary>
    public int GetRoomNum()
    {
        return RoomNum;
    }

    /// <summary>
    /// 미션이 완료되었을 때 실행할 함수
    /// </summary>
    public void MissionCompletedAction(int missionNum)
    {
    }
}
