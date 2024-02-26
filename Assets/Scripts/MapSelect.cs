using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;
using UnityEditor;
using SimpleFileBrowser;
using TigerForge;


namespace SpatialNotes
{
    public class MapSelect : MonoBehaviour
    {
        public GameObject buttonPrefab;

        // Default path to open file explorer
        private string _defaultPath;
        private string[] _foundPaths;
        private bool _explorerActive = false;

        public GameObject _imageToCreateWith;
        public GameObject _mapNameInput;

        //pick mode enum { Files = 0, Folders = 1, FilesAndFolders = 2 };


        // Refresh map panel
        public void RefreshMapSelectPanel()
        {
            // Clear the current list of items
            GameObject scrollViewContent = this.gameObject.transform.Find("Viewport").Find("Content").gameObject;
            foreach (Transform child in scrollViewContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            // Create a new list of items

            string mapsDirectory = Application.streamingAssetsPath + "/Maps/";

            // Get only directories, not files
            string[] directories = Directory.GetDirectories(mapsDirectory);

            List<string> items = new List<string>();

            foreach (string directoryPath in directories)
            {
                // Extract the folder name from the path
                string folderName = Path.GetFileName(directoryPath);
                items.Add(folderName);
            }

            // Create gameobjects as buttons that are children of the scroll view
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

            // Set file browser default path
            _defaultPath = Application.streamingAssetsPath + "/Maps";
        }



        // Start is called before the first frame update
        void Start()
        {
            // Refresh the map select panel
            RefreshMapSelectPanel();
        }

        // Generic button function
        public void ButtonFunction()
        {
            string mapName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TMP_Text>().text;
            string buttonName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
            Debug.Log("Button " + buttonName + " was clicked, map name: " + mapName);
        }

        // upload new map function
        public void OnClickNewMap()
        {

            bool valid = _validateMapNameAndImage();
            if (!valid)
            {
                return;
            }
            TriggerPopup(text: "Map " + _mapNameInput.GetComponent<TMP_InputField>().text + " created (NOT ACTUALLY, WIP)", title: "Map Created", imageName: "checkmark");
        }

        // upload image 
        public void OnClickUploadImage()
        {
            _openFileExplorerToLoadImages();
        }

        // Load existing folder
        public void OnClickLoad()
        {
            //select folder
            _openFileExplorerToLoadFolder();
        }

        // Helper to open file explorer
        private void _openFileExplorerToLoadFolder()
        {
            if (_explorerActive)
            {
                Debug.Log("Explorer already active");
                return;
            }
            //open in maps directory, if it exists, else create it
            string path = Application.streamingAssetsPath + "/Maps";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            _setFileBrowserFilterFolderOnly();

            // Show a save file dialog, await response from dialog
            StartCoroutine(ShowLoadDialogCoroutineFoldersOnly());

            return;
        }

        // Helper to open the file explorer to pick only images
        private void _openFileExplorerToLoadImages()
        {
            if (_explorerActive)
            {
                Debug.Log("Explorer already active");
                TriggerPopup("Explorer already active", "Explorer Active", "xmark");
                return;
            }
            //open in maps directory, if it exists, else create it
            string path = Application.streamingAssetsPath + "/Maps";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            _setFileBrowserFilterImagesOnly();

            // Show a save file dialog, await response from dialog
            StartCoroutine(ShowLoadDialogCoroutineImagesOnly());

            return;
        }

        private void _setFileBrowserFilterFolderOnly()
        {
            //only allow directories to be selected
            FileBrowser.DisplayedEntriesFilter += (entry) =>
            {
                if (entry.IsDirectory)
                    return true; // Don't filter folders

                return false; // Filter files
            };

        }

        // Filter for only images
        private void _setFileBrowserFilterImagesOnly()
        {
            //only allow directories to be selected
            FileBrowser.DisplayedEntriesFilter += (entry) =>
            {
                if (entry.IsDirectory)
                    return false; // Don't filter folders


                string extension = Path.GetExtension(entry.Path).ToLower();
                if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".bmp" || extension == ".tiff")
                {
                    return true;
                }
                

                return false; // Filter files
            };

        }




        // Coroutine to show the load file dialog
        IEnumerator ShowLoadDialogCoroutineFoldersOnly()
        {
            _explorerActive = true;
            // Directories only
            yield return FileBrowser.WaitForLoadDialog(SimpleFileBrowser.FileBrowser.PickMode.Folders, true, _defaultPath, null, "Select Map Folder", "Load");
            Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);
            if (FileBrowser.Success)
            {
                OnFolderSelectedLoadFolderMap(FileBrowser.Result);
            }
            else
            {
                string[] error = FileBrowser.Result;
                string errorString = string.Join(", ", error);
                Debug.Log("Error: " + errorString);
                TriggerPopup("Error: " + errorString, "Error", "xmark");
                
            }
            _explorerActive = false;
        }

        // Coroutine to show the load file dialog for images
        IEnumerator ShowLoadDialogCoroutineImagesOnly()
        {
            _explorerActive = true;
            // Directories only
            yield return FileBrowser.WaitForLoadDialog(SimpleFileBrowser.FileBrowser.PickMode.Files, true, _defaultPath, null, "Select Map Folder", "Load");
            Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);
            if (FileBrowser.Success)
            {
                OnFolderSelectedLoadFileImages(FileBrowser.Result);
            }
            else
            {
                string[] error = FileBrowser.Result;
                string errorString = string.Join(", ", error);
                Debug.Log("Error: " + errorString);
                TriggerPopup("Error: " + errorString, "Error", "xmark");
            }
            _explorerActive = false;
        }

        // Logs the paths of the selected files
        void OnFolderSelectedLoadFileImages(string[] paths)
        {
            if (paths.Length == 0)
            {
                Debug.Log("No files were selected");
                TriggerPopup("No files were selected", "No Files Selected", "xmark");
                return;
            }
            foreach (string path in paths)
            {
                Debug.Log("Selected file: " + path);
            }

            // select only one 
            string selectedPath = paths[0];

            //Try to load the image
            Texture2D texture = new Texture2D(2, 2);
            byte[] fileData;
            if (File.Exists(selectedPath))
            {
                fileData = File.ReadAllBytes(selectedPath);
                texture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                _imageToCreateWith.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.Log("File does not exist: " + selectedPath);
                TriggerPopup("File does not exist: " + selectedPath, "File Not Found", "xmark");
            }

            // Refresh the map select panel
            RefreshMapSelectPanel();

            // Trigger popup
            TriggerPopup(text: "Image " + Path.GetFileName(selectedPath) + " loaded", title: "Image Loaded", imageName: "checkmark");

        }

        // Logs the paths of the selected files
        void OnFolderSelectedLoadFolderMap(string[] paths)
        {
            if (paths.Length == 0)
            {
                Debug.Log("No files were selected");
                TriggerPopup("No files were selected", "No Files Selected", "xmark");
                return;
            }
            foreach (string path in paths)
            {
                Debug.Log("Selected file: " + path);
            }

            // select only one 
            string selectedPath = paths[0];

            //Check if already exists in streaming assets
            string mapName = Path.GetFileName(selectedPath);
            string mapPath = Application.streamingAssetsPath + "/Maps/" + mapName;
            if (System.IO.Directory.Exists(mapPath))
            {
                TriggerPopup("Map " + mapName + " already exists", "Map Exists", "xmark");
                return;
            }

            // Copy the folder to the streaming assets
            string destinationPath = Application.streamingAssetsPath + "/Maps/" + mapName;
            FileUtil.CopyFileOrDirectory(selectedPath, destinationPath);

            // Refresh the map select panel
            RefreshMapSelectPanel();
            TriggerPopup(text: "Map " + mapName + " loaded", title: "Map Loaded", imageName: "checkmark");

        }

        // Invoke popup test
        public void OnClickPopup()
        {
            TriggerPopup("This is a test popup", "Test Popup", "test");
        }

        // Wrapper trigger popup
        public void TriggerPopup(string text, string title, string imageName)
        {
            EventManager.SetData("MODEL_POPUP", text);
            EventManager.SetData("MODEL_POPUP_TITLE", title);
            EventManager.SetData("MODEL_POPUP_IMAGE", imageName);
            EventManager.EmitEvent(eventName: "MODEL_POPUP", delay: 0, sender: gameObject);
        }

        // Validate the map name and image
        private bool _validateMapNameAndImage()
        {
            // Check if the map name is valid, get from input field
            string mapName = _mapNameInput.GetComponent<TMP_InputField>().text;
            mapName = mapName.Trim().Replace(" ", "_");
            string mapPath = Application.streamingAssetsPath + "/Maps/" + mapName;
            if (System.IO.Directory.Exists(mapPath))
            {
                TriggerPopup("Map " + mapName + " already exists", "Map Exists", "xmark");
                return false;
            }

            // Check if the image is valid
            if (_imageToCreateWith.GetComponent<UnityEngine.UI.Image>().sprite == null)
            {
                TriggerPopup("No image selected", "No Image", "xmark");
                return false;
            }
            return true;
        }
    }
}