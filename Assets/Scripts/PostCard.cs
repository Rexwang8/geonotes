using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialNotes
{
    [System.Serializable]
    public class PostCard
    {
        public string title;
        public System.DateTime date;
        public string dateString = "";
        public string timeString = "";

        public System.DateTime dateCreated;
        public string dateCreatedString = "";
        public string timeCreatedString = "";
        //store location
        public LocationInfo location;

        //store media
        public List<string> mediaPaths = new List<string>();

        // Constructor
        public PostCard(string _title, System.DateTime _date)
        {
            title = _title;
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

        // Edit Variables
        public void UpdateTitle(string _title)
        {
            title = _title;
        }

        // Print title and date
        public void DisplayContent()
        {
            Debug.Log("Title: " + title + "Date: " + date);
        }
    }

    [System.Serializable]
    public class LocationInfo
    {
        public string locationName; // Name of the location
        public string description; // Path of location Image

        public Vector3 coordinate; // Coordinate of the location

        public LocationInfo(string _locationName, string _description, Vector3 _coordinate)
        {
            locationName = _locationName;
            description = _description;
            coordinate = _coordinate;
        }

        public Vector3 GetLocationInUnityCoords()
        {
            // Convert coordinate to Unity coordinates
            return new Vector3(coordinate.x, coordinate.z, coordinate.y);
        }
    }
}
