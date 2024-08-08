using System.Collections;
using UnityEngine;

public class ItemMedicBag : ItemBase
{
    [SerializeField]
    private GameObject      hpEffectPrefab;                     // 회복아이템 오브젝트와 상호 작용 시 실행될 이펙트 프리팹
    [SerializeField]
    private int             increaseHP = 50;                    // 체력 증가 수치
    [SerializeField]
    private float           moveDistance = 0.2f;                // 위아래로 움직이는 속도
    [SerializeField]
    private float           pingpongSpeed = 0.5f;               // pingpong 속도
    [SerializeField]
    private float           rotateSpeed = 50.0f;                // 회전 속도

    /// <summary>
    /// 미션과 관련된 변수들
    /// </summary>
    private const int       objectMissionNumber = 0;
    private bool            itemInteractionMission = false;

    private Vector3         pos;                                // 초기 위치
    private GameObject      effect;                             // 실제 게임에서 쓰일 hpEffect

    /// <summary>
    /// ItemMedicBag 오브젝트가 활성화 될 때 effect 오브젝트 비활성화 및 StartPingPong 코루틴 실행
    /// </summary>
    private void OnEnable()
    {
        effect.SetActive(false);
        StartCoroutine("StartPingPong");
    }

    /// <summary>
    /// hpEffectPrefab 저장된 프리팹을 effect 오브젝트 변수에 저장, 초기 위치를 변수에 저장
    /// </summary>
    private void Awake()
    {
        pos = transform.position;
        effect = Instantiate(hpEffectPrefab, transform.position, Quaternion.identity);
    }

    /// <summary>
    /// ItemMedicBag 오브젝트 를 PingPong시키는 코루틴 함수
    /// </summary>
    private IEnumerator StartPingPong()
    {
        float y = pos.y;
        while (true)
        {
            // y축 기준 회전
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

            // 처음 배치된 위치를 기준으로 y위치를 위, 아래로 이동
            Vector3 position = pos;
            position.y = Mathf.Lerp(y, y + moveDistance, Mathf.PingPong(Time.time * pingpongSpeed, 1));
            transform.position = position;

            yield return null;
        }
    }

    /// <summary>
    /// ItemBase에서 상속받은 추상 메소드를 오버로드, 해당 아이템(ItemMedicBag 오브젝트) 사용 시 할 일
    /// </summary>
    public override void Use(GameObject entity)
    {
        // 최초 1회 상호 작용 시 itemInteractionMission 이 클리어 되어있지 않으면 미션 클리어
        if (itemInteractionMission == false)
        {
            Mission_ItemInteraction();
        }

        // 상호작용한 대상의 체력을 증가시키는 IncreaseHP 함수 실행
        entity.GetComponent<Status>().IncreaseHP(increaseHP);

        // 체력 회복 이펙트 재생
        effect.SetActive(true);
        effect.GetComponent<ParticleSystem>().Play();

        // 아이템 매니저로 ItemMedicbag 오브젝트 리스폰
        GetComponentInParent<ItemRespawnManager>().ReSpawnItem(gameObject);

        // ItemMedicbag 오브젝트 비활성화
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ItemInteraction 미션이 클리어될 때 호출될 함수
    /// </summary>
    private void Mission_ItemInteraction() 
    {
        // 활성화된 미션이 있고 그 미션이 Object의 미션일 때
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) &&
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.Object);
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.Object, objectMissionNumber);
            itemInteractionMission = true;
        }
    }
}
