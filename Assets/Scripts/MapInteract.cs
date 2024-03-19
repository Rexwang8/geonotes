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
        public Vector3 location;
        public GameObject rightClickObject;
        public GameObject sideMenu;
        [SerializeField]
        private GameObject emptySideMenuNoSelection;
        [SerializeField]
        private GameObject sideMenuCreateLocation;
        private GameObject locationButtonAdd;
        private GameObject locationButtonCancel;
        private GameObject locationNameField;
        private GameObject locationDescriptionField;
        public GameObject pinsFolder;
        public GameObject locationPinsFolder;
        public GameObject pinPrefab;
        public Button createButton;
        public Camera cam;
        public Main main;
        public MapObject map;
        public GameObject mapCanvas;
        public GameObject mapImageZoom;
        private Vector3 savedUiPosOnClick;

        public float ZoomLevel = 1.0f;
        public float maxZoom = 5.0f;
        public float minZoom = 1.0f;

        private enum mouseButton { Left, Right, Middle };

        void Start()
        {
            createButton.onClick.AddListener(addButtonClick);
            map = main.map;
            map.DisplayMapInfo();

            // Get the empty side menu
            emptySideMenuNoSelection = sideMenu.transform.Find("SideMenuNoSelect").gameObject;
            sideMenuCreateLocation = sideMenu.transform.Find("SideMenuCreateLocation").gameObject;

            // Get the location button
            GameObject AddPanel = sideMenuCreateLocation.transform.Find("AddLocation").gameObject;
            locationButtonAdd = AddPanel.transform.Find("Add").gameObject;
            locationButtonCancel = AddPanel.transform.Find("Cancel").gameObject;
            locationDescriptionField = AddPanel.transform.Find("Description").gameObject;
            locationNameField = AddPanel.transform.Find("Name").gameObject;
            // Add button listeners
            locationButtonAdd.GetComponent<Button>().onClick.AddListener(LocationAdd);
            locationButtonCancel.GetComponent<Button>().onClick.AddListener(LocationCancel);
            

            // Get the map canvas
            mapCanvas = GameObject.Find("MapCanvas");
            if (mapCanvas == null)
            {
                Debug.Log("MapCanvas not found");
                return;
            }
            // Get the map image
            mapImageZoom = mapCanvas.transform.Find("ImageZoom").gameObject;

            rightClickObject.SetActive(false);
            sideMenu.SetActive(false);

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
            Vector2 uipos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            // move right click menu to mouse position
            if (Input.GetMouseButtonDown((int)mouseButton.Right))
            {
                _removeAllPins(pinsFolder);
                _hideRightClickMenu();
                rightClickObject.transform.position = new Vector3(uipos.x + 100, uipos.y + 10, 0);

                //Instantiate Pin
                GameObject pin = Instantiate(pinPrefab, new Vector3(uipos.x, uipos.y, 0), Quaternion.identity);
                pin.transform.SetParent(pinsFolder.transform);
                pin.transform.position = new Vector3(uipos.x, uipos.y, 0);


                if (!_isRightClickMenuActive())
                {
                    savedUiPosOnClick = uipos;
                    _showRightClickMenu();
                }
                _HideAllSideMenuBranches();
                _showSideMenuNoSelection();

            }

            // middle mouse or esc button
            if (Input.GetMouseButtonDown((int)mouseButton.Middle) || Input.GetKeyDown(KeyCode.Escape))
            {
                _removeAllPins(pinsFolder);
                _hideRightClickMenu();
                _hideSideMenu();
                Debug.Log("Left Mouse Button Clicked");
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
                Vector3 coord = _convertCoord2MousePos(map._convertCoordStr2Vec3(loc.Key));
                Debug.Log("Adding pin to the map for location: " + loc.Value.locationName + "at location: " + coord);
                GameObject pin = Instantiate(pinPrefab, new Vector3(coord.x, coord.y, 0), Quaternion.identity);
                pin.transform.SetParent(locationPinsFolder.transform);
                pin.transform.position = new Vector3(coord.x, coord.y, 0);


            }
            
        }


        public void LocationAdd()
        {
            Debug.Log("Location Added");
            Vector3 coord = _convertMousePos2Coord(savedUiPosOnClick);
            string locationName = locationNameField.GetComponent<InputField>().text;
            string locationDescription = locationDescriptionField.GetComponent<InputField>().text;
            Debug.Log(savedUiPosOnClick + " " + coord + " " + locationName + " " + locationDescription);
            //Add location to the database
            map.AddLocation(locCoord: coord, locName: locationName, locDescription: locationDescription);
            Debug.Log("Location Added to the database, new location count: " + map.GetNumberOfLocations());

            //add pin to the map
            GameObject pin = Instantiate(pinPrefab, new Vector3(savedUiPosOnClick.x, savedUiPosOnClick.y, 0), Quaternion.identity);
            pin.transform.SetParent(locationPinsFolder.transform);
            pin.transform.position = new Vector3(savedUiPosOnClick.x, savedUiPosOnClick.y, 0);

            //call for save to file
            map.SaveAll();


            //Close the add location canvas
            _hideSideMenu();
        }
        public void LocationCancel()
        {
            _hideSideMenu();
        }
        private void _hideRightClickMenu()
        {
            rightClickObject.SetActive(false);
        }

        private void _showRightClickMenu()
        {
            rightClickObject.SetActive(true);

        }
        private bool _isRightClickMenuActive()
        {
            return rightClickObject.activeInHierarchy;
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
        }

        private void _zoomIn()
        {
            if (ZoomLevel >= maxZoom)
            {
                return;
            }

            // Get all pins
            List<GameObject> pins = new List<GameObject>();
            List<Vector3> pinPositions = new List<Vector3>();
            foreach (Transform child in pinsFolder.transform)
            {
                pins.Add(child.gameObject);
                pinPositions.Add(_convertMousePos2Coord(child.position));
            }
            List<GameObject> locationPins = new List<GameObject>();
            List<Vector3> locationPinPositions = new List<Vector3>();
            foreach (Transform child in locationPinsFolder.transform)
            {
                locationPins.Add(child.gameObject);
                locationPinPositions.Add(_convertMousePos2Coord(child.position));
            }
            ZoomLevel += 0.1f;
            ZoomLevel = Mathf.Clamp(ZoomLevel, minZoom, maxZoom);
            //change scale on the image
            mapImageZoom.transform.localScale = new Vector3(ZoomLevel, ZoomLevel, 1);
            _calcVisibleImageDims();

            // Move the pins based on map zoom
            for (int i = 0; i < pins.Count; i++)
            {
                pins[i].transform.position = _convertCoord2MousePos(pinPositions[i]);
            }
            for (int i = 0; i < locationPins.Count; i++)
            {
                locationPins[i].transform.position = _convertCoord2MousePos(locationPinPositions[i]);
            }


        }

        private void _zoomOut()
        {
            //guards
            if (ZoomLevel <= minZoom)
            {
                return;
            }

            // Get all pins
            List<GameObject> pins = new List<GameObject>();
            List<Vector3> pinPositions = new List<Vector3>();
            foreach (Transform child in pinsFolder.transform)
            {
                pins.Add(child.gameObject);
                pinPositions.Add(_convertMousePos2Coord(child.position));
            }
            List<GameObject> locationPins = new List<GameObject>();
            List<Vector3> locationPinPositions = new List<Vector3>();
            foreach (Transform child in locationPinsFolder.transform)
            {
                locationPins.Add(child.gameObject);
                locationPinPositions.Add(_convertMousePos2Coord(child.position));
            }
            ZoomLevel -= 0.1f;
            ZoomLevel = Mathf.Clamp(ZoomLevel, minZoom, maxZoom);
            //change scale on the image
            mapImageZoom.transform.localScale = new Vector3(ZoomLevel, ZoomLevel, 1);
            _calcVisibleImageDims();

            // Move the pins based on map zoom
            for (int i = 0; i < pins.Count; i++)
            {
                pins[i].transform.position = _convertCoord2MousePos(pinPositions[i]);
            }
            for (int i = 0; i < locationPins.Count; i++)
            {
                locationPins[i].transform.position = _convertCoord2MousePos(locationPinPositions[i]);
            }

            ZoomLevel = Mathf.Clamp(ZoomLevel, minZoom, maxZoom);
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
            Vector2 uipos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector3 coord = _convertMousePos2Coord(uipos);

            //Set text to the location
            TextMeshProUGUI locationText = emptySideMenuNoSelection.transform.Find("LocationText").GetComponent<TextMeshProUGUI>();
            locationText.text = "Location: (" + coord.x + ", " + coord.y + ", " + coord.z + ")";
            Vector4 visibleDims = _calcVisibleImageDims();
            TextMeshProUGUI DescriptionText = emptySideMenuNoSelection.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
            DescriptionText.text = "Visible Dims: (" + visibleDims.x + ", " + visibleDims.y + ", " + visibleDims.z + ", " + visibleDims.w + ")";
            DescriptionText.text += "\n" + "Zoom Level: " + ZoomLevel;
            Vector2 centerPoint = _calcCenterpoint(visibleDims);
            DescriptionText.text += "\n" + "Center Point: (" + centerPoint.x + ", " + centerPoint.y + ")";
            Vector2 visibleSize = _calcVisibleSize(visibleDims);
            DescriptionText.text += "\n" + "Visible Size: (" + visibleSize.x + ", " + visibleSize.y + ")";
            DescriptionText.text += "\n" + "UIPos as a percentage: (" + uipos.x / Screen.width + ", " + uipos.y / Screen.height + ")";
            Vector2 coordActualLocation = _convertMousePos2Coord(_convertMousePos2Coord(uipos));
            DescriptionText.text += "\n" + "Coord Actual Location: (" + coordActualLocation.x + ", " + coordActualLocation.y + ")";


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
