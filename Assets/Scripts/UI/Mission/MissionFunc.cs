/// <summary>
/// 미션을 가지는 클래스가 상속받을 인터페이스
/// </summary>
public interface MissionFunc
{
    public int GetRoomNum();
    public void MissionCompletedAction(int missionNum);
}
