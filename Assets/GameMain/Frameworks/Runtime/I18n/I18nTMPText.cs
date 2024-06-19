using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class I18nTMPText : I18nBaseText
{
    //UGUI Text 句柄
    TextMeshProUGUI textHandler = null;

    //可以指定内容
    public override void UpdateText(string customContent = null)
    {
        if (null == textHandler)
        {
            textHandler = GetComponent<TextMeshProUGUI>();
        }

        if (customContent == null)
        {
            customContent = I18n.GetInstance().GetText(mulID);
        }

        if (!string.IsNullOrEmpty(customContent))
        {
            textHandler.text = customContent;
        }
    }
}
