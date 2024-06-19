//==========================
// - FileName:      Assets/Frameworks/Scripts/Debugger/Component/DebuggerComponent.InputCompassInformationWindow.cs
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
        private sealed class InputCompassInformationWindow : ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Input Compass Information</b>");
                GUILayout.BeginVertical("box");
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Enable", GUILayout.Height(30f)))
                        {
                            Input.compass.enabled = true;
                        }
                        if (GUILayout.Button("Disable", GUILayout.Height(30f)))
                        {
                            Input.compass.enabled = false;
                        }
                    }
                    GUILayout.EndHorizontal();

                    DrawItem("Enabled", Input.compass.enabled.ToString());
                    if (Input.compass.enabled)
                    {
                        DrawItem("Heading Accuracy", Input.compass.headingAccuracy.ToString());
                        DrawItem("Magnetic Heading", Input.compass.magneticHeading.ToString());
                        DrawItem("Raw Vector", Input.compass.rawVector.ToString());
                        DrawItem("Timestamp", Input.compass.timestamp.ToString());
                        DrawItem("True Heading", Input.compass.trueHeading.ToString());
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}
