using BepInEx.Configuration;

namespace xsoverlay_font_changer
{
    internal class XConfig
    {
        public static ConfigEntry<bool> KeyboardEnable;
        public static ConfigEntry<string> KeyboardPath;
        public static ConfigEntry<int> KeyboardScale;

        public static ConfigEntry<bool> NotificationEnable;
        public static ConfigEntry<string> NotificationPath;
        public static ConfigEntry<int> NotificationScale;

        public static ConfigEntry<bool> SettingsEnable;
        public static ConfigEntry<string> SettingsPath;
        public static ConfigEntry<int> SettingsScale;

        public static ConfigEntry<bool> TooltipEnable;
        public static ConfigEntry<string> TooltipPath;
        public static ConfigEntry<int> TooltipScale;

        public static ConfigEntry<bool> WindowSettingsEnable;
        public static ConfigEntry<string> WindowSettingsPath;
        public static ConfigEntry<int> WindowSettingsScale;

        public static ConfigEntry<bool> WristEnable;
        public static ConfigEntry<string> WristPath;
        public static ConfigEntry<int> WristScale;

        public static void AllConfig(ConfigFile cfg)
        {
            KeyboardEnable = cfg.Bind("Keyboard", "Enable", true, "Apply Keyboard font modification.");
            KeyboardPath = cfg.Bind("Keyboard", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Keyboard font name.");
            KeyboardScale = cfg.Bind("Keyboard", "Scale", 0, "Keyboard font scale.");

            NotificationEnable = cfg.Bind("Notification", "Enable", true, "Apply Notification font modification.");
            NotificationPath = cfg.Bind("Notification", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Notification font name.");
            NotificationScale = cfg.Bind("Notification", "Scale", 0, "Notification font scale.");

            SettingsEnable = cfg.Bind("Settings", "Enable", true, "Apply Settings font modification.");
            SettingsPath = cfg.Bind("Settings", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Settings font name.");
            SettingsScale = cfg.Bind("Settings", "Scale", 0, "Settings font scale.");

            TooltipEnable = cfg.Bind("Tooltip", "Enable", true, "Apply Tooltip font modification.");
            TooltipPath = cfg.Bind("Tooltip", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Tooltip font name.");
            TooltipScale = cfg.Bind("Tooltip", "Scale", 0, "Tooltip font scale.");

            WindowSettingsEnable = cfg.Bind("WindowSettings", "Enable", true, "Apply Window Overlay modificationpatching.");
            WindowSettingsPath = cfg.Bind("WindowSettings", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Window Overlay font name.");
            WindowSettingsScale = cfg.Bind("WindowSettings", "Scale", 0, "Window Overlay font scale.");

            WristEnable = cfg.Bind("Wrist", "Enable", true, "Apply Wrist font modification.");
            WristPath = cfg.Bind("Wrist", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Wrist font name.");
            WristScale = cfg.Bind("Wrist", "Scale", 0, "Wrist font scale.");
        }
    }
}
