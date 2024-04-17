using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TigerForge;
using TMPro;

namespace SpatialNotes
{

    public class MapInteract : MonoBehaviour
    {
        private GameObject sideMenuNoSelectAddLocButton;
        private GameObject sideMenu;
        [SerializeField]
        private GameObject emptySideMenuNoSelection;
        [SerializeField]
        private GameObject sideMenuCreateLocation;
        private GameObject sideMenuShowLocation;
        private GameObject sideMenuCreateLocationButtonAdd;
        private GameObject sideMenuCreateLocationButtonCancel;
        private GameObject sideMenuCreateLocationNameField;
        private GameObject sideMenuCreateLocationDescriptionField;
        public GameObject pinsFolder;
        public GameObject locationPinsFolder;
        public GameObject pinPrefab;
        public Button createButton;
        public Camera cam;
        public Main main;
        public MapObject map;
        private GameObject mapCanvas;
        private GameObject mapImageZoom;
        private Vector3 savedUiPosOnClick;

        private GameObject zoomScrollBar;
        private GameObject staticCanvas;

        public float ZoomLevel = 1.0f;
        public float maxZoom = 50.0f;
        public float startingZoomPercent = 0.25f;
        private float currentZoom = 0.0f;
        public float minZoom = 0.0f;
        public float zoomSpeed = 2.0f;
        public float panSpeed = 1.3f;

        private enum mouseButton { Left, Right, Middle };
        
        //Variables needed for drag
        private Vector3 dragOrigin;
        private Vector3 worldSpaceOrigin;
        private bool dragging = false;
        private Vector3 dragChange;

        private Vector3 lastSelectedLocation;
        private Vector3 lastCandidateUiPos;

        void Start()
        {
            createButton.onClick.AddListener(addButtonClick);
            map = main.map;
            map.DisplayMapInfo();

            //get canvas
            staticCanvas = GameObject.Find("StaticCanvas");
            mapCanvas = GameObject.Find("MapCanvas");
            if (mapCanvas == null)
            {
                Debug.Log("MapCanvas not found");
                return;
            }
            // Get the map image
            mapImageZoom = mapCanvas.transform.Find("MapImage").gameObject;

            // Get Scroll Bar for zooming
            zoomScrollBar = GameObject.Find("ScrollZoomBar").transform.GetChild(0).gameObject;
            //define initial, min, max zoom
            zoomScrollBar.GetComponent<Scrollbar>().value = startingZoomPercent;
            currentZoom = maxZoom * startingZoomPercent;

            // Get the empty side menu
            sideMenu = staticCanvas.transform.Find("SideMenuEmpty").gameObject;
            emptySideMenuNoSelection = sideMenu.transform.Find("SideMenuNoSelect").gameObject;
            sideMenuCreateLocation = sideMenu.transform.Find("SideMenuCreateLocation").gameObject;
            sideMenuShowLocation = sideMenu.transform.Find("SideMenuExistingLocation").gameObject;

            // Get the location button
            sideMenuCreateLocationButtonAdd = sideMenuCreateLocation.transform.Find("Add").gameObject;
            sideMenuCreateLocationButtonCancel = sideMenuCreateLocation.transform.Find("Cancel").gameObject;
            sideMenuCreateLocationDescriptionField = sideMenuCreateLocation.transform.Find("Description").gameObject;
            sideMenuCreateLocationNameField = sideMenuCreateLocation.transform.Find("Name").gameObject;
            sideMenuNoSelectAddLocButton = emptySideMenuNoSelection.transform.Find("AddLocButton").gameObject;
            
            // Add button listeners
            sideMenuCreateLocationButtonAdd.GetComponent<Button>().onClick.AddListener(LocationAdd);
            sideMenuCreateLocationButtonCancel.GetComponent<Button>().onClick.AddListener(LocationCancel);
            sideMenuNoSelectAddLocButton.GetComponent<Button>().onClick.AddListener(_existingMenuAddLocButton);
            

            _removeAllPins(pinsFolder);
            _hideSideMenu();

            //load locations
            OnStartLoadLocations();

            Debug.Log("MapInteract Start");
        }

        void Update()
        {
            //Handle zoom in and out WIP
            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                _zoomIn();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
            {
                _zoomOut();
            }

            // move right click menu to mouse position
            if (Input.GetMouseButtonDown((int)mouseButton.Right))
            {
                
                _removeAllPins(pinsFolder);
                //Instantiate Pin
                lastCandidateUiPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
                lastSelectedLocation = lastCandidateUiPos;
                Vector3 worldClickPos = _getWorldClickPosition(lastSelectedLocation);
                _debugShowNearbyPinsToClick(worldClickPos);
                EventManager.SetData("CURSOR_NAME", "CIRCLE");
                EventManager.EmitEvent("CURSOR_REFRESH");
                
                

                GameObject clickedPin = checkIfLocationPinClicked(worldClickPos);
                if (clickedPin != null)
                {
                    Debug.Log("Clicked on location pin: " + clickedPin.transform.Find("PinName").GetComponent<TextMeshProUGUI>().text);
                    _showSideMenuShowLocation(clickedPin);
                }
                else
                {
                    Debug.Log("Clicked on empty space");
                    
                    GameObject pin = Instantiate(pinPrefab, new Vector3(worldClickPos.x, worldClickPos.y, 1), Quaternion.identity);
                    pin.transform.SetParent(pinsFolder.transform);
                    //pin.transform.position = new Vector3(worldClickPos.x, worldClickPos.y, 1);
                    //Populate pin name
                    GameObject pinName = pin.transform.Find("PinName").gameObject;
                    pinName.GetComponent<TextMeshProUGUI>().text = "Selected Location";

                    //populate pinID
                    PinID id = pin.GetComponent<PinID>();
                    id.locationInfo = new LocationInfo("Selected Location", "Selected Location", worldClickPos);
                    id.type = "Selected";

                    _HideAllSideMenuBranches();
                    _showSideMenuNoSelection();
                }
            }

            // middle mouse or esc button
            if (Input.GetMouseButtonDown((int)mouseButton.Middle) || Input.GetKeyDown(KeyCode.Escape))
            {
                _removeAllPins(pinsFolder);
                _hideSideMenu();
                EventManager.SetData("CURSOR_NAME", "NORMAL");
                EventManager.EmitEvent("CURSOR_REFRESH");
                Debug.Log("Middle Mouse Button Clicked");
            }
            

            // Left Click Drag
            if (Input.GetMouseButtonDown((int)mouseButton.Left))
            {
                dragging = true;
                dragOrigin = Input.mousePosition;
                worldSpaceOrigin = cam.transform.position;
                EventManager.SetData("CURSOR_NAME", "NORMAL");
                EventManager.EmitEvent("CURSOR_REFRESH");
                return;
            } 
            else if (Input.GetMouseButtonUp((int)mouseButton.Left))
            {
                dragging = false;
                return;
            }
            
            // Dragging
            if (dragging)
            {
                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
                float zoomAdjustedPanSpeed = Mathf.Clamp((float)(panSpeed * (1 - currentZoom / maxZoom)), 0.15f * panSpeed, 2.0f * panSpeed);
                //Debug.Log("Pan Speed: " + zoomAdjustedPanSpeed + " " + panSpeed + " " + maxZoom + " " + currentZoom + " " + (maxZoom - currentZoom / maxZoom));
                Vector3 move = new Vector3(pos.x * 100 * zoomAdjustedPanSpeed, pos.y * 100 * zoomAdjustedPanSpeed, 0);
                if (dragChange == null || move != dragChange)
                {
                    dragChange = move;
                    cam.transform.position = worldSpaceOrigin - move;
                }
            }
        }

        public void OnStartLoadLocations()
        {
            // Get all locations from the database
            Debug.Log("Exists; map: " + map.GetNumberOfLocations());
            Dictionary<string, LocationInfo> locations = map.GetLocations();

            foreach (KeyValuePair<string, LocationInfo> loc in locations)
            {
                //add pin to the map
                Vector3 coord = map._convertCoordStr2Vec3(loc.Key);
                //Debug.Log("Adding pin to the map for location: " + loc.Value.locationName + "at location: " + coord);
                GameObject pin = Instantiate(pinPrefab, new Vector3(coord.x, coord.y, 1), Quaternion.identity);
                pin.transform.SetParent(locationPinsFolder.transform);

                //Populate pin name
                GameObject pinName = pin.transform.Find("PinName").gameObject;
                pinName.GetComponent<TextMeshProUGUI>().text = loc.Value.locationName;

                //populate pinID
                PinID id = pin.GetComponent<PinID>();
                id.locationInfo = loc.Value;
                id.type = "Location";

                //pins of locations are green
                pin.GetComponent<Image>().color = Color.green;

            }
            
        }

        private void _existingMenuAddLocButton()
        {
            _showSideMenuCreateLocation();
        }


        public void LocationAdd()
        {
            Debug.Log("Location Added");
            Vector3 coord = _getWorldClickPosition(lastSelectedLocation);
            string locationName = sideMenuCreateLocationNameField.GetComponent<InputField>().text;
            string locationDescription = sideMenuCreateLocationDescriptionField.GetComponent<InputField>().text;
            Debug.Log(savedUiPosOnClick + " " + coord + " " + locationName + " " + locationDescription);
            //Add location to the database
            map.AddLocation(locCoord: coord, locName: locationName, locDescription: locationDescription);
            Debug.Log("Location Added to the database, new location count: " + map.GetNumberOfLocations());

            //add pin to the map
            GameObject pin = Instantiate(pinPrefab, new Vector3(coord.x, coord.y, 1), Quaternion.identity);
            pin.transform.SetParent(locationPinsFolder.transform);
            //Populate pin name
            GameObject pinName = pin.transform.Find("PinName").gameObject;
            pinName.GetComponent<TextMeshProUGUI>().text = locationName;

            //populate pinID
            PinID id = pin.GetComponent<PinID>();
            id.locationInfo = new LocationInfo(locationName, locationDescription, coord);
            id.type = "Location";

            //pins of locations are green
            pin.GetComponent<Image>().color = Color.green;

            //call for save to file
            map.SaveAll();

            //clear the fields for the next location
            sideMenuCreateLocationNameField.GetComponent<InputField>().text = "";
            sideMenuCreateLocationDescriptionField.GetComponent<InputField>().text = "";


            //Close the add location canvas
            _hideSideMenu();
        }
        public void LocationCancel()
        {
            _hideSideMenu();
        }

        private bool _getSideMenUIsActive()
        {
            return sideMenu.activeInHierarchy;
        }

        private void _hideSideMenu()
        {
            sideMenu.SetActive(false);
        }

        private void _HideAllSideMenuBranches()
        {
            emptySideMenuNoSelection.SetActive(false);
            sideMenuCreateLocation.SetActive(false);
            _hideSideMenuShowLocation();
        }

        private GameObject _getClosestLocationToThreshold(Vector2 worldpos, float threshold = 10.0f)
        {
            // Get all pins
            List<GameObject> locationPins = new List<GameObject>();
            List<Vector3> locationPinPositions = new List<Vector3>();
            foreach (Transform child in locationPinsFolder.transform)
            {
                locationPins.Add(child.gameObject);
                locationPinPositions.Add(child.position);
            }

            // Get the distance to each pin
            for (int i = 0; i < locationPins.Count; i++)
            {
                float distance = Vector3.Distance(worldpos, locationPinPositions[i]);
                if (distance < threshold)
                {
                    return locationPins[i];
                }
            }
            return null;
        }

        private GameObject checkIfLocationPinClicked(Vector2 worldpos)
        {
            float distThreshold = 3.5f / ZoomLevel;
            GameObject closestPin = _getClosestLocationToThreshold(worldpos, distThreshold);
            if (closestPin != null)
            {
                return closestPin;
            }
            return null;
        }


        private void _debugShowNearbyPinsToClick(Vector2 worldpos)
        {
            float distThreshold = 3.5f / ZoomLevel;
            // Get all pins
            List<GameObject> pins = new List<GameObject>();
            List<Vector3> pinPositions = new List<Vector3>();
            foreach (Transform child in pinsFolder.transform)
            {
                pins.Add(child.gameObject);
                pinPositions.Add(child.position);
            }
            List<GameObject> locationPins = new List<GameObject>();
            List<Vector3> locationPinPositions = new List<Vector3>();
            foreach (Transform child in locationPinsFolder.transform)
            {
                locationPins.Add(child.gameObject);
                locationPinPositions.Add(child.position);
            }
            // Get the distance to each pin
            for (int i = 0; i < pins.Count; i++)
            {
                float distance = Vector3.Distance(worldpos, pinPositions[i]);
                Debug.Log("Distance to pin " + i + " is " + distance + ", this is within the threshold: " + (distance < distThreshold) + " threshold: " + distThreshold);
            }
            for (int i = 0; i < locationPins.Count; i++)
            {
                float distance = Vector3.Distance(worldpos, locationPinPositions[i]);
                Debug.Log("Distance to location pin " + i + " is " + distance + ", this is within the threshold: " + (distance < distThreshold) + " threshold: " + distThreshold + "(name: " + locationPins[i].transform.Find("PinName").GetComponent<TextMeshProUGUI>().text + ")");
            }

            GameObject closestPin = _getClosestLocationToThreshold(worldpos, distThreshold);
            if (closestPin != null)
            {
                Debug.Log("Closest pin to click is: " + closestPin.transform.Find("PinName").GetComponent<TextMeshProUGUI>().text);
            }

            
        }

        private void _zoomIn()
        {
            // Zoom camera in 
            float _zoomAmt = zoomSpeed;
            //don't zoom if we are at the max zoom
            if (currentZoom + _zoomAmt > maxZoom)
            {
                return;
            }
            currentZoom += _zoomAmt;
            
            zoomScrollBar.GetComponent<Scrollbar>().value = currentZoom / maxZoom;

            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z + _zoomAmt);


        }

        private void _zoomOut()
        {
            // Zoom camera out
            float _zoomAmt = zoomSpeed;
            //don't zoom if we are at the min zoom
            if (currentZoom - _zoomAmt < minZoom)
            {
                return;
            }
            currentZoom -= _zoomAmt;
            zoomScrollBar.GetComponent<Scrollbar>().value = currentZoom / maxZoom;

            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z - zoomSpeed);
        }

        private Vector3 _getWorldClickPosition(Vector3 lastCandidateUiPos)
        {
            //adjust for camera offset
            lastCandidateUiPos.z = Mathf.Abs(cam.transform.position.z);
            //lastCandidateUiPos.x = lastCandidateUiPos.x;
            //lastCandidateUiPos.y = lastCandidateUiPos.y;

            return cam.ScreenToWorldPoint(lastCandidateUiPos);
        }

        private Vector4 _calcVisibleImageDims()
        {
            Vector2 fullSize = new Vector2(1600, 900); // Full size of the image (before zooming)
            Vector2 visibleSize = new Vector2(Screen.width / ZoomLevel, Screen.height / ZoomLevel);
            Vector2 centerPoint = new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 topLeft = centerPoint - (visibleSize / 2);
            Vector2 bottomRight = centerPoint + (visibleSize / 2);
            Vector2 visibleDims = bottomRight - topLeft;
            return new Vector4(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        }

        private Vector2 _calcCenterpoint(Vector4 visibleDims)
        {
            return new Vector2((visibleDims.z - visibleDims.x) / 2, (visibleDims.w - visibleDims.y) / 2);
        }

        private Vector2 _calcVisibleSize(Vector4 visibleDims)
        {
            return new Vector2(visibleDims.z - visibleDims.x, visibleDims.w - visibleDims.y);
        }

        private void _showSideMenuNoSelection()
        {
            sideMenu.SetActive(true);
            _HideAllSideMenuBranches();

            Vector3 coord = _getWorldClickPosition(lastSelectedLocation);

            //Set text to the location
            TextMeshProUGUI locationText = emptySideMenuNoSelection.transform.Find("Coordinates").GetComponent<TextMeshProUGUI>();
            locationText.text = "(" + coord.x + ", " + coord.y + ")";
            TextMeshProUGUI DescriptionText = emptySideMenuNoSelection.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
            DescriptionText.text = "Description: ";

            emptySideMenuNoSelection.SetActive(true);

        }

        private void _showSideMenuEmpty()
        {
            sideMenu.SetActive(true);
            _HideAllSideMenuBranches();
        }

        private void _showSideMenuCreateLocation()
        {
            _showSideMenuEmpty();
            sideMenuCreateLocation.SetActive(true);
        }

        private void _showSideMenuShowLocation(GameObject locationPin)
        {
            _showSideMenuEmpty();
            

            //Set text to the location
            LocationInfo locInfo = locationPin.GetComponent<PinID>().locationInfo;
            TextMeshProUGUI locationText = sideMenuShowLocation.transform.Find("LocationText").GetComponent<TextMeshProUGUI>();
            locationText.text = locInfo.locationName;
            TextMeshProUGUI CoordinatesText = sideMenuShowLocation.transform.Find("CoordinateText").GetComponent<TextMeshProUGUI>();
            CoordinatesText.text = "Coordinates: (" + locInfo.coordinate.x + ", " + locInfo.coordinate.y + ")";
            TextMeshProUGUI DescriptionText = sideMenuShowLocation.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
            DescriptionText.text = "Description: " + locInfo.description;

            sideMenuShowLocation.SetActive(true);
        }
        private void _hideSideMenuShowLocation()
        {
            sideMenuShowLocation.SetActive(false);
        }




        private Vector3 _convertMousePos2Coord(Vector2 mousePos)
        {
            //Get data
            Vector4 visibleDims = _calcVisibleImageDims();
            Vector2 centerPoint = _calcCenterpoint(visibleDims);
            Vector2 visibleSize = _calcVisibleSize(visibleDims);

            // add the visible dims to the mouse pos
            Vector2 mousePosPercentage = new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height);
            Vector2 mousePosAddVisSize = new Vector2(mousePosPercentage.x * visibleSize.x, mousePosPercentage.y * visibleSize.y);
            Vector2 mousePosAddVisDims = new Vector2(mousePosAddVisSize.x + visibleDims.x, mousePosAddVisSize.y + visibleDims.y);
            // convert the mouse pos to a percentage of the Actual dims
            Vector2 coord = new Vector2(mousePosAddVisDims.x / 16, mousePosAddVisDims.y / 9);

            // round to 3 decimal places
            coord.x = Mathf.Round(coord.x * 1000) / 1000;
            coord.y = Mathf.Round(coord.y * 1000) / 1000;

            //clamp to 0-100
            coord.x = Mathf.Clamp(coord.x, 0, 100);
            coord.y = Mathf.Clamp(coord.y, 0, 100);
            return coord;
        }

        private Vector3 _convertCoord2MousePos(Vector3 coord)
        {
            //Get data
            Vector4 visibleDims = _calcVisibleImageDims();
            Vector2 centerPoint = _calcCenterpoint(visibleDims);
            Vector2 visibleSize = _calcVisibleSize(visibleDims);

            Vector2 coordActualLocation = new Vector2(coord.x * 16, coord.y * 9);
            Vector2 coordSubVisDims = new Vector2(coordActualLocation.x - visibleDims.x, coordActualLocation.y - visibleDims.y);
            Vector2 coordSubVisDimsPercentage = new Vector2(coordSubVisDims.x / visibleSize.x, coordSubVisDims.y / visibleSize.y);

            // convert to mouse pos
            Vector2 mousePos = new Vector2(coordSubVisDimsPercentage.x * Screen.width, coordSubVisDimsPercentage.y * Screen.height);

            // round to 2 decimal places
            mousePos.x = Mathf.Round(mousePos.x * 1000) / 1000;
            mousePos.y = Mathf.Round(mousePos.y * 1000) / 1000;

            //clamp to 0-screen width/height
            mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
            mousePos.y = Mathf.Clamp(mousePos.y, 0, Screen.height);


            return mousePos;
        }

        private void _removeAllPins(GameObject folder)
        {
            foreach (Transform child in folder.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }


        void addButtonClick()
        {
            Debug.Log("Button Clicked");
            _showSideMenuCreateLocation();
            //addLocationCanvas.enabled = true;
        }

        // Wrapper trigger popup
        public void TriggerPopup(string text, string title, string imageName)
        {
            EventManager.SetData("MODEL_POPUP", text);
            EventManager.SetData("MODEL_POPUP_TITLE", title);
            EventManager.SetData("MODEL_POPUP_IMAGE", imageName);
            EventManager.EmitEvent(eventName: "MODEL_POPUP", delay: 0, sender: gameObject);
        }
    }

}
