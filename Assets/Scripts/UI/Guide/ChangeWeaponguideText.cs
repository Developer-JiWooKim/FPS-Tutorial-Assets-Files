using System.Collections.Generic;

public class ChangeWeaponguideText : GuideTextBase
{
    private List<string>    guideTexts;     // 가이드 텍스트를 담을 string List

    /// <summary>
    /// guideTexts 리스트를 생성 후 리스트에 가이드 텍스트들을 추가
    /// </summary>
    private void Awake()
    {
        guideTexts = new List<string>();
        InputText();
    }

    /// <summary>
    /// 리스트에 가이드 텍스트들을 저장하는 InputText함수를 오버로드
    /// </summary>
    public override void InputText()
    {
        guideTexts.Add("Try pressing 1,2,3,4 on your keyboard!");
        guideTexts.Add("If you are fitted with a gun,\nyou can right-click to enter or release the aiming mode.");
        guideTexts.Add("If you have a Knife equipped,\nright-click to see different attack motions!");
    }

    /// <summary>
    /// 가이드 텍스트가 저장된 리스트를 반환
    /// </summary>
    public override List<string> GetList()
    {
        return guideTexts;
    }
}
