using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RawImage))]
public class VideoManager : MonoBehaviour
{
    private VideoPlayer video;
    private CanvasGroup canvasGroup;
    private RawImage rawImage;
    [HideInInspector]
    public bool OnShow = true;

    public string CurrentVideoName = "";
    public bool HideOnAwake = true;
    public bool HideOnError = false;
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
    public SelectPanel SelectPanel;

    private void Start() {
        video = GetComponent<VideoPlayer>();
        canvasGroup = GetComponent<CanvasGroup>();
        rawImage = GetComponent<RawImage>();
        HideFailTip();

        if (SelectPanel) {
            SelectPanel.SelectEvent += PlayHandle;
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


    #region [事件处理]

    public void PlayHandle(object sender, string name) {
        Play(name);
    }

    private void VideoReady(VideoPlayer source) {
        // RenderTexture
        RenderTexture texture = RenderTexture.GetTemporary((int)video.width, (int)video.height);
        video.targetTexture = texture;
        rawImage.texture = texture;

        source.Play();
        HideFailTip();
        HideMask();
        CurrentVideoName = preparedNewVideoName;
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
