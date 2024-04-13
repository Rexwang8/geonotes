using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor_Main : MonoBehaviour
{
    // Start is called before the first frame update
    public Texture2D cursorFire;
    public Texture2D cursorArrow;
    void Start()
    {
        Cursor.SetCursor(cursorFire, Vector2.zero, CursorMode.ForceSoftware);
    }

    void OnMouseEnter()
    {
         Cursor.SetCursor(cursorArrow, Vector2.zero, CursorMode.ForceSoftware);
    }


}
