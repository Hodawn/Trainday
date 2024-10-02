using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;

namespace VideoPlayer360.Prefabs
{    
    //[CreateAssetMenu(menuName="Create VideoPlayer360 Prefabs", fileName="VideoPlayer360Prefabs")]
    public class VideoPlayer360Prefabs : ScriptableObject
    {
        public Transform Prefab_Player;
        public Transform Prefab_MovieScreen;
        public Transform Prefab_MediaPlayerController;
        public Transform Prefab_VideoWall;

        private static VideoPlayer360Prefabs m_instance;

        public static VideoPlayer360Prefabs Instance
        {
            get
            {
                if (m_instance == null) m_instance = Resources.Load<VideoPlayer360Prefabs>("VideoPlayer360Prefabs");

                return m_instance;
            }
        }
    }
}