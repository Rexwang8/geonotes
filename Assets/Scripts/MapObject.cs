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
        
        //Images
        public Texture2D image; // Image of the map
        public Texture2D thumbnail; // Thumbnail of the map
        public string imagePath; // Path of the image
        public string thumbnailPath; // Path of the thumbnail
        
        
        public List<PostCard> notes; // List of notes on the map
        Dictionary<string, List<PostCard>> notesDict; // Dictionary of notes on the map
        Dictionary<string, LocationInfo> locationDict; // Dictionary of locations on the map
        

        public Vector2Int imgSize; // Size of the image
        public Vector2Int tbnSize; // Size of the thumbnail

        //Paths
        public string notesJsonPath; // Path to the notes folder
        public string locationJsonPath; // Path to the notes folder
        public string mapJsonPath; // Path to the map folder
        public string mapDirPath; // Path to the map folder



        [System.Serializable]
        private class MapObjectData
        {
            public string name; // Name of the map
            public string imagePath; // Path of the image
            public string thumbnailPath; // Path of the thumbnail
            public Vector2Int imgSize; // Size of the image
            public Vector2Int tbnSize; // Size of the thumbnail
            public string notesJsonPath; // Path to the notes folder
            public string locationJsonPath; // Path to the notes folder
            public string mapDirPath; // Path to the map folder
            public string mapJsonPath; // Path to the map folder
        }

        public string SerializeToJson()
        {
            MapObjectData data = new MapObjectData();
            data.name = name;
            data.imagePath = imagePath;
            data.thumbnailPath = thumbnailPath;
            data.imgSize = imgSize;
            data.tbnSize = tbnSize;
            data.notesJsonPath = notesJsonPath;
            data.locationJsonPath = locationJsonPath;
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
            Debug.Log("Number of Locations: " + locationDict.Count);
        }

        // Print Postcard info from notes
        // Need to rewrite this function to display the map name and number of notes 
        public void DisplayPostCardInfo()
        {
            Debug.Log("Need to rewrite this function for dictionary");
        }

        // Print Location info from locationDict
        public void DisplayLocationInfo()
        {
            Debug.Log("----Displaying All locations");
            foreach (KeyValuePair<string, LocationInfo> location in locationDict)
            {   
                
                Debug.Log("Location: " + location.Key + " " + location.Value.locationName + " " + location.Value.description);
            }
            Debug.Log("----Finished");
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

        public void CreateMap(string _TEMP_IMAGE_PATH, string _name)
        {   
            if (_TEMP_IMAGE_PATH == null)
            {
                Debug.LogWarning("Please enter an image for the map");
                return;
            }

            if (_name == null)
            {
                Debug.LogWarning("Please enter a name for the map");
                return;
            }
            name = _name;

            // Create maps folder if it doesn't exist
            string folderPath = Application.streamingAssetsPath + "/Maps";
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }

            // Create Map Subfolder; if exist, return error
            string nameOfMapLowerCaseStripped = name.ToLower().Replace(" ", "");
            string mapFolderPath = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped;
            if (System.IO.Directory.Exists(mapFolderPath))
            {
                Debug.LogWarning("map already exists, please choose a different name for the map.");
                return;
            }
            System.IO.Directory.CreateDirectory(mapFolderPath);

            // Load Data 
            UpdateImage(_TEMP_IMAGE_PATH);
            UpdateThumbnail(_TEMP_IMAGE_PATH);
            notesJsonPath = "/Maps/" + nameOfMapLowerCaseStripped + "/notes/postcard.json";
            locationJsonPath = "/Maps/" + nameOfMapLowerCaseStripped + "/notes/location.json";
            mapJsonPath = "/Maps/" + nameOfMapLowerCaseStripped + "/" + nameOfMapLowerCaseStripped + ".json";
            mapDirPath = "/Maps/" + nameOfMapLowerCaseStripped;

            // Save image and thumbnail as img and tbn
            string imgPath = mapFolderPath + "/img.png";
            byte[] imgBytes = image.EncodeToPNG();
            System.IO.File.WriteAllBytes(imgPath, imgBytes);
            string tbnPath = mapFolderPath + "/tbn.png";
            byte[] tbnBytes = thumbnail.EncodeToPNG();
            System.IO.File.WriteAllBytes(tbnPath, tbnBytes);
            imagePath = "/Maps/" + nameOfMapLowerCaseStripped + "/img.png";
            thumbnailPath = "/Maps/" + nameOfMapLowerCaseStripped + "/tbn.png";

            //Create notes folder
            string notesFolderPath = mapFolderPath + "/notes";
            if (!System.IO.Directory.Exists(notesFolderPath))
            {
                System.IO.Directory.CreateDirectory(notesFolderPath);
            }
            //Create postcard.json
            string noteJson = JsonUtility.ToJson("");
            System.IO.File.WriteAllText(Application.streamingAssetsPath + notesJsonPath, noteJson);
            //Create locations.json
            string locationJson = JsonUtility.ToJson("");
            System.IO.File.WriteAllText(Application.streamingAssetsPath + locationJsonPath, locationJson);

            //Create Assets folder
            string assetsFolderPath = mapFolderPath + "/assets";
            if (!System.IO.Directory.Exists(assetsFolderPath))
            {
                System.IO.Directory.CreateDirectory(assetsFolderPath);
            }

            // Save the map data to streamable assets/maps
            string json = SerializeToJson();
            System.IO.File.WriteAllText(Application.streamingAssetsPath + mapJsonPath, json);

            //Instantiate notes dics
            notesDict = new Dictionary<string, List<PostCard>>();
            locationDict = new Dictionary<string, LocationInfo>();
        }

        //Add postcard to the List
        public void AddLocation(string locCoord, string locName, string locDescription)
        {
            if (locationDict.ContainsKey(locCoord))
            {
                Debug.LogWarning("Location already exists. Please choose a different location.");
                return;
            }
            else
            {
                LocationInfo newLocation = new LocationInfo(locName, locDescription);
                locationDict.Add(locCoord, newLocation);   
            }
        }
        
        public void AddPostcard(string locCoord, PostCard postcard)
        {
            if (locationDict.ContainsKey(locCoord))
            {
                if (notesDict.ContainsKey(locCoord))
                {
                    notesDict[locCoord].Add(postcard);
                }
                else
                {
                    notesDict.Add(locCoord, new List<PostCard> {postcard});
                }
            }
            else
            {
                Debug.LogWarning("Location where you want to add the postcard does not exist. Please add the location first.");
            }
        }

        // Save the postcard list to a file (JSON); 
        public void SaveNoteJson()
        {
            List<string> jsonLines = new List<string>();

            foreach (KeyValuePair<string, List<PostCard>> note in notesDict)
            {
                string json = "\"" + note.Key + "\":" + ConvertNotesList2Json(note.Value);
                jsonLines.Add(json);
            }

            string jsonArray = "{" + string.Join(",", jsonLines.ToArray()) + "}";

            // Write the JSON array to the file
            System.IO.File.WriteAllText(Application.streamingAssetsPath + notesJsonPath, jsonArray);
        }

        //helper function for savenotesjson()
        public string ConvertNotesList2Json(List<PostCard> notes)
        {
            List<string> jsonLines = new List<string>();
            foreach (PostCard postcard in notes)
            {
                string json = JsonUtility.ToJson(postcard);
                jsonLines.Add(json);
            }
            string jsonArray = "[" + string.Join(",", jsonLines.ToArray()) + "]";
            return jsonArray;
        }

        // Save location list to a file (JSON)
        public void SaveLocationJson()
        {        
            List<string> jsonLines = new List<string>();

            foreach (KeyValuePair<string, LocationInfo> location in locationDict)
            {
                string json = JsonUtility.ToJson(location.Value);
                json = "\"" + location.Key + "\":" + json;
                jsonLines.Add(json);
            }

            string jsonArray = "{" + string.Join(",", jsonLines.ToArray()) + "}";

            // Write the JSON array to the file
            System.IO.File.WriteAllText(Application.streamingAssetsPath + locationJsonPath, jsonArray);
        }
        
        // Load the map from a file (JSON)
        public void LoadMap(string folderName)
        {
            string nameOfMapLowerCaseStripped = folderName.ToLower().Replace(" ", "");
            string path = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped + "/" + nameOfMapLowerCaseStripped + ".json";
            Debug.Log("1" + path);
            string json = System.IO.File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, this); 
            
            //THIS IS FOR TESTING ONLY
            //changes the absolute path in the json file to relative path to absolute path to your local machine
            notesJsonPath = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped + "/notes/postcard.json";
            mapJsonPath = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped + "/" + nameOfMapLowerCaseStripped + ".json";
            mapDirPath = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped;
            
            Debug.Log("2");
            // Load image and thumbnail
            string imgPath = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped + "/img.png";
            byte[] imgBytes = System.IO.File.ReadAllBytes(imgPath);
            image = new Texture2D(imgSize.x, imgSize.y);
            image.LoadImage(imgBytes);
            Debug.Log("3");
            
            string tbnPath = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped + "/tbn.png";
            byte[] tbnBytes = System.IO.File.ReadAllBytes(tbnPath);
            thumbnail = new Texture2D(tbnSize.x, tbnSize.y);
            thumbnail.LoadImage(tbnBytes);
            Debug.Log("4");
            
            //Read notes dictionary from json
            json = System.IO.File.ReadAllText(notesJsonPath);
            json = json.TrimStart('{').TrimEnd('}');
            string[] jsonLines = json.Split("},{");
            foreach (string part in jsonLines)
            {
                Debug.Log("5" + part);
                PostCard note = new PostCard("temp", System.DateTime.Now);
                JsonUtility.FromJsonOverwrite("{" + part + "}", note);
                notes.Add(note);
            }
        }   

        //CONTINUE THIS
        private void LoadLocationJson()
        {   
            //Initialize locationDict
            locationDict = new Dictionary<string, LocationInfo>();

            string json = System.IO.File.ReadAllText(Application.streamingAssetsPath + locationJsonPath);
            json = json.TrimStart('{').TrimEnd('}');
            string[] jsonLines = json.Split("},");
            foreach (string part in jsonLines)
            {
                string[] twoparts = part.Split(":{");

                string key = twoparts[0].TrimStart('"').TrimEnd('"');

            }
        }        

    }
}


