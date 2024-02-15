using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SpatialNotes;

namespace SpatialNotes {
// This class represents a Map, which contains both a 2D image and notes (for now)
public class MapObject
{
    public string name; // Name of the map
    public Texture2D image; // Image of the map
    public List<PostCard> notes; // List of notes on the map
    public Texture2D thumbnail; // Thumbnail of the map

    public Vector2Int imgSize; // Size of the image
    public Vector2Int tbnSize; // Size of the thumbnail

    //Paths
    public string notesJsonPath; // Path to the notes folder
    public string mapJsonPath; // Path to the map folder
    public string mapDirPath; // Path to the map folder

    [System.Serializable]
    private class MapObjectData
    {
        public string name; // Name of the map
        public Texture2D image; // Image of the map
        public Texture2D thumbnail; // Thumbnail of the map
        public Vector2Int imgSize; // Size of the image
        public Vector2Int tbnSize; // Size of the thumbnail
        public string notesJsonPath; // Path to the notes folder
        public string mapDirPath; // Path to the map folder
        public string mapJsonPath; // Path to the map folder
    }

    public string SerializeToJson()
    {
        MapObjectData data = new MapObjectData();
        data.name = name;
        data.image = image;
        data.thumbnail = thumbnail;
        data.imgSize = imgSize;
        data.tbnSize = tbnSize;
        data.notesJsonPath = notesJsonPath;
        data.mapDirPath = mapDirPath;
        data.mapJsonPath = mapJsonPath;

        return JsonUtility.ToJson(data);
    }


    // Print name of Image
    public void DisplayImage()
    {
        Debug.Log("Displaying image: " + name);
    }

    // Print name of thumbnail
    public void DisplayThumbnail()
    {
        Debug.Log("Displaying thumbnail: " + name);
    }
    
    // Print name of Map and number of notes
    public void DisplayMapInfo()
    {
        Debug.Log("Map Name: " + name);
        Debug.Log("Number of Notes: " + notes.Count);
    }

    // Print Postcard info from notes
    public void DisplayPostCardInfo()
    {
        foreach (PostCard note in notes)
        {
            note.DisplayContent();
        }
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

    public void CreateMap(string _TEMP_IMAGE_PATH, string _name="defaultName")
    {   
        if (_TEMP_IMAGE_PATH == null)
        {
            Debug.LogWarning("Please enter an image for the map");
            return;
        }

        if (_name == "defaultName")
        {
            if (name != null)
            {
                _name = name;
            }
            else
            {
                Debug.LogWarning("Please enter a name for the map");
                return;
            }
        }

        // Create maps folder if it doesn't exist
        string folderPath = Application.streamingAssetsPath + "/Maps";
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }

        // Create Map Subfolder; if exist, return error
        string nameOfMapLowerCaseStripped = _name.ToLower().Replace(" ", "");
        string mapFolderPath = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped;
        if (System.IO.Directory.Exists(mapFolderPath))
        {
            Debug.LogWarning("map already exists, please choose a different name for the map.");
            return;
        }
        System.IO.Directory.CreateDirectory(mapFolderPath);

        // Load Data 
        name = _name;
        UpdateImage(_TEMP_IMAGE_PATH);
        UpdateThumbnail(_TEMP_IMAGE_PATH);
        notesJsonPath = mapFolderPath + "/notes/postcard.json";
        mapJsonPath = mapFolderPath + "/" + nameOfMapLowerCaseStripped + ".json";
        mapDirPath = mapFolderPath;

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

        //Creates note json
        string noteJson = JsonUtility.ToJson("");
        System.IO.File.WriteAllText(notesJsonPath, noteJson);

        // Save the map data to streamable assets/maps
        string json = SerializeToJson();
        System.IO.File.WriteAllText(mapJsonPath, json);

        //Instantiate notes list
        notes = new List<PostCard>();
    }

    //Add postcard to the List
    public void AddPostcard(PostCard postcard)
    {
        notes.Add(postcard);
    }


    // Save the postcard list to a file (JSON); 
    public void SaveNoteJson()
    {
        //TODO: It rewrites the json file every time a new note is added. Need to implement a way to append the json file instead of rewriting it.
        //TODO: currently this json file saves duplicate notes, need to fix. Need to implement condition to check if note already exists in the json file and updates accordingly.
        List<string> jsonLines = new List<string>();

        foreach (PostCard postcard in notes)
        {
            string json = JsonUtility.ToJson(postcard);
            jsonLines.Add(json);
            Debug.Log("Iteration");
            foreach (string line in jsonLines)
            {
                Debug.Log(line);
            }
        }

        string jsonArray = "{" + string.Join(",", jsonLines.ToArray()) + "}";

        // Write the JSON array to the file
        System.IO.File.WriteAllText(notesJsonPath, jsonArray);
    }
    

    // // Save the map to a file (JSON)
    // public void SaveMap()
    // {
    //     // Create folder if it doesn't exist
    //     string folderPath = Application.streamingAssetsPath + "/Maps";
    //     if (!System.IO.Directory.Exists(folderPath))
    //     {
    //         System.IO.Directory.CreateDirectory(folderPath);
    //     }
    //     // Create Map Subfolder if it doesn't exist
    //     string nameOfMapLowerCaseStripped = name.ToLower().Replace(" ", "");
    //     string mapFolderPath = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped;
    //     if (!System.IO.Directory.Exists(mapFolderPath))
    //     {
    //         System.IO.Directory.CreateDirectory(mapFolderPath);
    //     }

    //     // Save image and thumbnail as img and tbn
    //     string imgPath = mapFolderPath + "/img.png";
    //     byte[] imgBytes = image.EncodeToPNG();
    //     System.IO.File.WriteAllBytes(imgPath, imgBytes);
    //     string tbnPath = mapFolderPath + "/tbn.png";
    //     byte[] tbnBytes = thumbnail.EncodeToPNG();
    //     System.IO.File.WriteAllBytes(tbnPath, tbnBytes);

    //     //Create notes folder
    //     string notesFolderPath = mapFolderPath + "/notes";
    //     if (!System.IO.Directory.Exists(notesFolderPath))
    //     {
    //         System.IO.Directory.CreateDirectory(notesFolderPath);
    //     }
    //     // For note in notes, save note as json
    //     for (int i = 0; i < notes.Count; i++)
    //     {
    //         string notePath = notesFolderPath + "/note" + i + ".json";
    //         string noteJson = JsonUtility.ToJson(notes[i]);
    //         System.IO.File.WriteAllText(notePath, noteJson);
    //         Debug.Log("Note Titled " + notes[i].title + "with date " + notes[i].date + " saved");
    //     }

    //     // Save the map to streamable assets/maps
    //     string path = mapFolderPath + "/" + nameOfMapLowerCaseStripped + ".json";
    //     string json = JsonUtility.ToJson(this);
    //     System.IO.File.WriteAllText(path, json);
    // }

    // Load the map from a file (JSON)
    public void LoadMap(string folderName)
    {
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

        json = System.IO.File.ReadAllText(notesJsonPath);
        json = json.TrimStart('{').TrimEnd('}');
        string[] jsonLines = json.Split("},{");
        foreach (string part in jsonLines)
        {
            PostCard note = new PostCard("temp", System.DateTime.Now);
            JsonUtility.FromJsonOverwrite("{" + part + "}", note);
            notes.Add(note);
        }

        // DisplayPostCardInfo();
    }   

    // Return count of notes
    public int GetNoteCount()
    {
        return notes.Count;
    }

}
}


