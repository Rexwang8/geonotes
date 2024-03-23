using UnityEngine;
using UnityEngine.UI;

public class ChangeColorOnButtonClick : MonoBehaviour
{
    // Array of references to the panels' Image components
    public Image[] panelImages;

    // Function to change the color of the panels
    public void ChangePanelColors()
    {
        // Loop through each panel and change its color
        foreach (Image panelImage in panelImages)
        {
            // Change the background color of the panel to light grey
            panelImage.color = new Color(0.8f, 0.8f, 0.8f, 1f); // Light grey color
        }
    }
}
