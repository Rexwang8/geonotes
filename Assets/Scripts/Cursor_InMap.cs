using UnityEngine;
using TigerForge;

namespace SpatialNotes
{
    public class Cursor_inMap : MonoBehaviour
    {
        public Texture2D cursorNormal;
        public Texture2D cursorQuestionMark;
        public Texture2D cursorDrag;
        public Texture2D cursorExit;
        public Texture2D customLeftClickCursor; // Custom cursor for left-click
        public Texture2D customRightClickCursor; // Custom cursor for right-click

        void Start()
        {
            SetCursor(cursorNormal);
            EventManager.StartListening("CURSOR_REFRESH", ListenToCursorEvent);
        }

        void Update()
        {
            // Check for left mouse button click to change cursor
            if (Input.GetMouseButtonDown(0)) // 0 corresponds to the left mouse button
            {
                SetCursor(customLeftClickCursor);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                SetCursor(cursorNormal);
            }

            // Check for right mouse button click to change cursor
            if (Input.GetMouseButtonDown(1)) // 1 corresponds to the right mouse button
            {
                SetCursor(customRightClickCursor);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                SetCursor(cursorNormal);
            }
        }

        void ListenToCursorEvent()
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
                    cursor = cursorDrag;
                    break;
                case "EXIT":
                    cursor = cursorExit;
                    break;
            }

            if (cursor != null && cursorName != "CIRCLE")
                SetCursor(cursor);
        }

        void SetCursor(Texture2D cursor)
        {
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
        }

        void OnDestroy()
        {
            // Reset cursor to default when script is disabled or GameObject is destroyed
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
