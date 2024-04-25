using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class AutoAdjustingInputFieldController : MonoBehaviour
{
    private TMP_InputField inputField;
    public float verticalPadding = 10f; // Adjust this value to set the vertical padding

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();

        // Ensure that the TMP Input Field has the vertical scrollbar disabled
        inputField.verticalScrollbar = null;
    }

    private void LateUpdate()
    {
        // Calculate the preferred height based on the text content, line spacing, and padding
        float preferredHeight = inputField.textComponent.preferredHeight + verticalPadding * 2; // Double the padding for both top and bottom

        // Adjust the TMP Input Field height
        RectTransform rectTransform = inputField.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, preferredHeight);
    }
}