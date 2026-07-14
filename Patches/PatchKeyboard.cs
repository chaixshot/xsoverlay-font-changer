using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using XSOverlay;

namespace xsoverlay_font_changer.Patches
{
    internal class PatchKeyboard
    {
        private static bool IsKeyboardExist = false;
        private static Overlay_Manager KeyboardOverlay;
        private static TMP_FontAsset CachedFontAsset;
        private static readonly Dictionary<int, (TMP_FontAsset fontAsset, float fontSize)> Original = [];

        private static readonly FieldInfo DisplayTextFontScale = AccessTools.Field(typeof(KeyboardKey), "DisplayTextFontScale");
        private static readonly FieldInfo CornerTextFontScale = AccessTools.Field(typeof(KeyboardKey), "CornerTextFontScale");
        private static readonly FieldInfo MainText = AccessTools.Field(typeof(KeyboardKey), "MainText");

        private static float GetMainTextSize() => 24f + (IsEnabled() ? XConfig.KeyboardScale.Value : 0f);
        private static float GetSecondaryTextSize() => 40f + (IsEnabled() ? XConfig.KeyboardScale.Value : 0f);

        [HarmonyPatch(typeof(Overlay_Manager), "Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {
            Plugin.Logger.LogInfo($"Keyboard font patcher is loaded");

            XConfig.KeyboardEnable.SettingChanged += (sender, args) =>
            {
                if (IsKeyboardExist)
                {
                    if (IsEnabled())
                        ApplyKeyboardFont(true);
                    else
                        RestoreKeyboardFont();
                }
            };

            XConfig.KeyboardPath.SettingChanged += (sender, args) =>
            {
                if (IsKeyboardExist && IsEnabled())
                    ApplyKeyboardFont(true);
            };

            XConfig.KeyboardScale.SettingChanged += (sender, args) =>
            {
                if (IsKeyboardExist && IsEnabled() && KeyboardOverlay != null)
                    ApplyKeyboardFont(false);
            };
        }

        [HarmonyPatch(typeof(Overlay_Manager), nameof(Overlay_Manager.EnableKeyboard))]
        [HarmonyPostfix]
        public static void PatchFont(Overlay_Manager __instance)
        {
            if (IsKeyboardExist) return;

            KeyboardOverlay = __instance;
            KeyboardGlobalManager keyboardManager = KeyboardOverlay.Keyboard_Overlay.gameObject.GetComponentInChildren<KeyboardGlobalManager>(true);

            if (keyboardManager != null && keyboardManager.HasKeyboardBeenOpened)
            {
                // Store original font and font size for each TextMeshProUGUI in the keyboard overlay
                {
                    // Default keyboard
                    foreach (KeyboardKey key in KeyboardOverlay.Keyboard.GetComponentsInChildren<KeyboardKey>(true))
                    {
                        TMP_Text mainText = (TMP_Text)MainText.GetValue(key);
                        Original[key.gameObject.GetInstanceID()] = (mainText.font, key.FontSize);
                    }

                    // KeyboardOSC
                    foreach (TextMeshProUGUI textMesh in GetKeyboardOscCanvas())
                        Original[textMesh.gameObject.GetInstanceID()] = (textMesh.font, textMesh.fontSize);
                }

                if (IsEnabled())
                    ApplyKeyboardFont(true);

                IsKeyboardExist = true;
            }
        }

        [HarmonyPatch(typeof(KeyboardKey), "SetupKeyState")]
        [HarmonyPostfix]
        public static void ListenForKeyboardKeyFontSizeChanged(KeyboardKey __instance, TMP_Text ___MainText)
        {
            if (IsEnabled())
                ___MainText.fontSize = __instance.FontSize + XConfig.KeyboardScale.Value;
        }

        [HarmonyPatch(typeof(KeyboardKey), "UpdateKeyTextPreview")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> UpdateKeyTextPreviewAnimation(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            bool patched24 = false;
            bool patched40 = false;

            var getMainTextMethod = AccessTools.Method(typeof(PatchKeyboard), nameof(GetMainTextSize));
            var getSecondaryTextMethod = AccessTools.Method(typeof(PatchKeyboard), nameof(GetSecondaryTextSize));

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4 && codes[i].operand != null)
                {
                    float val = System.Convert.ToSingle(codes[i].operand);

                    if (Mathf.Approximately(val, 24f)) // Match and safely rewrite 24f
                    {
                        codes[i].opcode = OpCodes.Call;
                        codes[i].operand = getMainTextMethod;
                        patched24 = true;
                    }
                    else if (Mathf.Approximately(val, 40f)) // Match and safely rewrite 40f
                    {
                        codes[i].opcode = OpCodes.Call;
                        codes[i].operand = getSecondaryTextMethod;
                        patched40 = true;
                    }
                }
            }

            if (!patched24)
                Plugin.Logger.LogError("[Keyboard Patch] Failed to find the 24f scale constant inside UpdateKeyTextPreview!");
            if (!patched40)
                Plugin.Logger.LogError("[Keyboard Patch] Failed to find the 40f scale constant inside UpdateKeyTextPreview!");

            return codes;
        }

        private static void ApplyKeyboardFont(bool changeFontFace)
        {
            if (File.Exists(XConfig.KeyboardPath.Value))
            {
                if (changeFontFace || CachedFontAsset == null)
                    CachedFontAsset = TMP_FontAsset.CreateFontAsset(new(XConfig.KeyboardPath.Value));

                // Default keyboard
                foreach (KeyboardKey key in KeyboardOverlay.Keyboard.GetComponentsInChildren<KeyboardKey>(true))
                {
                    if (key == null) continue;
                    TMP_Text mainText = (TMP_Text)MainText.GetValue(key);

                    if (mainText != null)
                    {
                        if (mainText.font != CachedFontAsset)
                            mainText.font = CachedFontAsset;

                        mainText.fontSize = key.FontSize + XConfig.KeyboardScale.Value;
                    }

                    DisplayTextFontScale.SetValue(key, 42f + XConfig.KeyboardScale.Value);
                    CornerTextFontScale.SetValue(key, 16f + XConfig.KeyboardScale.Value);
                }

                // KeyboardOSC
                foreach (TextMeshProUGUI textMesh in GetKeyboardOscCanvas())
                {
                    if (textMesh == null) continue;

                    if (Original.TryGetValue(textMesh.gameObject.GetInstanceID(), out (TMP_FontAsset fontAsset, float fontSize) original))
                    {
                        if (textMesh.font != CachedFontAsset)
                            textMesh.font = CachedFontAsset;

                        textMesh.fontSize = original.fontSize + XConfig.KeyboardScale.Value;
                        textMesh.fontSizeMax = textMesh.fontSize;
                    }
                }
            }
            else
                Plugin.Logger.LogError($"Keyboard - \"{XConfig.KeyboardPath.Value}\" does not exist.");
        }

        private static void RestoreKeyboardFont()
        {
            // Default keyboard
            foreach (KeyboardKey key in KeyboardOverlay.Keyboard.GetComponentsInChildren<KeyboardKey>(true))
            {
                if (Original.TryGetValue(key.gameObject.GetInstanceID(), out (TMP_FontAsset fontAsset, float fontSize) original))
                {
                    TMP_Text mainText = (TMP_Text)MainText.GetValue(key);

                    mainText.font = original.fontAsset;
                    mainText.fontSize = original.fontSize;

                    DisplayTextFontScale.SetValue(key, 42f);
                    CornerTextFontScale.SetValue(key, 16f);
                }
            }

            // KeyboardOSC
            foreach (TextMeshProUGUI textMesh in GetKeyboardOscCanvas())
            {
                if (Original.TryGetValue(textMesh.gameObject.GetInstanceID(), out (TMP_FontAsset fontAsset, float fontSize) original))
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
