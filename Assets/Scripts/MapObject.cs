using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class represents a Map, which contains both a 2D image and notes (for now)
public class MapObject
{
    public string name; // Name of the map
    public Texture2D image; // Image of the map
    public List<PostCard> notes; // List of notes on the map
    public Texture2D thumbnail; // Thumbnail of the map

    public Vector2Int imgSize; // Size of the image
    public Vector2Int tbnSize; // Size of the thumbnail

    // Print name of Image
    public void DisplayImage()
    {
        Debug.Log("Displaying image: " + name);
    }
    
    // Print name of Map and number of notes
    public void DisplayMapInfo()
    {
        Debug.Log("Map Name: " + name);
        Debug.Log("Number of Notes: " + notes.Count);
    }

    // Update the image of the map from a Texture2D
    public void UpdateImage(Texture2D _image)
    {
        image = _image;
        imgSize = new Vector2Int(image.width, image.height);
    }

    // Update the image of the map from a file
    public void UpdateImage(string path)
    {
        // Check if file exists
        if (!System.IO.File.Exists(path))
        {
            Debug.LogError("File does not exist: " + path);
            return;
        }
        
        byte[] imgBytes = System.IO.File.ReadAllBytes(path);
        image = new Texture2D(imgSize.x, imgSize.y);
        image.LoadImage(imgBytes);
        imgSize = new Vector2Int(image.width, image.height);

    }

    // Update the thumbnail of the map from a Texture2D
    public void UpdateThumbnail(Texture2D _thumbnail)
    {
        thumbnail = _thumbnail;
        // Downsize to 256x256 if necessary
        if (thumbnail.width > 256 || thumbnail.height > 256)
        {
            thumbnail = new Texture2D(256, 256);
            thumbnail.SetPixels(_thumbnail.GetPixels(0, 0, _thumbnail.width, _thumbnail.height));
            thumbnail.Apply();
        }
        tbnSize = new Vector2Int(thumbnail.width, thumbnail.height);
    }

    // Update the thumbnail of the map from a file
    public void UpdateThumbnail(string path)
    {
        // Check if file exists
        if (!System.IO.File.Exists(path))
        {
            Debug.LogError("File does not exist: " + path);
            return;
        }


        byte[] tbnBytes = System.IO.File.ReadAllBytes(path);
        thumbnail = new Texture2D(tbnSize.x, tbnSize.y);

        // Downsize to 256x256 if necessary
        if (thumbnail.width > 256 || thumbnail.height > 256)
        {
            thumbnail = new Texture2D(256, 256);
            thumbnail.LoadImage(tbnBytes);
            thumbnail.Apply();
        } 
        else
        {
            thumbnail.LoadImage(tbnBytes);
        }
        tbnSize = new Vector2Int(thumbnail.width, thumbnail.height);
    }

    // Save the map to a file (JSON)
    public void SaveMap()
    {
        // Create folder if it doesn't exist
        string folderPath = Application.streamingAssetsPath + "/Maps";
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }
        // Create Map Subfolder if it doesn't exist
        string nameOfMapLowerCaseStripped = name.ToLower().Replace(" ", "");
        string mapFolderPath = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped;
        if (!System.IO.Directory.Exists(mapFolderPath))
        {
            System.IO.Directory.CreateDirectory(mapFolderPath);
        }

        // Save image and thumbnail as img and tbn
        string imgPath = mapFolderPath + "/img.png";
        byte[] imgBytes = image.EncodeToPNG();
        System.IO.File.WriteAllBytes(imgPath, imgBytes);
        string tbnPath = mapFolderPath + "/tbn.png";
        byte[] tbnBytes = thumbnail.EncodeToPNG();
        System.IO.File.WriteAllBytes(tbnPath, tbnBytes);

        //Create notes folder
        string notesFolderPath = mapFolderPath + "/notes";
        if (!System.IO.Directory.Exists(notesFolderPath))
        {
            System.IO.Directory.CreateDirectory(notesFolderPath);
        }
        // For note in notes, save note as json
        for (int i = 0; i < notes.Count; i++)
        {
            string notePath = notesFolderPath + "/note" + i + ".json";
            string noteJson = JsonUtility.ToJson(notes[i]);
            System.IO.File.WriteAllText(notePath, noteJson);
            Debug.Log("Note Titled " + notes[i].title + "with date " + notes[i].date + " saved");
        }



        // Save the map to streamable assets/maps
        string path = mapFolderPath + "/" + nameOfMapLowerCaseStripped + ".json";
        string json = JsonUtility.ToJson(this);
        System.IO.File.WriteAllText(path, json);
    }

    // Load the map from a file (JSON)
    public void LoadMap(string folderName)
    {
        // Load the map from streamable assets/maps
        string nameOfMapLowerCaseStripped = folderName.ToLower().Replace(" ", "");
        string path = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped + "/" + nameOfMapLowerCaseStripped + ".json";
        string json = System.IO.File.ReadAllText(path);
        JsonUtility.FromJsonOverwrite(json, this);

        // Load image and thumbnail
        string imgPath = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped + "/img.png";
        byte[] imgBytes = System.IO.File.ReadAllBytes(imgPath);
        image = new Texture2D(imgSize.x, imgSize.y);
        image.LoadImage(imgBytes);

        string tbnPath = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped + "/tbn.png";
        byte[] tbnBytes = System.IO.File.ReadAllBytes(tbnPath);
        thumbnail = new Texture2D(tbnSize.x, tbnSize.y);
        thumbnail.LoadImage(tbnBytes);

        // Load notes
        string notesFolderPath = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped + "/notes";
        string[] notePaths = System.IO.Directory.GetFiles(notesFolderPath);
        notes = new List<PostCard>();
        for (int i = 0; i < notePaths.Length; i++)
        {
            string noteJson = System.IO.File.ReadAllText(notePaths[i]);
            //System.DateTime date = System.IO.File.GetCreationTime(notePaths[i]);
            PostCard noteJSON = JsonUtility.FromJson<PostCard>(noteJson);
            noteJSON.LoadDateTime();
            notes.Add(noteJSON);
        }



    }

}
