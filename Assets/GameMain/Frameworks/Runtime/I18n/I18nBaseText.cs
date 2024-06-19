using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
///
/// Multilingual components
/// Why not just inherit Text?
/// Answer:
/// 1. If you need to port to another framework, another framework may have an extension class that inherits Text
/// 2. If you need to inherit Text to implement some effect, then it is not appropriate for this component to inherit it. Because you inherited the latter two logic will cross the need for you to manually change! This is something that I don't allow. I want components that are components to be trusted to work without changes. If there is a Unity version of the problem, it should be solved by macros and compatible
/// 3. After comprehensive consideration, decide to handle it in the form of a pendant
/// 
/// </summary>
public abstract class I18nBaseText : MonoBehaviour
{
    [SerializeField]
    protected int m_mulID = 0;

    /// <summary>
    /// 切换多语言
    /// 切换的时候 如果目标语言没有配置则使用默认的
    /// </summary>
    public virtual int mulID
    {
        get
        {
            return m_mulID;
        }
        set
        {
            var newTextValue = I18n.GetInstance().GetText(value);
            if (!string.IsNullOrEmpty(newTextValue))
            {
                m_mulID = value;
                UpdateText(newTextValue);
            }
            else
            {
                Log.Error($"当前多语言Key不存在: {value}");
            }
        }
    }

    /// <summary>
    /// 可以指定内容
    /// </summary>
    public abstract void UpdateText(string customContent = null);

    private void Awake()
    {
        I18n.GetInstance().___AddLanguageTextComponent(this);
    }

    private void OnEnable()
    {
        if (!I18n.GetInstance().inite)
        {
            //未初始完成 后续初始化完成后会由Manager再调用更新的 不用担心
            return;
        }
        UpdateText();
    }

    //有可能切出去修改了设备语言
    private void OnApplicationFocus(bool focus)
    {
        if (focus) UpdateText();
    }

    private void OnDestroy()
    {
        I18n.GetInstance().___RemoveLanguageTextComponent(this);
    }
}
