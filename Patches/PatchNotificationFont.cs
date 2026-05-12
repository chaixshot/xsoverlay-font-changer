namespace xsoverlay_font_changer.Patches
{
    internal class PatchNotificationFont
    {
        public static void PatchCSS()
        {
            Plugin.Logger.LogInfo($"Notification font patcher is loaded");

            Utils.ApplyHtmlStyle("Notification", XConfig.NotificationPath.Value, ".notification-popup-title, .notification-popup-bodytext");
        }
    }
}
