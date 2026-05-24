using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Huye.Features.Menu
{
    public class MainMenuBootstrap : MonoBehaviour
    {
        private const string MouseSensitivityKey = "Huye.Menu.MouseSensitivity";
        private const string MasterVolumeKey = "Huye.Menu.MasterVolume";
        private const string MusicVolumeKey = "Huye.Menu.MusicVolume";
        private const string SfxVolumeKey = "Huye.Menu.SfxVolume";
        private const string FullscreenKey = "Huye.Menu.Fullscreen";

        [SerializeField] private float defaultMouseSensitivity = 200f;
        [SerializeField] private float defaultMasterVolume = 0.85f;
        [SerializeField] private float defaultMusicVolume = 0.7f;
        [SerializeField] private float defaultSfxVolume = 0.8f;
        [SerializeField] private Sprite backgroundSprite;

        private PlayerMovement playerMovement;
        private WendigoAI wendigoAI;
        private Canvas canvas;
        private GameObject menuPanel;
        private GameObject settingsPanel;
        private Text mouseSensitivityValue;
        private Text masterVolumeValue;
        private Text musicVolumeValue;
        private Text sfxVolumeValue;
        private Text fullscreenValue;
        private GameObject settingsContent;
        private float mouseSensitivity;
        private float masterVolume;
        private float musicVolume;
        private float sfxVolume;
        private bool fullscreenEnabled;
        private bool pausedFromGameplay;

        private void Awake()
        {
            playerMovement = FindAnyObjectByType<PlayerMovement>();
            wendigoAI = FindAnyObjectByType<WendigoAI>();
            ResolveBackgroundSprite();

            SetGameplayEnabled(false);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Start()
        {
            LoadSettings();
            BuildUi();
            ApplySettingsToRuntime();
            ShowMenu();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ResolveBackgroundSprite();
        }
#endif

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (canvas != null && canvas.gameObject.activeSelf && settingsPanel != null && settingsPanel.activeSelf)
                {
                    ShowMenu();
                }
                else
                {
                    OpenPauseMenu();
                }
            }
        }

        private void LoadSettings()
        {
            mouseSensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey, defaultMouseSensitivity);
            masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, defaultMasterVolume);
            musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, defaultMusicVolume);
            sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, defaultSfxVolume);
            fullscreenEnabled = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;
        }

        private void ResolveBackgroundSprite()
        {
            if (backgroundSprite != null)
            {
                return;
            }

#if UNITY_EDITOR
            Texture2D backgroundTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/images/IMG_MENU.jpeg");
            if (backgroundTexture == null)
            {
                return;
            }

            backgroundSprite = Sprite.Create(
                backgroundTexture,
                new Rect(0f, 0f, backgroundTexture.width, backgroundTexture.height),
                new Vector2(0.5f, 0.5f),
                100f);
#endif
        }

        private void BuildUi()
        {
            if (canvas != null)
            {
                return;
            }

            if (EventSystem.current == null)
            {
                GameObject eventSystemGo = new GameObject("EventSystem");
                eventSystemGo.AddComponent<EventSystem>();
                eventSystemGo.AddComponent<StandaloneInputModule>();
            }

            GameObject canvasGo = new GameObject("MainMenuCanvas");
            canvasGo.transform.SetParent(transform, false);
            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            GameObject background = CreatePanel(canvas.transform, "Background", new Color(0.02f, 0.02f, 0.025f, 0.98f), fullScreen: true);
            CreateBackgroundImage(background.transform);
            CreateTintOverlay(background.transform, new Color(0.02f, 0.02f, 0.03f, 0.42f));
            CreateTintOverlay(background.transform, new Color(0f, 0f, 0f, 0.18f));
            CreateGlow(background.transform, new Vector2(0.5f, 0.6f), new Vector2(0.75f, 0.7f), new Color(0.5f, 0.05f, 0.05f, 0.12f));
            CreateGlow(background.transform, new Vector2(0.3f, 0.25f), new Vector2(0.55f, 0.5f), new Color(0.1f, 0.1f, 0.1f, 0.15f));

            GameObject titleBlock = CreateContainer(background.transform, "TitleBlock", TextAnchor.MiddleCenter, 0f, 18f);
            CreateTitle(titleBlock.transform, "HUYE", 88f);
            CreateSubtitle(titleBlock.transform, "Escapa o perece", 22f);

            menuPanel = CreateMenuCard(background.transform, "MenuPanel");

            settingsPanel = CreateSettingsCard(background.transform, "SettingsPanel");
            CreateTitle(settingsPanel.transform, "OPCIONES", 44f);
            CreateSubtitle(settingsPanel.transform, "Configura audio, controles y pantalla.", 20f);

            settingsContent = CreateScrollArea(settingsPanel.transform, "SettingsScrollArea");

            CreateSection(settingsContent.transform, "AUDIO");
            masterVolumeValue = CreateSliderRow(settingsContent.transform, "Volumen general", masterVolume, OnMasterVolumeChanged, 0f, 1f);
            musicVolumeValue = CreateSliderRow(settingsContent.transform, "Música", musicVolume, OnMusicVolumeChanged, 0f, 1f);
            sfxVolumeValue = CreateSliderRow(settingsContent.transform, "Efectos", sfxVolume, OnSfxVolumeChanged, 0f, 1f);

            CreateSection(settingsContent.transform, "CONTROLES");
            mouseSensitivityValue = CreateSliderRow(settingsContent.transform, "Sensibilidad del mouse", mouseSensitivity, OnMouseSensitivityChanged, 20f, 600f);

            CreateSection(settingsContent.transform, "PANTALLA");
            fullscreenValue = CreateToggleRow(settingsContent.transform, "Pantalla completa", fullscreenEnabled, ToggleFullscreen);

            CreateActionRow(settingsPanel.transform,
                CreateButton(settingsPanel.transform, "APLICAR CAMBIOS", ApplySettings, accent: true),
                CreateButton(settingsPanel.transform, "VOLVER", ShowMenu, accent: false),
                CreateButton(settingsPanel.transform, "SALIR", QuitGame, accent: false));
        }

        private GameObject CreateMenuCard(Transform parent, string name)
        {
            GameObject card = CreatePanel(parent, name, new Color(0.05f, 0.05f, 0.06f, 0.9f), false);
            RectTransform rectTransform = card.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.45f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.45f);
            rectTransform.sizeDelta = new Vector2(
                Mathf.Clamp(Screen.width * 0.34f, 520f, 720f),
                Mathf.Clamp(Screen.height * 0.46f, 380f, 520f));

            VerticalLayoutGroup layout = card.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(48, 48, 42, 42);
            layout.spacing = 16f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            CreateSubtitle(card.transform, "Menú principal", 18f);
            CreateButton(card.transform, "NUEVA PARTIDA", StartGame, accent: true);
            CreateButton(card.transform, "OPCIONES", ShowSettings, accent: false);
            CreateButton(card.transform, "SALIR", QuitGame, accent: false);

            return card;
        }

        private GameObject CreateSettingsCard(Transform parent, string name)
        {
            GameObject card = CreatePanel(parent, name, new Color(0.05f, 0.05f, 0.06f, 0.92f), false);
            RectTransform rectTransform = card.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(
                Mathf.Clamp(Screen.width * 0.62f, 760f, 1040f),
                Mathf.Clamp(Screen.height * 0.82f, 700f, 920f));

            VerticalLayoutGroup layout = card.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(52, 52, 44, 44);
            layout.spacing = 14f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            return card;
        }

        private GameObject CreateScrollArea(Transform parent, string name)
        {
            GameObject scrollArea = new GameObject(name, typeof(RectTransform), typeof(LayoutElement), typeof(Image), typeof(ScrollRect));
            scrollArea.transform.SetParent(parent, false);

            LayoutElement layoutElement = scrollArea.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = 1f;
            layoutElement.flexibleHeight = 1f;

            Image scrollBackground = scrollArea.GetComponent<Image>();
            scrollBackground.color = new Color(0f, 0f, 0f, 0.12f);

            ScrollRect scrollRect = scrollArea.GetComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 38f;

            GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
            viewport.transform.SetParent(scrollArea.transform, false);

            RectTransform viewportRect = viewport.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = new Vector2(0f, 0f);
            viewportRect.offsetMax = new Vector2(-20f, 0f);

            Image viewportImage = viewport.GetComponent<Image>();
            viewportImage.color = new Color(0f, 0f, 0f, 0.01f);

            Mask mask = viewport.GetComponent<Mask>();
            mask.showMaskGraphic = false;

            GameObject content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            content.transform.SetParent(viewport.transform, false);

            RectTransform contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.offsetMin = new Vector2(0f, 0f);
            contentRect.offsetMax = new Vector2(0f, 0f);

            VerticalLayoutGroup contentLayout = content.GetComponent<VerticalLayoutGroup>();
            contentLayout.padding = new RectOffset(0, 10, 0, 0);
            contentLayout.spacing = 12f;
            contentLayout.childAlignment = TextAnchor.UpperCenter;
            contentLayout.childControlHeight = true;
            contentLayout.childControlWidth = true;
            contentLayout.childForceExpandHeight = false;
            contentLayout.childForceExpandWidth = true;

            ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.viewport = viewportRect;
            scrollRect.content = contentRect;

            GameObject scrollbarObject = new GameObject("Scrollbar", typeof(RectTransform), typeof(Image), typeof(Scrollbar));
            scrollbarObject.transform.SetParent(scrollArea.transform, false);

            RectTransform scrollbarRect = scrollbarObject.GetComponent<RectTransform>();
            scrollbarRect.anchorMin = new Vector2(1f, 0f);
            scrollbarRect.anchorMax = new Vector2(1f, 1f);
            scrollbarRect.pivot = new Vector2(1f, 1f);
            scrollbarRect.sizeDelta = new Vector2(14f, 0f);
            scrollbarRect.anchoredPosition = Vector2.zero;

            Image scrollbarBackground = scrollbarObject.GetComponent<Image>();
            scrollbarBackground.color = new Color(1f, 1f, 1f, 0.08f);

            Scrollbar scrollbar = scrollbarObject.GetComponent<Scrollbar>();
            scrollbar.direction = Scrollbar.Direction.BottomToTop;

            GameObject handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handle.transform.SetParent(scrollbarObject.transform, false);

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.anchorMin = Vector2.zero;
            handleRect.anchorMax = Vector2.one;
            handleRect.offsetMin = new Vector2(1f, 1f);
            handleRect.offsetMax = new Vector2(-1f, -1f);

            Image handleImage = handle.GetComponent<Image>();
            handleImage.color = new Color(0.92f, 0.2f, 0.15f, 0.95f);

            scrollbar.targetGraphic = handleImage;
            scrollbar.handleRect = handleRect;
            scrollRect.verticalScrollbar = scrollbar;
            scrollRect.verticalScrollbarSpacing = 2f;

            return content;
        }

        private GameObject CreatePanel(Transform parent, string name, Color color, bool fullScreen)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(VerticalLayoutGroup));
            panel.transform.SetParent(parent, false);

            RectTransform rectTransform = panel.GetComponent<RectTransform>();
            if (fullScreen)
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
            }

            Image image = panel.GetComponent<Image>();
            image.color = color;

            VerticalLayoutGroup layout = panel.GetComponent<VerticalLayoutGroup>();
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            return panel;
        }

        private void CreateBackgroundImage(Transform parent)
        {
            if (backgroundSprite == null)
            {
                return;
            }

            GameObject imageObject = new GameObject("BackgroundImage", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            imageObject.transform.SetParent(parent, false);

            RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            Image image = imageObject.GetComponent<Image>();
            image.sprite = backgroundSprite;
            image.preserveAspect = false;
            image.color = new Color(0.82f, 0.82f, 0.82f, 1f);

            LayoutElement layoutElement = imageObject.GetComponent<LayoutElement>();
            layoutElement.ignoreLayout = true;

            imageObject.transform.SetSiblingIndex(0);
        }

        private void CreateTintOverlay(Transform parent, Color color)
        {
            GameObject overlay = new GameObject("TintOverlay", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            overlay.transform.SetParent(parent, false);

            RectTransform rectTransform = overlay.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            Image image = overlay.GetComponent<Image>();
            image.color = color;

            LayoutElement layoutElement = overlay.GetComponent<LayoutElement>();
            layoutElement.ignoreLayout = true;

            overlay.transform.SetAsLastSibling();
        }

        private void CreateGlow(Transform parent, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            GameObject glow = new GameObject("Glow", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            glow.transform.SetParent(parent, false);
            RectTransform rectTransform = glow.GetComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            glow.GetComponent<Image>().color = color;

            LayoutElement layoutElement = glow.GetComponent<LayoutElement>();
            layoutElement.ignoreLayout = true;
        }

        private GameObject CreateContainer(Transform parent, string name, TextAnchor alignment, float width, float height)
        {
            GameObject container = new GameObject(name, typeof(RectTransform), typeof(VerticalLayoutGroup));
            container.transform.SetParent(parent, false);

            RectTransform rectTransform = container.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.82f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.82f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(width, height);

            VerticalLayoutGroup layout = container.GetComponent<VerticalLayoutGroup>();
            layout.childAlignment = alignment;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            layout.spacing = 8f;

            return container;
        }

        private void CreateSection(Transform parent, string title)
        {
            GameObject header = new GameObject(title + "Header", typeof(RectTransform), typeof(LayoutElement));
            header.transform.SetParent(parent, false);
            LayoutElement layoutElement = header.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = 34f;
            CreateText(header.transform, title, 20f, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.95f, 0.35f, 0.22f, 1f));
        }

        private void CreateSubtitle(Transform parent, string textValue, float fontSize)
        {
            CreateText(parent, textValue, fontSize, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.8f, 0.8f, 0.8f, 0.95f));
        }

        private void CreateTitle(Transform parent, string textValue, float fontSize)
        {
            Text title = CreateText(parent, textValue, fontSize, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.88f, 0.12f, 0.12f, 1f));
            title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        private Text CreateText(Transform parent, string textValue, float fontSize, FontStyle fontStyle, TextAnchor alignment, Color color)
        {
            GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(Text), typeof(LayoutElement));
            textObject.transform.SetParent(parent, false);

            RectTransform rectTransform = textObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            Text text = textObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = textValue;
            text.fontSize = Mathf.RoundToInt(fontSize);
            text.fontStyle = fontStyle;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            LayoutElement layoutElement = textObject.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = fontSize + 18f;
            layoutElement.flexibleWidth = 1f;

            return text;
        }

        private Button CreateButton(Transform parent, string label, UnityEngine.Events.UnityAction onClick, bool accent)
        {
            GameObject buttonObject = new GameObject(label + "Button", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            buttonObject.transform.SetParent(parent, false);

            RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(0f, 60f);

            Image image = buttonObject.GetComponent<Image>();
            image.color = accent ? new Color(0.26f, 0.06f, 0.08f, 0.9f) : new Color(0.14f, 0.14f, 0.16f, 0.88f);

            Button button = buttonObject.GetComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = image.color;
            colors.highlightedColor = accent ? new Color(0.45f, 0.09f, 0.12f, 0.96f) : new Color(0.22f, 0.22f, 0.25f, 0.96f);
            colors.pressedColor = accent ? new Color(0.65f, 0.14f, 0.14f, 1f) : new Color(0.3f, 0.3f, 0.33f, 1f);
            colors.selectedColor = colors.highlightedColor;
            colors.colorMultiplier = 1f;
            button.colors = colors;

            GameObject border = new GameObject("Border", typeof(RectTransform), typeof(Image));
            border.transform.SetParent(buttonObject.transform, false);
            RectTransform borderRect = border.GetComponent<RectTransform>();
            borderRect.anchorMin = Vector2.zero;
            borderRect.anchorMax = Vector2.one;
            borderRect.offsetMin = new Vector2(0f, 0f);
            borderRect.offsetMax = new Vector2(0f, 0f);
            border.GetComponent<Image>().color = accent ? new Color(0.93f, 0.24f, 0.16f, 0.18f) : new Color(1f, 1f, 1f, 0.06f);

            GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
            labelObject.transform.SetParent(buttonObject.transform, false);
            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            Text text = labelObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = label;
            text.fontSize = 24;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;

            LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = 60f;

            if (onClick != null)
            {
                button.onClick.AddListener(onClick);
            }

            return button;
        }

        private void CreateActionRow(Transform parent, params Button[] buttons)
        {
            GameObject row = new GameObject("ActionRow", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            row.transform.SetParent(parent, false);

            LayoutElement layoutElement = row.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = 70f;

            HorizontalLayoutGroup layout = row.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = 14f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            if (buttons == null)
            {
                return;
            }

            foreach (Button button in buttons)
            {
                if (button != null)
                {
                    button.transform.SetParent(row.transform, false);
                }
            }
        }

        private Text CreateSliderRow(Transform parent, string label, float value, UnityEngine.Events.UnityAction<float> onValueChanged, float minValue, float maxValue)
        {
            GameObject row = new GameObject(label + "Row", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(LayoutElement));
            row.transform.SetParent(parent, false);

            LayoutElement layoutElement = row.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = 92f;

            VerticalLayoutGroup layout = row.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 6f;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            GameObject header = new GameObject("Header", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            header.transform.SetParent(row.transform, false);

            HorizontalLayoutGroup headerLayout = header.GetComponent<HorizontalLayoutGroup>();
            headerLayout.childAlignment = TextAnchor.MiddleLeft;
            headerLayout.childForceExpandWidth = true;
            headerLayout.childForceExpandHeight = false;

            Text title = CreateText(header.transform, label, 18f, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
            title.GetComponent<LayoutElement>().preferredHeight = 28f;

            Text valueText = CreateText(header.transform, FormatSettingValue(label, value), 18f, FontStyle.Bold, TextAnchor.MiddleRight, new Color(0.95f, 0.78f, 0.58f, 1f));
            valueText.GetComponent<LayoutElement>().preferredWidth = 150f;

            GameObject sliderObject = new GameObject(label + "Slider", typeof(RectTransform), typeof(Slider));
            sliderObject.transform.SetParent(row.transform, false);

            Slider slider = sliderObject.GetComponent<Slider>();
            slider.minValue = minValue;
            slider.maxValue = maxValue;
            slider.value = value;
            slider.wholeNumbers = false;
            slider.onValueChanged.AddListener(onValueChanged);

            Image background = sliderObject.AddComponent<Image>();
            background.color = new Color(0.16f, 0.16f, 0.18f, 1f);
            slider.targetGraphic = background;

            RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
            sliderRect.sizeDelta = new Vector2(0f, 18f);

            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderObject.transform, false);
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0f, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1f, 0.75f);
            fillAreaRect.offsetMin = new Vector2(8f, 0f);
            fillAreaRect.offsetMax = new Vector2(-8f, 0f);

            GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(fillArea.transform, false);
            Image fillImage = fill.GetComponent<Image>();
            fillImage.color = new Color(0.9f, 0.18f, 0.14f, 1f);

            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            slider.fillRect = fillRect;
            slider.fillRect.offsetMin = Vector2.zero;
            slider.fillRect.offsetMax = Vector2.zero;

            GameObject handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handle.transform.SetParent(sliderObject.transform, false);
            Image handleImage = handle.GetComponent<Image>();
            handleImage.color = new Color(0.97f, 0.88f, 0.72f, 1f);
            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(18f, 18f);
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImage;

            slider.direction = Slider.Direction.LeftToRight;
            slider.transition = Selectable.Transition.ColorTint;
            slider.interactable = true;

            return valueText;
        }

        private Text CreateToggleRow(Transform parent, string label, bool value, UnityEngine.Events.UnityAction onClick)
        {
            GameObject row = new GameObject(label + "Row", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            row.transform.SetParent(parent, false);

            LayoutElement layoutElement = row.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = 54f;

            HorizontalLayoutGroup layout = row.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = 12f;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childForceExpandWidth = false;

            Text title = CreateText(row.transform, label, 18f, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
            title.GetComponent<LayoutElement>().preferredWidth = 320f;

            Button toggleButton = CreateButton(row.transform, value ? "SÍ" : "NO", onClick, accent: true);
            toggleButton.GetComponent<LayoutElement>().preferredWidth = 140f;

            Text valueText = CreateText(row.transform, value ? "Activado" : "Desactivado", 16f, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.86f, 0.86f, 0.86f, 0.95f));
            valueText.GetComponent<LayoutElement>().preferredWidth = 180f;

            return valueText;
        }

        private string FormatSettingValue(string label, float value)
        {
            if (label.Contains("Mouse"))
            {
                return value.ToString("0");
            }

            return Mathf.RoundToInt(value * 100f) + "%";
        }

        private void SetGameplayEnabled(bool enabled)
        {
            if (playerMovement != null)
            {
                playerMovement.enabled = enabled;
            }

            if (wendigoAI != null)
            {
                wendigoAI.enabled = enabled;
            }
        }

        private void ApplySettingsToRuntime()
        {
            if (playerMovement != null)
            {
                playerMovement.mouseSensitivity = mouseSensitivity;
            }

            AudioListener.volume = Mathf.Clamp01(masterVolume);
            Screen.fullScreen = fullscreenEnabled;

            RefreshValues();
        }

        private void RefreshValues()
        {
            if (mouseSensitivityValue != null)
            {
                mouseSensitivityValue.text = FormatSettingValue("Mouse", mouseSensitivity);
            }

            if (masterVolumeValue != null)
            {
                masterVolumeValue.text = Mathf.RoundToInt(masterVolume * 100f) + "%";
            }

            if (musicVolumeValue != null)
            {
                musicVolumeValue.text = Mathf.RoundToInt(musicVolume * 100f) + "%";
            }

            if (sfxVolumeValue != null)
            {
                sfxVolumeValue.text = Mathf.RoundToInt(sfxVolume * 100f) + "%";
            }

            if (fullscreenValue != null)
            {
                fullscreenValue.text = fullscreenEnabled ? "Activado" : "Desactivado";
            }
        }

        private void ShowMenu()
        {
            if (pausedFromGameplay)
            {
                ResumeGame();
                return;
            }

            if (menuPanel != null)
            {
                menuPanel.SetActive(true);
            }

            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }

            SetGameplayEnabled(false);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void ShowSettings()
        {
            if (menuPanel != null)
            {
                menuPanel.SetActive(false);
            }

            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
            }

            pausedFromGameplay = false;
            SetGameplayEnabled(false);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void OpenPauseMenu()
        {
            pausedFromGameplay = true;

            if (canvas != null)
            {
                canvas.gameObject.SetActive(true);
            }

            if (menuPanel != null)
            {
                menuPanel.SetActive(false);
            }

            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
            }

            SetGameplayEnabled(false);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void ResumeGame()
        {
            pausedFromGameplay = false;

            if (canvas != null)
            {
                canvas.gameObject.SetActive(false);
            }

            SetGameplayEnabled(true);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void StartGame()
        {
            if (canvas != null)
            {
                canvas.gameObject.SetActive(false);
            }

            pausedFromGameplay = false;
            SetGameplayEnabled(true);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void ApplySettings()
        {
            PlayerPrefs.SetFloat(MouseSensitivityKey, mouseSensitivity);
            PlayerPrefs.SetFloat(MasterVolumeKey, masterVolume);
            PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
            PlayerPrefs.SetFloat(SfxVolumeKey, sfxVolume);
            PlayerPrefs.SetInt(FullscreenKey, fullscreenEnabled ? 1 : 0);
            PlayerPrefs.Save();
            ApplySettingsToRuntime();
        }

        private void OnMouseSensitivityChanged(float value)
        {
            mouseSensitivity = value;
            ApplySettings();
        }

        private void OnMasterVolumeChanged(float value)
        {
            masterVolume = value;
            ApplySettings();
        }

        private void OnMusicVolumeChanged(float value)
        {
            musicVolume = value;
            ApplySettings();
        }

        private void OnSfxVolumeChanged(float value)
        {
            sfxVolume = value;
            ApplySettings();
        }

        private void ToggleFullscreen()
        {
            fullscreenEnabled = !fullscreenEnabled;
            ApplySettings();
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            Debug.Log("Quit requested from main menu.");
#else
            Application.Quit();
#endif
        }
    }
}