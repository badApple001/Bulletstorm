using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using System.IO;

public class AnimUtils
{
    [MenuItem("Assets/🐕🐕🐕‍🦺🐩🐶/Anime/压缩单个", false, 10)]
    public static void Anim()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path.EndsWith(".anim"))
        {
            AnimationClip animationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            Debug.Log(animationClip);
            CompressAnimationClip(animationClip);
            RefreshAssets();
        }
        else
        {
            Debug.LogError("选中的不是动画文件");
        }
    }

    private static void RefreshAssets()
    {
        Resources.UnloadUnusedAssets();
        AssetDatabase.SaveAssets();
        GC.Collect();
    }

    [MenuItem( "Assets/🐕🐕🐕‍🦺🐩🐶/Anime/压缩目录下所有", false, 10)]
    public static void Optimize()
    {
        UnityEngine.Object[] objs = Selection.GetFiltered(typeof(object), SelectionMode.DeepAssets);
        if (objs.Length > 0)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].GetType() != typeof(AnimationClip))
                    continue;
                string path = AssetDatabase.GetAssetPath(objs[i]);
                AnimationClip animationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                Debug.Log(animationClip);
                CompressAnimationClip(animationClip);
            }
        }
        RefreshAssets();
    }

    public static void CompressAnimationClip(AnimationClip clip)
    {
        string format = "f3";
        AnimationClipCurveData[] curves = AnimationUtility.GetAllCurves(clip);
        for (int ii = 0; ii < curves.Length; ++ii)
        {
            AnimationClipCurveData curveDate = curves[ii];
            if (curveDate.curve == null || curveDate.curve.keys == null)
            {
                continue;
            }
            Keyframe[] keyFrames = curveDate.curve.keys;
            for (int i = 0; i < keyFrames.Length; i++)
            {
                Keyframe key = keyFrames[i];
                key.time = float.Parse(key.time.ToString(format));
                key.value = float.Parse(key.value.ToString(format));
                key.inTangent = float.Parse(key.inTangent.ToString(format));
                key.outTangent = float.Parse(key.outTangent.ToString(format));
                key.inWeight = float.Parse(key.inWeight.ToString(format));
                key.outWeight = float.Parse(key.outWeight.ToString(format));
                keyFrames[i] = key;
            }
            curveDate.curve.keys = keyFrames;
            clip.SetCurve(curveDate.path, curveDate.type, curveDate.propertyName, curveDate.curve);
        }
    }
}

