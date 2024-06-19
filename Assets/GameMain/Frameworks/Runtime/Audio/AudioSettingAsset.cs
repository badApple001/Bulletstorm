//==========================
// - FileName:      Assets/Frameworks/Scripts/Audio/AudioSettingAsset.cs      
// - Created:       ChenJC	
// - CreateTime:    2023-06-29-14:43:21
// - UnityVersion:  2021.3.22f1
// - Version:       1.0
// - Description:   
//==========================

using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettingAsset : ScriptableObject
{
    [SerializeField] public List<AudioSettingItem> settings = new List<AudioSettingItem>( );
    [ReadOnly] public string createdTime = string.Empty;
    [ReadOnly] public string updatedTime = string.Empty;
}

[Serializable]
public class AudioSettingItem
{
    public string url;
    public string name;
}