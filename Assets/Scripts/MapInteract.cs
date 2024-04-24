using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TigerForge;
using TMPro;
using SimpleFileBrowser;
using System.Linq;
using System.IO;

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
        private GameObject sideMenuEditLocation;
        private GameObject sideMenuShowLocationContent;
        private GameObject sideMenuCreateLocationButtonAdd;
        private GameObject sideMenuCreateLocationButtonCancel;
        private GameObject sideMenuCreateLocationNameField;
        private GameObject sideMenuCreateLocationDescriptionField;
        private GameObject sideMenuCreateLocationIsImageSelected;
        private GameObject showMorePanel;
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

        public GameObject zoomScrollBar;
        private GameObject staticCanvas;

        public float ZoomLevel = 1.0f;
        public float maxZoom = 50.0f;
        public float startingZoomPercent = 0.25f;
        private float currentZoom = 0.0f;
        public float minZoom = 0.0f;
        public float zoomSpeed = 2.0f;
        public float panSpeed = 1.3f;

        private bool _explorerActive = false;

        private GameObject _imageToCreateWith;
        private string _pathToImageBeingCreated = "";

        // Default path to open file explorer
        private string _defaultPath;
        private string[] _foundPaths;

        private enum mouseButton { Left, Right, Middle };

        //Variables needed for drag
        private Vector3 dragOrigin;
        private Vector3 worldSpaceOrigin;
        private bool dragging = false;
        private Vector3 dragChange;

        private Vector3 lastSelectedLocation;
        private Vector3 lastCandidateUiPos;

        //placeholder image for location if none
        public Sprite placeholderImage;

        public LocationInfo _selectedLocationInfo; // The location info of the selected location

        // ----------------- Variables for postcard -----------------
        public GameObject postcardScrollView;
        private GameObject _postcardScrollViewContent;
        private GameObject _postcardAddPostButton;

        public GameObject prefabTextPost;
        public GameObject prefabImagePost;
        private GameObject _postCardContentSection;

        public GameObject makepostcardMenu;
        private GameObject _makePostCartMenuCancel;
        private GameObject _makePostCartMenuSubmit;
        private GameObject _makePostCartMenuTitle;
        private GameObject _makePostCartMenuAddTextContent;
        private GameObject _makePostCartMenuAddImageContent;
        private GameObject _makePostCartMenuContentHolder;

        public TextMediaPost _currentTextMediaPost;

        public GameObject prefabPostCard;

        public GameObject prefabTextPostSubmitted;
        public GameObject prefabImagePostSubmitted;


        // ----------------- Methods -----------------
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
            sideMenuEditLocation = sideMenu.transform.Find("SideMenuEditLocation").gameObject;
            //get content child inside viewport of scroll view of sidemenushowlocation
            sideMenuShowLocationContent = sideMenuShowLocation.transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content").gameObject;
            showMorePanel = sideMenuShowLocationContent.transform.Find("LocationImage").gameObject.transform.Find("ShowMore").gameObject.transform.Find("Panel").gameObject;

            // Get the location button
            sideMenuCreateLocationButtonAdd = sideMenuCreateLocation.transform.Find("Add").gameObject;
            sideMenuCreateLocationButtonCancel = sideMenuCreateLocation.transform.Find("Cancel").gameObject;
            sideMenuCreateLocationDescriptionField = sideMenuCreateLocation.transform.Find("Description").gameObject;
            sideMenuCreateLocationNameField = sideMenuCreateLocation.transform.Find("Name").gameObject;
            sideMenuCreateLocationIsImageSelected = sideMenuCreateLocation.transform.Find("IsImage").gameObject;
            _imageToCreateWith = sideMenuCreateLocation.transform.Find("DisplayIMG").gameObject;
            sideMenuNoSelectAddLocButton = emptySideMenuNoSelection.transform.Find("AddLocButton").gameObject;
            _postcardScrollViewContent = postcardScrollView.transform.Find("Viewport").gameObject;
            _postcardScrollViewContent = _postcardScrollViewContent.transform.Find("Content").gameObject;
            _postCardContentSection = _postcardScrollViewContent.transform.Find("PostContent").gameObject;
            //_postCardContentSection = _postCardContentSection.transform.Find("Viewport").gameObject;
            //_postCardContentSection = _postCardContentSection.transform.Find("Content").gameObject;
            _postcardAddPostButton = _postcardScrollViewContent.transform.Find("AddPostSection").gameObject;
            _postcardAddPostButton = _postcardAddPostButton.transform.Find("AddPostCard").gameObject;
            _makePostCartMenuCancel = makepostcardMenu.transform.Find("Trash").gameObject;
            _makePostCartMenuSubmit = makepostcardMenu.transform.Find("Post").gameObject;
            _makePostCartMenuTitle = makepostcardMenu.transform.Find("Subject").gameObject;
            _makePostCartMenuContentHolder = makepostcardMenu.transform.Find("Scroll View").gameObject;
            _makePostCartMenuContentHolder = _makePostCartMenuContentHolder.transform.Find("Viewport").gameObject;
            _makePostCartMenuContentHolder = _makePostCartMenuContentHolder.transform.Find("Content").gameObject;
            _makePostCartMenuAddImageContent = makepostcardMenu.transform.Find("AddImage").gameObject;
            _makePostCartMenuAddTextContent = makepostcardMenu.transform.Find("AddTextbox").gameObject;

            _imageToCreateWith.SetActive(false);

            // Add button listeners
            sideMenuCreateLocationButtonAdd.GetComponent<Button>().onClick.AddListener(LocationAdd);
            sideMenuCreateLocationButtonCancel.GetComponent<Button>().onClick.AddListener(LocationCancel);
            sideMenuNoSelectAddLocButton.GetComponent<Button>().onClick.AddListener(_existingMenuAddLocButton);
            sideMenuEditLocation.transform.Find("Submit").GetComponent<Button>().onClick.AddListener(SubmitEditLocation);
            sideMenuEditLocation.transform.Find("Cancel").GetComponent<Button>().onClick.AddListener(CancelEditLocation);
            _postcardAddPostButton.GetComponent<Button>().onClick.AddListener(OpenPostcardMenu);
            _makePostCartMenuCancel.GetComponent<Button>().onClick.AddListener(ClosepostcardMenu);
            _makePostCartMenuSubmit.GetComponent<Button>().onClick.AddListener(SubmitPostcard);
            _makePostCartMenuAddTextContent.GetComponent<Button>().onClick.AddListener(AddTextContent);
            _makePostCartMenuAddImageContent.GetComponent<Button>().onClick.AddListener(AddImageContent);



            // Hide the side menu

            showMorePanel.SetActive(false);

            _removeAllPins(pinsFolder);
            _hideSideMenu();

            //load locations
            OnStartLoadLocations();

            Debug.Log("MapInteract Start");
        }

        void Update()
        {
            bool _IsGamePaused = EventManager.GetBool("GAME_PAUSED");
            if (_IsGamePaused)
            {
                return;
            }
            bool IsInMenu = _getSideMenUIsActive();


            //Handle zoom in and out WIP
            if (!_explorerActive && !IsInMenu)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
                {
                    _zoomIn();
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
                {
                    _zoomOut();
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



            //set cursor to normal if in menu and if right click is not held
            if (IsInMenu && EventManager.GetString("CURSOR_NAME") != "NORMAL" && !Input.GetMouseButton((int)mouseButton.Right))
            {
                EventManager.SetData("CURSOR_NAME", "NORMAL");
                EventManager.EmitEvent("CURSOR_REFRESH");
                return;
            }
            else if (EventManager.GetString("CURSOR_NAME") != "CIRCLE" && Input.GetMouseButton((int)mouseButton.Right))
            {
                EventManager.SetData("CURSOR_NAME", "CIRCLE");
                EventManager.EmitEvent("CURSOR_REFRESH");
            }
            //is in menu but not right click held
            if (IsInMenu)
            {
                return;
            }

            // move right click menu to mouse position
            if (Input.GetMouseButtonUp((int)mouseButton.Right))
            {

                _removeAllPins(pinsFolder);
                //Instantiate Pin
                lastCandidateUiPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
                lastSelectedLocation = lastCandidateUiPos;
                Vector3 worldClickPos = _getWorldClickPosition(lastSelectedLocation);



                GameObject clickedPin = checkIfLocationPinClicked(worldClickPos);
                if (clickedPin != null)
                {
                    Debug.Log("Clicked on location pin: " + clickedPin.transform.Find("PinName").GetComponent<TextMeshProUGUI>().text);
                    _selectedLocationInfo = clickedPin.GetComponent<PinID>().locationInfo;
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
                    id.locationInfo = new LocationInfo("Selected Location", "Selected Location", worldClickPos, "");
                    id.type = "Selected";

                    _HideAllSideMenuBranches();
                    _showSideMenuNoSelection();
                }
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
            if (dragging && !_explorerActive)
            {
                //change cursor if not already changed
                if (EventManager.GetString("CURSOR_NAME") != "DRAG")
                {
                    EventManager.SetData("CURSOR_NAME", "DRAG");
                    EventManager.EmitEvent("CURSOR_REFRESH");
                }

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
            else
            {
                if (EventManager.GetString("CURSOR_NAME") == "DRAG")
                {
                    EventManager.SetData("CURSOR_NAME", "NORMAL");
                    EventManager.EmitEvent("CURSOR_REFRESH");
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
                loc.Value.coordinate = coord;
                PinID id = pin.GetComponent<PinID>();
                id.locationInfo = loc.Value;
                id.type = "Location";
                Debug.Log("Location info: " + loc.Value.locationName + " " + loc.Value.description + " " + loc.Value.coordinate + " " + loc.Value.imagePath);

                //pins of locations are green
                pin.GetComponent<Image>().color = Color.green;

            }

        }

        private void _existingMenuAddLocButton()
        {
            _showSideMenuCreateLocation();
        }

        // upload image from file explorer for location
        public void OnClickUploadImage()
        {
            _openFileExplorerToLoadImages();
        }

        // Helper to open the file explorer to pick only images
        private void _openFileExplorerToLoadImages()
        {
            if (_explorerActive)
            {
                Debug.Log("Explorer already active");
                TriggerPopup("Explorer already active", "Explorer Active", "xmark");
                return;
            }
            //open in maps directory, if it exists, else create it
            string path = Application.streamingAssetsPath + "/Maps";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            _setFileBrowserFilterImagesOnly();

            // Show a save file dialog, await response from dialog
            StartCoroutine(ShowLoadDialogCoroutineImagesOnly());

            return;
        }

        // Filter for only images
        private void _setFileBrowserFilterImagesOnly()
        {
            //only allow directories to be selected
            FileBrowser.DisplayedEntriesFilter += (entry) =>
            {
                if (entry.IsDirectory)
                    return true; // Don't filter folders


                string extension = Path.GetExtension(entry.Path).ToLower();
                if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".bmp" || extension == ".tiff")
                {
                    return true;
                }


                return false; // Filter files
            };

        }

        // Coroutine to show the load file dialog for images
        IEnumerator ShowLoadDialogCoroutineImagesOnly()
        {
            _explorerActive = true;
            // Directories only
            yield return FileBrowser.WaitForLoadDialog(SimpleFileBrowser.FileBrowser.PickMode.Files, true, _defaultPath, null, "Select Map Folder", "Load");
            if (FileBrowser.Success)
            {
                OnFolderSelectedLoadFileImages(FileBrowser.Result);
            }
            else
            {
                _explorerActive = false;
                try
                {
                    string[] error = FileBrowser.Result;
                    string errorString = string.Join(", ", error);
                    Debug.Log("Error: " + errorString);
                    TriggerPopup("Error: " + errorString, "Error", "xmark");
                }
                catch (System.Exception e)
                {
                    Debug.Log("Error: " + e.Message);

                }
            }
            _explorerActive = false;
        }

        private void _openFileExplorerToLoadImages_PostContent(string uuid)
        {
            if (_explorerActive)
            {
                Debug.Log("Explorer already active");
                TriggerPopup("Explorer already active", "Explorer Active", "xmark");
                return;
            }
            //open in maps directory, if it exists, else create it
            string path = Application.streamingAssetsPath + "/Maps";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            _setFileBrowserFilterImagesOnly();

            // Show a save file dialog, await response from dialog
            StartCoroutine(ShowLoadDialogCoroutineImagesOnly_PostContent(uuid));

            return;
        }

        IEnumerator ShowLoadDialogCoroutineImagesOnly_PostContent(string uuid)
        {
            _explorerActive = true;
            // Directories only
            yield return FileBrowser.WaitForLoadDialog(SimpleFileBrowser.FileBrowser.PickMode.Files, true, _defaultPath, null, "Select Image", "Load");
            if (FileBrowser.Success)
            {
                OnFolderSelectedLoadFileImages_PostContent(FileBrowser.Result, uuid);
            }
            else
            {
                _explorerActive = false;
                try
                {
                    string[] error = FileBrowser.Result;
                    string errorString = string.Join(", ", error);
                    Debug.Log("Error: " + errorString);
                    TriggerPopup("Error: " + errorString, "Error", "xmark");
                }
                catch (System.Exception e)
                {
                    Debug.Log("Error: " + e.Message);

                }
            }
            _explorerActive = false;
        }

        // Logs the paths of the selected files
        void OnFolderSelectedLoadFileImages_PostContent(string[] paths, string uuid)
        {
            if (paths.Length == 0)
            {
                Debug.Log("No files were selected");
                TriggerPopup("No files were selected", "No Files Selected", "xmark");
                return;
            }

            // select only one 
            string selectedPath = paths[0];

            //move image file to map directory and change path to new path
            string newImagePath = Application.streamingAssetsPath + map.mapAssetsPath + "/" + Path.GetFileName(selectedPath);
            File.Copy(selectedPath, newImagePath, true);
            
            selectedPath = newImagePath;

            //get postcard menu image from uuid
            GameObject postcardImage = _makePostCartMenuContentHolder.transform.Find(uuid).gameObject;

            //Try to load the image
            Texture2D texture = new Texture2D(2, 2);
            byte[] fileData;
            if (File.Exists(selectedPath))
            {
                fileData = File.ReadAllBytes(selectedPath);
                texture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                postcardImage.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.Log("File does not exist: " + selectedPath);
                TriggerPopup("File does not exist: " + selectedPath, "File Not Found", "xmark");
                _explorerActive = false;
                return;
            }

            // get placeholder tmp_text
            GameObject tmp_text = postcardImage.transform.Find("image_path").gameObject;
            tmp_text.GetComponent<TMP_Text>().text = selectedPath;
            //hide add image button
            GameObject addImageButton = postcardImage.transform.Find("AddImage").gameObject.transform.Find("Upload").gameObject;
            addImageButton.SetActive(false);


            // Trigger popup
            TriggerPopup(text: "Image " + Path.GetFileName(selectedPath) + " loaded", title: "Image Loaded", imageName: "checkmark");

        }

        // Logs the paths of the selected files
        void OnFolderSelectedLoadFileImages(string[] paths)
        {
            if (paths.Length == 0)
            {
                Debug.Log("No files were selected");
                TriggerPopup("No files were selected", "No Files Selected", "xmark");
                return;
            }

            // select only one 
            string selectedPath = paths[0];

            //move image file to map directory and change path to new path
            string newImagePath = Application.streamingAssetsPath + map.mapAssetsPath + "/" + Path.GetFileName(selectedPath);
            File.Copy(selectedPath, newImagePath, true);
            _pathToImageBeingCreated = newImagePath;
            selectedPath = newImagePath;





            //Try to load the image
            Texture2D texture = new Texture2D(2, 2);
            byte[] fileData;
            if (File.Exists(selectedPath))
            {
                fileData = File.ReadAllBytes(selectedPath);
                texture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                _imageToCreateWith.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                _pathToImageBeingCreated = selectedPath;
            }
            else
            {
                Debug.Log("File does not exist: " + selectedPath);
                TriggerPopup("File does not exist: " + selectedPath, "File Not Found", "xmark");
                _explorerActive = false;
                return;
            }

            // show image and hide placeholder
            _imageToCreateWith.SetActive(true);
            sideMenuCreateLocationIsImageSelected.SetActive(false);




            // Trigger popup
            TriggerPopup(text: "Image " + Path.GetFileName(selectedPath) + " loaded", title: "Image Loaded", imageName: "checkmark");

        }


        public void LocationAdd()
        {
            Debug.Log("Location Added");
            Vector3 coord = _getWorldClickPosition(lastSelectedLocation);
            string locationName = sideMenuCreateLocationNameField.GetComponent<TMP_InputField>().text;
            string locationDescription = sideMenuCreateLocationDescriptionField.GetComponent<TMP_InputField>().text;
            Debug.Log(savedUiPosOnClick + " " + coord + " " + locationName + " " + locationDescription);

            string locationImagePath = "";
            if (_pathToImageBeingCreated != "")
            {
                locationImagePath = _pathToImageBeingCreated;
            }
            Debug.Log("Location Image Path: " + locationImagePath);
            //
            //Add location to the database
            map.AddLocation(locCoord: coord, locName: locationName, locDescription: locationDescription, locImagePath: locationImagePath);
            Debug.Log("Location Added to the database, new location count: " + map.GetNumberOfLocations());

            //add pin to the map
            GameObject pin = Instantiate(pinPrefab, new Vector3(coord.x, coord.y, 1), Quaternion.identity);
            pin.transform.SetParent(locationPinsFolder.transform);
            //Populate pin name
            GameObject pinName = pin.transform.Find("PinName").gameObject;
            pinName.GetComponent<TextMeshProUGUI>().text = locationName;

            //populate pinID
            PinID id = pin.GetComponent<PinID>();
            id.locationInfo = new LocationInfo(locationName, locationDescription, coord, locationImagePath);
            id.type = "Location";

            //pins of locations are green
            pin.GetComponent<Image>().color = Color.green;

            //call for save to file
            map.SaveAll();

            //clear the fields for the next location
            sideMenuCreateLocationNameField.GetComponent<TMP_InputField>().text = "";
            sideMenuCreateLocationDescriptionField.GetComponent<TMP_InputField>().text = "";
            _pathToImageBeingCreated = "";
            //hide the image
            _imageToCreateWith.SetActive(false);
            sideMenuCreateLocationIsImageSelected.SetActive(true);

            //remove temporary pins
            _removeAllPins(pinsFolder);


            //Close the add location canvas
            _hideSideMenu();
        }
        public void LocationCancel()
        {
            _hideSideMenu();

            //clear the fields for the next location
            sideMenuCreateLocationNameField.GetComponent<TMP_InputField>().text = "";
            sideMenuCreateLocationDescriptionField.GetComponent<TMP_InputField>().text = "";
            _pathToImageBeingCreated = "";
            _imageToCreateWith.SetActive(false);
            sideMenuCreateLocationIsImageSelected.SetActive(true);
        }

        private bool _getSideMenUIsActive()
        {
            return sideMenuCreateLocation.activeInHierarchy || sideMenuEditLocation.activeInHierarchy || makepostcardMenu.activeInHierarchy || sideMenuShowLocation.activeInHierarchy;
        }

        private void _hideSideMenu()
        {
            _HideAllSideMenuBranches();
            sideMenu.SetActive(false);
        }

        private void _HideAllSideMenuBranches()
        {
            emptySideMenuNoSelection.SetActive(false);
            sideMenuCreateLocation.SetActive(false);
            sideMenuEditLocation.SetActive(false);
            ClosepostcardMenu();
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
            _drawAndUpdateSideBarForLocation();



            //Set text to the location
            LocationInfo locInfo = locationPin.GetComponent<PinID>().locationInfo;
            TextMeshProUGUI locationText = sideMenuShowLocationContent.transform.Find("LocationText").GetComponent<TextMeshProUGUI>();
            locationText.text = locInfo.locationName;
            TextMeshProUGUI CoordinatesText = sideMenuShowLocationContent.transform.Find("CoordinateText").GetComponent<TextMeshProUGUI>();
            CoordinatesText.text = "Coordinates: (" + locInfo.coordinate.x + ", " + locInfo.coordinate.y + ")";
            TextMeshProUGUI DescriptionText = sideMenuShowLocationContent.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
            DescriptionText.text = "Description: " + locInfo.description;

            //show image if it exists else show placeholder
            GameObject displayIMG = sideMenuShowLocationContent.transform.Find("LocationImage").gameObject;
            if (locInfo.imagePath != "" && locInfo.imagePath != null)
            {
                Texture2D texture = new Texture2D(2, 2);
                byte[] fileData;
                if (File.Exists(locInfo.imagePath))
                {
                    fileData = File.ReadAllBytes(locInfo.imagePath);
                    texture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                    displayIMG.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
                else
                {
                    Debug.Log("File does not exist: " + locInfo.imagePath);
                    TriggerPopup("File does not exist: " + locInfo.imagePath, "File Not Found", "xmark");
                }
            }
            else
            {
                displayIMG.GetComponent<UnityEngine.UI.Image>().sprite = placeholderImage;
            }

            sideMenuShowLocation.SetActive(true);
        }

        public void DeleteLocation()
        {
            //delete a single location
            Debug.Log("Delete Location" + _selectedLocationInfo.locationName);
            map.DeleteLocation(_selectedLocationInfo.coordinate);

            //call for save to file
            map.SaveAll();

            //remove the pin
            _removeAllPins(locationPinsFolder);
            //reload the locations
            OnStartLoadLocations();
            //hide the side menu
            _hideSideMenu();
            _HideAllSideMenuBranches();
            CloseSideExistingMenu();




        }
        private void _hideSideMenuShowLocation()
        {
            sideMenuShowLocation.SetActive(false);
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


        /*
        Side Menu Functions
        */

        // Start is called before the first frame update

        public void Toggle()
        {
            //If the side menu is active
            if (!showMorePanel.activeSelf)
            {
                //Hide the side menu
                showMorePanel.SetActive(true);
            }
        }

        public void CloseShowMore()
        {
            //If the side menu is active
            if (showMorePanel.activeSelf)
            {
                //Hide the side menu
                showMorePanel.SetActive(false);
            }
        }

        public void EditLocation()
        {
            // show the edit location menu
            Debug.Log("Edit Location");
            _HideAllSideMenuBranches();
            sideMenuEditLocation.SetActive(true);

            // define gameobjects to change
            GameObject nameField = sideMenuEditLocation.transform.Find("Name").gameObject;
            GameObject descriptionField = sideMenuEditLocation.transform.Find("Description").gameObject;
            GameObject imageField = sideMenuEditLocation.transform.Find("DisplayIMG").gameObject;
            //GameObject isImageSelected = sideMenuEditLocation.transform.Find("IsImage").gameObject;

            // fill in the fields
            nameField.GetComponent<TMP_InputField>().text = _selectedLocationInfo.locationName;
            descriptionField.GetComponent<TMP_InputField>().text = _selectedLocationInfo.description;
            if (_selectedLocationInfo.imagePath != "" && _selectedLocationInfo.imagePath != null)
            {
                Texture2D texture = new Texture2D(2, 2);
                byte[] fileData;
                if (File.Exists(_selectedLocationInfo.imagePath))
                {
                    fileData = File.ReadAllBytes(_selectedLocationInfo.imagePath);
                    texture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                    imageField.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
                else
                {
                    Debug.Log("File does not exist: " + _selectedLocationInfo.imagePath);
                    TriggerPopup("File does not exist: " + _selectedLocationInfo.imagePath, "File Not Found", "xmark");
                }
            }
            else
            {
                imageField.GetComponent<UnityEngine.UI.Image>().sprite = placeholderImage;
            }

            _imageToCreateWith = imageField;
            _pathToImageBeingCreated = _selectedLocationInfo.imagePath;
            imageField.SetActive(true);





        }

        public void SubmitEditLocation()
        {
            Debug.Log("Submit Edit Location");
            // get the fields
            GameObject nameField = sideMenuEditLocation.transform.Find("Name").gameObject;
            GameObject descriptionField = sideMenuEditLocation.transform.Find("Description").gameObject;
            GameObject imageField = sideMenuEditLocation.transform.Find("DisplayIMG").gameObject;

            // get the values
            string locationName = nameField.GetComponent<TMP_InputField>().text;
            string locationDescription = descriptionField.GetComponent<TMP_InputField>().text;
            string locationImagePath = _selectedLocationInfo.imagePath;
            if (_pathToImageBeingCreated != "")
            {
                locationImagePath = _pathToImageBeingCreated;
                Debug.Log("New Image Path: " + locationImagePath);
            }


            // update the location
            map.UpdateLocation(_selectedLocationInfo.coordinate, locationName, locationDescription, locationImagePath);
            Debug.Log("Location Updated");

            // update the pin
            foreach (Transform child in locationPinsFolder.transform)
            {
                if (child.GetComponent<PinID>().locationInfo == _selectedLocationInfo)
                {
                    // update the pin
                    GameObject pinName = child.transform.Find("PinName").gameObject;
                    pinName.GetComponent<TextMeshProUGUI>().text = locationName;
                    //populate pinID
                    PinID id = child.GetComponent<PinID>();
                    id.locationInfo = new LocationInfo(locationName, locationDescription, _selectedLocationInfo.coordinate, locationImagePath);
                    id.type = "Location";
                    //pins of locations are green
                    child.GetComponent<Image>().color = Color.green;
                    break;
                }
            }

            //call for save to file
            map.SaveAll();

            //clear the fields for the next location
            nameField.GetComponent<TMP_InputField>().text = "";
            descriptionField.GetComponent<TMP_InputField>().text = "";
            _pathToImageBeingCreated = "";
            //hide the image
            imageField.SetActive(false);

            //remove temporary pins
            _removeAllPins(pinsFolder);

            //Close the add location canvas
            _hideSideMenu();

            //hide show more panel
            CloseSideExistingMenu();
        }

        public void CancelEditLocation()
        {
            Debug.Log("Cancel Edit Location");
            //hide show more panel
            CloseSideExistingMenu();
            _HideAllSideMenuBranches();
            _hideSideMenu();
        }

        public void FavoriteLocation()
        {
            Debug.Log("Favorite Location");
        }

        public void CloseSideExistingMenu()
        {
            showMorePanel.SetActive(false);
            sideMenuShowLocation.SetActive(false);
            ClosepostcardMenu();
        }

        public void OpenPostcardMenu()
        {
            Debug.Log("Open Postcard Menu");
            _showPostcardMenu();
            _drawAndUpdateSideBarForLocation();
        }

        public void ClosepostcardMenu()
        {
            Debug.Log("Close Postcard Menu");

            _clearPostcardMenu();
            _clearPostcardPosts();
            _hidePostcardMenu();
            _conditionalClearPostcardPosts();
        }
        private void _clearPostcardPosts()
        {
            //clear the postcard menu in janky way by checking name length
            if (_postcardScrollViewContent.transform.childCount > 7)
            {
                //if there are children named postcard, remove them
                foreach (Transform child in _postcardScrollViewContent.transform)
                {
                    //janky way to detect uuid
                    if (child.gameObject.name.Length > 18)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                }
            }
        }

        private void _clearPostcardMenu()
        {
            //clear the title
            _makePostCartMenuTitle.GetComponent<TMP_InputField>().text = "";
        }
        private void _conditionalClearPostcardPosts()
        {
            //clear the postcard menu
            if (_selectedLocationInfo.postCard == null || _selectedLocationInfo.postCard.posts == null || _selectedLocationInfo.postCard.posts.Count == 0)
            {
                if (_postcardScrollViewContent.transform.childCount > 7)
                {
                    //if there are children named postcard, remove them
                    foreach (Transform child in _postcardScrollViewContent.transform)
                    {
                        //janky way to detect uuid
                        if (child.gameObject.name.Length > 18)
                        {
                           _clearPostcardMenu();
                            GameObject.Destroy(child.gameObject);
                        }
                    }
                }
            }
        }

        private void _showPostcardMenu()
        {
            //show the postcard menu
            makepostcardMenu.SetActive(true);
        }

        private void _hidePostcardMenu()
        {
            //hide the postcard menu
            makepostcardMenu.SetActive(false);
        }

        public void SubmitPostcard()
        {
            //create temp postcard
            TextMediaPost _tempPost = new TextMediaPost();
            
            string title = _makePostCartMenuTitle.GetComponent<TMP_InputField>().text;
            //update current postcard
            for (int i = 0; i < _currentTextMediaPost.mediaComponents.Count; i++)
            {
                if (_currentTextMediaPost.mediaComponents[i].mediaType == "Text")
                {
                    TextComponent textComp = new TextComponent(_currentTextMediaPost.mediaComponents[i]);
                    GameObject textPostGameObject = GameObject.Find(textComp.uuid);
                    textComp.textContent = textPostGameObject.transform.GetChild(0).GetComponent<TMP_InputField>().text;
                    _tempPost.mediaComponents.Add(textComp);


                }
                else if (_currentTextMediaPost.mediaComponents[i].mediaType == "Image")
                {
                    ImageComponent imgComp = new ImageComponent(_currentTextMediaPost.mediaComponents[i]);
                    GameObject imgPostGameObject = GameObject.Find(imgComp.uuid);
                    imgComp.mediaPath = imgPostGameObject.transform.GetChild(2).GetComponent<TMP_Text>().text;
                    _tempPost.mediaComponents.Add(imgComp);
                }
            }
            
            _currentTextMediaPost.title = title;
            _tempPost.title = title;
            if (_currentTextMediaPost.title == "")
            {
                _currentTextMediaPost.title = "No Title";
            }
            _currentTextMediaPost.updateDate(System.DateTime.Now);
            _tempPost.updateDate(System.DateTime.Now);


            //get postcard that is selected
            LocationInfo locInfo = _selectedLocationInfo;
            if (locInfo == null)
            {
                Debug.Log("No location selected");
                return;
            }
            if (locInfo.postCard == null || locInfo.postCard.posts == null || locInfo.postCard.posts.Count == 0)
            {
                locInfo.postCard = new PostCard(_date: System.DateTime.Now);
            }
            else
            {
                locInfo.postCard.date = System.DateTime.Now;
            }
            locInfo.postCard.posts.Add(_tempPost);
            //call for save to file
            map.SaveAll();
            Debug.Log("Postcard submitted");
            //Reset the new postcard
            _currentTextMediaPost = new TextMediaPost();

            //clear the postcard menu
            _clearPostcardMenu();
            _hidePostcardMenu();
            _removeAllPostsFromEdit();

            //call for update to postcard
            _drawAndUpdateSideBarForLocation();
        }

        private void _drawAndUpdateSideBarForLocation()
        {
            //Clear the postcard menu
            _clearPostcardPosts();
            
            //get the location info
            LocationInfo locInfo = _selectedLocationInfo;
            if (locInfo == null)
            {
                Debug.Log("No location selected");
                return;
            }
            if (locInfo.postCard == null || locInfo.postCard.posts == null || locInfo.postCard.posts.Count == 0)
            {
                Debug.Log("No postcards for location");
                return;
            }
            Debug.Log("Drawing Postcard for location: " + locInfo.locationName + " with " + locInfo.postCard.posts.Count + " postcards");

            //populate the side bar
            //Set text to the location
            TextMeshProUGUI locationText = sideMenuShowLocationContent.transform.Find("LocationText").GetComponent<TextMeshProUGUI>();
            locationText.text = locInfo.locationName;
            TextMeshProUGUI CoordinatesText = sideMenuShowLocationContent.transform.Find("CoordinateText").GetComponent<TextMeshProUGUI>();
            CoordinatesText.text = "Coordinates: (" + locInfo.coordinate.x + ", " + locInfo.coordinate.y + ")";
            TextMeshProUGUI DescriptionText = sideMenuShowLocationContent.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
            DescriptionText.text = "Description: " + locInfo.description;

            //show image if it exists else show placeholder
            GameObject displayIMG = sideMenuShowLocationContent.transform.Find("LocationImage").gameObject;
            if (locInfo.imagePath != "" && locInfo.imagePath != null)
            {
                Texture2D texture = new Texture2D(2, 2);
                byte[] fileData;
                if (File.Exists(locInfo.imagePath))
                {
                    fileData = File.ReadAllBytes(locInfo.imagePath);
                    texture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                    displayIMG.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
                else
                {
                    Debug.Log("File does not exist: " + locInfo.imagePath);
                    TriggerPopup("File does not exist: " + locInfo.imagePath, "File Not Found", "xmark");
                }
            }
            else
            {
                displayIMG.GetComponent<UnityEngine.UI.Image>().sprite = placeholderImage;
            }

            sideMenuShowLocation.SetActive(true);

            //if there are postcards, draw them
            if (locInfo.postCard != null && locInfo.postCard.posts != null)
            {
                //for each post in the postcard
                Debug.Log("Drawing Postcard for location: " + locInfo.locationName + " with " + locInfo.postCard.posts.Count + " postcards");
                foreach (TextMediaPost post in locInfo.postCard.posts)
                {
                    //WORKAROUND - create individual posts only, no postcards

                    foreach (_postMediaComponent comp in post.mediaComponents)
                    {
                        if (comp.mediaType == "Text")
                        {
                            TextComponent textComp = new TextComponent(comp);
                            //create the text content
                            GameObject textPostGameObject = Instantiate(prefabTextPostSubmitted, new Vector3(0, 0, 0), Quaternion.identity);
                            textPostGameObject.name = textComp.uuid;
                            textPostGameObject.transform.SetParent(_postcardScrollViewContent.transform);
                            textPostGameObject.transform.localScale = new Vector3(1, 1, 1);
                            GameObject posttxt = textPostGameObject.transform.GetChild(1).gameObject;
                            string poststring = textComp.textContent.Trim();
                            if (poststring.Length == 0 || poststring == "")
                            {
                                poststring = "No Description!";
                            }
                            posttxt.GetComponent<TextMeshProUGUI>().text = poststring;

                            GameObject postdata = textPostGameObject.transform.GetChild(2).gameObject;
                            string postdate = post.GetDate();
                            postdata.GetComponent<TextMeshProUGUI>().text = "Posted on: " + postdate + " under: " + post.title;
                            //assign remove component button
                            GameObject rembutton = textPostGameObject.transform.GetChild(0).gameObject;
                            //rembutton.GetComponent<Button>().onClick.AddListener(() => _removePostComponent(textComp.uuid));
                        }
                        else if (comp.mediaType == "Image")
                        {
                            ImageComponent imageComp = new ImageComponent(comp);
                            Debug.Log("Image Path: " + imageComp.mediaPath);
                            //create the image content
                            GameObject imagePostGameObject = Instantiate(prefabImagePostSubmitted, new Vector3(0, 0, 0), Quaternion.identity);
                            imagePostGameObject.name = imageComp.uuid;
                            imagePostGameObject.transform.SetParent(_postcardScrollViewContent.transform);
                            imagePostGameObject.transform.localScale = new Vector3(1, 1, 1);
                            GameObject postimg = imagePostGameObject.transform.GetChild(1).gameObject;
                            //posttxt.GetComponent<TextMeshProUGUI>().text = "Image";
                            GameObject postdata = imagePostGameObject.transform.GetChild(3).gameObject;
                            string postdate = post.GetDate();
                            postdata.GetComponent<TextMeshProUGUI>().text = "Posted on: " + postdate + " under: " + post.title;

                            //Try to load the image
                            Texture2D texture = new Texture2D(2, 2);
                            byte[] fileData;
                            if (File.Exists(imageComp.mediaPath))
                            {
                                fileData = File.ReadAllBytes(imageComp.mediaPath);
                                texture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                                postimg.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                            }
                            else
                            {
                                Debug.Log("File does not exist: " + imageComp.mediaPath);
                                TriggerPopup("File does not exist: " + imageComp.mediaPath, "File Not Found", "xmark");
                            }

                            //assign remove component button
                            GameObject rembutton = imagePostGameObject.transform.GetChild(4).gameObject;
                            //rembutton.GetComponent<Button>().onClick.AddListener(() => _removePostComponent(imageComp.uuid));
                        }
                    }
                }

                //Create an empty object with no prefab
                GameObject emptyPost = new GameObject();
                emptyPost.transform.SetParent(_postcardScrollViewContent.transform);
                emptyPost.transform.localScale = new Vector3(1, 1, 1);
                emptyPost.AddComponent<RectTransform>();
                emptyPost.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                emptyPost.name = "Iamconformingtoyourjankyways:v";

            }

            //trigger layout rebuild
            Debug.Log("Rebuilding Layout for Postcard");
            _postcardScrollViewContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_postcardScrollViewContent.GetComponent<RectTransform>());

        }

        public void AddImageContent()
        {
            Debug.Log("Add Image Content");
            //create code datastructure for image content
            ImageComponent imagePost = new ImageComponent();
            imagePost.mediaType = "Image";
            imagePost.mediaPath = "Path";
            string uuid = imagePost.CreateUUID();
            _currentTextMediaPost.AddImageComponent(imagePost);

            //create the image content
            GameObject imagePostGameObject = Instantiate(prefabImagePost, new Vector3(0, 0, 0), Quaternion.identity);
            imagePostGameObject.name = uuid;
            imagePostGameObject.transform.SetParent(_makePostCartMenuContentHolder.transform);
            imagePostGameObject.transform.localScale = new Vector3(2, 2, 2);

            //assign remove component button
            GameObject rembutton = imagePostGameObject.transform.Find("Remove").gameObject;
            int index = _currentTextMediaPost.mediaComponents.Count - 1;
            rembutton.GetComponent<Button>().onClick.AddListener(() => _removePostComponent(uuid));

            //Add image logic to the image content
            // need to write a new function for this
            GameObject uploadButton = imagePostGameObject.transform.Find("AddImage").gameObject.transform.Find("Upload").gameObject;
            uploadButton.GetComponent<Button>().onClick.AddListener(() => _openFileExplorerToLoadImages_PostContent(uuid));


            LayoutRebuilder.ForceRebuildLayoutImmediate(_makePostCartMenuContentHolder.GetComponent<RectTransform>());
        }

        public void AddTextContent()
        {
            Debug.Log("Add Text Content");
            //create code datastructure for text content
            TextComponent textPost = new TextComponent();
            textPost.mediaType = "Text";
            //yyyy-MM-dd HH:mm:ss
            textPost.textContent = "Description";
            string uuid = textPost.CreateUUID();
            _currentTextMediaPost.AddTextComponent(textPost);

            //create the text content
            GameObject textPostGameObject = Instantiate(prefabTextPost, new Vector3(0, 0, 0), Quaternion.identity);
            textPostGameObject.name = uuid;
            textPostGameObject.transform.SetParent(_makePostCartMenuContentHolder.transform);
            textPostGameObject.transform.localScale = new Vector3(1, 1, 1);

            //assign remove component button
            GameObject rembutton = textPostGameObject.transform.GetChild(1).gameObject;
            int index = _currentTextMediaPost.mediaComponents.Count - 1;
            rembutton.GetComponent<Button>().onClick.AddListener(() => _removePostComponent(uuid));

            //trigger layout rebuild
            LayoutRebuilder.ForceRebuildLayoutImmediate(_makePostCartMenuContentHolder.GetComponent<RectTransform>());


        }

        private void _removePostComponent(string uuid)
        {
            GameObject component = GameObject.Find(uuid); //get the component
            //remove the component from the postcard
            _currentTextMediaPost.mediaComponents.RemoveAt(_currentTextMediaPost.mediaComponents.FindIndex(x => x.uuid == uuid));
            //remove the component from the view
            Destroy(component);

            //trigger layout rebuild
            LayoutRebuilder.ForceRebuildLayoutImmediate(_postcardScrollViewContent.GetComponent<RectTransform>());
        }
        private void _removeAllPostsFromEdit()
        {
            //clear the postcard menu
            foreach (Transform child in _makePostCartMenuContentHolder.transform)
            {
                child.gameObject.SetActive(false);
                GameObject.Destroy(child.gameObject);
            }
        }
        private void _debugsmushtextToPostcard()
        {
            string txt = "DEBUG:  ";
            //combine all the text of all the text components
            for (int i = 0; i < _currentTextMediaPost.mediaComponents.Count; i++)
            {
                if (_currentTextMediaPost.mediaComponents[i].mediaType == "Text")
                {
                    TextComponent textComp = (TextComponent)_currentTextMediaPost.mediaComponents[i];
                    txt += textComp.textContent + " ";
                }
            }

            //set the description of the location to this
            _selectedLocationInfo.description = txt;
            Debug.Log("Description: " + txt);

            //hide side menu
            _hideSideMenu();
        }
    }

}
