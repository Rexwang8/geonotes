using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

// Class for testing
public class Testing : MonoBehaviour
{
    // Attach to button
    [Button("Test")]
    public void Test()
    {
        // Create a new map
        MapObject map = new MapObject();
        map.name = "Test Map";
        //pull demomap from streamable assets
        string mapImagePath = Application.streamingAssetsPath + "/demomap.png";
        map.image = new Texture2D(50, 50);
        map.notes = new List<PostCard>();
        map.thumbnail = new Texture2D(50, 50);

        // Create a new note
        PostCard note = new PostCard("Test Note", System.DateTime.Now);
        map.notes.Add(note);

        // Display the note
        note.DisplayContent();

        //show image
        map.DisplayImage();

        //show map info
        map.DisplayMapInfo();
    }



}
