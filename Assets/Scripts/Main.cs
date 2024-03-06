using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialNotes;
using TigerForge;

public class Main : MonoBehaviour
{
    public MapObject map;
    private string mapToLoad;
    public bool debug = true;
    public string debugMap = "dfgdfgdfg";
    private void Awake() {
        //subscribe to the event
        EventManager.StartListening("LOAD_MAP", LoadMap);
        // Raise event if debug set to true
        if (debug) {
            EventManager.SetData("MAP_TO_LOAD", debugMap);
            EventManager.EmitEvent("LOAD_MAP");
        }
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
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        GameObject mapImage = mapCanvas.transform.Find("ImageZoom").GetChild(0).gameObject; //get child of mapImage
        mapImage.GetComponent<UnityEngine.UI.Image>().sprite = sprite;

        // Scale the map to fit the screen, until tex is scaled to screen size
        float scale = 0.1f;
        float amtScaledToScreen = 0.95f;
        while (1600 * scale < amtScaledToScreen * screenWidth && 900 * scale < amtScaledToScreen * screenHeight) {
            scale += 0.01f;
        }
        mapImage.transform.localScale = new Vector3(scale, scale, 1);
        GameObject EveryThingElse = GameObject.Find("EverythingElseScaled");
        EveryThingElse.transform.localScale = new Vector3(scale, scale, 1);


        // Set the map canvas to active
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
