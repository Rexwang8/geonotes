using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialNotes;
using TigerForge;

public class Main : MonoBehaviour
{
    public MapObject map;
    private string mapToLoad;
    private void Awake() {
        //subscribe to the event
        EventManager.StartListening("LOAD_MAP", LoadMap);
    }

    private void LoadMap() {
        mapToLoad = (string)EventManager.GetData("MAP_TO_LOAD");
        // Load the map
        if (mapToLoad == null) {
            Debug.Log("No map to load");
            return;
        } else {
            Debug.Log("Loading map: " + mapToLoad);
        }

        map = new MapObject();
        map.LoadAll(mapToLoad);

        // Get the map canvas
        GameObject mapCanvas = GameObject.Find("MapCanvas");
        if (mapCanvas == null) {
            Debug.Log("MapCanvas not found");
            return;
        }
        // Set image
        Texture2D tex = map.image;
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        GameObject mapImage = mapCanvas.transform.Find("Image").gameObject;
        mapImage.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
        Debug.Log("Map loaded");
    }

    void Update()
    {
        //if s is pressed, save the map
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Saved Map");
            map.SaveAll();
        }
    }

}
