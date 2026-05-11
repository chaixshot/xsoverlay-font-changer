using WebSocketSharp;

namespace xsoverlay_font_changer.Patches
{
    internal class PatchNotificationFont
    {
        public static void PatchCSS()
        {
            Plugin.configData.TryGetValue("NotificationFontPath", out string NotificationFontPath);

            if (NotificationFontPath.IsNullOrEmpty())
            {
                Plugin.Logger.LogError($"Config NotificationFontPath is missing. Fallback to default");
                return;
            }

            Utils.ApplyHtmlStyle("Notification", NotificationFontPath, ".notification-popup-title, .notification-popup-bodytext");
        }
    }
}
