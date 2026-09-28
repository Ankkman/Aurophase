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

            // Diameter
            CreateStat(content.transform, "DIAMETER", "12,742 km");

            // Orbital period
            CreateStat(content.transform, "ORBITAL PERIOD", "365.25 days");

            // Distance
            CreateStat(content.transform, "DISTANCE FROM SUN", "149.6 million km");

            Debug.Log("Planet Info Panel built successfully.");
        }

        private void CreateStat(
            Transform parent,
            string label,
            string value)
        {
            GameObject stat = CreateUIObject(label + "_Container", parent);

            VerticalLayoutGroup layout =
                stat.AddComponent<VerticalLayoutGroup>();

            layout.spacing = 2;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            CreateText(
                label + "_Label",
                label,
                stat.transform,
                16,
                FontStyles.Bold
            );

            CreateText(
                label + "_Value",
                value,
                stat.transform,
                26,
                FontStyles.Normal
            );

            LayoutElement statLayout =
                stat.AddComponent<LayoutElement>();

            statLayout.minHeight = 55;
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
            tmp.overflowMode = TextOverflowModes.Ellipsis;

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
