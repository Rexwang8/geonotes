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

}
