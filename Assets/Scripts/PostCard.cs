using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialNotes
{
    [System.Serializable]
    public class PostCard
    {
        public System.DateTime date;
        public string dateString = "";
        public string timeString = "";

        public System.DateTime dateCreated;
        public string dateCreatedString = "";
        public string timeCreatedString = "";

       [SerializeField]
        public List<TextMediaPost> posts = new List<TextMediaPost>();

        // Constructor
        public PostCard(System.DateTime _date)
        {
            date = _date;
            dateCreated = System.DateTime.Now;

            // Save date as string
            if (dateString == "" || timeString == "")
            {
                SaveDateTime();
            }
        }

        // Load Date from string
        public void LoadDateTime()
        {
            date = System.DateTime.Parse(dateString + " " + timeString);
            dateCreated = System.DateTime.Parse(dateCreatedString + " " + timeCreatedString);
        }

        // Save Date as string
        public void SaveDateTime()
        {
            dateString = date.ToString("yyyy-MM-dd");
            timeString = date.ToString("HH:mm:ss");
            dateCreatedString = dateCreated.ToString("yyyy-MM-dd");
            timeCreatedString = dateCreated.ToString("HH:mm:ss");
        }

    }

    [System.Serializable]
    public class LocationInfo
    {
        public string locationName; // Name of the location
        public string description; // Path of location Image

        [SerializeField]
        public Vector3 coordinate; // Coordinate of the location
        public string imagePath; // Path of location Image
        public PostCard postCard; // Postcard of the location

        

        public LocationInfo(string _locationName, string _description, Vector3 _coordinate, string _imagePath="") // Constructor for LocationInfo
        {
            locationName = _locationName;
            description = _description;
            coordinate = _coordinate;
            imagePath = _imagePath;
        }
        public Vector3 GetLocationInUnityCoords()
        {
            // Convert coordinate to Unity coordinates
            return new Vector3(coordinate.x, coordinate.z, coordinate.y);
        }
    }

    [System.Serializable]
    public class _postMediaComponent
    {
        public string mediaType;
        //uuid
        public string uuid;
        public string CreateUUID()
        {
            uuid = System.Guid.NewGuid().ToString();
            return uuid;
        }

    }

    [System.Serializable]
    public class TextComponent : _postMediaComponent
    {
        public string textContent;
    }

    [System.Serializable]
    public class ImageComponent : _postMediaComponent
    {
        public string mediaPath;
    }

    [System.Serializable]
    public class TextMediaPost
    {
        public string title;
        public string description;
        public string date;
        public string dateString = "";
        public string timeString = "";
        public List<_postMediaComponent> mediaComponents = new List<_postMediaComponent>();
        

        public TextMediaPost(string _title, string _description, string _date)
        {
            title = _title;
            description = _description;
            date = _date;
        }

        public TextMediaPost()
        {
            title = "";
            description = "";
            date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public void AddMediaComponent(string _mediaPath, string _mediaType, string _mediaDescription, string _mediaTitle, string _mediaDate)
        {
            _postMediaComponent newComponent = new _postMediaComponent();
            newComponent.mediaType = _mediaType;

            mediaComponents.Add(newComponent);
        }
        public void AddTextComponent(TextComponent _textComponent)
        {
            mediaComponents.Add(_textComponent);
        }
        

        public void updateDate(string _date)
        {
            date = _date;
        }

        public void updateDate(System.DateTime _date)
        {
            date = _date.ToString("yyyy-MM-dd HH:mm:ss");

            // Save date as string
            saveDate();
        }

        public void loadDate()
        {
            date = System.DateTime.Parse(dateString + " " + timeString).ToString("yyyy-MM-dd HH:mm:ss");
        }

        public void saveDate()
        {
            dateString = System.DateTime.Parse(date).ToString("yyyy-MM-dd");
            timeString = System.DateTime.Parse(date).ToString("HH:mm:ss");
        }

        


    }
}
