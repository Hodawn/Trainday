using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

namespace VideoPlayer360
{
    public class MediaPlayerController : MonoBehaviour
    {
        public eVideoPlayerType videoPlayerType;

        public enum ePlayListMode { SingleVideo, MultipleVideos };
        public ePlayListMode playListMode = ePlayListMode.SingleVideo;

        public VideoInfo currentVideo;
        public List<VideoInfo> videoPlaylist = new List<VideoInfo>();
        //public string fileName;
        //public List<string> fileNames = new List<string>();
        public Transform target;
        public bool playOnStart = true;

        public GameObject menuPrefab;
        public Transform menuSpawnPosition;
        public Sprite iconPlay, iconPause, iconPrevious, iconNext, iconVideoWall;
        public enum eMenuSpawnMode { GazeTrigger, InputFire1 };
        public eMenuSpawnMode menuSpawnMode = eMenuSpawnMode.GazeTrigger;

        private GameObject currentMenu;
        private Transform mainCamera;
        private GameObject screenFadeObject;

        private VideoPlayerController m_videoPlayerController;
        private VideoPlayerController videoPlayerController
        {
            get
            {
                if (m_videoPlayerController == null)
                {
                    m_videoPlayerController = GetNewVideoControllerInstance();

                    if (m_videoPlayerController != null) m_videoPlayerController.Initialize(this, target);
                }

                return m_videoPlayerController;
            }
        }

        public enum eState { PlayingVideo, InMenu };
        [HideInInspector, System.NonSerialized]
        public eState State = eState.PlayingVideo;

        private VideoWall videoWall;

        #region Play / Pause Button
        private Image playPauseImage
        {
            get
            {
                if (currentMenu == null) return null;

                return currentMenu.transform.GetChild(0).FindChild("ButtonPlayPause").gameObject.GetComponent<Image>();
            }
        }

        private Text playPauseText
        {
            get
            {
                if (currentMenu == null) return null;

                return currentMenu.transform.GetChild(0).FindChild("ButtonPlayPause").GetChild(0).gameObject.GetComponent<Text>();
            }
        }

        public void HandlePlayPauseButton()
        {
            if (currentMenu != null)
            {
                if (!videoPlayerController.IsPlaying)
                {
                    playPauseImage.sprite = iconPlay;
                    playPauseText.text = "Play";
                }
                else
                {
                    playPauseImage.sprite = iconPause;
                    playPauseText.text = "Pause";
                }
            }
        }
        #endregion

        void Start()
        {
            mainCamera = Camera.main.transform;
            screenFadeObject = GameObject.Find("ScreenFadeCube");
            videoWall = GameObject.FindObjectOfType<VideoWall>();

            if (videoWall) State = eState.InMenu;

            FadeToClear();

            if ((currentVideo == null || playListMode == ePlayListMode.MultipleVideos) && videoPlaylist.Any())
            {
                currentVideo = videoPlaylist.FirstOrDefault();
            }

            if (videoPlayerController == null)
            {
                Debug.LogWarning("[VideoPlayer360][Warning] Failed to initialize a new Video Controller instance. Please ensure that a valid Video Player is installed (VideoPlayer360 requires EasyMovieTextures or Unity 5.6+).");
                return;
            }

            if (currentVideo != null) videoPlayerController.SetClip(currentVideo);

            if (playOnStart) Play();
            else Pause();
        }

        private VideoPlayerController GetNewVideoControllerInstance()
        {
            switch (videoPlayerType)
            {
#if EMT_PRESENT
                case eVideoPlayerType.EasyMovieTexture:
                    return new VideoPlayerControllerEMT();
#endif
#if UNITY_5_6_OR_NEWER
                case eVideoPlayerType.UnityMoviePlayer:
                    return new VideoPlayerControllerUVP();
#endif
                default:
                    return null;
            }
        }

        public void Play()
        {
            if (videoPlayerController == null) return;

            videoPlayerController.Play();
            HandlePlayPauseButton();
        }

        public void Pause()
        {
            if (videoPlayerController == null) return;

            videoPlayerController.Pause();
            HandlePlayPauseButton();
        }

        public void PlayPause()
        {
            if (videoPlayerController == null) return;

            if (videoPlayerController.IsPlaying) Pause();
            else Play();
        }

        public void Next()
        {
            if (videoPlayerController == null) return;

            if (videoPlaylist.Count > 0)
            {
                var currentIndex = videoPlaylist.IndexOf(currentVideo);
                var nextIndex = currentIndex + 1;
                if (nextIndex >= videoPlaylist.Count) nextIndex = 0;

                currentVideo = videoPlaylist[nextIndex];

                videoPlayerController.SetClip(currentVideo);
                videoPlayerController.Seek(0f);
            }
        }

        public void Previous()
        {
            if (videoPlayerController == null) return;

            if (videoPlaylist.Count > 0)
            {
                var currentIndex = videoPlaylist.IndexOf(currentVideo);
                var nextIndex = currentIndex - 1;
                if (nextIndex < 0) nextIndex = videoPlaylist.Count - 1;

                currentVideo = videoPlaylist[nextIndex];

                videoPlayerController.SetClip(currentVideo);
                videoPlayerController.Seek(0f);
            }
        }

        public void Seek(float time)
        {
            if (videoPlayerController == null) return;

            videoPlayerController.Seek(time);
        }

        public void SetClip(VideoInfo video)
        {
            if (videoPlayerController == null) return;

            videoPlayerController.SetClip(video);
            this.currentVideo = video;
        }

        public float PlaybackTime
        {
            get
            {
                if (videoPlayerController == null) return 0f;

                return videoPlayerController.PlaybackTime;
            }
        }

        public void GazeTriggered()
        {
            if (videoPlayerController == null) return;
            if (State == eState.InMenu) return;

            if (menuSpawnMode == eMenuSpawnMode.GazeTrigger)
            {
                SpawnMenu();
            }
        }

        void Update()
        {
            if (videoPlayerController == null) return;
            if (State == eState.InMenu) return;

            if (menuSpawnMode == eMenuSpawnMode.InputFire1)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    if (currentMenu == null)
                    {
                        SpawnMenu();
                    }
                }
            }

            // We'll probably need to map this to the back button as well
            if (Input.GetMouseButtonDown(1))
            {
                if (currentMenu != null)
                {
                    HideMenu();
                }
                else
                {
                    if (videoWall != null)
                    {
                        ReturnToWall();
                    }
                }

            }
        }

        public void SpawnMenu()
        {
            if (currentMenu == null)
            {
                // Create a new menu
                currentMenu = (GameObject)Instantiate(menuPrefab, menuSpawnPosition.position, mainCamera.rotation);
                currentMenu.transform.eulerAngles = new Vector3(currentMenu.transform.eulerAngles.x, currentMenu.transform.eulerAngles.y, 0f);

                // Animate the menu scaling from 0 to 1
                currentMenu.transform.GetChild(0).localScale = Vector3.zero;
                iTween.ScaleTo(currentMenu.transform.GetChild(0).gameObject, Vector3.one, 1f);

                CreateEvent(currentMenu.transform.GetChild(0).gameObject, EventTriggerType.PointerExit, HideMenu);

                if (menuSpawnMode == eMenuSpawnMode.InputFire1)
                {
                    CreateEvent(currentMenu.transform.GetChild(0).gameObject, EventTriggerType.PointerDown, HideMenu);
                }

                //  Set Menu Button events                
                SetMenuButtonEvent("ButtonPlayPause", PlayPause);

                if (playListMode == ePlayListMode.SingleVideo)
                {
                    HideButton("ButtonPrevious");
                    HideButton("ButtonNext");
                    HideButton("ButtonVideoWall");
                }
                else
                {
                    SetMenuButtonEvent("ButtonPrevious", Previous);
                    SetMenuButtonEvent("ButtonNext", Next);
                    SetMenuButtonEvent("ButtonVideoWall", ReturnToWall);
                }

                if (videoWall == null)
                {
                    HideButton("ButtonVideoWall");
                }

                var seekBar = currentMenu.GetComponentInChildren<VRSeekBar>();
                if (seekBar != null)
                {
                    seekBar.Initialize(this);
                }

                HandlePlayPauseButton();
            }
        }

        public void HideMenu()
        {
            if (currentMenu == null) return;

            iTween.MoveTo(currentMenu.transform.GetChild(0).gameObject, currentMenu.transform.forward * 5f, 1f);
            iTween.ScaleTo(currentMenu.transform.GetChild(0).gameObject, iTween.Hash("scale", Vector3.zero, "time", 1f, "oncomplete", "DestroyMenu", "oncompletetarget", gameObject));
        }

        public void DestroyMenu()
        {
            if (currentMenu)
            {
                Destroy(currentMenu);
            }
        }

        private void HideButton(string buttonName)
        {
            var buttonGameObject = currentMenu.transform.GetChild(0).FindChild(buttonName).gameObject;
            buttonGameObject.SetActive(false);
        }

        private void SetMenuButtonEvent(string buttonName, UnityAction action)
        {
            EventTrigger buttonTrigger = currentMenu.transform.GetChild(0).FindChild(buttonName).gameObject.GetComponent<EventTrigger>();
            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((eventData) => action());
            buttonTrigger.triggers.Add(pointerDown);
        }

        private void CreateEvent(GameObject o, EventTriggerType type, UnityAction action)
        {
            EventTrigger trigger = o.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener((eventData) => { action.Invoke(); });
            trigger.triggers.Add(entry);
        }

        public void FadeToBlack()
        {
            iTween.ColorTo(screenFadeObject, Color.black, 1f);
        }

        public void FadeToClear()
        {
            iTween.ColorTo(screenFadeObject, Color.clear, 1f);
        }

        public void ReturnToWall()
        {
            StartCoroutine(_ReturnToWall());
        }

        private IEnumerator _ReturnToWall()
        {
            HideMenu();
            FadeToBlack();
            yield return new WaitForSeconds(1f);

            // hide the video
            target.gameObject.SetActive(false);
            State = MediaPlayerController.eState.InMenu;

            // rotate the camera to face the wall
            // this isn't working with the editor mouselook script, but it might work on the device itself
            // will look into it some more
            mainCamera.localRotation = new Quaternion();
            mainCamera.rotation = new Quaternion();

            // show the wall
            videoWall.gameObject.SetActive(true);

            // hide the overlay
            FadeToClear();
        }
    }
}