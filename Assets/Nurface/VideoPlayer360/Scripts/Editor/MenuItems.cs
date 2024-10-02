using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

using VideoPlayer360.Prefabs;

namespace VideoPlayer360.MenuItems
{    
    public static class VideoPlayer360MenuItems
    {        
        [MenuItem("GameObject/UI/Video Player 360/New Video Player 360", false, 1)]
        public static MediaPlayerController AddNewVideoPlayer360()
        {
            var parent = new GameObject("VideoPlayer360");
            
            var mediaPlayerControllerGO = InstantiatePrefab(VideoPlayer360Prefabs.Instance.Prefab_MediaPlayerController);
            var screenGO = InstantiatePrefab(VideoPlayer360Prefabs.Instance.Prefab_MovieScreen);
            var playerGO = InstantiatePrefab(VideoPlayer360Prefabs.Instance.Prefab_Player);                        

            var mediaPlayerController = mediaPlayerControllerGO.GetComponent<MediaPlayerController>();
            mediaPlayerController.target = screenGO.transform;
            
            mediaPlayerController.menuSpawnPosition = playerGO.transform.GetChild(0).GetChild(1);            

            mediaPlayerControllerGO.transform.SetParent(parent.transform);
            screenGO.transform.SetParent(parent.transform);
            playerGO.transform.SetParent(parent.transform);

            // Add an event system if necessary
            var eventSystem = GameObject.FindObjectOfType<EventSystem>();
            GameObject eventSystemGO = null;
            
            if (eventSystem == null)
            {
                eventSystemGO = new GameObject("Event System");
                eventSystemGO.AddComponent<EventSystem>();
            }
            else
            {
                eventSystemGO = eventSystem.gameObject;
            }

            // Add the Gaze Input Module if necessary
            var gazeInputModule = eventSystemGO.GetComponent<GazeInputModuleMediaPlayer>();
            if (gazeInputModule == null)
            {
                eventSystemGO.AddComponent<GazeInputModuleMediaPlayer>();
            }

            return mediaPlayerController;
        }

        [MenuItem("GameObject/UI/Video Player 360/New Video Player 360 Wall", false, 2)]
        public static void AddNewVideoPlayer360Wall()
        {
            var mediaPlayerController = AddNewVideoPlayer360();

            // hide the movie screen
            mediaPlayerController.target.gameObject.SetActive(false);

            InstantiatePrefab(VideoPlayer360Prefabs.Instance.Prefab_VideoWall);
        }

        public static GameObject InstantiatePrefab(Transform prefab, bool generateUndo = true)
        {            
            Transform parent = null;

            parent = UnityEditor.Selection.activeTransform;

            var gameObject = GameObject.Instantiate(prefab.gameObject) as GameObject;
            gameObject.name = prefab.name;

            if (parent != null)
            {
                gameObject.transform.SetParent(parent);
            }

            FixInstanceTransform(gameObject.transform, prefab.transform);

            if (generateUndo)
            {
                UnityEditor.Undo.RegisterCreatedObjectUndo(gameObject, "Created " + gameObject.name);
            }

            return gameObject;
        }

        public static void FixInstanceTransform(Transform transform, Transform prefabTransform)
        {            
            transform.localPosition = Vector3.zero;            
            transform.position = Vector3.zero;

            transform.rotation = prefabTransform.rotation;
            transform.localScale = prefabTransform.localScale;            
        }
    }
}