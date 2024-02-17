using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;
using UnityEditor;
using SimpleFileBrowser;



public class MapSelect : MonoBehaviour
{
    // DEBUG - number of items
    public int numItems = 10;
    public GameObject buttonPrefab;

    // Default path to open file explorer
    private string _defaultPath;

    //pick mode enum { Files = 0, Folders = 1, FilesAndFolders = 2 };

    // Start is called before the first frame update
    void Start()
    {
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

        // Set file browser default path
        _defaultPath = Application.streamingAssetsPath + "/Maps";

    }

    // Generic button function
    public void ButtonFunction()
    {
        string mapName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TMP_Text>().text;
        string buttonName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
        Debug.Log("Button " + buttonName + " was clicked, map name: " + mapName);
    }

    // upload new map function
    public void OnClickUpload()
    {
        _openFileExplorer();
        //OnFileSelected(returnedPaths);
    }

    // Helper to open file explorer
    private void _openFileExplorer()
    {
        //open in maps directory, if it exists, else create it
        string path = Application.streamingAssetsPath + "/Maps";
        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }        
        _setFileBrowserFilterFolderOnly();

        // Show a save file dialog
        StartCoroutine(ShowLoadDialogCoroutineFoldersOnly());

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




    // Coroutine to show the load file dialog
    IEnumerator ShowLoadDialogCoroutineFoldersOnly()
    {
        // Directories only
        yield return FileBrowser.WaitForLoadDialog(SimpleFileBrowser.FileBrowser.PickMode.Folders, true, _defaultPath, null, "Select Map Folder", "Load");
        Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);
        if (FileBrowser.Success)
        {
            OnFileSelected(FileBrowser.Result);
        }
    }

    // Logs the paths of the selected files
    void OnFileSelected(string[] paths)
    {
        if (paths.Length == 0)
        {
            Debug.Log("No files were selected");
            return;
        }
        foreach (string path in paths)
        {
            Debug.Log("Selected file: " + path);
        }
    }
}