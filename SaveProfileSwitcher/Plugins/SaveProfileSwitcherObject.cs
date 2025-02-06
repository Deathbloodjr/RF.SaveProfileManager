using Cysharp.Threading.Tasks.Linq;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveProfileSwitcher.Plugins
{
    internal class SaveProfileSwitcherObject : MonoBehaviour
    {
        static SaveProfileSwitcherObject() => ClassInjector.RegisterTypeInIl2Cpp<SaveProfileSwitcherObject>();

        public void Start()
        {

        }

        public void Update()
        {
            GetInput();
        }


        float inputBuffer = 0.25f;
        float currentBuffer = 0;
        public void GetInput()
        {
            if (currentBuffer != 0)
            {
                currentBuffer -= Time.deltaTime;
                currentBuffer = Math.Max(currentBuffer, 0);
            }
            ControllerManager.Dir dir = TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetDirectionButton(ControllerManager.ControllerPlayerNo.Player1, ControllerManager.Prio.None, false);
            if (dir == ControllerManager.Dir.None)
            {
                currentBuffer = 0;
            }
            else if (currentBuffer != 0)
            {
                return;
            }
            if (dir != ControllerManager.Dir.None)
            {
                currentBuffer = inputBuffer;
            }
            if (dir == ControllerManager.Dir.Left)
            {
                // Change to previous profile
                Logger.Log("Save Profile changed to DEBUG");
                GetSavePathHook.ProfileName = "DEBUG";
            }
            else if (dir == ControllerManager.Dir.Right)
            {
                // Change to next profile
                Logger.Log("Save Profile changed to Drum");
                GetSavePathHook.ProfileName = "Drum";
            }
            else if (dir == ControllerManager.Dir.Up)
            {
                // Change to next profile
                Logger.Log("Save Profile changed to Default");
                GetSavePathHook.ProfileName = "Default";
            }
            else if (TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetOkDown(ControllerManager.ControllerPlayerNo.Player1))
            {
                if (TestingHooks.titleSceneInstance.skipped || !TestingHooks.titleSceneInstance.TitleAnimator.IsPlaying("In"))
                {
                    if (GetSavePathHook.ProfileChanged())
                    {
                        TaikoSingletonMonoBehaviour<CommonObjects>.Instance.SaveData.LoadAsync();
                    }
                }
            }
        }
    }
}
