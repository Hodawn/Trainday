using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace VideoPlayer360
{
    public class VRSeekBar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler
    {
        // Video currently playing
        //public MediaPlayerCtrl currentVideo;
        private MediaPlayerController mediaPlayerController;
        
        public float dragTime = 0.2f;

        private Slider playbackSlider;
        private bool activeDrag = true;
        private bool update = true;
        private float deltaTime = 0.0f;
        private float lastValue = 0.0f;
        private float lastSetValue = 0.0f;

        public void Initialize(MediaPlayerController mediaPlayerController)
        {
            this.mediaPlayerController = mediaPlayerController;
        }

        // Use this for initialization
        void Start()
        {
            playbackSlider = GetComponent<Slider>();            
        }

        // Update is called once per frame
        void Update()
        {
            if (activeDrag == false)
            {
                deltaTime += Time.deltaTime;
                if (deltaTime > dragTime)
                {
                    activeDrag = true;
                    deltaTime = 0.0f;
                    //if(m_fLastSetValue != m_fLastValue)
                    //	m_srcVideo.SetSeekBarValue (m_fLastValue);
                }
            }

            if (update == false)
            {
                return;
            }

            if (mediaPlayerController != null)
            {
                if (playbackSlider != null)
                {
                    playbackSlider.value = mediaPlayerController.PlaybackTime;
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            update = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            update = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            mediaPlayerController.Seek(playbackSlider.value);            
        }

        public void OnDrag(PointerEventData eventData)
        {                        
            if (activeDrag == false)
            {
                lastValue = playbackSlider.value;
                return;
            }

            mediaPlayerController.Seek(playbackSlider.value);            
            lastSetValue = playbackSlider.value;
            activeDrag = false;
        }
    }
}