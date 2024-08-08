using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeaponAssultRifle : WeaponBase
{
    private enum E_MessageIndex
    {
        AmmoReload = 0,
        MagazineReload,
    }

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip           audioClipTakeOutWeapon;     // ���� ���� ����
    [SerializeField]
    private AudioClip           audioClipFire;              // ���� ����
    [SerializeField]
    private AudioClip           audioClipReload;            // ������ ����

    [Header("Fire Effects")]
    [SerializeField]
    private GameObject          muzzleFlashEffect;

    [Header("Spawn Points")]
    [SerializeField]
    private Transform           casingSpawnPoint;           // ź�� ���� ��ġ
    [SerializeField]
    private Transform           bulletSpawnPoint;           // �Ѿ� ���� ��ġ

    [Header("UI")]
    [SerializeField]
    private Image               imageAim;
    [SerializeField]
    private MessageController   MessageController;

    private bool                isModeChange = false;       // ��� ��ȯ ���� üũ��
    private float               defaultModeFOV = 60;        // �⺻ ��� ī�޶� FOV
    private float               AimModeFOV = 30;            // ���� ��� ī�޶� FOV
    
    private CasingMemoryPool    casingMemoryPool;           // ź�� ���� �� Ȱ�� / ��Ȱ�� ����(Ǯ��)
    private ImpactMemoryPool    impactMemoryPool;           // ���� ȿ�� ���� �� Ȱ�� / ��Ȱ�� ����(Ǯ��)
    private Camera              mainCamera;                 

    /// <summary>
    /// �̼ǰ� ���õ� ����
    /// </summary>
    private const int           weaponChangeMissionNumber = 1; // ���Ӹ�� �̼� ��ȣ
    private bool                aimModeMission = false;

    /// <summary>
    /// ź��, ���� �ʱ�ȭ
    /// </summary>
    private void Awake()
    {
        base.Setup();

        casingMemoryPool = GetComponent<CasingMemoryPool>();
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera = Camera.main;

        // ó�� źâ, ź ���� �ִ�� ����
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;

        // �޼������� �迭�� ����
        InputMessage();
    }
    private void OnEnable()
    {
        PlaySound(audioClipTakeOutWeapon); // ���� ���� ���� ���
        muzzleFlashEffect.SetActive(false); // �ѱ� ����Ʈ ��Ȱ��ȭ
        ResetVariables();

        // �Ѿ� ������ �ȵǾ�������
        if (isLimited == false) 
        {
            onMagazineEvent.Invoke(0);
            onAmmoEvent.Invoke(999, 999);
            weaponSetting.currentAmmo = weaponSetting.maxAmmo;
            weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        }
        // �Ѿ��� ���ѵǾ�������
        else
        {
            // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ źâ ������ ����
            onMagazineEvent.Invoke(weaponSetting.currentMagazine);

            // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ ź �� ������ ����
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
        }
    }

    /// <summary>
    /// AimMode �̼� Ŭ���� �� ȣ��� �Լ�
    /// </summary>
    private void Mission_AimMode()
    {
        // Ȱ��ȭ�� �̼��� �ְ� �� �̼��� WeaponChangeRoom�� �̼��� ��
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) &&
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.ChangeWeapon);
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.ChangeWeapon, weaponChangeMissionNumber);
            aimModeMission = true;
        }
    }

    /// <summary>
    /// �޼��� �迭�� �޼��� �Է�
    /// </summary>
    private void InputMessage()
    {
        messages = new string[2];
        checkMessages = new bool[2];

        messages[(int)E_MessageIndex.AmmoReload] = "Please reload!\nPress Key : R";
        checkMessages[(int)E_MessageIndex.AmmoReload] = false;

        messages[(int)E_MessageIndex.MagazineReload] = "Run out of magazines!";
        checkMessages[(int)E_MessageIndex.MagazineReload] = false;
    }

    /// <summary>
    /// ���� �� Ray�� ���� �ε��� ��� ���� ������ ó���ϴ� �Լ�
    /// </summary>
    private void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;
        bool isBump = false;

        // ȭ���� �߾� ��ǥ(Aim�������� Raycast����)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        isBump = Physics.Raycast(ray, out hit, weaponSetting.attackDistance);

        // ���� ��Ÿ�(attackDistance) �ȿ� �ε����� ������Ʈ�� ������ targetPoint�� ������ �ε��� ��ġ
        if (isBump)
        {
            targetPoint = hit.point;
        }
        // ���� ��Ÿ� �ȿ� �ε����� ������Ʈ�� ������ targetPoint�� �ִ� ��Ÿ� ��ġ
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }
        // TODO#:������ ����� �߻�ǰ� �ִ��� Ȯ���ϴ� �׽�Ʈ�� / �׽�Ʈ ���Ŀ��� ����, �������� ��ó���� �����, ������ ����
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

        // ù ��° Raycast �������� ����� targetPoint�� ��ǥ�������� ����, �ѱ��� ������������ �Ͽ� Raycast���� 
        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
        {
            impactMemoryPool.SpawnImpact(hit);

            if (hit.transform.CompareTag("ImpactEnemy"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponSetting.damage);
            }
            else if (hit.transform.CompareTag("InteractionObject"))
            {
                hit.transform.GetComponent<InteractionObject>().TakeDamage(weaponSetting.damage);
            }
        }
        Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance, Color.blue);

    }

    /// <summary>
    /// ���� ��ư Ŭ�� �� ȣ��� �Լ�
    /// </summary>
    public override void StartWeaponAction(int type = 0)
    {
        // ������ ���̸� ���� �׼� �Ұ�
        if (isReload == true)
        {
            return;
        }
        // ��� ��ȯ ���̸� ���� �׼� �Ұ�
        if (isModeChange == true)
        {
            return;
        }
        // ���콺 ���� Ŭ�� -> ���� ����
        if (type == 0)
        {
            // ���� ����
            if (weaponSetting.isAutomaticAttack == true)
            {
                StartCoroutine("OnAttackLoop");
            }
            // �ܹ� ����
            else
            {
                OnAttack();
            }
        }
        // ���콺 ������ Ŭ�� -> ��� ��ȯ
        else
        {
            // ���� ���� ���� ��� ��ȯ �Ұ�
            if (isAttack == true)
            {
                return;
            }
            // �̼��� Ŭ���� �Ǿ����� ������ �̼� Ŭ����
            if (aimModeMission == false)
            {
                Mission_AimMode();
            }
            
            StartCoroutine("OnModeChange");
        }
    }

    /// <summary>
    /// ������ �� ȣ��� �Լ�
    /// </summary>
    public override void StartReload()
    {
        if (isLimited == false)
        {
            return; // �Ѿ� ���� X -> ������ X
        }

        // ���� ������ ���̸� ������ �Ұ� / ���� �ܿ� źâ�� 0�����̸� ������ �Ұ�
        if (isReload == true)
        {
            return;
        }

        if (weaponSetting.currentMagazine <= 0)
        {
            // źâ�� �� ���������� �˸��� �޼����� �� �� �� ����
            if (checkMessages[(int)E_MessageIndex.MagazineReload] == false)
            {
                MessageController.MessageSave(messages[(int)E_MessageIndex.MagazineReload]);
                checkMessages[(int)E_MessageIndex.MagazineReload] = true;
            }
            return;
        }

        // ���� �׼� ���߿� 'R'Ű�� ���� �������� �õ��ϸ� ���� �׼� ���� �� ������
        StopWeaponAction();

        // ���� ���Ӹ�忡�� �������� �õ� ���̸� ���Ӹ�带 ���� �� ������ �� �ٽ� ���Ӹ���
        if (animator.AimModeIs == true)
        {
            StartCoroutine("AimModeReload");

        }
        else // ���Ӹ�尡 �ƴ� ���¿��� ������ �õ� �� ���ε� �ڷ�ƾ ����
        {
            StartCoroutine("OnReload");
        }
    }

    /// <summary>
    /// ������ ���� �� ȣ��� �Լ�
    /// </summary>
    public override void StopWeaponAction(int type = 0)
    {
        // ���콺 ���� Ŭ��(���� ����)
        if (type == 0)
        {
            isAttack = false;
            StopCoroutine("OnAttackLoop");
        }
    }

    /// <summary>
    /// �޼��� �ʱ�ȭ �Լ�(ResetMagazine)
    /// </summary>
    public override void ResetMagazineMessage()
    {
        this.checkMessages[(int)E_MessageIndex.MagazineReload] = false;
    }

    /// <summary>
    /// Aim ����� �� ������ �� (���� ��� ���� - ������ - �ٽ� ���Ӹ��)�� �����ϴ� �ڷ�ƾ �Լ�
    /// </summary>
    private IEnumerator AimModeReload()
    {
        yield return StartCoroutine("OnModeChange");
        yield return StartCoroutine("OnReload");
        yield return StartCoroutine("OnModeChange");
    }
    
    /// <summary>
    /// ���Ӱ���(���� ��ư�� ��� ������ ������) ȣ��Ǵ� �ڷ�ƾ �Լ�
    /// </summary>
    private IEnumerator OnAttackLoop()
    {
        while(true)
        {
            OnAttack();
            yield return null;
        }
    }

    /// <summary>
    /// ���� �� ȣ��Ǵ� �Լ�
    /// </summary>
    private void OnAttack()
    {
        bool isAttack = Time.time - lastAttackTime > weaponSetting.attackRate;
        if (isAttack)
        {
            // �ٰ� ���� �� ���� �Ұ�
            if (animator.MoveSpeed > 0.5f)
            {
                return;
            }

            lastAttackTime = Time.time; // ���� �ֱⰡ �Ǿ�� ���� �� �� �ְ� ���� �ð��� ����

            // ���� ź���� 0���� �̸� ���� �Ұ�
            if (weaponSetting.currentAmmo <= 0)
            {
                if (this.checkMessages[(int)E_MessageIndex.AmmoReload] == false)
                {
                    // ź�� �� ���������� �˸��� �޼��� ���
                    MessageController.MessageSave(messages[(int)E_MessageIndex.AmmoReload]);
                    checkMessages[(int)E_MessageIndex.AmmoReload] = true;
                }
                return;
            }

            if (isLimited == true) // �Ѿ��� ���ѵǾ������� �������
            {
                weaponSetting.currentAmmo--; // ���� ���� �� currentAmmo �ϳ� ����
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo); // �Ѿ��� �Һ��� �� ���� UI�� ������ �̺�Ʈ(onAmmoEvent) �߻� - �ش� �Լ� ����(��ϵ� �Լ�)  
            }// ���ѵǾ� ���� ������ UI���� X
            

            // ���� �ִϸ��̼� ���(��忡 ���� AimFire or Fire �ִϸ��̼� ���)
            //animator.Play("Fire", -1, 0); 
            string animation = animator.AimModeIs == true ? "AimFire" : "Fire";
            animator.Play(animation, -1, 0);

            //�ѱ� ����Ʈ ���(default mode�� ���� ���)
            if (animator.AimModeIs == false)
            {
                StartCoroutine("OnMuzzleFlashEffect"); // ����Ʈ ��� �ڷ�ƾ
            }
            
            PlaySound(audioClipFire); // ���� ����
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right); // ź�� ����

            // ������ �߻��� ���ϴ� ��ġ ����
            TwoStepRaycast();
        }
    }

    /// <summary>
    /// MuzzleFlashEffect�� ����ϴ� �ڷ�ƾ �Լ�
    /// </summary>
    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);

        muzzleFlashEffect.SetActive(false);
    }

    /// <summary>
    /// ������ �ڷ�ƾ �Լ�
    /// </summary>
    private IEnumerator OnReload()
    {
        isReload = true;
        animator.OnReload();

        PlaySound(audioClipReload);

        while(true)
        {
            // ���� ������� �ƴϰ�, ���� �ִϸ��̼��� Movement�̸� ������ �ִϸ��̼�, ���� ����� ����Ǿ��ٴ� ��
            if (audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
            {
                isReload = false;

                // ���� źâ ���� 1����, �ٲ� źâ ������ Text UI�� ������Ʈ
                weaponSetting.currentMagazine--;
                onMagazineEvent.Invoke(weaponSetting.currentMagazine);

                // ���� ź���� �ִ�� ����, �ٲ� ź �� ������ Text UI�� ������Ʈ
                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                // �Ѿ��� �� ���� �Ǿ����Ƿ� �ٽ� �޼����� ���� �� �ְ�
                checkMessages[(int)E_MessageIndex.AmmoReload] = false;

                yield break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// ���� ����(����������, ���ݿ���) �ʱ�ȭ
    /// </summary>
    private void ResetVariables()
    {
        isReload = false;
        isAttack = false;
        isModeChange = false;
    }

    /// <summary>
    /// ��� ���� �ڷ�ƾ �Լ�(Aim < - > Normal)
    /// </summary>
    private IEnumerator OnModeChange()
    {
        float current = 0f;
        float percent = 0f;
        float time = 0.35f;

        animator.AimModeIs = !animator.AimModeIs;
        imageAim.enabled = !imageAim.enabled;

        float start = mainCamera.fieldOfView;
        float end = animator.AimModeIs == true ? AimModeFOV : defaultModeFOV;

        isModeChange = true;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);

            yield return null;
        }

        isModeChange = false;
    }
}
