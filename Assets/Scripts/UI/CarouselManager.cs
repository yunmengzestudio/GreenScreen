﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Video;

public class CarouselManager : MonoBehaviour
{
    private int currentIndex;
    private VideoManager videoManager;
    private List<RectTransform> images = new List<RectTransform>();
    
    // 按键设置
    public bool KeyControl = true;
    public float KeyIntervalTime = .28f;
    private float keyTimer = 0;

    public GameObject CarouselImagePrefab;

    [Header("Advanced Config")]
    public Color BehindShadow = new Color(1, 1, 1, .3f);
    public Vector2 LeftFigPos = new Vector2(-200, 0);
    public Vector2 RightFigPos = new Vector2(200, 0);
    public float BehindScale = 0.9f;
    public float AnimDuration = 0.35f;
    

    private void Start() {
        videoManager = GetComponentInChildren<VideoManager>();
        LoadImages();
        Reset(0);
        if (images.Count > 0) {
            transform.Find("NoImageTip").gameObject.SetActive(false);
        }
    }

    private void Update() {
        if (keyTimer > 0) {
            keyTimer -= Time.deltaTime;
        }
        else if (keyTimer < 0) {
            keyTimer = 0;
        }

        if (KeyControl && keyTimer == 0) {
            if (Input.GetKey(KeyCode.RightArrow)) {
                Next();
                keyTimer = KeyIntervalTime;
            }
            else if (Input.GetKey(KeyCode.LeftArrow)) {
                Prev();
                keyTimer = KeyIntervalTime;
            }
            else if (Input.GetKeyDown(KeyCode.Space) && !videoManager.OnShow) {
                Click();
            }
        }
    }


    // 初始化加载图片 | Sprite 路径：Resources/Thumbnails/...
    private void LoadImages() {
        Transform imagesParent = transform.Find("Images");

        List<string> imageNames;        // => 获取缩略图 Names (即视频标志符)
        imageNames = new List<string>() { "Demo/demo1", "Demo/demo2", "Demo/demo3" }; // TEST

        foreach (string imageName in imageNames) {
            GameObject go = Instantiate(CarouselImagePrefab, imagesParent);
            go.transform.localPosition = Vector3.zero;
            go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Thumbnails/" + imageName);
            images.Add(go.GetComponent<RectTransform>());
        }
    }


    // 重置图片位置
    private void Reset(int index) {
        if (images.Count == 0) return;
        foreach (RectTransform image in images) {
            image.localPosition = Vector3.zero;
            image.localScale = new Vector3(BehindScale, BehindScale, 1);
            image.GetComponent<Image>().color = BehindShadow;
        }
        currentIndex = index;
        images[Index(-1)].localPosition = LeftFigPos;
        images[Index(1)].localPosition = RightFigPos;
        images[currentIndex].localPosition = Vector3.zero;
        images[currentIndex].localScale = Vector3.one;
        images[currentIndex].GetComponent<Image>().color = Color.white;
        images[currentIndex].SetAsLastSibling();
        videoManager.Stop();
    }


    // 点击当前主图片
    public void Click() {
        if (images.Count == 0) return;
        string imageName = "";
        Sprite sprite = images[currentIndex].GetComponent<Image>().sprite;
        if (sprite) {
            imageName = sprite.name;
        }
        videoManager.Play(imageName);
    }
    
    public void Next(float duration = 0) {
        if (images.Count <= 1) return;
        if (duration == 0) duration = AnimDuration;
        currentIndex = Index(1);
        videoManager.Stop();

        images[Index(-2)].DOLocalMove(Vector3.zero, duration);
        images[Index(-1)].DOLocalMove(LeftFigPos, duration);
        images[Index(0)].DOLocalMove(Vector3.zero, duration);
        images[Index(1)].DOLocalMove(RightFigPos, duration);

        images[Index(-1)].DOScale(BehindScale, duration);
        images[Index(0)].DOScale(1f, duration);
        images[Index(-1)].GetComponent<Image>().DOColor(BehindShadow, duration);
        images[Index(0)].GetComponent<Image>().DOColor(Color.white, duration);

        images[Index(0)].SetAsLastSibling();
    }

    public void Prev(float duration = 0) {
        if (images.Count <= 1) return;
        if (duration == 0) duration = AnimDuration;
        currentIndex = Index(-1);
        videoManager.Stop();

        images[Index(2)].DOLocalMove(Vector3.zero, duration);
        images[Index(-1)].DOLocalMove(LeftFigPos, duration);
        images[Index(0)].DOLocalMove(Vector3.zero, duration);
        images[Index(1)].DOLocalMove(RightFigPos, duration);

        images[Index(1)].DOScale(BehindScale, duration);
        images[Index(0)].DOScale(1f, duration);
        images[Index(1)].GetComponent<Image>().DOColor(BehindShadow, duration);
        images[Index(0)].GetComponent<Image>().DOColor(Color.white, duration);

        images[Index(0)].SetAsLastSibling();
    }

    private int Index(int offset = 0) {
        return offset == 0
            ? currentIndex
            : (images.Count + currentIndex + offset) % images.Count;
    }
}