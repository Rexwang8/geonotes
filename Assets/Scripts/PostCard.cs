using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PostCard
{
    public string title;
    public System.DateTime date;
    public string dateString = "";
    public string timeString = "";
    //store coordinate
    
    //store media

    // Constructor
    public PostCard(string _title, System.DateTime _date)
    {
        title = _title;
        date = _date;

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
    }
    public void SaveDateTime()
    {
        dateString = date.ToString("yyyy-MM-dd");
        timeString = date.ToString("HH:mm:ss");
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
