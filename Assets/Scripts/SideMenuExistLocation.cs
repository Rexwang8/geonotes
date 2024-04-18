using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TigerForge;
using TMPro;

namespace SpatialNotes
{
    public class SideMenuExistLocation : MonoBehaviour
    {
        public GameObject showMorePanel;
        private enum mouseButton { Left, Right, Middle };
        // Start is called before the first frame update
        void Start()
        {
            showMorePanel = this.gameObject.transform.Find("ShowMore").gameObject.transform.Find("Panel").gameObject;
            showMorePanel.SetActive(false);
        }

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

        public void DeleteLocation()
        {
            Debug.Log("Delete Location");
        }

        public void EditLocation()
        {
            Debug.Log("Edit Location");
        }

        public void FavoriteLocation()
        {
            Debug.Log("Favorite Location");
        }

        public void Close()
        {
            showMorePanel.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }
}