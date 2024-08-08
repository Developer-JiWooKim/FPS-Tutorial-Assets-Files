using UnityEngine;

public class DestructibleBarrel : InteractionObject
{
    [Header("Destructible Barrel")]
    [SerializeField]
    private GameObject      destructibleBarrelPieces;   // destructibleBarrelPieces ������

    private bool            isDestroyed = false;        // �ı� ���� ���� �� ����

    private Vector3         pos;                        // ������ ��� �ʱ� ��ġ �� ����
    private Quaternion      rot;                        // ������ ��� �ʱ� ȸ�� �� ����

    /// <summary>
    /// �ʱ� ��ġ, �ʱ� ȸ�� ���� ������ ����
    /// </summary>
    private void Awake()
    {
        pos = transform.position;
        rot = transform.rotation;
    }

    /// <summary>
    /// �������� �Ծ��� �� ����� �Լ�
    /// </summary>
    public override void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0 && isDestroyed == false)
        {
            isDestroyed = true;

            GameObject pieces = Instantiate(destructibleBarrelPieces, transform.position, transform.rotation);

            // DestructibleBarrel�� ������
            gameObject.GetComponentInParent<BarrelManager>().ReSpawnBarrel(gameObject); 

            Destroy(pieces, 4f); // ������ destructibleBarrelPieces ����
            gameObject.SetActive(false); // DestructibleBarrel ��Ȱ��ȭ
        }
    }

    /// <summary>
    /// ��Ȧ��ȭ �� �� DestructibleBarrel ������Ʈ �ʱ�ȭ
    /// </summary>
    private void OnDisable()
    {
        Setup();
    }

    /// <summary>
    /// DestructibleBarrel ������Ʈ �ʱ�ȭ �Լ�
    /// </summary>
    private void Setup()
    {
        isDestroyed = false;
        currentHP = maxHP;
        gameObject.transform.position = pos;
        gameObject.transform.rotation = rot;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
