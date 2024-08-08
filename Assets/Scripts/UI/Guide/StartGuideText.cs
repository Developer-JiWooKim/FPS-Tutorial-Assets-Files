using System.Collections.Generic;

public class StartGuideText : GuideTextBase
{
    public List<string>     guideTexts;     // ���̵� �ؽ�Ʈ�� ���� string List

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
        guideTexts.Add("When you enter the room, the weapon is activated,\nand when you leave the room, it is deactivated.");
        guideTexts.Add("Go into the room \nand experience all the features i prepared for you!");
    }

    /// <summary>
    /// ���̵� �ؽ�Ʈ�� ����� ����Ʈ�� ��ȯ
    /// </summary>
    public override List<string> GetList()
    {
        return guideTexts;
    }
}
