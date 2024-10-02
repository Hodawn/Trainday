#if EMT_PRESENT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VideoPlayer360
{
    public class VideoPlayerControllerEMT : VideoPlayerController
    {
        private MediaPlayerCtrl mediaPlayerCtrl;
        private MediaPlayerController m_controller;
        private Transform m_target;

        public override void Initialize(MediaPlayerController controller, Transform target)
        {
            m_controller = controller;

            if (target == null)
            {
                Debug.LogWarning("[VideoPlayer360][Warning] The 'target' property is required.");
                return;
            }

            m_target = target;

            // EMT has code in Awake() which processes m_TargetMaterial, so if you set the target material after Awake() is called,
            // it doesn't work correctly. To avoid this problem, we disable the target object first, then set the target material,
            // then re-enable the target object again (at the end of the frame)
            var wasActive = m_target.gameObject.activeInHierarchy;
            m_target.gameObject.SetActive(false);

            mediaPlayerCtrl = target.GetComponent<MediaPlayerCtrl>() ?? m_target.gameObject.AddComponent<MediaPlayerCtrl>();
                        
            mediaPlayerCtrl.m_TargetMaterial = new GameObject[] { m_target.gameObject };
            mediaPlayerCtrl.m_bLoop = true;

            mediaPlayerCtrl.OnEnd += OnVideoEnd;
            mediaPlayerCtrl.OnVideoFirstFrameReady += OnVideoReady;

            if(wasActive) controller.StartCoroutine(EnableTarget());   
                                    
        }

        private IEnumerator EnableTarget()
        {
            yield return new WaitForEndOfFrame();

            m_target.gameObject.SetActive(true);
        }

        public override void SetClip(VideoInfo video)
        {
            var fileName = video.fileName;

            if (!fileName.EndsWith(".mp4")) fileName = fileName + ".mp4";

            if (video.material != null)
            {
                mediaPlayerCtrl.GetComponent<MeshRenderer>().material = video.material;
            }

            mediaPlayerCtrl.Load(fileName);
        }

        public override void Play() 
        {
            mediaPlayerCtrl.Play();
        }
        
        public override void Pause() 
        {            
            mediaPlayerCtrl.Pause();
        }

        public override void Seek(float time) 
        {            
            mediaPlayerCtrl.SetSeekBarValue(time);
        }        

        public override bool IsPlaying
        {
            get { return mediaPlayerCtrl.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING; }
        }

        public override float PlaybackTime
        {
            get { return mediaPlayerCtrl.GetSeekBarValue(); }
        }

        private void OnVideoEnd()
        {
            if(m_controller.playListMode == MediaPlayerController.ePlayListMode.MultipleVideos) m_controller.Next();
        }

        private void OnVideoReady()
        {            
            m_controller.HandlePlayPauseButton();
        }
    }
}
#endif