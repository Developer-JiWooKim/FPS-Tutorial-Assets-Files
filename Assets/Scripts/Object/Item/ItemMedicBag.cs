using System.Collections;
using UnityEngine;

public class ItemMedicBag : ItemBase
{
    [SerializeField]
    private GameObject      hpEffectPrefab;                     // ȸ�������� ������Ʈ�� ��ȣ �ۿ� �� ����� ����Ʈ ������
    [SerializeField]
    private int             increaseHP = 50;                    // ü�� ���� ��ġ
    [SerializeField]
    private float           moveDistance = 0.2f;                // ���Ʒ��� �����̴� �ӵ�
    [SerializeField]
    private float           pingpongSpeed = 0.5f;               // pingpong �ӵ�
    [SerializeField]
    private float           rotateSpeed = 50.0f;                // ȸ�� �ӵ�

    /// <summary>
    /// �̼ǰ� ���õ� ������
    /// </summary>
    private const int       objectMissionNumber = 0;
    private bool            itemInteractionMission = false;

    private Vector3         pos;                                // �ʱ� ��ġ
    private GameObject      effect;                             // ���� ���ӿ��� ���� hpEffect

    /// <summary>
    /// ItemMedicBag ������Ʈ�� Ȱ��ȭ �� �� effect ������Ʈ ��Ȱ��ȭ �� StartPingPong �ڷ�ƾ ����
    /// </summary>
    private void OnEnable()
    {
        effect.SetActive(false);
        StartCoroutine("StartPingPong");
    }

    /// <summary>
    /// hpEffectPrefab ����� �������� effect ������Ʈ ������ ����, �ʱ� ��ġ�� ������ ����
    /// </summary>
    private void Awake()
    {
        pos = transform.position;
        effect = Instantiate(hpEffectPrefab, transform.position, Quaternion.identity);
    }

    /// <summary>
    /// ItemMedicBag ������Ʈ �� PingPong��Ű�� �ڷ�ƾ �Լ�
    /// </summary>
    private IEnumerator StartPingPong()
    {
        float y = pos.y;
        while (true)
        {
            // y�� ���� ȸ��
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

            // ó�� ��ġ�� ��ġ�� �������� y��ġ�� ��, �Ʒ��� �̵�
            Vector3 position = pos;
            position.y = Mathf.Lerp(y, y + moveDistance, Mathf.PingPong(Time.time * pingpongSpeed, 1));
            transform.position = position;

            yield return null;
        }
    }

    /// <summary>
    /// ItemBase���� ��ӹ��� �߻� �޼ҵ带 �����ε�, �ش� ������(ItemMedicBag ������Ʈ) ��� �� �� ��
    /// </summary>
    public override void Use(GameObject entity)
    {
        // ���� 1ȸ ��ȣ �ۿ� �� itemInteractionMission �� Ŭ���� �Ǿ����� ������ �̼� Ŭ����
        if (itemInteractionMission == false)
        {
            Mission_ItemInteraction();
        }

        // ��ȣ�ۿ��� ����� ü���� ������Ű�� IncreaseHP �Լ� ����
        entity.GetComponent<Status>().IncreaseHP(increaseHP);

        // ü�� ȸ�� ����Ʈ ���
        effect.SetActive(true);
        effect.GetComponent<ParticleSystem>().Play();

        // ������ �Ŵ����� ItemMedicbag ������Ʈ ������
        GetComponentInParent<ItemRespawnManager>().ReSpawnItem(gameObject);

        // ItemMedicbag ������Ʈ ��Ȱ��ȭ
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
