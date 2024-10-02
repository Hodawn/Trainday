#if UNITY_5_6_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Video;

namespace VideoPlayer360
{
    public class VideoPlayerControllerUVP : VideoPlayerController
    {
        private UnityEngine.Video.VideoPlayer videoPlayer;
        private MediaPlayerController m_controller;
        private Transform m_target;
        private bool m_seeking = false;        

        public override void Initialize(MediaPlayerController controller, Transform target)
        {
            m_controller = controller;

            if (target == null)
            {
                Debug.LogWarning("[VideoPlayer360][Warning] The 'target' property is required.");
                return;
            }

            m_target = target;

            videoPlayer = target.gameObject.AddComponent<VideoPlayer>(); // Normally I would check to see if the component already existed, but for some reason GetComponent<VideoPlayer>() doesn't return null even if there isn't a component
            videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
            videoPlayer.targetMaterialRenderer = target.GetComponent<MeshRenderer>();
            
            var audioSource = videoPlayer.gameObject.AddComponent<AudioSource>();
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.SetTargetAudioSource(0, audioSource);

            videoPlayer.skipOnDrop = true;
            
            videoPlayer.isLooping = true;
            videoPlayer.prepareCompleted += (e) => { OnVideoReady(); };
            videoPlayer.loopPointReached += (e) => { OnVideoEnd(); };
            videoPlayer.seekCompleted += (e) => { OnSeekComplete(); };
        }        

        public override void SetClip(VideoInfo video)
        {
            var fileName = video.fileName;            

            if (string.IsNullOrEmpty(fileName)) return;

            if (!fileName.EndsWith(".mp4")) fileName = fileName + ".mp4";

            var url = "file://" + Application.streamingAssetsPath + "/" + fileName;

#if !UNITY_EDITOR && UNITY_ANDROID
     url = "jar:" + url;
#endif

            if (video.material != null)
            {                
                videoPlayer.GetComponent<MeshRenderer>().material = video.material;                
            }
            
            videoPlayer.url = url;
            videoPlayer.Prepare();            
        }

        public override void Play() 
        {
            videoPlayer.Play();
        }
        
        public override void Pause() 
        {
            videoPlayer.Pause();
        }

        public override void Seek(float time) 
        {
            if (m_seeking) return;
     
            time = Math.Min(time, 1);
            time = Math.Max(time, 0);

            // seems to work better if we pause while seeking
            Pause();  

            // time is 0 - 1f                        
            videoPlayer.frame = (long)(time * videoPlayer.frameCount);

            m_seeking = true;                                               
        }

        private void OnSeekComplete()
        {
            m_seeking = false;
            
            Play();
        }

        public override bool IsPlaying
        {
            get { return videoPlayer.isPlaying; }
        }

        public override float PlaybackTime
        {
            get { return ((float)videoPlayer.frame / (float)videoPlayer.frameCount); }
        }

        private void OnVideoEnd()
        {
            if (m_seeking) return;

            if(m_controller.playListMode == MediaPlayerController.ePlayListMode.MultipleVideos) m_controller.Next();
        }

        private void OnVideoReady()
        {            
            m_controller.HandlePlayPauseButton();
        }        
    }
}
#endif