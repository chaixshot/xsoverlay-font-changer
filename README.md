<div align="center">

  # XSOverlay Font Changer
  ### Change the [XSOverlay](https://store.steampowered.com/app/1173510/XSOverlay/) font to your own lovely one
  <img width="1440" height="720" alt="Gemini_Generated_Image_rbhst1rbhst1rbhs" src="https://github.com/user-attachments/assets/ee9e058b-963e-4ce1-9ca0-5d590a97448e" />
  
</div>

## 🛠️ Features
- Using a Windows pre-installed font
- Change font setting in the XSOverlay settings menu
- Support changing at runtime
- Support Keyboard font patching
- Support WebView overlay CSS (Wrist, Setting, Notification) font patching
- Compatible with [xsoverlay-keyboard-osc](https://github.com/nyakowint/xsoverlay-keyboard-osc) custom input canvas

## 🖥️ Screenshot
<img src="./img/screenshot_1.jpeg" width="800"> <img src="./img/screenshot_2.jpeg" width="400"> <img src="./img/screenshot_3.jpeg" width="400">

## ⛏️ Installation
1. [Follow the BepInEx install guide](https://github.com/BepInEx/BepInEx/wiki/Installation) (BepInEx_win_x64) to [Steam]/steamapps/common/[XSOverlay].
2. Download the plugin ZIP from [Releases](https://github.com/chaixshot/xsoverlay-font-changer/releases/latest)
3. Extract the ZIP file and move folders inside ``xsoverlay-font-changer`` to ``[XSOverlay]/BepInEx``
4. You can change your lovely font in ``[XSOverlay]/BepInEx/config/xsoverlay.font.changer.cfg`` file with Notepad
5. Edit ``[XSOverlay]/BepInEx/config/BepInEx.cfg`` file with Notepad and change **HideManagerGameObject = true**
6. Start XSOverlay and enjoy!

## ⛔ Disable
Go to ``[Steam]/steamapps/common/[XSOverlay]/BepInEx/plugins/``, remove **xsoverlay_font_changer.dll**

## 🗑️ Uninstall
Go to ``[Steam]/steamapps/common/[XSOverlay]``, remove **BepInEx**, **doorstop_config.ini**, **winhttp.dll**
