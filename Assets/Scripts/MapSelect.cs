using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;
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
        public string _pathToImageBeingCreated;
        public GameObject _mapNameInput;

        //pick mode enum { Files = 0, Folders = 1, FilesAndFolders = 2 };
        private GameObject _contentScrollView;


        // Destroy all contentScrollView children 
        public void DestroyAllChildren()
        {
            for (var i = _contentScrollView.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(_contentScrollView.transform.GetChild(i).gameObject);
            }
        }

        // Refresh map panel in a coroutine
        public IEnumerator RefreshMapSelectPanelCoroutine()
        {
            DestroyAllChildren();

            // wait for a 200ms
            yield return new WaitForSeconds(0.2f);
            Debug.Log("Remaing number of children: " + _contentScrollView.transform.childCount);
            if (_contentScrollView.transform.childCount > 0)
            {
                Debug.Log("Children not destroyed (could be first frame)");
                yield return null;
            }

            // Create a new list of items
            _defaultPath = Application.streamingAssetsPath + "/Maps";

            // Get only directories, not files
            string[] directories = Directory.GetDirectories(_defaultPath);
            List<string> items = new List<string>();
            List<string> tbnPaths = new List<string>();
            foreach (string directoryPath in directories)
            {
                // Extract the folder name from the path
                string folderName = Path.GetFileName(directoryPath);
                items.Add(folderName);
                tbnPaths.Add(directoryPath + "/tbn.png");
            }

            // Create gameobjects as buttons that are children of the scroll view
            for (int i = 0; i < items.Count; i++)
            {
                //generic tmp button
                GameObject button = Instantiate(buttonPrefab) as GameObject;

                
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                string buttonTextString = items[i];
                //capitalize first letter
                buttonTextString = char.ToUpper(buttonTextString[0]) + buttonTextString.Substring(1);
                if (buttonTextString.Length > 20)
                {
                    buttonTextString = buttonTextString.Substring(0, 20) + "...";
                }
                buttonText.text = buttonTextString;
                buttonText.fontSize = 20;
                button.GetComponent<Button>().onClick.AddListener(ButtonFunction);
                button.transform.SetParent(_contentScrollView.transform);
                button.transform.localScale = new Vector3(1.50f, 1.25f, 1.25f);
                button.transform.localPosition = new Vector3(250, (-i * 1.5f * buttonPrefab.GetComponent<RectTransform>().rect.height) - 100, 0);

                // Add image to button
                string tbnPath = tbnPaths[i];
                if (File.Exists(tbnPath))
                {
                    Texture2D texture = new Texture2D(2, 2);
                    byte[] fileData = File.ReadAllBytes(tbnPath);
                    texture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                    UnityEngine.UI.Image buttonImg = button.transform.Find("Image").GetComponent<UnityEngine.UI.Image>();
                    buttonImg.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }


            }

            //update viewport size based on number of items
            RectTransform contentRectTransform = _contentScrollView.GetComponent<RectTransform>();
            contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, items.Count * 1.5f * buttonPrefab.GetComponent<RectTransform>().rect.height + 100);


            yield return null;
        }

        // Refresh map panel in a coroutine, delayed by 1s
        public IEnumerator RefreshMapSelectPanelCoroutineDelayed()
        {
            yield return new WaitForSeconds(0.25f);
            RefreshMapSelectPanel();
        }


        // Refresh map panel
        public void RefreshMapSelectPanel()
        {
            StartCoroutine(RefreshMapSelectPanelCoroutine());
        }

        // Refresh map panel about 1s from start
        public void RefreshMapSelectPanelDelayed()
        {
            StartCoroutine(RefreshMapSelectPanelCoroutineDelayed());
        }



        // Start is called before the first frame update
        void Start()
        {
            _contentScrollView = this.gameObject.transform.Find("Viewport").Find("Content").gameObject;
            // Refresh the map select panel
            RefreshMapSelectPanel();
            RefreshMapSelectPanelDelayed();
        }

        // Generic button function
        public void ButtonFunction()
        {
            string mapName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TMP_Text>().text;
            string buttonName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
            Debug.Log("Button " + buttonName + " was clicked, map name: " + mapName);

            // Instantiate messenger object
            GameObject messenger = new GameObject();
            messenger.tag = "Messenger";
            messenger.AddComponent<MessengerObject>();
            messenger.GetComponent<MessengerObject>().message = mapName;
            messenger.name = "MessengerObject";

            //Change scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        }

        // upload new map function
        public void OnClickNewMap()
        {

            bool valid = _validateMapNameAndImage();
            if (!valid)
            {
                return;
            }
            // Create the map
            string mapName = _mapNameInput.GetComponent<TMP_InputField>().text;
            mapName = mapName.Trim().Replace(" ", "_");
            string mapPath = Application.streamingAssetsPath + "/Maps/" + mapName;
            if (System.IO.Directory.Exists(mapPath))
            {
                TriggerPopup("Map " + mapName + " already exists", "Map Exists", "xmark");
                return;
            }
            string mapImagePath = Application.streamingAssetsPath + "/Maps/" + mapName + "/map.jpg";
            MapObject mapObject = new MapObject();
            mapObject.CreateMap(_name: mapName, _TEMP_IMAGE_PATH: _pathToImageBeingCreated);
            mapObject.SaveAll();
            // Refresh the map select panel
            RefreshMapSelectPanel();


            TriggerPopup(text: "Map " + _mapNameInput.GetComponent<TMP_InputField>().text + " created (NOT ACTUALLY, WIP)", title: "Map Created", imageName: "checkmark");
        }

        // upload image 
        public void OnClickUploadImage()
        {
            _openFileExplorerToLoadImages();
            RefreshMapSelectPanel();
        }

        // Debug - refresh map select panel
        public void OnClickRefresh()
        {
            RefreshMapSelectPanel();
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
                try
                {
                    string[] error = FileBrowser.Result;
                    string errorString = string.Join(", ", error);
                    Debug.Log("Error: " + errorString);
                    TriggerPopup("Error: " + errorString, "Error", "xmark");
                }
                catch (System.Exception e)
                {
                    Debug.Log("Error: " + e.Message);
                }
                

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
                _pathToImageBeingCreated = selectedPath;
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
            System.IO.Directory.CreateDirectory(destinationPath);
            string[] files = System.IO.Directory.GetFiles(selectedPath);
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destFile = System.IO.Path.Combine(destinationPath, fileName);
                System.IO.File.Copy(file, destFile, true);
            }


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