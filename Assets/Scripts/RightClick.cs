using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RightClick : MonoBehaviour
{
    public Canvas rightClickCanvas;
    public Canvas addLocationCanvas;
    public Button createButton;
    public Camera cam;

    void Start()
    {
        createButton.onClick.AddListener(addButtonClick);
        rightClickCanvas.enabled = false;
        addLocationCanvas.enabled = false;
    }
    
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {            
            // Get the mouse position in screen coordinates
            Vector3 mousePosition = Input.mousePosition;
            Debug.Log("-------------------");
            Debug.Log("mouse " + mousePosition);
            Debug.Log("canvas " + rightClickCanvas.transform.position); 
           
            // Convert the screen coordinates to world coordinates
            Vector3 worldPosition = cam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10));
            Debug.Log("cam " + worldPosition);

            // Set the button's position to the mouse position
            rightClickCanvas.transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);

            if(!rightClickCanvas.enabled)
            {
                rightClickCanvas.enabled = true;
            }
        }   

        if(Input.GetMouseButtonDown(2))
        {
            rightClickCanvas.enabled = false;
        } 
    }

    void addButtonClick()
    {
        Debug.Log("Button Clicked");
        rightClickCanvas.enabled = false;
        addLocationCanvas.enabled = true;
    }
}
