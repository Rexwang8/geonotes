using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SpatialNotes;

namespace SpatialNotes
{
    // This class represents a Map, which contains both a 2D image and notes (for now)
    public class MapObject
    {
        public string name; // Name of the map

        //Images
        public Texture2D image; // Image of the map
        public Texture2D thumbnail; // Thumbnail of the map
        public string imagePath; // Path of the image
        public string thumbnailPath; // Path of the thumbnail


        public SerializableDictionary<string, JsonableListWrapper<PostCard>> notesDict; // Dictionary of notes on the map
        public SerializableDictionary<string, LocationInfo> locationDict; // Dictionary of locations on the map


        public Vector2Int imgSize; // Size of the image
        public Vector2Int tbnSize; // Size of the thumbnail

        //Paths
        public string notesJsonPath; // Path to the notes folder
        public string locationJsonPath; // Path to the notes folder
        public string mapJsonPath; // Path to the map folder
        public string mapDirPath; // Path to the map folder
        public string mapAssetsPath; // Path to the map assets folder



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
            public string mapAssetsPath; // Path to the map assets folder
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
            data.mapAssetsPath = mapDirPath + "/assets";

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
            notesDict = new SerializableDictionary<string, JsonableListWrapper<PostCard>>();
            locationDict = new SerializableDictionary<string, LocationInfo>();

        }

        //Get number of notes
        public int GetNumberOfNotes()
        {
            int count = 0;
            foreach (KeyValuePair<string, JsonableListWrapper<PostCard>> note in notesDict)
            {
                count += note.Value.Count;
            }
            return count;
        }

        //Get number of locations
        public int GetNumberOfLocations()
        {
            int count = locationDict.Count;
            return count;
        }

        // returns locations in dict format
        public Dictionary<string, LocationInfo> GetLocations()
        {
            Dictionary<string, LocationInfo> locations = new Dictionary<string, LocationInfo>();
            foreach (KeyValuePair<string, LocationInfo> location in locationDict)
            {
                locations.Add(location.Key, location.Value);
            }
            return locations;
        }

        // Delete self
        public void DeleteMap()
        {
            // League moment
            string path = Application.streamingAssetsPath + mapDirPath;
            // error if mapDirPath is null or empty
            if (string.IsNullOrEmpty(mapDirPath))
            {
                Debug.LogError("Map directory path is null or empty");
                return;
            }
            if (System.IO.Directory.Exists(path))
            {
                System.IO.Directory.Delete(path, true);
                Debug.Log("Map deleted: " + path);
            }
            else
            {
                Debug.LogError("Map directory does not exist");
            }
        }

        //Add postcard to the List
        public void AddLocation(string locCoord, string locName, string locDescription, string locImagePath="")
        {
            if (locationDict.ContainsKey(locCoord))
            {
                Debug.LogWarning("Location already exists. Please choose a different location.");
                return;
            }
            else
            {
                LocationInfo newLocation = new LocationInfo(locName, locDescription, new Vector3(0, 0, 0), locImagePath);
                locationDict.Add(locCoord, newLocation);
            }
        }
        // Add location to the map (vec3 overload)
        public void AddLocation(Vector3 locCoord, string locName, string locDescription, string locImagePath="")
        {
            string locCoordStr = locCoord.x + "," + locCoord.y + "," + locCoord.z;
            AddLocation(locCoordStr, locName, locDescription, locImagePath);
        }

        // Add location to map (locationinfo overload)
        public void AddLocation(LocationInfo location)
        {
            string locCoordStr = location.coordinate.x + "," + location.coordinate.y + "," + location.coordinate.z;
            AddLocation(locCoordStr, location.locationName, location.description);
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
                    List<PostCard> postcardList = new List<PostCard> { postcard };
                    notesDict.Add(locCoord, new JsonableListWrapper<PostCard>(postcardList));
                }
            }
            else
            {
                Debug.LogWarning("Location where you want to add the postcard does not exist. Please add the location first.");
            }
        }

        // Add postcard to the map (locationinfo overload)
        public void AddPostcard(LocationInfo location, PostCard postcard)
        {
            string locCoordStr = location.coordinate.x + "," + location.coordinate.y + "," + location.coordinate.z;
            AddPostcard(locCoordStr, postcard);
        }

        // helper to convert location coord string to vector3
        public Vector3 _convertCoordStr2Vec3(string coordStr)
        {
            string[] coordStrArr = coordStr.Split(',');
            float x = float.Parse(coordStrArr[0]);
            float y = float.Parse(coordStrArr[1]);
            float z = float.Parse(coordStrArr[2]);
            return new Vector3(x, y, z);
        }

        // Save the postcard list to a file (JSON); 
        private void _saveNoteJson()
        {
            string json = JsonUtility.ToJson(notesDict);
            System.IO.File.WriteAllText(Application.streamingAssetsPath + notesJsonPath, json);
        }

        // Save location list to a file (JSON)
        private void _saveLocationJson()
        {
            string json = JsonUtility.ToJson(locationDict);
            System.IO.File.WriteAllText(Application.streamingAssetsPath + locationJsonPath, json);
        }

        // Save All
        public void SaveAll()
        {
            _saveNoteJson();
            _saveLocationJson();
            string json = SerializeToJson();
            System.IO.File.WriteAllText(Application.streamingAssetsPath + mapJsonPath, json);
        }

        public void LoadAll(string nameOfMapLowerCaseStripped)
        {
            _loadMap(nameOfMapLowerCaseStripped);
        }


        // Load the map from a file (JSON)
        private void _loadMap(string folderName)
        {
            string nameOfMapLowerCaseStripped = folderName.ToLower().Replace(" ", "");
            string path = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped + "/" + nameOfMapLowerCaseStripped + ".json";
            string json = System.IO.File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, this);

            //THIS IS FOR TESTING ONLY
            //changes the absolute path in the json file to relative path to absolute path to your local machine
            notesJsonPath = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped + "/notes/postcard.json";
            mapJsonPath = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped + "/" + nameOfMapLowerCaseStripped + ".json";
            mapDirPath = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped;

            // Load image and thumbnail
            string imgPath = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped + "/img.png";
            byte[] imgBytes = System.IO.File.ReadAllBytes(imgPath);
            image = new Texture2D(imgSize.x, imgSize.y);
            image.LoadImage(imgBytes);

            string tbnPath = Application.streamingAssetsPath + "/Maps/" + nameOfMapLowerCaseStripped + "/tbn.png";
            byte[] tbnBytes = System.IO.File.ReadAllBytes(tbnPath);
            thumbnail = new Texture2D(tbnSize.x, tbnSize.y);
            thumbnail.LoadImage(tbnBytes);

            //get json path to notes and location
            notesJsonPath = "/Maps/" + nameOfMapLowerCaseStripped + "/notes/postcard.json";
            locationJsonPath = "/Maps/" + nameOfMapLowerCaseStripped + "/notes/location.json";
            mapJsonPath = "/Maps/" + nameOfMapLowerCaseStripped + "/" + nameOfMapLowerCaseStripped + ".json";
            mapDirPath = "/Maps/" + nameOfMapLowerCaseStripped;
            mapAssetsPath = "/Maps/" + nameOfMapLowerCaseStripped + "/assets";

            // Load notes and locations from JSON
            json = System.IO.File.ReadAllText(Application.streamingAssetsPath + notesJsonPath);
            notesDict = JsonUtility.FromJson<SerializableDictionary<string, JsonableListWrapper<PostCard>>>(json);
            json = System.IO.File.ReadAllText(Application.streamingAssetsPath + locationJsonPath);
            locationDict = JsonUtility.FromJson<SerializableDictionary<string, LocationInfo>>(json);
            //Debug.Log(notesDict);
            //Debug.Log(locationDict);
            //Debug.Log("number of notes: " + GetNumberOfNotes());
            //Debug.Log("number of locations: " + GetNumberOfLocations());
            //Debug.Log("map name: " + name);
        }


    }

    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();

        [SerializeField]
        private List<TValue> values = new List<TValue>();

        // save the dictionary to lists
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            this.Clear();
            if (keys.Count != values.Count)
                throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

            for (int i = 0; i < keys.Count; i++)
                this.Add(keys[i], values[i]);
        }
    }

    [System.Serializable]
    public class JsonableListWrapper<T>
    {
        public List<T> list;
        public JsonableListWrapper(List<T> list) => this.list = list;
        public int Count => list.Count;
        public T this[int index] => list[index];
        public void Add(T item) => list.Add(item);
        public void Remove(T item) => list.Remove(item);

    }
}


