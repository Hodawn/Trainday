using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace VideoPlayer360.Prefabs
{        
    public class VideoPlayer360AssetPostProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            /// This creates the /StreamingAssets/ folder and moves the demo videos into the folder.
            /// I don't want this running all the time, so perhaps later I'll make it only happen when opening the demo scene.

            /*
            // check for the existence of a streamingassets folder
            var streamingAssetsPath = Application.dataPath + "/StreamingAssets";

            if (!Directory.Exists(streamingAssetsPath))
            {                
                Directory.CreateDirectory(streamingAssetsPath);
                Debug.Log("[VideoPlayer360] Created Streaming Assets folder.");
            }

            // load all .mp4 files in the /Nurface directory
            // a) locate the Nurface directory (it may not be in the default location of Assets/Nurface)
            var nurFacePath = Directory.GetDirectories(Application.dataPath, "Nurface", SearchOption.AllDirectories).FirstOrDefault();
            var mp4Files = Directory.GetFiles(nurFacePath, "*.mp4");
            
            foreach (var mp4File in mp4Files)
            {
                var fileInfo = new FileInfo(mp4File);
                var newPath = streamingAssetsPath + "/" + fileInfo.Name;

                // if the file already exists in the destination directory, delete it so we can replace it with our new one
                if (File.Exists(newPath))
                {
                    File.Delete(newPath);
                }

                Debug.LogFormat("[VideoPlayer360] Moved '{0}' to the Streaming Assets folder.", fileInfo.Name);
                File.Move(mp4File, newPath);
            }

            if (mp4Files.Any())
            {
                AssetDatabase.Refresh();
            }
            */
        }
    }
}