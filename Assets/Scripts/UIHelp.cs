using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
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
        }

        void Hide()
        {
            helpPanel.SetActive(false);
        }

        public void Toggle()
        {
            helpPanel.SetActive(!helpPanel.activeSelf);
        }
    }
}