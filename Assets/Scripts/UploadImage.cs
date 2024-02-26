using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class ImageUpload : MonoBehaviour
{
    public Button uploadButton;
    public InputField filePathInput;

    public void Start()
    {
        uploadButton.onClick.AddListener(UploadImage);
    }

    public void UploadImage()
    {
        string imagePath = filePathInput.text;

        if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
        {
            // Read image file as bytes
            byte[] imageData = File.ReadAllBytes(imagePath);

            // Process the image data as needed
            Debug.Log("Image selected: " + imagePath);
        }
        else
        {
            Debug.LogWarning("Invalid or empty file path");
        }
    }
}
