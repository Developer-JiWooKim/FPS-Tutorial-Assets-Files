using System.Collections;
using UnityEngine;

public class Target : InteractionObject
{
    /// <summary>
    /// Ÿ�ٿ� ���� ����� Ŭ��
    /// </summary>
    [SerializeField]
    private AudioClip       clipTargetUp;
    [SerializeField]
    private AudioClip       clipTargetDown;

    [SerializeField]
    private float           targetUpDelayTime = 3.0f;       // Ÿ�� ������Ʈ�� �ٽ� �ö������� ���� �ð�

    /// <summary>
    /// �̼ǰ� ���õ� ����
    /// </summary>
    private const int       objectMissionNumber = 1;
    private bool            attackTargetMission = false;

    private bool            isPossibleHit = true;           // Ÿ�� �ǰ� ���� ���� ����

    private AudioSource     audioSource;

    /// <summary>
    /// ������Ʈ�� ������ ������ ����(AudioSource)
    /// </summary>
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// �������� ���� �� ������ �Լ�
    /// </summary>
    public override void TakeDamage(int damage)
    {
        currentHP -= damage;
        // ���� 1ȸ �ǰ� �� attackTargetMission�� Ŭ���� �Ǿ� ���� ������ �̼��� Ŭ���� 
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
    /// AttackTarget �̼� Ŭ���� �� ����� �Լ�
    /// </summary>
    private void Mission_AttackTarget()
    {
        // Ȱ��ȭ�� �̼��� �ְ� �� �̼��� Object �̼��� ��
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) &&
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.Object);
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.Object, objectMissionNumber);
            attackTargetMission = true;
        }
    }

    /// <summary>
    /// Ÿ���� �ǰݵǾ��� �� ����� �ڷ�ƾ �Լ�
    /// </summary>
    private IEnumerator OnTargetDown()
    {
        audioSource.clip = clipTargetDown;
        audioSource.Play();

        yield return StartCoroutine(OnAnimation(0, 90));
        StartCoroutine("OnTargetUp");
    }

    /// <summary>
    /// targetUpDelayTime �ð��� ���� �� Ÿ���� ���� ���·� �ǵ����� �ڷ�ƾ �Լ�
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
    /// Ÿ�� ȸ�� �ִϸ��̼�(Ÿ���� �Ʒ� ���� �������� ȸ���ϵ��� �� ������Ʈ�� �θ�� ����)
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
