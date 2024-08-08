using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeaponRevolver : WeaponBase
{
    private enum E_MessageIndex
    {
        AmmoReload = 0,
        MagazineReload,
    }

    [Header("Fire Effects")]
    [SerializeField]
    private GameObject              muzzleFlashEffect;              // �ѱ� ����Ʈ(on/off)

    [Header("Spawn Points")]
    [SerializeField]
    private Transform               bulletSpawnPoint;               // �Ѿ� ���� ��ġ

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip               audioClipFire;                  // ���� ���� 
    [SerializeField]
    private AudioClip               audioClipReload;                // ���� ���� 

    [Header("UI")]
    [SerializeField]
    private Image                   imageAim;
    [SerializeField]
    private MessageController       MessageController;

    private ImpactMemoryPool        impactMemoryPool;               // ���� ȿ�� ���� �� Ȱ�� / ��Ȱ�� ����
    private Camera                  mainCamera;                     // ���� �߻�

    /// <summary>
    /// �̼ǰ� ���� ����
    /// </summary>
    private const int               weaponChangeMissionNumber = 1;  // ���Ӹ�� �̼� ��ȣ
    private bool                    aimModeMission = false;

    private bool                    isModeChange = false;           // ��� ��ȯ ���� üũ��
    private float                   defaultModeFOV = 60;            // �⺻ ��� ī�޶� FOV
    private float                   AimModeFOV = 50;                // ���� ��� ī�޶� FOV

    /// <summary>
    /// ���� Ȱ��ȭ(����) �� �� �ϵ�
    /// </summary>
    private void OnEnable()
    {
        // �ѱ� ����Ʈ �������� ��Ȱ��ȭ
        muzzleFlashEffect.SetActive(false);
        ResetVariables();
        if (isLimited == false)
        {
            onMagazineEvent.Invoke(0);

            // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ ź �� ������ ����
            onAmmoEvent.Invoke(999,999);
            weaponSetting.currentAmmo = weaponSetting.maxAmmo;
            weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        }
        else
        {
            // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ źâ ������ ����
            onMagazineEvent.Invoke(weaponSetting.currentMagazine);

            // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ ź �� ������ ����
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
        }
        
    }

    /// <summary>
    /// ���� �ʱ�ȭ, �޼��� �ʱ�ȭ
    /// </summary>
    private void Awake()
    {
        base.Setup();

        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera = Camera.main;

        // ó�� źâ, ź ���� �ִ�� ����
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;

        InputMessage();
    }

    /// <summary>
    /// ������ �Լ�
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

        // ���� �׼� ���� 'R'Ű�� ���� �������� �õ��ϸ� ���� �׼� ���� �� ������
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
    /// �÷��̾� �Է¿� ���� ����, ������
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


        if (type == 0)
        {
            if (isAttack == false && isReload == false)
            {
                OnAttack();
            }
        }
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
    /// ������ ���ߴ� �Լ�
    /// </summary>
    public override void StopWeaponAction(int type = 0)
    {
        isAttack = false;
    }

    /// <summary>
    /// ���� �� ȣ��Ǵ� �Լ�
    /// </summary>
    public void OnAttack()
    {
        bool isAttack = Time.time - lastAttackTime > weaponSetting.attackRate;
        if (isAttack)
        {
            // �ٰ� ���� �� ���� �Ұ�
            if (animator.MoveSpeed > 0.5f)
            {
                return;
            }

            // ���� �ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ����
            lastAttackTime = Time.time;

            // ź ���� ������ ���� �Ұ���
            if (weaponSetting.currentAmmo <= 0)
            {
                // �ѹ��� �޼����� ����
                if (checkMessages[(int)E_MessageIndex.AmmoReload] == false)
                {
                    // ź�� �� ���������� �˸��� �޼��� ���
                    MessageController.MessageSave(messages[(int)E_MessageIndex.AmmoReload]);
                    checkMessages[(int)E_MessageIndex.AmmoReload] = true;
                }
                return;
            }

            // �Ѿ��� ���ѵǾ������� �������
            if (isLimited == true)
            {
                weaponSetting.currentAmmo--; // ���� ���� �� currentAmmo �ϳ� ����
                // �Ѿ��� �Һ��� �� ���� UI�� ������ �̺�Ʈ(onAmmoEvent) �߻� - �ش� �Լ� ����(��ϵ� �Լ�)  
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo); 
            }// ���ѵǾ� ���� ������ UI���� X

            string animation = animator.AimModeIs == true ? "AimFire" : "Fire";
            animator.Play(animation, -1, 0);

            //�ѱ� ����Ʈ ���(default mode�� ���� ���)
            if (animator.AimModeIs == false)
            {
                StartCoroutine("OnMuzzleFlashEffect");
            }
            PlaySound(audioClipFire);

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

        while (true)
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
    }

    /// <summary>
    /// ���� ����(����������, ���ݿ���) �ʱ�ȭ
    /// </summary>
    private void ResetVariables()
    {
        isReload = false;
        isAttack = false;
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
    /// �޼��� �ʱ�ȭ(ResetMagazine) �Լ�
    /// </summary>
    public override void ResetMagazineMessage()
    {
        checkMessages[(int)E_MessageIndex.MagazineReload] = false;
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
