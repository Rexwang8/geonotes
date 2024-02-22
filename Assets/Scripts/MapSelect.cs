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
    public GameObject buttonPrefab;

    // Default path to open file explorer
    private string _defaultPath;
    private string[] _foundPaths;
    private bool _explorerActive = false;

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
    public void OnClickUpload()
    {
        
        //OnFileSelected(returnedPaths);
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
        _explorerActive = true;
        // Directories only
        yield return FileBrowser.WaitForLoadDialog(SimpleFileBrowser.FileBrowser.PickMode.Folders, true, _defaultPath, null, "Select Map Folder", "Load");
        Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);
        if (FileBrowser.Success)
        {
            OnFolderSelected(FileBrowser.Result);
        }
        _explorerActive = false;
    }

    // Logs the paths of the selected files
    void OnFolderSelected(string[] paths)
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

        // select only one 
        string selectedPath = paths[0];
        Debug.Log("Selected path: " + selectedPath);

        //Check if already exists in streaming assets
        string mapName = Path.GetFileName(selectedPath);
        string mapPath = Application.streamingAssetsPath + "/Maps/" + mapName;
        if (System.IO.Directory.Exists(mapPath))
        {
            Debug.Log("Map already exists in streaming assets");
            return;
        }

        // Copy the folder to the streaming assets
        Debug.Log("Copying folder to streaming assets");
        string destinationPath = Application.streamingAssetsPath + "/Maps/" + mapName;
        FileUtil.CopyFileOrDirectory(selectedPath, destinationPath);

        // Refresh the map select panel
        RefreshMapSelectPanel();

    }
}