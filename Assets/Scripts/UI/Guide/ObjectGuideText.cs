using System.Collections.Generic;

public class ObjectGuideText : GuideTextBase
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
        guideTexts.Add("This is a collection of objects \nthat can be used in FPS games.");
        guideTexts.Add("Interact with items or attack objects!");
    }

    /// <summary>
    /// ���̵� �ؽ�Ʈ�� ����� ����Ʈ�� ��ȯ
    /// </summary>
    public override List<string> GetList()
    {
        return guideTexts;
    }
}
