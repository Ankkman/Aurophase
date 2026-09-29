using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace MyGame.Ui
{
    public class PlanetInfoUI : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private RectTransform infoPanel;

        [Header("Text Settings")]
        [SerializeField] private TMP_FontAsset font;

        [ContextMenu("Build Info Panel")]
        public void BuildInfoPanel()
        {
            if (infoPanel == null)
            {
                Debug.LogError("PlanetInfoUI: Info Panel is not assigned.");
                return;
            }

            // Remove existing generated children
            for (int i = infoPanel.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(infoPanel.GetChild(i).gameObject);
            }

            // Main vertical container
            GameObject content = CreateUIObject("Content", infoPanel);
            RectTransform contentRect = content.GetComponent<RectTransform>();

            contentRect.anchorMin = new Vector2(0, 0);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.offsetMin = new Vector2(35, 30);
            contentRect.offsetMax = new Vector2(-35, -30);

            VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 12;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            // Planet name
            CreateText(
                "PlanetName",
                "EARTH",
                content.transform,
                42,
                FontStyles.Bold
            );

            // Description
            TMP_Text description = CreateText(
                "Description",
                "Earth is the only known planet to support life.",
                content.transform,
                22,
                FontStyles.Normal
            );

            LayoutElement descriptionLayout =
                description.gameObject.AddComponent<LayoutElement>();

            descriptionLayout.minHeight = 90;

            // Divider
            GameObject divider = CreateUIObject("Divider", content.transform);

            Image dividerImage = divider.AddComponent<Image>();
            dividerImage.color = new Color(1f, 1f, 1f, 0.25f);

            LayoutElement dividerLayout =
                divider.AddComponent<LayoutElement>();

            dividerLayout.minHeight = 2;

            // Statistics Grid
            CreateStatsGrid(content.transform);

            Debug.Log("Planet Info Panel built successfully.");
        }

        private void CreateStatsGrid(Transform parent)
{
    GameObject gridObject = CreateUIObject("StatsGrid", parent);

    RectTransform gridRect = gridObject.GetComponent<RectTransform>();

    GridLayoutGroup grid = gridObject.AddComponent<GridLayoutGroup>();

    // Two-column layout
    grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
    grid.constraintCount = 2;

    // Size of each information block
    grid.cellSize = new Vector2(300f, 70f);

    // Space between columns and rows
    grid.spacing = new Vector2(20f, 18f);

    grid.childAlignment = TextAnchor.UpperLeft;

    // Make the grid fit inside the panel
    gridRect.anchorMin = new Vector2(0f, 0f);
    gridRect.anchorMax = new Vector2(1f, 0f);

    gridRect.pivot = new Vector2(0.5f, 0f);

    gridRect.offsetMin = Vector2.zero;
    gridRect.offsetMax = Vector2.zero;

    // Current content:
    // 3 stats = 2 rows
    LayoutElement gridLayout = gridObject.AddComponent<LayoutElement>();
    gridLayout.preferredHeight = 158f;

    // -----------------------------
    // Row 1
    // -----------------------------

    CreateStat(
        gridObject.transform,
        "DIAMETER",
        "12,742 km"
    );

    CreateStat(
        gridObject.transform,
        "ORBITAL PERIOD",
        "365.25 days"
    );

    // -----------------------------
    // Row 2
    // -----------------------------

    CreateStat(
        gridObject.transform,
        "DISTANCE FROM SUN",
        "149.6 million km"
    );

    Debug.Log("Planet statistics grid created.");
}


    private void CreateStat(
        Transform parent,
        string label,
        string value)
    {
        GameObject stat = CreateUIObject(
            label + "_Container",
            parent
        );

        RectTransform statRect =
            stat.GetComponent<RectTransform>();

        // -----------------------------------------
        // Vertical layout inside each grid cell
        // -----------------------------------------

        VerticalLayoutGroup layout =
            stat.AddComponent<VerticalLayoutGroup>();

        layout.spacing = 3f;

        layout.childControlWidth = true;
        layout.childControlHeight = false;

        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        layout.childAlignment = TextAnchor.UpperLeft;

        // -----------------------------------------
        // Label
        // -----------------------------------------

        TMP_Text labelText = CreateText(
            label + "_Label",
            label,
            stat.transform,
            15,
            FontStyles.Bold
        );

        labelText.color =
            new Color(1f, 1f, 1f, 0.65f);

        // -----------------------------------------
        // Value
        // -----------------------------------------

        TMP_Text valueText = CreateText(
            label + "_Value",
            value,
            stat.transform,
            25,
            FontStyles.Normal
        );

        valueText.color = Color.white;

        // -----------------------------------------
        // Cell height
        // -----------------------------------------

        LayoutElement statLayout =
            stat.AddComponent<LayoutElement>();

        statLayout.preferredHeight = 70f;
    }
        private TMP_Text CreateText(
            string objectName,
            string text,
            Transform parent,
            float fontSize,
            FontStyles style)
        {
            GameObject obj = CreateUIObject(objectName, parent);

            TextMeshProUGUI tmp =
                obj.AddComponent<TextMeshProUGUI>();

            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.fontStyle = style;
            tmp.color = Color.white;

            tmp.alignment = TextAlignmentOptions.Left;
            tmp.enableWordWrapping = true;
            tmp.overflowMode = TextOverflowModes.Overflow;

            LayoutElement layout =
                obj.AddComponent<LayoutElement>();

            layout.minHeight = fontSize + 8;

            if (font != null)
                tmp.font = font;

            return tmp;
        }

        private GameObject CreateUIObject(
            string objectName,
            Transform parent)
        {
            GameObject obj = new GameObject(
                objectName,
                typeof(RectTransform)
            );

            obj.transform.SetParent(parent, false);

            return obj;
        }
    }
}
