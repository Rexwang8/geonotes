using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TigerForge;

namespace SpatialNotes
{
    public class Cursor_Main : MonoBehaviour
    {
        // Start is called before the first frame update
        public Texture2D cursorNormal;
        public Texture2D cursorQuestionMark;
        public Texture2D cusorDrag;
        public Texture2D cursorExit;
        void _setCursor(Texture2D cursor)
        {
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
        }

        void _listenToCursorEvent()
        {
            string cursorName = EventManager.GetString("CURSOR_NAME");
            Texture2D cursor = cursorNormal;
            switch(cursorName)
            {
                case "NORMAL":
                    cursor = cursorNormal;
                    break;
                case "QUESTION":
                    cursor = cursorQuestionMark;
                    break;
                case "DRAG":
                    cursor = cusorDrag;
                    break;
                case "EXIT":
                    cursor = cursorExit;
                    break;
                case "CIRCLE":
                    cursor = cursorNormal;
                    DrawThresholdCircle(Color.red, 32);
                    break;
            }
            if (cursor != null && cursorName != "CIRCLE")
                _setCursor(cursor);
                return;
        }

        void Start()
        {
            _setCursor(cursorNormal);
            //DrawThresholdCircle(Color.red, 20);
            EventManager.StartListening("CURSOR_REFRESH",_listenToCursorEvent);
        }



        void DrawThresholdCircle(Color color, float radius, int thickness = 2)
        {
            //draw a hollow red circle around the cursor
            Debug.Log("Drawing Circle");
            Vector2 cursorPos = Input.mousePosition;
            Texture2D tex = new Texture2D((int)radius * 2, (int)radius * 2);
            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(tex.width / 2, tex.height / 2));
                    if (dist > radius - thickness && dist < radius + thickness)
                    {
                        tex.SetPixel(x, y, color);
                    }
                    else
                    {
                        tex.SetPixel(x, y, new Color(0, 0, 0, 0));
                    }
                }
            }
            //offset the cursor position
            float offsetX = (tex.width / 2) - (tex.width / 4);
            float offsetY = (tex.height / 2) - (tex.height / 4);
            tex.Apply();
            Cursor.SetCursor(tex, new Vector2(offsetX, offsetY), CursorMode.Auto);


        }



    }
}

