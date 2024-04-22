using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using SpatialNotes;

namespace SpatialNotesTest
{

    // Test the creation of MapObject and PostCard objects

    [TestFixture]
    public class ObjectCreationTests
    {
        // Test the creation of a MapObject and PostCard
        [Test]
        public void test_print()
        {
            Debug.Log("Hello World");
        }

        // Test creation of location info
        [Test]
        public void InstantiateLocationInfo()
        {
            // Create a new location info
            SpatialNotes.LocationInfo location = new SpatialNotes.LocationInfo(_locationName: "Test Location", _description: "Test Description", _coordinate: new Vector3(0, 0, 0), "");
            Assert.AreEqual(location.locationName, "Test Location");
            Debug.Log("Test Passed");
        }

        // Test Creation of postcard with no media or location
        [Test]
        public void InstantiatePostCardNoMedia()
        {
            // Create a new note
            PostCard note = new PostCard(_date: System.DateTime.Now);
            note.SaveDateTime();
            note.LoadDateTime();
            //Assert.AreEqual(note.title, "Test Note 2");
            Debug.Log("Test Passed");
        }

        // Test Creation of postcard with no media but with location
        [Test]
        public void InstantiatePostCardNoMediaWithLocation()
        {
            // Create a new note
            PostCard note = new PostCard(_date: System.DateTime.Now);
            note.SaveDateTime();
            note.LoadDateTime();
            SpatialNotes.LocationInfo location = new SpatialNotes.LocationInfo(_locationName: "Test Location 2", _description: "Test Description 2", _coordinate: new Vector3(0, 0, 0), "");
            //note.location = location;
            //Assert.AreEqual(note.title, "Test Note 2");
            //Assert.AreEqual(note.location.locationName, "Test Location 2");
            Debug.Log("Test Passed");
        }

        // Test instantiation of empty map
        [Test]
        public void InstantiateEmptyMap()
        {
            // Create a new map object
            MapObject map1 = new MapObject();
            Assert.AreEqual(map1.GetNumberOfNotes(), 0);
            Debug.Log("Test Passed");

            // Clear used asset folder
            map1.DeleteMap();
            map1 = null;
        }

        // Test instantiation of map with name, tbn and img
        [Test]
        public void InstantiateMapWithNameTbnImg()
        {
            // Create a new map object
            MapObject map2 = new MapObject();
            string mapImagePath = Application.streamingAssetsPath + "/demomap.jpg";
            map2.CreateMap(_name: "Test Map 6", _TEMP_IMAGE_PATH: mapImagePath);
            Assert.AreEqual(map2.name, "Test Map 6");
            Assert.AreEqual(map2.GetNumberOfNotes(), 0);
            Debug.Log("Test Passed");

            // Clear used asset folder
            map2.DeleteMap();
            map2 = null;
        }

        // Test instantiation of map with name, tbn and img, 20 notes
        [Test]
        public void InstantiateMapWithNameTbnImg20Notes()
        {
            // Create a new map object
            MapObject map3 = new MapObject();
            string mapImagePath = Application.streamingAssetsPath + "/demomap.jpg";
            map3.CreateMap(_name: "ObjectCreationTest MapName 3", _TEMP_IMAGE_PATH: mapImagePath);
            // Create location and add 20 notes
            SpatialNotes.LocationInfo location = new SpatialNotes.LocationInfo(_locationName: "Test Location 3", _description: "Test Description 3", _coordinate: new Vector3(1, 2, 3), "");
            map3.AddLocation(location: location);
            for (int i = 0; i < 20; i++)
            {
                PostCard note = new PostCard(_date: System.DateTime.Now);
                map3.AddPostcard(location, note);
            }

            Assert.AreEqual(map3.name, "ObjectCreationTest MapName 3");
            Assert.AreEqual(map3.GetNumberOfNotes(), 20);
            Assert.AreEqual(map3.GetNumberOfLocations(), 1);
            Debug.Log("Test Passed");

            // Clear used asset folder
            map3.DeleteMap();
            map3 = null;
        }

        // Test instantiation of map with name, tbn and img, 20 notes then save only
        [Test]
        public void InstantiateMapWithNameTbnImg20NotesSaveOnly()
        {
            // Create a new map object
            MapObject map4 = new MapObject();
            string mapImagePath = Application.streamingAssetsPath + "/demomap.jpg";
            map4.CreateMap(_name: "ObjectCreationTest MapName 4", _TEMP_IMAGE_PATH: mapImagePath);
            // Create location and add 20 notes
            SpatialNotes.LocationInfo location = new SpatialNotes.LocationInfo(_locationName: "Test Location 4", _description: "Test Description 4", _coordinate: new Vector3(1, 2, 3), "");
            map4.AddLocation(location: location);
            for (int i = 0; i < 20; i++)
            {
                PostCard note = new PostCard(_date: System.DateTime.Now);
                map4.AddPostcard(location, note);
            }

            map4.SaveAll();

            // Clear used asset folder
            map4.DeleteMap();
            map4 = null;
        }

        // Test instantiation of map with name, tbn and img, 20 notes then save and load
        [Test]
        public void InstantiateMapWithNameTbnImg20NotesSaveAndLoad()
        {
            // Create a new map object
            MapObject map5 = new MapObject();
            string mapImagePath = Application.streamingAssetsPath + "/demomap.jpg";
            map5.CreateMap(_name: "ObjectCreationTest MapName 5", _TEMP_IMAGE_PATH: mapImagePath);
            // Create location and add 20 notes
            SpatialNotes.LocationInfo location = new SpatialNotes.LocationInfo(_locationName: "Test Location 5", _description: "Test Description 5", _coordinate: new Vector3(1, 2, 3), "");
            map5.AddLocation(location: location);
            for (int i = 0; i < 20; i++)
            {
                PostCard note = new PostCard( _date: System.DateTime.Now);
                map5.AddPostcard(location, note);
            }

            map5.SaveAll();
            map5.LoadAll("objectcreationtestmapname5");

            Debug.Log("Number of notes: " + map5.GetNumberOfNotes());
            Assert.AreEqual(map5.name, "ObjectCreationTest MapName 5");
            Assert.AreEqual(map5.GetNumberOfNotes(), 20);
            Assert.AreEqual(map5.GetNumberOfLocations(), 1);
        }

        // Test seerialize and deserialize of a dictionary<string, List<string>>
        [Test]
        public void SerializeDeserializeDictionary()
        {
            // Create a new dictionary
            Dictionary<string, SpatialNotes.JsonableListWrapper<string>> dict = new SpatialNotes.SerializableDictionary<string, SpatialNotes.JsonableListWrapper<string>>();
            List<string> list1 = new List<string>();
            SpatialNotes.JsonableListWrapper<string> wrapper1 = new SpatialNotes.JsonableListWrapper<string>(list1);
            list1.Add("TestValue1");
            list1.Add("TestValue2");
            list1.Add("TestValue3");
            dict.Add("TestKey1", wrapper1);
            List<string> list2 = new List<string>();
            SpatialNotes.JsonableListWrapper<string> wrapper2 = new SpatialNotes.JsonableListWrapper<string>(list2);
            list2.Add("TestValue4");
            list2.Add("TestValue5");
            list2.Add("TestValue6");
            dict.Add("TestKey2", wrapper2);


            // Serialize and deserialize as json string
            string json = JsonUtility.ToJson(dict);
            Debug.Log("Serialized: " + json);
            SpatialNotes.SerializableDictionary<string, SpatialNotes.JsonableListWrapper<string>> dict2 = JsonUtility.FromJson< SpatialNotes.SerializableDictionary<string, SpatialNotes.JsonableListWrapper<string>>>(json);
            Debug.Log("Deserialized: " + dict2);

            // Check if the two dictionaries are the same
            Assert.AreEqual(dict.Count, dict2.Count);



        }
    }


}