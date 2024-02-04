using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MapSelect : MonoBehaviour
{
    // DEBUG - number of items
    public int numItems = 10;
    public GameObject buttonPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // Create a new list of items
        List<string> items = new List<string>();
        for (int i = 0; i < numItems; i++)
        {
            items.Add("Item " + i);
        }

        // Create gameobjects as buttons that are children of the scroll view
        GameObject scrollViewContent = this.gameObject.transform.Find("Viewport").Find("Content").gameObject;
        for (int i = 0; i < items.Count; i++)
        {
            //generic tmp button
            GameObject button = Instantiate(buttonPrefab) as GameObject;
            button.transform.SetParent(scrollViewContent.transform);
            button.transform.localScale = new Vector3(1, 1, 1);
            button.gameObject.name = "Button " + i;
        }

        // Offset content by twice the height of the button
        for (int i = 0; i < items.Count; i++)
        {
            GameObject buttonObj = scrollViewContent.transform.GetChild(i).gameObject;
            float buttonObjHeight = buttonObj.GetComponent<RectTransform>().rect.height;
            buttonObj.transform.localPosition = new Vector3(0, (-i * 2 * buttonObjHeight) - 50, 0);

            // Set text for tmp
            scrollViewContent.transform.GetChild(i).GetComponentInChildren<TMP_Text>().text = items[i];

            // Add a button function to the button
            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(ButtonFunction);
        }
        
    }

    // Generic button function
    public void ButtonFunction()
    {
        Debug.Log("Button Clicked from button: " + UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
    }
}
