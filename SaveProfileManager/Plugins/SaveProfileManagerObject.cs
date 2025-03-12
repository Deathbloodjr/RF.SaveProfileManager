using Cysharp.Threading.Tasks.Linq;
using Il2CppInterop.Runtime.Injection;
using Platform.Steam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SaveProfileManager.Plugins
{
    internal class SaveProfileManagerObject : MonoBehaviour
    {
        static SaveProfileManagerObject() => ClassInjector.RegisterTypeInIl2Cpp<SaveProfileManagerObject>();

        TextMeshProUGUI? ProfileText = null;
        Image? ProfileImage = null;

        bool isInputEnabled = true;

        int profileIndex = 0;

        public void Start()
        {
            isInputEnabled = true;
            if (ProfileText is null)
            {
                var nameObject = new GameObject("ProfileName");
                nameObject.transform.SetParent(transform);
                ProfileText = nameObject.AddComponent<TextMeshProUGUI>();
                ProfileText.alignment = TextAlignmentOptions.Center;
                ProfileText.transform.position = new Vector2(750, 270);
                // Probably not perfect, but should be good enough
                ProfileText.rectTransform.sizeDelta = new Vector2(300, 150);
            }
            if (ProfileImage is null)
            {
                var imageObject = new GameObject("ProfileImage");
                imageObject.transform.SetParent(transform);
                ProfileImage = imageObject.AddComponent<Image>();
                imageObject.transform.position = new Vector2(750, 360);
            }

            UpdateProfileDisplay(SaveDataManager.GetCurrentSaveProfile());
            profileIndex = SaveDataManager.GetIndexOfCurrentProfile();
        }

        public void Update()
        {
            GetInput();
        }


        float inputBuffer = 0.25f;
        float currentBuffer = 0;
        public void GetInput()
        {
            if (!isInputEnabled)
            {
                return;
            }
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
                ChangeProfilePrev();
            }
            else if (dir == ControllerManager.Dir.Right)
            {
                // Change to next profile
                ChangeProfileNext();
            }
            else if (TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetOkDown(ControllerManager.ControllerPlayerNo.Player1))
            {
                if (TestingHooks.titleSceneInstance.skipped || !TestingHooks.titleSceneInstance.TitleAnimator.IsPlaying("In"))
                {
                    if (SaveDataManager.ChangeProfile(profileIndex))
                    {
                        TaikoSingletonMonoBehaviour<CommonObjects>.Instance.SaveData.LoadAsync();
                    }
                    isInputEnabled = false;
                }
            }
        }

        void ChangeProfileNext()
        {
            profileIndex++;
            var list = SaveDataManager.SaveProfiles;
            if (profileIndex >= list.Count)
            {
                profileIndex = 0;
            }
            ChangeSaveProfile(profileIndex);
        }
        void ChangeProfilePrev()
        {
            profileIndex--;
            var list = SaveDataManager.SaveProfiles;
            if (profileIndex < 0)
            {
                profileIndex = list.Count - 1;
            }
            ChangeSaveProfile(profileIndex);
        }

        void ChangeSaveProfile(int newIndex)
        {
            var list = SaveDataManager.SaveProfiles;
            if (newIndex < 0 || newIndex >= list.Count)
            {
                return;
            }

            UpdateProfileDisplay(list[profileIndex]);
        }

        void UpdateProfileDisplay(SaveProfile profile)
        {
            ProfileText!.text = profile.ProfileName;
            ProfileImage!.color = profile.ProfileColor;
        }
    }
}
