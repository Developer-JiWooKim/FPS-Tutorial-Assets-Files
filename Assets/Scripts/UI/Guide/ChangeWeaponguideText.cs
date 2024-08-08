using System.Collections.Generic;

public class ChangeWeaponguideText : GuideTextBase
{
    private List<string>    guideTexts;     // ���̵� �ؽ�Ʈ�� ���� string List

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
        guideTexts.Add("Try pressing 1,2,3,4 on your keyboard!");
        guideTexts.Add("If you are fitted with a gun,\nyou can right-click to enter or release the aiming mode.");
        guideTexts.Add("If you have a Knife equipped,\nright-click to see different attack motions!");
    }

    /// <summary>
    /// ���̵� �ؽ�Ʈ�� ����� ����Ʈ�� ��ȯ
    /// </summary>
    public override List<string> GetList()
    {
        return guideTexts;
    }
}
