using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace VideoPlayer360
{
    public class VideoWallUnit : MonoBehaviour
    {

        public VideoWallUnitConfig config;
        public VideoInfo videoInfo;

        // Private vars
        private RectTransform myRect, parentRect, thumbnailImageTransform, glossImage, descriptionTextTransform;
        private GazeInputModuleMediaPlayer gazeInputModule;
        private Camera mainCam;
        private bool gazedAt;
        private float gazeTimer;
        private Vector3 lastMenuRotation, lastThumbPosition, lastGlossPosition;

        private Image thumbnailImage;
        private Text titleText, dateText, descriptionText;


        // Use this for initialization
        void Awake()
        {
            myRect = GetComponent<RectTransform>();
            parentRect = transform.parent.gameObject.GetComponent<RectTransform>();
            thumbnailImageTransform = transform.Find("Sprite_Thumbnail").gameObject.GetComponent<RectTransform>();
            glossImage = transform.Find("Sprite_Gloss").gameObject.GetComponent<RectTransform>();
            descriptionTextTransform = transform.Find("Text_Description").gameObject.GetComponent<RectTransform>();
            mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            gazeInputModule = GameObject.FindObjectOfType<GazeInputModuleMediaPlayer>().GetComponent<GazeInputModuleMediaPlayer>();

            thumbnailImage = thumbnailImageTransform.GetComponent<Image>();
            descriptionText = descriptionTextTransform.GetComponent<Text>();
            titleText = transform.Find("Text_Title").GetComponent<Text>();
            dateText = transform.Find("Text_Date").GetComponent<Text>();            
        }

        // Update is called once per frame
        void Update()
        {
            if (gazedAt == true)
            {
                // Set rotation/positions based on gaze position over UI
                Vector2 localGazePoint = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(myRect, gazeInputModule.pointerData.position, mainCam, out localGazePoint);
                Vector3 desiredRotation = new Vector3(localGazePoint.y * -config.rotationAmount.y, localGazePoint.x * config.rotationAmount.x, 0f);
                Vector3 desiredThumbPosition = new Vector3(localGazePoint.x * config.backgroundPanAmount.x, localGazePoint.y * config.backgroundPanAmount.y, 0f);
                Vector3 desiredGlossPosition = new Vector3(localGazePoint.x * config.glossPanAmount.x, localGazePoint.y * config.glossPanAmount.y, 0f);

                if (gazeTimer < 1f)
                {
                    gazeTimer += Time.deltaTime * config.lerpSpeed;
                    if (gazeTimer > 1f) { gazeTimer = 1f; }
                    myRect.sizeDelta = new Vector2(160f, Mathf.Lerp(config.minSizeY, config.maxSizeY, gazeTimer));
                    myRect.localEulerAngles = new Vector3(Mathf.LerpAngle(0f, desiredRotation.x, gazeTimer), Mathf.LerpAngle(0f, desiredRotation.y, gazeTimer), 0f);
                    thumbnailImageTransform.localPosition = Vector3.Lerp(Vector3.zero, desiredThumbPosition, gazeTimer);
                    glossImage.localPosition = Vector3.Lerp(Vector3.zero, desiredGlossPosition, gazeTimer);
                    parentRect.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(0f, 0f, config.maxPosZ), gazeTimer);
                    descriptionTextTransform.anchoredPosition = new Vector3(10f, Mathf.Lerp(config.descriptionYLower, config.descriptionYUpper, gazeTimer), 0f);

                }
                else if (gazeTimer == 1f)
                {
                    myRect.localEulerAngles = desiredRotation;
                    thumbnailImageTransform.localPosition = new Vector3(localGazePoint.x * config.backgroundPanAmount.x, localGazePoint.y * config.backgroundPanAmount.y, 0f);
                    glossImage.localPosition = new Vector3(localGazePoint.x * config.glossPanAmount.x, localGazePoint.y * config.glossPanAmount.y, 0f);
                }
            }
            else
            {
                if (gazeTimer > 0f)
                {
                    gazeTimer -= Time.deltaTime * config.lerpSpeed;
                    if (gazeTimer < 0f) { gazeTimer = 0f; }
                    myRect.sizeDelta = new Vector2(160f, Mathf.Lerp(config.minSizeY, config.maxSizeY, gazeTimer));
                    myRect.localEulerAngles = new Vector3(Mathf.LerpAngle(0f, lastMenuRotation.x, gazeTimer), Mathf.LerpAngle(0f, lastMenuRotation.y, gazeTimer), 0f);
                    thumbnailImageTransform.localPosition = Vector3.Lerp(Vector3.zero, lastThumbPosition, gazeTimer);
                    glossImage.localPosition = Vector3.Lerp(Vector3.zero, lastGlossPosition, gazeTimer);
                    parentRect.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(0f, 0f, config.maxPosZ), gazeTimer);
                    descriptionTextTransform.anchoredPosition = new Vector3(10f, Mathf.Lerp(config.descriptionYLower, config.descriptionYUpper, gazeTimer), 0f);
                }
            }
        }

        public void OnPointerEnter()
        {
            gazedAt = true;
        }

        public void OnPointerExit()
        {
            gazedAt = false;
            lastMenuRotation = myRect.localEulerAngles;
            lastThumbPosition = thumbnailImageTransform.localPosition;
            lastGlossPosition = glossImage.localPosition;
        }

        public void UpdateDisplay()
        {
            if (videoInfo == null) return;

            if(thumbnailImage != null) thumbnailImage.sprite = videoInfo.thumbnailSprite;
            if(titleText != null) titleText.text = videoInfo.title;
            if(dateText != null) dateText.text = videoInfo.date;
            if(descriptionText != null) descriptionText.text = videoInfo.description;
        }
    }
}