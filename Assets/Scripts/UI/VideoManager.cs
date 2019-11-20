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

    public bool HideOnAwake = true;
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
        FailLoadTip.SetActive(false);

        if (SelectPanel) {
            SelectPanel.SelectEvent += PlayHandle;
        }
        if (HideOnAwake)
            Hide();
    }

    private void Update() {
        if (!OnShow) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            Click();
        }
    }


    public void Play(string name) {
        // 已经显示 Video
        if (canvasGroup.alpha > 0 && video.clip && video.clip.name == name) {
            Click();
            return;
        }

        Show();
        //StartCoroutine(AsyncPlay(ResourcePath + name));
        string AbsolutePath = VideoResourceAPI.FillVideoPath(name);
        UrlPlay("file://" + AbsolutePath);
    }

    private void UrlPlay(string path) {
        video.url = path;
        
        video.prepareCompleted += VideoReady;
        video.Prepare();
    }

    private IEnumerator AsyncPlay(string path) {
        ShowMask();
        ResourceRequest request = Resources.LoadAsync<VideoClip>(path);
        yield return request;
        
        VideoClip clip = request.asset as VideoClip;

        if (clip == null) {
            video.clip = null;
            FailLoadTip.SetActive(true);
            Debug.Log("加载视频失败：" + path);
            yield break;
        }

        // RenderTexture
        RenderTexture texture = RenderTexture.GetTemporary((int)clip.width, (int)clip.height);
        video.targetTexture = texture;
        rawImage.texture = texture;

        video.clip = clip;
        video.prepareCompleted += VideoReady;
        video.Prepare();
    }

    private void VideoReady(VideoPlayer source) {
        // RenderTexture
        RenderTexture texture = RenderTexture.GetTemporary((int)video.width, (int)video.height);
        video.targetTexture = texture;
        rawImage.texture = texture;

        source.Play();
        HideMask();
    }

    public void Stop() {
        video.Stop();
        video.clip = null;
        Hide();
    }


    private Transform _iconTransform;
    private Image _iconImage;
    public void Click() {
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


    public void PlayHandle(object sender, string name) {
        Play(name);
    }

    private void ShowMask() {
        MaskImage.color = Color.black;
        FailLoadTip.SetActive(false);
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
