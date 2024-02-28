using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialNotes;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MapObject mapObject = new MapObject();
        string mapImagePath = Application.streamingAssetsPath + "/demomap.jpg";
        mapObject.CreateMap(_name: "KevinTest", _TEMP_IMAGE_PATH: mapImagePath);

        mapObject.AddLocation("123", "Location 1", "Description 1");
        mapObject.AddLocation("12", "Location 2", "Description 2");
        mapObject.AddLocation("1", "Location 3", "Description 3");

        PostCard note = new PostCard(_title: "Test Note 1", _date: System.DateTime.Now);
        mapObject.AddPostcard("123", note);
        note = new PostCard(_title: "Test Note 2", _date: System.DateTime.Now);
        mapObject.AddPostcard("123", note);
        note = new PostCard(_title: "Test Note 3", _date: System.DateTime.Now);
        mapObject.AddPostcard("12", note);
        note = new PostCard(_title: "Test Note 4", _date: System.DateTime.Now);
        mapObject.AddPostcard("1", note);

        mapObject.SaveLocationJson();
        mapObject.SaveNoteJson();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
