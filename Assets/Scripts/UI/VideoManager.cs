using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System;

[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(CanvasGroup))]
public class VideoManager : MonoBehaviour
{
    private VideoPlayer video;
    private CanvasGroup canvasGroup;
    [HideInInspector]
    public bool OnShow = true;

    public string CurrentVideoName = "";
    public string CurrentImageName = "";
    public bool HideOnAwake = true;
    public bool HideOnError = false;
    public bool AutoResize = true;
    public string ResourcePath = "Videos/";

    [Header("Icon Conf")]
    public GameObject PlayIcon;
    public GameObject PauseIcon;
    public float AnimDuration = .35f;
    public Color IconColor = Color.white;
    private float MaxIconScale = 2f;

    [Header("Reference")]
    public Image MaskImage;
    public GameObject FailLoadTip;
    public SelectPanel[] SelectPanels;
    public RawImage VideoRawImage;


    private void Awake() {
        video = GetComponent<VideoPlayer>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start() {
        HideFailTip();

        foreach (SelectPanel panel in SelectPanels) {
            panel.SelectEvent += PlayHandle;
        }

        if (HideOnAwake)
            Hide();

        if (video) {
            video.prepareCompleted += VideoReady;
            video.errorReceived += Video_errorReceived;
        }

    }


    private void Update() {
        if (!OnShow) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            Click();
        }
    }


    private string preparedNewVideoName;    // 暂存视频名，播放成功的才会被赋值给 CurrentVideoName
    public void Play(string name) {
        // 已经显示 Video
        if (canvasGroup.alpha > 0 && CurrentVideoName == name) {
            Click();
            return;
        }

        Show();
        ShowMask();
        HideFailTip();

        preparedNewVideoName = name;
        UrlPlay("file://" + VideoResourceAPI.FillVideoPath(name));
    }

    private void UrlPlay(string path) {
        video.url = path;
        video.Prepare();
    }


    public void Stop() {
        video.Stop();
        video.clip = null;
        CurrentVideoName = "";
        CurrentImageName = "";
        Hide();
    }


    private Transform _iconTransform;
    private Image _iconImage;
    public void Click() {
        if (CurrentVideoName == "")
            return;

        if (video.isPlaying) {
            video.Pause();
            _iconTransform = PauseIcon.transform;
            _iconImage = PauseIcon.GetComponent<Image>();
        }
        else {
            video.Play();
            _iconTransform = PlayIcon.transform;
            _iconImage = PlayIcon.GetComponent<Image>();
        }
        _iconTransform.localScale = Vector3.one;
        _iconTransform.DOScale(MaxIconScale, AnimDuration);
        _iconImage.color = IconColor;
        _iconImage.DOFade(0, AnimDuration);
    }


    // 播放图片
    public void PlayImage(string name) {
        Stop();
        Show();
        HideMask();
        HideFailTip();

        string fullPath = VideoResourceAPI.FillVideoPath(name);
        Texture2D texture = ResourceLoader.LoadTexture(fullPath);
        VideoRawImage.texture = texture;
        CurrentImageName = name;

        if (AutoResize) {
            Resize(texture.width, texture.height);
        }
    }
    
    // 重置 RawImage 的大小以适应视频尺寸，参数为视频的尺寸
    private void Resize(float width, float height) {
        Vector2 target;
        Rect rect = GetComponent<RectTransform>().rect;

        // 视频的宽高比比较小时（视频较方），左右两边留空，否则上下留空
        if ((width / height) < (rect.width / rect.height))
            target = new Vector2((width / height) * rect.height, rect.height);
        else
            target = new Vector2(rect.width, rect.width * (height / width));
        VideoRawImage.GetComponent<RectTransform>().sizeDelta = target;
    }


    #region [事件处理]

    public void PlayHandle(object sender, string name) {
        string prefix = name.Split('_')[0];
        VideoResourceAPI.VideoType type;
        try {
            type = VideoResourceAPI.Prefix2Type[prefix];
        }
        catch (Exception) {
            Video_errorReceived(null, "[VideoManager.PlayHandle]: Convert Prefix to Type Error");
            throw;
        }

        if (VideoResourceAPI.TypeIsVideo(type))
            Play(name);
        else
            PlayImage(name);
    }

    private void VideoReady(VideoPlayer source) {
        // RenderTexture
        RenderTexture texture = RenderTexture.GetTemporary(video.texture.width, video.texture.height);
        video.targetTexture = texture;
        VideoRawImage.texture = texture;
        if (AutoResize)
            Resize(texture.width, texture.height);

        source.Play();
        HideFailTip();
        HideMask();
        CurrentVideoName = preparedNewVideoName;
        CurrentImageName = "";
    }

    private void Video_errorReceived(VideoPlayer source, string message) {
        Debug.Log(message);
        video.Stop();
        CurrentVideoName = "";
        ShowMask();
        ShowFailTip();
        if (HideOnError) Hide();
    }

    #endregion

    private void ShowFailTip() {
        if (FailLoadTip && !FailLoadTip.activeSelf)
            FailLoadTip.SetActive(true);
    }
    private void HideFailTip() {
        if (FailLoadTip && FailLoadTip.activeSelf)
            FailLoadTip.SetActive(false);
    }

    private void ShowMask() {
        MaskImage.color = Color.black;
    }
    private void HideMask(float duration = .35f) {
        MaskImage.DOColor(Color.clear, duration).SetEase(Ease.InCubic);
    }

    public void Show(float duration = 0) {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(1, duration);
        OnShow = true;
    }
    public void Hide(float duration = 0) {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOFade(0, duration);
        OnShow = false;
    }

}
