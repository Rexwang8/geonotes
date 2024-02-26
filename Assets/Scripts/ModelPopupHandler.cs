using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TigerForge;
using TMPro;

namespace SpatialNotes {
public class ModelPopupHandler : MonoBehaviour
{
    public GameObject modelPopup;
    public TextMeshProUGUI modelPopupText;
    public TextMeshProUGUI modelPopupTitle;
    //array of sprites
    public Sprite[] modelPopupImages;
    public GameObject modelPopupImage;



    // Awake listen to events
    void Awake()
    {
        EventManager.StartListening("MODEL_POPUP", ModelPopup);
    }

    // Set text and title of model popup
    private void _setModelPopupText(string text, string title)
    {
        modelPopupText.text = text;
        modelPopupTitle.text = title;
    }

    // Show model popup
    public void ModelPopup()
    {
        // get data
        string text = EventManager.GetString("MODEL_POPUP");
        string title = EventManager.GetString("MODEL_POPUP_TITLE");
        _setModelPopupText(text, title);
        //get image name
        string imageName = EventManager.GetString("MODEL_POPUP_IMAGE").ToLower().Replace(" ", "_");
        //resolve image
        Sprite image = null;
        foreach (Sprite sprite in modelPopupImages)
        {
            if (sprite.name == imageName)
            {
                image = sprite;
                break;
            }
        }
        //set image
        modelPopupImage.GetComponent<UnityEngine.UI.Image>().sprite = image;
        //show popup
        modelPopup.SetActive(true);
        // move to middle of screen
        modelPopup.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);

    }

    // Close model popup
    public void CloseModelPopup()
    {
        modelPopup.SetActive(false);
        // move away from screen
        modelPopup.transform.position = new Vector3(Screen.width / 2, Screen.height * 2, 0);
    }



}
}
