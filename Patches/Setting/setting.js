if (window.XSOverlayFONTCHANGER_SETTING) return 'XSOverlayFONTCHANGER_SETTING already injected';
window.XSOverlayFONTCHANGER_SETTING = true;

function InjectKBOSCTab() {
    var scr = document.createElement('script');
    scr.type = 'module';
    scr.textContent = "import * as Ui from './_Shared/js/uiComponents.js'; (" + function (Ui) {
        // --- Configuration ---
        const CONFIG = {
            pageId: 'Page_XSOverlayFONTCHANGER',
            pageName: 'Font Changer',
            pageIcon: 'bi-fonts',
            targetIndex: 0 // 0 for top, 1 for after General, etc.
        };

        const SECTIONS = [
            {
                name: 'Keyboard', priority: 1, settings: [
                    { type: Ui.ComponentType.Toggle, id: 'XSOverlayFontChanger_KeyboardEnable', name: 'Enable', description: 'Enabled Keyboard font patching.', default: true },
                    { type: Ui.ComponentType.Dropdown, id: 'XSOverlayFontChanger_KeyboardPath', name: 'Font', description: 'Keyboard font name.', default: '<<KeyboardFont>>', options: [<<FontList>>] },
                    { type: Ui.ComponentType.Slider, id: 'XSOverlayFontChanger_KeyboardScale', name: 'Scale', description: 'Keyboard font scale.', default: 0, options: [-10, 10, 1], unit: '%' },
                ]
            },
            {
                name: 'Notification', priority: 2, settings: [
                    { type: Ui.ComponentType.Toggle, id: 'XSOverlayFontChanger_NotificationEnable', name: 'Enable', description: 'Enabled Notification font patching.', default: true },
                    { type: Ui.ComponentType.Dropdown, id: 'XSOverlayFontChanger_NotificationPath', name: 'Font', description: 'Notification font name.', default: '<<NotificationFont>>', options: [<<FontList>>] },
                    { type: Ui.ComponentType.Slider, id: 'XSOverlayFontChanger_NotificationScale', name: 'Scale', description: 'Notification font scale.', default: 0, options: [-10, 10, 1], unit: '%' },
                ]
            },
            {
                name: 'Settings', priority: 3, settings: [
                    { type: Ui.ComponentType.Toggle, id: 'XSOverlayFontChanger_SettingsEnable', name: 'Enable', description: 'Enabled Settings font patching.', default: true },
                    { type: Ui.ComponentType.Dropdown, id: 'XSOverlayFontChanger_SettingsPath', name: 'Font', description: 'Settings font name.', default: '<<SettingsFont>>', options: [<<FontList>>] },
                    { type: Ui.ComponentType.Slider, id: 'XSOverlayFontChanger_SettingsScale', name: 'Scale', description: 'Settings font scale.', default: 0, options: [-10, 10, 1], unit: '%' },
                ]
            },
            {
                name: 'Tooltip', priority: 4, settings: [
                    { type: Ui.ComponentType.Toggle, id: 'XSOverlayFontChanger_TooltipEnable', name: 'Enable', description: 'Enabled Tooltip font patching.', default: true },
                    { type: Ui.ComponentType.Dropdown, id: 'XSOverlayFontChanger_TooltipPath', name: 'Font', description: 'Tooltip font name.', default: '<<TooltipFont>>', options: [<<FontList>>] },
                    { type: Ui.ComponentType.Slider, id: 'XSOverlayFontChanger_TooltipScale', name: 'Scale', description: 'Tooltip font scale.', default: 0, options: [-10, 10, 1], unit: '%' },
                ]
            },
            {
                name: 'WindowSettings', priority: 5, settings: [
                    { type: Ui.ComponentType.Toggle, id: 'XSOverlayFontChanger_WindowSettingsEnable', name: 'Enable', description: 'Enabled Window Overlay font patching.', default: true },
                    { type: Ui.ComponentType.Dropdown, id: 'XSOverlayFontChanger_WindowSettingsPath', name: 'Font', description: 'Window Overlay font name.', default: '<<WindowSettingsFont>>', options: [<<FontList>>] },
                    { type: Ui.ComponentType.Slider, id: 'XSOverlayFontChanger_WindowSettingsScale', name: 'Scale', description: 'Window Overlay font scale.', default: 0, options: [-10, 10, 1], unit: '%' },
                ]
            },
            {
                name: 'Wrist', priority: 6, settings: [
                    { type: Ui.ComponentType.Toggle, id: 'XSOverlayFontChanger_WristEnable', name: 'Enable', description: 'Enabled Wrist font patching.', default: true },
                    { type: Ui.ComponentType.Dropdown, id: 'XSOverlayFontChanger_WristPath', name: 'Font', description: 'Wrist font name.', default: '<<WristFont>>', options: [<<FontList>>] },
                    { type: Ui.ComponentType.Slider, id: 'XSOverlayFontChanger_WristScale', name: 'Scale', description: 'Wrist font scale.', default: 0, options: [-10, 10, 1], unit: '%' },
                ]
            },
            {
                name: 'About', priority: 5, settings: [
                    { type: Ui.ComponentType.Button, id: 'XSOverlayFontChanger_CheckForUpdate', name: 'Check for Updates', description: 'Check for the latest version of XSOverlay Font Changer.', default: true },
                    { type: Ui.ComponentType.Button, id: 'XSOverlayFontChanger_OpenGitHub', name: 'Open GitHub Page', description: 'Visit the XSOverlay Font Changer GitHub page.', default: true },
                ]
            }
        ];

        const sidebar = document.querySelector('.side-bar-button-container');
        const wrapper = document.querySelector('.page-wrapper');
        if (!sidebar || !wrapper || document.getElementById(CONFIG.pageId)) return;

        // --- Sidebar Navigation Button ---
        const existingBtns = Array.from(sidebar.querySelectorAll('.side-bar-button'));

        const navBtn = Ui.CreateElement(sidebar, 'button', ['side-bar-button']);
        Ui.CreateElement(navBtn, 'i', ['side-bar-button-icon', 'theme-font-contrast', 'bi', CONFIG.pageIcon]);
        const navLabel = Ui.CreateElement(navBtn, 'div', ['side-bar-button-text']);
        navLabel.innerHTML = CONFIG.pageName;

        // Determine insertion point for the button
        let referenceNodeForButton = null;
        if (CONFIG.targetIndex !== null && CONFIG.targetIndex < existingBtns.length) {
            referenceNodeForButton = existingBtns[CONFIG.targetIndex];
            sidebar.insertBefore(navBtn, referenceNodeForButton);
        }

        // Conditionally add a divider after the new button, mimicking existing sidebar behavior.
        // A divider is added after a button if there are other buttons following it.
        if (CONFIG.targetIndex !== null && CONFIG.targetIndex < existingBtns.length) {
            const newDivider = Ui.CreateElement(sidebar, 'div', ['sidebar-divider']);
            sidebar.insertBefore(newDivider, navBtn.nextSibling);
        }

        // --- Settings Page Layout ---
        const pageRoot = Ui.CreateElement(wrapper, 'div', ['page-container', 'theme-dark']);
        pageRoot.id = CONFIG.pageId;
        pageRoot.style.cssText = 'position:absolute; opacity:0; pointer-events:none;';

        const header = Ui.CreateElement(pageRoot, 'div', ['page-header']);
        const headerText = Ui.CreateElement(header, 'div', ['page-header-text']);
        headerText.innerHTML = CONFIG.pageName;


        // --- Setting Builder Helper ---
        const addSetting = (sectionObj, type, id, name, desc, defaultValue, opts, opts1) => {
            const setting = new Ui.Setting(type, name, desc, defaultValue, opts, opts1);
            setting.internalName = id;
            setting.sectionID = sectionObj.Name;

            const componentCreators = {
                [Ui.ComponentType.Toggle]: () => Ui.Toggle(setting, name, defaultValue, null, sectionObj.Background),
                [Ui.ComponentType.Button]: () => Ui.Button(setting, sectionObj.Background),
                [Ui.ComponentType.Slider]: () => {
                    Ui.Slider(setting, name, defaultValue, opts, opts1, sectionObj.Background, 300);
                    const el = document.getElementById(id);
                    if (el) Ui.UpdateSliderUI(el, defaultValue);
                },
                [Ui.ComponentType.Dropdown]: () => {
                    Ui.Dropdown(setting, name, defaultValue, opts, sectionObj.Background, 300);
                    const el = document.getElementById(id);
                    if (el) {
                        const options = el.querySelectorAll('.selectopt');
                        for (const opt of options) {
                            if (opt.getAttribute('internalName') === defaultValue || opt.getAttribute('index') === String(defaultValue)) {
                                opt.checked = true;
                                break;
                            }
                        }
                    }
                }
            };

            if (componentCreators[type]) componentCreators[type]();
            if (desc || type === Ui.ComponentType.Text) Ui.Description(sectionObj.Background, desc || '', id + '_Desc');

            Ui.Divider(sectionObj.Background, 'divider');
        };

        // --- Build Sections ---
        SECTIONS.forEach(s => {
            const section = new Ui.Section(s.name, s.priority, pageRoot);
            s.settings.forEach(set => {
                addSetting(section, set.type, set.id, set.name, set.description, set.default, set.options, set.unit);
            });
        });

        // --- Navigation Logic ---
        const switchPage = () => {
            wrapper.querySelectorAll('.page-container, .page-container-no-overflow').forEach(p => {
                if (p !== pageRoot) {
                    p.style.animation = '0.3s ease fade-out forwards';
                    p.style.pointerEvents = 'none';
                }
            });

            pageRoot.style.animation = '0.3s ease fade-in forwards';
            pageRoot.style.pointerEvents = 'auto';
        };

        sidebar.addEventListener('click', (e) => {
            const btn = e.target.closest('.side-bar-button');
            if (!btn) return;

            if (btn === navBtn) {
                switchPage();
            } else {
                pageRoot.style.animation = '0.3s ease fade-out forwards';
                pageRoot.style.pointerEvents = 'none';

                wrapper.querySelectorAll('.page-container, .page-container-no-overflow').forEach(p => {
                    if (p !== pageRoot) {
                        p.style.pointerEvents = 'auto';
                    }
                });
            }

            // Update visual selection for all buttons in sidebar
            sidebar.querySelectorAll('.side-bar-button').forEach(b => {
                const isTarget = b === btn;
                b.classList.toggle('side-bar-button-selected', isTarget);
                if (b.firstElementChild) b.firstElementChild.classList.toggle('selected-icon', isTarget);
            });
        });
    }.toString() + ") (Ui);";
    document.body.appendChild(scr);
}

InjectKBOSCTab()

return 'XSOverlayFONTCHANGER_SETTING injected';