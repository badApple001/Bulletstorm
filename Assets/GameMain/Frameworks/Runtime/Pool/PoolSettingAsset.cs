using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolSettingAsset : ScriptableObject
{

    [SerializeField] public List<PoolSettingItem> settings = new List<PoolSettingItem>( );
    [ReadOnly] public string createdTime = string.Empty;
    [ReadOnly] public string updatedTime = string.Empty;
}

[Serializable]
public class PoolSettingItem
{
    public string path;
    public int count;
}