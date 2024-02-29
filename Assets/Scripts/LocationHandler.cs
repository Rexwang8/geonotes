using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpatialNotes
{

public class LocationHandler : MonoBehaviour
{
    //Add location button
    public Canvas addLocationCanvas;
    public Button cancelButton;
    public Button addLocationButton;
    public InputField locationNameField;
    public InputField locationDescriptionField;
    private string locationName;
    private string locationDescription;

    public GameObject pinPrefab;

    //Other GameObjects
    public RightClick rightClick;
    public Main main;
    //Get Script
    // Start is called before the first frame update
    void Start()
    {
        cancelButton.onClick.AddListener(() => {
            addLocationCanvas.enabled = false;
        });

        addLocationButton.onClick.AddListener(() => {
            AddLocation();
        });
        locationNameField.onEndEdit.AddListener(delegate { locationName = locationNameField.text; });
        locationDescriptionField.onEndEdit.AddListener(delegate { locationDescription = locationDescriptionField.text; });
    
        AddPins();
    }

    public void AddLocation()
    {
        Debug.Log("Location Added");
        Debug.Log(rightClick.location.x + " " + rightClick.location.y + " " + rightClick.location.z);
        Debug.Log(locationName);
        Debug.Log(locationDescription);

        //Add location to main
        main.map.AddLocation(rightClick.location, locationName, locationDescription);
        main.map.DisplayLocationInfo();

        //Add pin prefab to map
        Instantiate(pinPrefab, rightClick.location, Quaternion.identity);

        //Close the add location canvas
        addLocationCanvas.enabled = false;
    } 

    private void AddPins()
    {
        foreach (var location in main.map.locationDict)
        { 
            Instantiate(pinPrefab, main.map._convertCoordStr2Vec3(location.Key), Quaternion.identity);
        }
    }
}
}