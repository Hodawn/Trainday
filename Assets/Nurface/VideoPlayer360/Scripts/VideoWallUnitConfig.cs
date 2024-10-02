using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VideoPlayer360
{
    [Serializable]
    public class VideoWallUnitConfig
    {
        [Tooltip("The amount the menu will rotate when gazed at.")]
        public Vector2 rotationAmount = new Vector2(0.2f, 0.2f);
        [Tooltip("The amount the thumbnail image will pan when gazed at.")]
        public Vector2 backgroundPanAmount = new Vector2(0.1f, 0.2f);
        [Tooltip("The amount the gloss image will pan when gazed at.")]
        public Vector2 glossPanAmount = new Vector2(-0.4f, -0.2f);
        [Tooltip("The minimum Y size of the window when NOT gazed at.")]
        public float minSizeY = 90f;
        [Tooltip("The maximum Y size of the window when gazed at.")]
        public float maxSizeY = 110f;
        [Tooltip("The Z position of the window when gazed at.")]
        public float maxPosZ = -0.2f;
        [Tooltip("The Y position of the description text when NOT gazed at.")]
        public float descriptionYLower = -53f;
        [Tooltip("The Y position of the description text when gazed at.")]
        public float descriptionYUpper = -46f;
        [Tooltip("How fast the menu open/close animations will play.")]
        public float lerpSpeed = 4f;
    }
}
