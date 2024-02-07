using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public class TestMapMaker : MonoBehaviour
{
    // Test Button
    [Button("MakeMap")]
    public void MakeMapTest()
    {
        // Create a new map object
        MapObject map = new MapObject();
        map.name = "Test Map";
        string mapImagePath = Application.streamingAssetsPath + "/demomap.jpg";
        map.UpdateImage(mapImagePath);
        map.UpdateThumbnail(mapImagePath);
        map.notes = new List<PostCard>();

        // Create a new note
        PostCard note = new PostCard("Test Note MapMaker", System.DateTime.Now);
        map.notes.Add(note);
        PostCard note2 = new PostCard("Test Note 2 MapMaker", System.DateTime.Now);
        map.notes.Add(note2);
        map.DisplayMapInfo();

        // Save the map
        map.SaveMap();

        Debug.Log("Map Created");

        MapObject loadedMap = new MapObject();
        loadedMap.LoadMap("testmap");
        loadedMap.DisplayMapInfo();
        for (int i = 0; i < loadedMap.notes.Count; i++)
        {
            loadedMap.notes[i].DisplayContent();
            Debug.Log("Datetime: " + loadedMap.notes[i].date + " " + loadedMap.notes[i].dateString + " " + loadedMap.notes[i].timeString);
        }
    }

    // Clear All Maps
    [Button("ClearMaps")]
    public void ClearMaps()
    {
        // Path
        string path = Application.streamingAssetsPath + "/Maps";
        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
        foreach (System.IO.FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
        foreach (System.IO.DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true);
        }

        Debug.Log("Maps Cleared from " + path);
    }
}
