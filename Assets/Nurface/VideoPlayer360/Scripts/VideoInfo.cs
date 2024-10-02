using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VideoPlayer360
{
    [Serializable]
    public class VideoInfo
    {
        public string fileName;
        public Sprite thumbnailSprite;
        public string title;
        public string date;
        public string description;
        public Material material;

        public VideoInfo(string fileName = null, Sprite thumbnailSprite = null, string title = null, string date = null, string description = null, Material material = null)
        {
            this.fileName = fileName;
            this.thumbnailSprite = thumbnailSprite;
            this.title = title;
            this.date = date;
            this.description = description;
            this.material = material;
        }
    }
}
