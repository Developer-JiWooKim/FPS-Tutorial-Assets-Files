using System.Collections;
using UnityEngine;

public class Target : InteractionObject
{
    /// <summary>
    /// 타겟에 사용될 오디오 클립
    /// </summary>
    [SerializeField]
    private AudioClip       clipTargetUp;
    [SerializeField]
    private AudioClip       clipTargetDown;

    [SerializeField]
    private float           targetUpDelayTime = 3.0f;       // 타겟 오브젝트가 다시 올라오기까지 지연 시간

    /// <summary>
    /// 미션과 관련된 변수
    /// </summary>
    private const int       objectMissionNumber = 1;
    private bool            attackTargetMission = false;

    private bool            isPossibleHit = true;           // 타겟 피격 가능 여부 변수

    private AudioSource     audioSource;

    /// <summary>
    /// 컴포넌트를 가져와 변수에 저장(AudioSource)
    /// </summary>
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 데미지를 입을 때 실행할 함수
    /// </summary>
    public override void TakeDamage(int damage)
    {
        currentHP -= damage;
        // 최초 1회 피격 시 attackTargetMission이 클리어 되어 있지 않으며 미션을 클리어 
        if (attackTargetMission == false)
        {
            Mission_AttackTarget();
        }

        if (currentHP <= 0 && isPossibleHit == true)
        {
            isPossibleHit = false;

            StartCoroutine("OnTargetDown");
        }
    }

    /// <summary>
    /// AttackTarget 미션 클리어 시 실행될 함수
    /// </summary>
    private void Mission_AttackTarget()
    {
        // 활성화된 미션이 있고 그 미션이 Object 미션일 때
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) &&
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.Object);
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.Object, objectMissionNumber);
            attackTargetMission = true;
        }
    }

    /// <summary>
    /// 타겟이 피격되었을 때 실행될 코루틴 함수
    /// </summary>
    private IEnumerator OnTargetDown()
    {
        audioSource.clip = clipTargetDown;
        audioSource.Play();

        yield return StartCoroutine(OnAnimation(0, 90));
        StartCoroutine("OnTargetUp");
    }

    /// <summary>
    /// targetUpDelayTime 시간이 지난 후 타겟을 원래 상태로 되돌리는 코루틴 함수
    /// </summary>
    private IEnumerator OnTargetUp()
    {
        yield return new WaitForSeconds(targetUpDelayTime);

        audioSource.clip = clipTargetUp;
        audioSource.Play();

        yield return StartCoroutine(OnAnimation(90, 0));
        isPossibleHit = true;
    }

    /// <summary>
    /// 타겟 회전 애니메이션(타겟이 아래 축을 기준으로 회전하도록 빈 오브젝트를 부모로 설정)
    /// </summary>
    private IEnumerator OnAnimation(float start, float end)
    {
        float percent = 0;
        float current = 0;
        float time = 1f;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            transform.rotation = Quaternion.Lerp(Quaternion.Euler(start, transform.eulerAngles.y, 0), Quaternion.Euler(end, transform.eulerAngles.y, 0), percent);

            yield return null;

        }
    }
}
