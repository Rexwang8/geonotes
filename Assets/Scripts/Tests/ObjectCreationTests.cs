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
    // Test the creation of a MapObject and PostCard
   [Test]
   public void test_print() {
         Debug.Log("Hello World");
   }

    // Test the creation of a MapObject and PostCard
    [Test]
    public void InstantiateAndCountNotes()
    {
        // Clear used asset folder
        string path = Application.streamingAssetsPath + "/Maps/testmap";
        if (System.IO.Directory.Exists(path))
        {
            System.IO.Directory.Delete(path, true);
        }

        // Create a new map
        MapObject map = new MapObject();
        map.name = "Test Map";
        //pull demomap from streamable assets
        string mapImagePath = Application.streamingAssetsPath + "/demomap.jpg";
        map.CreateMap(_name: "Test Map", _TEMP_IMAGE_PATH: mapImagePath);

        // Create a new note
        // PostCard note = new PostCard(_title: "Test Note", _date: System.DateTime.Now);
        // map.AddPostcard(note);
        // note = new PostCard(_title: "Test Note 2", _date: System.DateTime.Now);
        // map.AddPostcard(note);


        Assert.AreEqual(map.name, "Test Map");
        Assert.AreEqual(map.notes.Count, 2);
        Debug.Log("Test Passed");
    }

    // Test the creation of a MapObject and PostCard
    [Test]
    public void InstantiateSaveLoadCountNotes()
    {
        // Clear used asset folder
        string path = Application.streamingAssetsPath + "/Maps/testmaptest2";
        if (System.IO.Directory.Exists(path))
        {
            System.IO.Directory.Delete(path, true);
        }

        // Create a new map object
        MapObject map = new MapObject();
        string mapImagePath = Application.streamingAssetsPath + "/demomap.jpg";
        map.CreateMap(_name: "Test Map Test 2", _TEMP_IMAGE_PATH: mapImagePath);

        // Create a new note
        // PostCard note = new PostCard(_title: "Test Note", _date: System.DateTime.Now);
        // map.AddPostcard(note);
        // note = new PostCard(_title: "Test Note 2", _date: System.DateTime.Now);
        // map.AddPostcard(note);
        // note = new PostCard(_title: "Test Note 3", _date: System.DateTime.Now);
        // map.AddPostcard(note);

        // Save the map
        map.SaveNoteJson();

        Debug.Log("Map Created");

        MapObject loadedMap = new MapObject();
        loadedMap.LoadMap("testmaptest2");
        loadedMap.DisplayMapInfo();
        loadedMap.DisplayPostCardInfo();
        loadedMap.DisplayImage();
        loadedMap.DisplayThumbnail();
        loadedMap.DisplayMapInfo();
        loadedMap.UpdateImage(mapImagePath);
        loadedMap.UpdateThumbnail(mapImagePath);

        Assert.AreEqual(loadedMap.name, "Test Map Test 2");
        Assert.AreEqual(loadedMap.notes.Count, 3);
        Assert.AreEqual(loadedMap.notes[0].title, "Test Note");
        Assert.AreEqual(loadedMap.notes[1].title, "Test Note 2");
        Assert.AreEqual(loadedMap.notes[2].title, "Test Note 3");
        Debug.Log("Test Passed");

    }

    // Test the creation of a MapObject and PostCard
    [Test]
    public void InstantiatePostCard()
    {
        // Create a new note
        PostCard note = new PostCard(_title: "Test Note", _date: System.DateTime.Now);
        note.UpdateTitle("Test Note 2");
        note.DisplayContent();
        note.SaveDateTime();
        note.LoadDateTime();
        Assert.AreEqual(note.title, "Test Note 2");
        Debug.Log("Test Passed");
    }

}


}