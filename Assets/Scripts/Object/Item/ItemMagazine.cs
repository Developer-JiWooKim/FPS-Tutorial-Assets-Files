using System.Collections;
using UnityEngine;

public class ItemMagazine : ItemBase
{
    [SerializeField]
    private GameObject      magazineEffectPrefab;               // źâ ������Ʈ�� ��ȣ �ۿ� �� ����� ����Ʈ ������
    [SerializeField]
    private int             increaseMagazine = 2;               // źâ ���� ��ġ
    [SerializeField]
    private float           rotateSpeed = 50f;              

    /// <summary>
    /// �̼ǰ� ���õ� ������
    /// </summary>
    private const int       objectMissionNumber = 0;
    private bool            itemInteractionMission = false;

    private GameObject      effect;                             // ���� ���ӿ��� ���� magazineEffect

    /// <summary>
    /// magazineEffectPrefab�� ����� �������� effect ������Ʈ ������ ���� �� ��Ȱ��ȭ
    /// </summary>
    private void Awake()
    {
        effect = Instantiate(magazineEffectPrefab, transform.position, Quaternion.identity);
        effect.SetActive(false);
    }

    /// <summary>
    /// źâ ������Ʈ�� Ȱ��ȭ �� �� ����Ʈ�� ��Ȱ��ȭ, źâ ������Ʈ ȸ�� �ڷ�ƾ ����
    /// </summary>
    private void OnEnable()
    {
        effect.SetActive(false);
        StartCoroutine("StartRotate");
    }

    /// <summary>
    /// y�� �������� ȸ�������ִ� �ڷ�ƾ �Լ�
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
    /// ItemBase���� ��ӹ��� �߻� �޼ҵ带 �����ε�, �ش� ������(ItemMagazine ������Ʈ) ��� �� �� ��
    /// </summary>
    public override void Use(GameObject entity)
    {
        // ���� 1ȸ ��ȣ �ۿ� �� itemInteractionMission �� Ŭ���� �Ǿ����� ������ �̼� Ŭ����
        if (itemInteractionMission == false)
        {
            Mission_ItemInteraction();
        }

        // ���⸦ �����ϴ� WeaponSwitchSystemŬ������ IncreaseMagazine()�޼ҵ带 ȣ��
        entity.GetComponent<WeaponSwitchSystem>().IncreaseMagazine(entity.GetComponent<WeaponSwitchSystem>().GetCurrentWeaponType(), increaseMagazine);

        // źâ�� ���������Ƿ� źâ�� ���Ǹ� ����Ǵ� �޼����� �ʱ�ȭ�ؼ� ��Ÿ���� �ʰ�
        entity.GetComponentInChildren<WeaponBase>().ResetMagazineMessage();

        // ����Ʈ�� ���(Ȱ��ȭ)
        effect.SetActive(true);
        effect.GetComponent<ParticleSystem>().Play();

        // ������ �Ŵ����� źâ ������Ʈ�� n�� �ڿ� �ٽ� Ȱ��ȭ
        GetComponentInParent<ItemRespawnManager>().ReSpawnItem(gameObject);

        // �� ������Ʈ ��Ȱ��ȭ
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ItemInteraction �̼��� Ŭ����� �� ȣ��� �Լ�
    /// </summary>
    private void Mission_ItemInteraction()
    {
        // Ȱ��ȭ�� �̼��� �ְ� �� �̼��� Object�� �̼��� ��
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) &&
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.Object);
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.Object, objectMissionNumber);
            itemInteractionMission = true;
        }
    }
}
