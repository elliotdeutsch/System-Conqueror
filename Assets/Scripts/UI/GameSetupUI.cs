using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSetupUI : MonoBehaviour
{
    public GalaxyManager galaxyManager;

    private GameObject canvasObject;
    private TMP_InputField aiInput;
    private TMP_InputField widthInput;
    private TMP_InputField heightInput;
    private TMP_InputField starsInput;

    void Start()
    {
        if (galaxyManager == null)
        {
            galaxyManager = FindObjectOfType<GalaxyManager>();
        }

        CreateUI();
    }

    void CreateUI()
    {
        canvasObject = new GameObject("GameSetupCanvas");
        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(canvasObject.transform, false);
        var image = panel.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.8f);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(300, 260);
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;

        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(10, 10, 10, 10);
        layout.spacing = 10;
        layout.childControlHeight = true;
        layout.childControlWidth = true;

        aiInput = CreateLabeledInput(panel.transform, "Nombre d'IA", galaxyManager.numberOfAI.ToString());
        widthInput = CreateLabeledInput(panel.transform, "Largeur", galaxyManager.mapWidth.ToString());
        heightInput = CreateLabeledInput(panel.transform, "Hauteur", galaxyManager.mapHeight.ToString());
        starsInput = CreateLabeledInput(panel.transform, "Planètes", galaxyManager.numberOfStars.ToString());

        GameObject buttonObj = new GameObject("PlayButton");
        buttonObj.transform.SetParent(panel.transform, false);
        Button button = buttonObj.AddComponent<Button>();
        Image btnImage = buttonObj.AddComponent<Image>();
        btnImage.color = Color.white;
        RectTransform btnRect = buttonObj.GetComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(160, 30);

        GameObject btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(buttonObj.transform, false);
        TextMeshProUGUI btnText = btnTextObj.AddComponent<TextMeshProUGUI>();
        btnText.text = "Play";
        btnText.alignment = TextAlignmentOptions.Center;
        RectTransform txtRect = btnTextObj.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;

        button.onClick.AddListener(OnPlayClicked);
    }

    TMP_InputField CreateLabeledInput(Transform parent, string labelText, string defaultValue)
    {
        GameObject container = new GameObject(labelText + "Container");
        container.transform.SetParent(parent, false);

        HorizontalLayoutGroup hLayout = container.AddComponent<HorizontalLayoutGroup>();
        hLayout.childControlHeight = true;
        hLayout.childControlWidth = true;
        hLayout.spacing = 10;

        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(container.transform, false);
        TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
        label.text = labelText + ":";
        label.alignment = TextAlignmentOptions.MidlineLeft;
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.sizeDelta = new Vector2(120, 30);

        GameObject inputObj = new GameObject("InputField");
        inputObj.transform.SetParent(container.transform, false);
        TMP_InputField input = inputObj.AddComponent<TMP_InputField>();
        Image bg = inputObj.AddComponent<Image>();
        bg.color = Color.white;
        RectTransform inputRect = inputObj.GetComponent<RectTransform>();
        inputRect.sizeDelta = new Vector2(120, 30);

        GameObject placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(inputObj.transform, false);
        TextMeshProUGUI placeholder = placeholderObj.AddComponent<TextMeshProUGUI>();
        placeholder.text = defaultValue;
        placeholder.fontStyle = FontStyles.Italic;
        placeholder.color = new Color(0.5f,0.5f,0.5f,0.5f);
        RectTransform placeRect = placeholder.GetComponent<RectTransform>();
        placeRect.anchorMin = Vector2.zero;
        placeRect.anchorMax = Vector2.one;
        placeRect.offsetMin = Vector2.zero;
        placeRect.offsetMax = Vector2.zero;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(inputObj.transform, false);
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = defaultValue;
        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        input.textComponent = text;
        input.placeholder = placeholder;
        input.text = defaultValue;

        return input;
    }

    void OnPlayClicked()
    {
        if (galaxyManager == null) return;

        int ai = galaxyManager.numberOfAI;
        float width = galaxyManager.mapWidth;
        float height = galaxyManager.mapHeight;
        int stars = galaxyManager.numberOfStars;

        if (int.TryParse(aiInput.text, out int aiParsed))
        {
            ai = Mathf.Max(0, aiParsed);
        }
        if (float.TryParse(widthInput.text, out float widthParsed))
        {
            width = Mathf.Max(10f, widthParsed);
        }
        if (float.TryParse(heightInput.text, out float heightParsed))
        {
            height = Mathf.Max(10f, heightParsed);
        }
        if (int.TryParse(starsInput.text, out int starParsed))
        {
            stars = Mathf.Clamp(starParsed, 10, 1000);
        }

        galaxyManager.numberOfAI = ai;
        galaxyManager.mapWidth = width;
        galaxyManager.mapHeight = height;
        galaxyManager.numberOfStars = stars;

        canvasObject.SetActive(false);
        galaxyManager.InitializeGalaxy();
    }
}

