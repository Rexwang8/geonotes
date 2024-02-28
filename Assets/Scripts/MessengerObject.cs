using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System.Linq;
using UnityEditor;
using TigerForge;


namespace SpatialNotes
{
    public class MessengerObject : MonoBehaviour
    {
        public string currentScene = "MainMenuScene";
        public string message = "Hello World";
        // Start is called before the first frame update
        void Start()
        {
            //if already exists, destroy this object and update the message in the existing object
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Messenger");
            if (objs.Length > 1)
            {
                foreach (GameObject obj in objs)
                {
                    if (obj.GetComponent<MessengerObject>().message != message)
                    {
                        obj.GetComponent<MessengerObject>().setMessenger(message);
                    }
                    else
                    {
                        Destroy(this.gameObject);
                    }
                }
            }
            //get the current scene
            currentScene = SceneManager.GetActiveScene().name;
            //set do not destroy on load
            DontDestroyOnLoad(this.gameObject);
        }

        void OnLevelWasLoaded(int level)
        {
            //get the current scene
            currentScene = SceneManager.GetActiveScene().name;
            if (currentScene != "MainMenuScene")
            {
                // Raise the event
                RaiseEvent();
            }
        }

        void setMessenger(string _message)
        {
            message = _message;
        }

        // Raise event
        void RaiseEvent()
        {
            EventManager.SetData("MAP_TO_LOAD", message);
            EventManager.EmitEvent("LOAD_MAP");
        }


    }
}