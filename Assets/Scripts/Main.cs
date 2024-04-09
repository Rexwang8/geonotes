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
        Debug.Log("MapCanvas found");
        // Set image
        Texture2D tex = map.image;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        Debug.Log("Screen width: " + tex.width + " Screen height: " + tex.height);
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        GameObject mapImage = mapCanvas.transform.Find("MapImage").GetChild(0).gameObject; //get child of mapImage
        //Adjust the height and width of the map image to match the sprite
        RectTransform rt = mapImage.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(tex.width, tex.height);

        mapImage.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
        
        Debug.Log("Screen width: " + screenWidth + " Screen height: " + screenHeight);
        
        //Scale the map to fit the screen
        float scale = 0.1f;
        float amtScaledToScreen = 0.95f;
        while (tex.width * scale < amtScaledToScreen * screenWidth && tex.height * scale < amtScaledToScreen * screenHeight) {
            scale += 0.01f;
        }
        mapImage.transform.localScale = new Vector3(scale, scale, 1);
        GameObject EverythingElse = GameObject.Find("EverythingElseScaled");
        EverythingElse.transform.localScale = new Vector3(scale, scale, 1);

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
