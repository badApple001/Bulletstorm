using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Text))]
public class I18nText : I18nBaseText
{
    //UGUI Text 句柄
    Text textHandler = null;

    //可以指定内容
    public override void UpdateText(string customContent = null)
    {
        if (null == textHandler)
        {
            textHandler = GetComponent<Text>();
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
