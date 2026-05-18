using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using XSOverlay;

namespace xsoverlay_font_changer.Patches
{
    [HarmonyPatch(typeof(Overlay_Manager))]
    internal class PatchKeyboard
    {
        private static bool IsKeyboardExist = false;
        private static Overlay_Manager KeyboardOverlay;
        private static readonly Dictionary<int, (TMP_FontAsset fontAsset, float fontSize)> Original = [];

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {
            Plugin.Logger.LogInfo($"Keyboard font patcher is loaded");

            XConfig.KeyboardEnable.SettingChanged += (_, _) =>
            {
                if (IsKeyboardExist)
                    if (IsEnabled())
                        ApplyFontPatch(KeyboardOverlay);
                    else
                        RestoreFontPatch(KeyboardOverlay);
            };

            XConfig.KeyboardPath.SettingChanged += (_, _) =>
            {
                if (IsKeyboardExist)
                    if (IsEnabled())
                        ApplyFontPatch(KeyboardOverlay);
            };

            CancellationTokenSource debounceCts = null;
            XConfig.KeyboardScale.SettingChanged += async (_, _) =>
            {
                if (IsKeyboardExist)
                    if (IsEnabled())
                    {
                        debounceCts?.Cancel();
                        debounceCts?.Dispose();
                        debounceCts = new CancellationTokenSource();

                        await Task.Delay(500, debounceCts.Token);
                        ApplyFontPatch(KeyboardOverlay);
                    }
            };
        }

        [HarmonyPatch(nameof(Overlay_Manager.EnableKeyboard))]
        [HarmonyPostfix]
        public static void PatchFont(Overlay_Manager __instance)
        {
            if (IsKeyboardExist) return;

            KeyboardOverlay = __instance;
            KeyboardGlobalManager keyboardManager = KeyboardOverlay.Keyboard_Overlay.gameObject.GetComponentInChildren<KeyboardGlobalManager>(true);

            if (!keyboardManager != null && keyboardManager.HasKeyboardBeenOpened)
            {
                // Store original font and font size for each TextMeshProUGUI in the keyboard overlay
                {
                    // Default keyboard
                    foreach (TextMeshProUGUI textMesh in KeyboardOverlay.Keyboard.GetComponentsInChildren<TextMeshProUGUI>(true))
                        Original[textMesh.GetInstanceID()] = (textMesh.font, textMesh.fontSize);

                    // KeyboardOSC
                    foreach (TextMeshProUGUI textMesh in GetKeyboardOscCanvas())
                        Original[textMesh.GetInstanceID()] = (textMesh.font, textMesh.fontSize);
                }

                if (IsEnabled())
                    ApplyFontPatch(KeyboardOverlay);

                IsKeyboardExist = true;
            }
        }

        private static void ApplyFontPatch(Overlay_Manager instance)
        {
            if (File.Exists(XConfig.KeyboardPath.Value))
            {
                Font font = new(XConfig.KeyboardPath.Value);
                TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(font);

                // Default keyboard
                foreach (TextMeshProUGUI textMesh in instance.Keyboard.GetComponentsInChildren<TextMeshProUGUI>(true))
                {
                    if (Original.TryGetValue(textMesh.GetInstanceID(), out (TMP_FontAsset fontAsset, float fontSize) original))
                    {
                        textMesh.font = fontAsset;
                        textMesh.fontSize = original.fontSize + XConfig.KeyboardScale.Value;
                    }
                }

                // KeyboardOSC
                foreach (TextMeshProUGUI textMesh in GetKeyboardOscCanvas())
                {
                    if (Original.TryGetValue(textMesh.GetInstanceID(), out (TMP_FontAsset fontAsset, float fontSize) original))
                    {
                        textMesh.font = fontAsset;
                        textMesh.fontSize = original.fontSize + XConfig.KeyboardScale.Value;
                    }
                }
            }
            else
                Plugin.Logger.LogError($"Keyboard - \"{XConfig.KeyboardPath.Value}\" does not exist.");
        }

        private static void RestoreFontPatch(Overlay_Manager instance)
        {
            // Default keyboard
            foreach (TextMeshProUGUI textMesh in instance.Keyboard.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if (Original.TryGetValue(textMesh.GetInstanceID(), out (TMP_FontAsset fontAsset, float fontSize) original))
                {
                    textMesh.font = original.fontAsset;
                    textMesh.fontSize = original.fontSize;
                }
            }

            // KeyboardOSC
            foreach (TextMeshProUGUI textMesh in GetKeyboardOscCanvas())
            {
                if (Original.TryGetValue(textMesh.GetInstanceID(), out (TMP_FontAsset fontAsset, float fontSize) original))
                {
                    textMesh.font = original.fontAsset;
                    textMesh.fontSize = original.fontSize;
                }
            }

            Plugin.Logger.LogInfo($"Keyboard font patch removed.");
        }

        private static TextMeshProUGUI[] GetKeyboardOscCanvas()
        {
            if (Chainloader.PluginInfos.TryGetValue("nwnt.keyboardosc", out PluginInfo pluginInfo))
            {
                BaseUnityPlugin KeyboardOSC = pluginInfo.Instance;

                if (KeyboardOSC != null)
                {
                    // Use BindingFlags.Public since the field is public
                    // Include BindingFlags.Instance because it belongs to the plugin KeyboardOSC
                    FieldInfo field = KeyboardOSC.GetType().GetField("oscBarCanvas", BindingFlags.Public | BindingFlags.Instance);

                    if (field != null)
                    {
                        // Get the value and cast it to GameObject
                        GameObject oscBarCanvas = field.GetValue(KeyboardOSC) as GameObject;

                        if (oscBarCanvas != null)
                        {
                            return oscBarCanvas.GetComponentsInChildren<TextMeshProUGUI>(true);
                        }
                        else
                            Plugin.Logger.LogWarning("oscBarCanvas field found, but it is currently null.");
                    }
                    else
                        Plugin.Logger.LogError("Could not find a field named 'oscBarCanvas' in the target mod.");
                }
            }

            return [];
        }

        private static bool IsEnabled()
        {
            return XConfig.KeyboardEnable.Value;
        }
    }
}
