using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostCard : ScriptableObject
{
    public string title;
    public System.DateTime date;
    //store coordinate
    
    //store media

    // Constructor
    public PostCard(string _title, System.DateTime _date) 
    {
        title = _title;
        date = _date;
    }
    
    // Edit Variables
    public void UpdateTitle(string _title)
    {
        title = _title;
    }

    // Print title and date
    public void DisplayContent()
    {
        Debug.Log("Title: " + title);
        Debug.Log("Date: " + date);
    }
}
