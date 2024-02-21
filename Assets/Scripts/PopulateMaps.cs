using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;

namespace SpatialNotes
{
    public class PopulateMaps : MonoBehaviour
    {
        // populate or clear the maps folder
        [Button("Clear Maps")]
        public void ClearAllMaps()
        {
            string path = Application.streamingAssetsPath + "/Maps";
            if (System.IO.Directory.Exists(path))
            {
                System.IO.Directory.Delete(path, true);
            }
        }

        public void PopulateMap(int num, string mapName)
        {
            // Clear used asset folder
            string path = Application.streamingAssetsPath + "/Maps/" + mapName;
            if (System.IO.Directory.Exists(path))
            {
                System.IO.Directory.Delete(path, true);
            }

            // Create a new map
            MapObject map = new MapObject();
            map.name = mapName;
            //pull demomap from streamable assets
            string mapImagePath = Application.streamingAssetsPath + "/demomap.jpg";
            map.CreateMap(_name: mapName, _TEMP_IMAGE_PATH: mapImagePath);

            // Create a new note
            for (int i = 0; i < num; i++)
            {
                PostCard note = new PostCard(_title: "Test Note " + i, _date: System.DateTime.Now);
                map.AddPostcard(note);
            }

            Debug.Log("Map " + mapName + " created with " + num + " notes");
        }

        [Button("Populate Maps")]
        public void PopulateAllMaps()
        {
            PopulateMap(2, "Test Map");
            PopulateMap(3, "Test Map 2");
            PopulateMap(4, "Test Map 3");
        }
    }
}
