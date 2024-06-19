//==========================
// - FileName:      Assets/Frameworks/Scripts/Debugger/Component/DebuggerComponent.WebPlayerInformationWindow.cs
// - Created:       ChenJC	
// - CreateTime:    2023-06-19 10:10:45
// - UnityVersion:  2019.4.35f1
// - Version:       1.0
// - Description:   
//==========================
﻿using UnityEngine;

namespace Debugger
{
    public sealed partial class DebuggerComponent
    {
        private sealed class WebPlayerInformationWindow : ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Web Player Information</b>");
                GUILayout.BeginVertical("box");
                {
#if !UNITY_2017_2_OR_NEWER
                    DrawItem("Is Web Player", Application.isWebPlayer.ToString());
#endif
                    DrawItem("Absolute URL", Application.absoluteURL);
#if !UNITY_2017_2_OR_NEWER
                    DrawItem("Source Value", Application.srcValue);
#endif
#if !UNITY_2018_2_OR_NEWER
                    DrawItem("Streamed Bytes", Application.streamedBytes.ToString());
#endif
#if UNITY_5_3 || UNITY_5_4
                    DrawItem("Web Security Enabled", Application.webSecurityEnabled.ToString());
                    DrawItem("Web Security Host URL", Application.webSecurityHostUrl.ToString());
#endif
                }
                GUILayout.EndVertical();
            }
        }
    }
}
