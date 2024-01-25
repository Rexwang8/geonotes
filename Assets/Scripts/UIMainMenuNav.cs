using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenuNav : MonoBehaviour
{
    // Map Select panel
    public GameObject mapSelect;

    // Options panel
    public GameObject options;


    // Button click events for main menu

    // Play button
    public void OnClickPlay()
    {
        setObjectDisabled(options);
        setObjectEnabled(mapSelect);
    }

    // Options button
    public void OnClickOptions()
    {
        setObjectDisabled(mapSelect);
        setObjectEnabled(options);
    }

    // Back button (map select and options)
    public void OnClickBack()
    {
        setObjectDisabled(mapSelect);
        setObjectDisabled(options);
    }

    // Quit button
    public void OnClickQuit()
    {
        Application.Quit();
    }


    // Helpers

    // Toggle object
    private void toggleObject(GameObject obj)
    {
        if (obj)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }

    // Set object to disabled
    private void setObjectDisabled(GameObject obj)
    {
        if (obj)
        {
            obj.SetActive(false);
        }
    }


    // Set object to enabled
    private void setObjectEnabled(GameObject obj)
    {
        if (obj)
        {
            obj.SetActive(true);
        }
    }
}
