﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public enum VideoType
{
    Background = 0,
    Effect = 1,
    Product = 2
}

public class VideoResourceUIManager : MonoBehaviour
{
    public VideoManager video;
    public Text SelectTagText;
    public List<SelectPanel> Panels;
    public List<Image> ButtonImages;
    public Color SelectBtnColor;
    public Color NotSelectBtnColor;

    private int curPanelIndex = 0;
    private Dictionary<string, VideoType> Prefix2Type = new Dictionary<string, VideoType>() {
        {"BG", VideoType.Background}, {"EF", VideoType.Effect}, {"PRO", VideoType.Product}
    };
    private Dictionary<VideoType, string> Type2Tag = new Dictionary<VideoType, string>() {
        {VideoType.Background, "背景"}, {VideoType.Effect, "特效"}, { VideoType.Product, "成品"}
    };


    private void Start() {
        for (int i = 0; i < Panels.Count; i++) {
            Panels[i].gameObject.SetActive(i == 0);
            Panels[i].SelectEvent += video.PlayHandle;
        }

        // 按钮效果
        if (ButtonImages.Count > 0) {
            ButtonImages[0].color = SelectBtnColor;
        }
        for (int i = 1; i < ButtonImages.Count; i++) {
            ButtonImages[i].color = NotSelectBtnColor;
        }
        // Tag Text
        if (SelectTagText) {
            SelectTagText.text = Type2Tag[VideoType.Background];
        }

        // 导入视频、缩略图生成成功后触发事件
        GetImage getImage = transform.Find("GetImage")?.GetComponent<GetImage>();
        if (getImage) {
            getImage.Completed += (s, e) => RefreshCurrentPanel();
        }
    }

    // 按钮事件 切换 Panel
    public void SwitchPanel(int index) {
        if (index == curPanelIndex) return;
        Panels[curPanelIndex].gameObject.SetActive(false);
        Panels[index].gameObject.SetActive(true);

        // 按钮效果
        if (index < ButtonImages.Count) {
            ButtonImages[curPanelIndex].color = NotSelectBtnColor;
            ButtonImages[index].color = SelectBtnColor;
        }
        // Tag 标签 Text
        if (SelectTagText) {
            SelectTagText.text = Type2Tag[(VideoType)index];
        }
        curPanelIndex = index;
    }

    // 按钮事件 添加视频
    public void AddVideo() {
        string prefix = Prefix2Type.FirstOrDefault(q => q.Value == ((VideoType)curPanelIndex)).Key;
        VideoResourceAPI.OpenFile(((VideoType)curPanelIndex).ToString("g"), prefix);
    }

    // 按钮事件 删除视频
    public void DeleteVideo() {
        string name = video.CurrentVideoName;
        string prefix = name.Split('_')[0];
        string videoType = Prefix2Type[prefix].ToString("g");
        VideoResourceAPI.Delete(videoType, name.TrimEnd(".mp4".ToArray()));

        video.Stop();
        Panels[(int)Prefix2Type[prefix]].UpdateAll();
    }
    

    // 按钮事件 刷新当前Panel
    public void RefreshCurrentPanel() {
        Panels[curPanelIndex].UpdateAll();
    }
}
