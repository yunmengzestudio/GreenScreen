using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

public enum VideoType
{
    Background = 0,
    Effect = 1,
    Product = 2
}

public class VideoResourceManager : MonoBehaviour
{
    public VideoManager video;
    public List<SelectPanel> Panels;

    private int curPanelIndex = 0;
    private Dictionary<string, VideoType> Prefix2Type = new Dictionary<string, VideoType>() {
        {"BG", VideoType.Background}, {"EF", VideoType.Effect}, {"PRO", VideoType.Product}
    };


    private void Start() {
        if (Panels.Count > 0) {
            Panels[0].gameObject.SetActive(true);
        }
        for(int i = 1; i < Panels.Count; i++) {
            Panels[i].gameObject.SetActive(false);
        }
    }

    // 按钮事件 切换 Panel
    public void SwitchPanel(int index) {
        if (index == curPanelIndex) return;
        Panels[curPanelIndex].gameObject.SetActive(false);
        Panels[index].gameObject.SetActive(true);
        curPanelIndex = index;
    }

    // 按钮事件 添加视频
    public void AddVideo() {
        string prefix = Prefix2Type.FirstOrDefault(q => q.Value == ((VideoType)curPanelIndex)).Key;
        VideoResourceAPI.OpenFile(((VideoType)curPanelIndex).ToString("g"), prefix);
    }

    // 按钮事件 删除视频
    public void DeleteVideo() {
        string name = video.GetComponent<VideoPlayer>().clip.name;
        string prefix = name.Split('_')[0];
        string path = Prefix2Type[prefix].ToString("g");
        VideoResourceAPI.Delete(path, name);

        video.Stop();
        Panels[(int)Prefix2Type[prefix]].UpdateAll();
    }

}
