using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.EventSystems;
using TigerForge;
using TMPro;
namespace SpatialNotes
{
    public class UIHelp : MonoBehaviour
    {
        public GameObject helpPanel;
        // Start is called before the first frame update
        void Start()
        {
            helpPanel = this.gameObject.transform.Find("PanelHelp").gameObject;
            helpPanel.SetActive(false);

        }

        void Show()
        {
            helpPanel.SetActive(true);
            EventManager.SetData("CURSOR_NAME", "QUESTION");
            EventManager.EmitEvent("CURSOR_REFRESH");

        }

        void Hide()
        {
            helpPanel.SetActive(false);
            EventManager.SetData("CURSOR_NAME", "NORMAL");
            EventManager.EmitEvent("CURSOR_REFRESH");
        }

        public void Toggle()
        {
            if (helpPanel.activeSelf)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }
}