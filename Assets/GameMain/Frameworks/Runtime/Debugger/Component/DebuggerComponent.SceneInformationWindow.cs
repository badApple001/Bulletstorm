//==========================
// - FileName:      Assets/Frameworks/Scripts/Debugger/Component/DebuggerComponent.SceneInformationWindow.cs
// - Created:       ChenJC	
// - CreateTime:    2023-06-19 10:10:45
// - UnityVersion:  2019.4.35f1
// - Version:       1.0
// - Description:   
//==========================
﻿using UnityEngine;
using UnityEngine.SceneManagement;

namespace Debugger
{
    public sealed partial class DebuggerComponent
    {
        private sealed class SceneInformationWindow : ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Scene Information</b>");
                GUILayout.BeginVertical("box");
                {
                    DrawItem("Scene Count", SceneManager.sceneCount.ToString());
                    DrawItem("Scene Count In Build Settings", SceneManager.sceneCountInBuildSettings.ToString());

                    Scene activeScene = SceneManager.GetActiveScene();
#if UNITY_2018_3_OR_NEWER
                    DrawItem("Active Scene Handle", activeScene.handle.ToString());
#endif
                    DrawItem("Active Scene Name", activeScene.name);
                    DrawItem("Active Scene Path", activeScene.path);
                    DrawItem("Active Scene Build Index", activeScene.buildIndex.ToString());
                    DrawItem("Active Scene Is Dirty", activeScene.isDirty.ToString());
                    DrawItem("Active Scene Is Loaded", activeScene.isLoaded.ToString());
                    DrawItem("Active Scene Is Valid", activeScene.IsValid().ToString());
                    DrawItem("Active Scene Root Count", activeScene.rootCount.ToString());
#if UNITY_2019_1_OR_NEWER
                    DrawItem("Active Scene Is Sub Scene", activeScene.isSubScene.ToString());
#endif
                }
                GUILayout.EndVertical();
            }
        }
    }
}
