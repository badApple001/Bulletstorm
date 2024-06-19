//==========================
// - FileName:      Assets/Frameworks/Scripts/Debugger/Component/DebuggerComponent.InputGyroscopeInformationWindow.cs
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
        private sealed class InputGyroscopeInformationWindow : ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Input Gyroscope Information</b>");
                GUILayout.BeginVertical("box");
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Enable", GUILayout.Height(30f)))
                        {
                            Input.gyro.enabled = true;
                        }
                        if (GUILayout.Button("Disable", GUILayout.Height(30f)))
                        {
                            Input.gyro.enabled = false;
                        }
                    }
                    GUILayout.EndHorizontal();

                    DrawItem("Enabled", Input.gyro.enabled.ToString());
                    if (Input.gyro.enabled)
                    {
                        DrawItem("Update Interval", Input.gyro.updateInterval.ToString());
                        DrawItem("Attitude", Input.gyro.attitude.eulerAngles.ToString());
                        DrawItem("Gravity", Input.gyro.gravity.ToString());
                        DrawItem("Rotation Rate", Input.gyro.rotationRate.ToString());
                        DrawItem("Rotation Rate Unbiased", Input.gyro.rotationRateUnbiased.ToString());
                        DrawItem("User Acceleration", Input.gyro.userAcceleration.ToString());
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}
