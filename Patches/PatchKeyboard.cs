using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using XSOverlay;

namespace xsoverlay_font_changer.Patches
{
    [HarmonyPatch(typeof(Overlay_Manager))]
    internal class PatchKeyboard
    {
        private static bool IsPatched = false;
        private static Overlay_Manager KeyboardOverlay;
        private static readonly Dictionary<int, (TMP_FontAsset fontAsset, float fontSize)> Original = [];

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {
            Plugin.Logger.LogInfo($"Keyboard font patcher is loaded");

            XConfig.KeyboardEnable.SettingChanged += (_, _) =>
            {
                if (IsEnabled())
                    ApplyFontPatch(KeyboardOverlay);
                else
                    RestoreFontPatch(KeyboardOverlay);
            };

            XConfig.KeyboardPath.SettingChanged += (_, _) =>
            {
                if (IsEnabled())
                    if (IsPatched)
                        ApplyFontPatch(KeyboardOverlay);
            };

            XConfig.KeyboardScale.SettingChanged += (_, _) =>
            {
                if (IsEnabled())
                    if (IsPatched)
                        ApplyFontPatch(KeyboardOverlay);
            };
        }

        [HarmonyPatch(nameof(Overlay_Manager.EnableKeyboard))]
        [HarmonyPostfix]
        public static void PatchFont(Overlay_Manager __instance)
        {
            if (IsPatched) return;

            KeyboardOverlay = __instance;
            KeyboardGlobalManager keyboardManager = KeyboardOverlay.Keyboard_Overlay.gameObject.GetComponentInChildren<KeyboardGlobalManager>(true);

            if (!keyboardManager != null && keyboardManager.HasKeyboardBeenOpened)
            {
                foreach (TextMeshProUGUI textMesh in KeyboardOverlay.Keyboard.GetComponentsInChildren<TextMeshProUGUI>(true))
                    Original[textMesh.GetInstanceID()] = (textMesh.font, textMesh.fontSize);

                ApplyFontPatch(KeyboardOverlay);
            }
        }

        private static void ApplyFontPatch(Overlay_Manager instance)
        {
            if (File.Exists(XConfig.KeyboardPath.Value))
            {
                Font font = new(XConfig.KeyboardPath.Value);
                TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(font);

                foreach (TextMeshProUGUI textMesh in instance.Keyboard.GetComponentsInChildren<TextMeshProUGUI>(true))
                {
                    if (Original.TryGetValue(textMesh.GetInstanceID(), out (TMP_FontAsset fontAsset, float fontSize) original))
                    {
                        textMesh.font = fontAsset;
                        textMesh.fontSize = original.fontSize + XConfig.KeyboardScale.Value;
                    }
                }

                IsPatched = true;
            }
            else
                Plugin.Logger.LogError($"Keyboard - \"{XConfig.KeyboardPath.Value}\" does not exist.");
        }

        private static void RestoreFontPatch(Overlay_Manager instance)
        {
            foreach (TextMeshProUGUI textMesh in instance.Keyboard.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if (Original.TryGetValue(textMesh.GetInstanceID(), out (TMP_FontAsset fontAsset, float fontSize) original))
                {
                    textMesh.font = original.fontAsset;
                    textMesh.fontSize = original.fontSize;
                }
            }

            Plugin.Logger.LogInfo($"Keyboard font patch removed.");
        }

        private static bool IsEnabled()
        {
            return XConfig.KeyboardEnable.Value;
        }
    }
}
