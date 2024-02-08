using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
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
        map.notes = new List<PostCard>();

        // Create a new note
        PostCard note = new PostCard("Test Note", System.DateTime.Now);
        map.notes.Add(note);

        // Display the note
        note.DisplayContent();

        //show image
        map.DisplayImage();

        //show map info
        map.DisplayMapInfo();

        Assert.AreEqual(map.name, "Test Map");
        Assert.AreEqual(map.notes.Count, 1);
        Debug.Log("Test Passed");
    }



}
