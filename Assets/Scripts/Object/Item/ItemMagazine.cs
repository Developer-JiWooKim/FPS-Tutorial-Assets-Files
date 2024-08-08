using System.Collections;
using UnityEngine;

public class ItemMagazine : ItemBase
{
    [SerializeField]
    private GameObject      magazineEffectPrefab;               // 탄창 오브젝트와 상호 작용 시 실행될 이펙트 프리팹
    [SerializeField]
    private int             increaseMagazine = 2;               // 탄창 증가 수치
    [SerializeField]
    private float           rotateSpeed = 50f;              

    /// <summary>
    /// 미션과 관련된 변수들
    /// </summary>
    private const int       objectMissionNumber = 0;
    private bool            itemInteractionMission = false;

    private GameObject      effect;                             // 실제 게임에서 쓰일 magazineEffect

    /// <summary>
    /// magazineEffectPrefab에 저장된 프리팹을 effect 오브젝트 변수에 저장 후 비활성화
    /// </summary>
    private void Awake()
    {
        effect = Instantiate(magazineEffectPrefab, transform.position, Quaternion.identity);
        effect.SetActive(false);
    }

    /// <summary>
    /// 탄창 오브젝트가 활성화 될 때 이펙트를 비활성화, 탄창 오브젝트 회전 코루틴 실행
    /// </summary>
    private void OnEnable()
    {
        effect.SetActive(false);
        StartCoroutine("StartRotate");
    }

    /// <summary>
    /// y축 기준으로 회전시켜주는 코루틴 함수
    /// </summary>
    private IEnumerator StartRotate()
    {
        while (true)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

            yield return null;
        }
    }

    /// <summary>
    /// ItemBase에서 상속받은 추상 메소드를 오버로드, 해당 아이템(ItemMagazine 오브젝트) 사용 시 할 일
    /// </summary>
    public override void Use(GameObject entity)
    {
        // 최초 1회 상호 작용 시 itemInteractionMission 이 클리어 되어있지 않으면 미션 클리어
        if (itemInteractionMission == false)
        {
            Mission_ItemInteraction();
        }

        // 무기를 관리하는 WeaponSwitchSystem클래스의 IncreaseMagazine()메소드를 호출
        entity.GetComponent<WeaponSwitchSystem>().IncreaseMagazine(entity.GetComponent<WeaponSwitchSystem>().GetCurrentWeaponType(), increaseMagazine);

        // 탄창이 증가했으므로 탄창이 고갈되면 실행되는 메세지를 초기화해서 나타나지 않게
        entity.GetComponentInChildren<WeaponBase>().ResetMagazineMessage();

        // 이펙트를 재생(활성화)
        effect.SetActive(true);
        effect.GetComponent<ParticleSystem>().Play();

        // 아이템 매니저로 탄창 오브젝트를 n초 뒤에 다시 활성화
        GetComponentInParent<ItemRespawnManager>().ReSpawnItem(gameObject);

        // 이 오브젝트 비활성화
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
