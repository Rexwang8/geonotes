using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using SpatialNotes;

namespace SpatialNotesTest {

// Test the creation of MapObject and PostCard objects

[TestFixture]
public class ObjectCreationTests
{
   [Test]
   public void test_print() {
         Debug.Log("Hello World");
   }

    [Test]
    public void InstantiateAndCountNotes()
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

    [Test]
    public void InstantiateSaveLoadCountNotes()
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
        Assert.AreEqual(loadedMap.name, "Test Map");
        Assert.AreEqual(loadedMap.notes.Count, 2);
        Debug.Log("Test Passed");
    }

}


}