using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using System.Reflection;
using TMPro;
using UnityEngine;
using XSOverlay;

namespace xsoverlay_font_changer.Patches
{
    [HarmonyPatch(typeof(Overlay_Manager))]
    internal class PatchKeyboardOSCFont
    {
        private static bool isPatched = false;

        public static void PatchCSS()
        {
            if (Plugin.configData.TryGetValue("KeyboardOSCFontPath", out string KeyboardOSCFontPath))
            {
                Utils.ApplyHtmlStyle("SettingsKO", KeyboardOSCFontPath, ".side-bar-button-text, .page-container, .page-header-text, .page-section-text, .whitespace-pre");
            }
        }

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {
            Plugin.Logger.LogInfo($"Keyboard OSC font patcher loaded");
        }

        [HarmonyPatch(nameof(Overlay_Manager.EnableKeyboard))]
        [HarmonyPostfix]
        public static void PatchGameObject()
        {
            if (isPatched) return;

            KeyboardGlobalManager keyboardManager = Plugin.overlayManager.Keyboard_Overlay.gameObject.GetComponentInChildren<KeyboardGlobalManager>(true);

            if (!keyboardManager != null && keyboardManager.HasKeyboardBeenOpened)
            {
                if (Plugin.configData.TryGetValue("KeyboardFontPath", out string KeyboardFontPath))
                {
                    Font font = new(KeyboardFontPath.Trim('"'));
                    TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(font);

                    if (Chainloader.PluginInfos.TryGetValue("nwnt.keyboardosc", out PluginInfo pluginInfo))
                    {
                        object instance = pluginInfo.Instance;
                        if (instance == null) return;

                        // Use BindingFlags.Public since the field is public
                        // Include BindingFlags.Instance because it belongs to the plugin instance
                        FieldInfo field = instance.GetType().GetField("oscBarCanvas", BindingFlags.Public | BindingFlags.Instance);

                        if (field != null)
                        {
                            // Get the value and cast it to GameObject
                            GameObject oscBarCanvas = field.GetValue(instance) as GameObject;

                            if (oscBarCanvas != null)
                            {
                                Plugin.Logger.LogInfo("Successfully found oscBarCanvas!");

                                foreach (TextMeshProUGUI textMesh in oscBarCanvas.GetComponentsInChildren<TextMeshProUGUI>(true))
                                    textMesh.font = fontAsset;
                            }
                            else
                                Plugin.Logger.LogWarning("oscBarCanvas field found, but it is currently null.");
                        }
                        else
                            Plugin.Logger.LogError("Could not find a field named 'oscBarCanvas' in the target mod.");
                    }

                    isPatched = true;
                    Plugin.Logger.LogInfo($"Keyboard font patched \"{KeyboardFontPath}\"");
                }
                else
                    Plugin.Logger.LogError($"Config KeyboardFontPath is missing. Fallback to default");
            }
        }
    }
}
