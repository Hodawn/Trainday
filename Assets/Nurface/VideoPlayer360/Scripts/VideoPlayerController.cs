using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VideoPlayer360
{
    public abstract class VideoPlayerController
    {
        public abstract void Initialize(MediaPlayerController controller, Transform target);
        public abstract void SetClip(VideoInfo video);
        public abstract void Play();
        public abstract void Pause();        
        public abstract void Seek(float time);        

        public abstract bool IsPlaying { get; }
        public abstract float PlaybackTime { get; }
    }

    public enum eVideoPlayerType
    {
#if EMT_PRESENT
        EasyMovieTexture,
#endif

#if UNITY_5_6_OR_NEWER
        UnityMoviePlayer
#endif
    }
}
