using System.Collections.Generic;
public class EnemyGuideText : GuideTextBase
{
    private List<string>    guideTexts;    // ���̵� �ؽ�Ʈ�� ���� string List

    /// <summary>
    /// guideTexts ����Ʈ�� ���� �� ����Ʈ�� ���̵� �ؽ�Ʈ���� �߰�
    /// </summary>
    private void Awake()
    {
        guideTexts = new List<string>();
        InputText();
    }

    /// <summary>
    /// ����Ʈ�� ���̵� �ؽ�Ʈ���� �����ϴ� InputText�Լ��� �����ε�
    /// </summary>
    public override void InputText()
    {
        guideTexts.Add("Destroy the target and respawn your enemies!"); 
        guideTexts.Add("When you get close to an enemy,\nthe enemy will pursue you.");
        guideTexts.Add("And when you are within the enemy's attack range,\nthe enemy will attack.");
        guideTexts.Add("If you go out of the enemy's tracking range,\nthe enemy will wander again.");
    }

    /// <summary>
    /// ���̵� �ؽ�Ʈ�� ����� ����Ʈ�� ��ȯ
    /// </summary>
    public override List<string> GetList()
    {
        return guideTexts;
    }
}
