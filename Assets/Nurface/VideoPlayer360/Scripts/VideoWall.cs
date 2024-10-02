using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;

namespace VideoPlayer360
{
    [ExecuteInEditMode]
    public class VideoWall : MonoBehaviour
    {
        /// <summary>
        /// Data for the videos
        /// </summary>
        public List<VideoInfo> videos = new List<VideoInfo>();

        /// <summary>
        /// Configuration for the wall units
        /// </summary>
        public VideoWallUnitConfig unitConfig;        

        /// <summary>
        /// The actual units themselves
        /// </summary>
        private List<VideoWallUnit> videoWallUnits = new List<VideoWallUnit>();

        private MediaPlayerController mediaPlayerController;

        void Start()
        {            
            videoWallUnits = this.transform.GetComponentsInChildren<VideoWallUnit>().ToList();
            mediaPlayerController = GameObject.FindObjectOfType<MediaPlayerController>();

            InitializeWallUnits();            
        }

        private void InitializeWallUnits()
        {
            foreach (var videoWallUnit in videoWallUnits)
            {
                var _videoWall = videoWallUnit; // necessary, otherwise the event will use the foreach variable (videoWallUnit)

                var eventTrigger = videoWallUnit.GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener((eventData) => { ShowVideo(_videoWall.videoInfo); });
                eventTrigger.triggers.Add(entry);
            }

            UpdateWallUnits();
        }

        public void UpdateWallUnits()
        {
            for(var x = 0; x < videos.Count; x++)
            {                
                if (videoWallUnits.Count >= x)
                {
                    videoWallUnits[x].videoInfo = videos[x];
                    videoWallUnits[x].UpdateDisplay();
                }
            }

            foreach (var videoWallUnit in videoWallUnits)
            {
                videoWallUnit.config = unitConfig;                
            }
        }

        private void ShowVideo(VideoInfo videoInfo)
        {
            StartCoroutine(_ShowVideo(videoInfo));
        }

        private IEnumerator _ShowVideo(VideoInfo videoInfo)
        {
            // fade to black
            mediaPlayerController.FadeToBlack();
            yield return new WaitForSeconds(1f);
            
            // Make sure pointer exit happens on the previously selected video wall
            foreach (var videoWallUnit in videoWallUnits) {
                videoWallUnit.OnPointerExit();
            }
            
            // hide the video wall
            this.gameObject.SetActive(false);

            // show the movie screen
            mediaPlayerController.SetClip(videoInfo);
            mediaPlayerController.target.gameObject.SetActive(true);

            // set up the playlist
            mediaPlayerController.videoPlaylist = videos;

            mediaPlayerController.State = MediaPlayerController.eState.PlayingVideo;

            // hide the overlay
            mediaPlayerController.FadeToClear();
        }
    }
}