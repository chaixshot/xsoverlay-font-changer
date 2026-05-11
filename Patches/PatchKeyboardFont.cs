using HarmonyLib;
using TMPro;
using UnityEngine;
using XSOverlay;

namespace xsoverlay_font_changer.Patches
{
    [HarmonyPatch(typeof(Overlay_Manager))]
    internal class PatchKeyboardFont
    {
        private static bool isPatched = false;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {
            Plugin.Logger.LogInfo($"Keyboard font patcher loaded");
        }

        [HarmonyPatch(nameof(Overlay_Manager.EnableKeyboard))]
        [HarmonyPostfix]
        public static void Patch()
        {
            if (isPatched) return;

            KeyboardGlobalManager keyboardManager = Plugin.overlayManager.Keyboard_Overlay.gameObject.GetComponentInChildren<KeyboardGlobalManager>(true);

            if (!keyboardManager != null && keyboardManager.HasKeyboardBeenOpened)
            {
                if (Plugin.configData.TryGetValue("KeyboardFontPath", out string KeyboardFontPath))
                {
                    Font font = new(KeyboardFontPath.Trim('"'));
                    TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(font);

                    foreach (TextMeshProUGUI textMesh in Plugin.overlayManager.Keyboard.GetComponentsInChildren<TextMeshProUGUI>(true))
                        textMesh.font = fontAsset;

                    isPatched = true;
                    Plugin.Logger.LogInfo($"Keyboard font patched \"{KeyboardFontPath}\"");
                }
                else
                    Plugin.Logger.LogError($"Config KeyboardFontPath is missing. Fallback to default");
            }
        }
    }
}
