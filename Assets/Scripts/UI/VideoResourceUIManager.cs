using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static ResAPI;


[RequireComponent(typeof(TabControl))]
public class VideoResourceUIManager : MonoBehaviour
{
    public VideoManager video;
    public Text SelectTagText;
    public List<SelectPanel> Panels;

    private int curPanelIndex = 0;
    private Dictionary<string, VideoType> Prefix2Type = ResAPI.Prefix2Type;
    private Dictionary<VideoType, string> Type2Tag = ResAPI.Type2Tag;


    private void Start() {
        for (int i = 0; i < Panels.Count; i++) {
            Panels[i].SelectEvent += video.PlayHandle;
        }

        // TabControl Index 联动
        TabControl tabControl = GetComponent<TabControl>();
        if (tabControl) {
            tabControl.IndexChanged += (s, e) => {
                curPanelIndex = tabControl.Index;
                if (SelectTagText) {
                    SelectTagText.text = Type2Tag[(VideoType)curPanelIndex];
                }
            };
        }
        
        // Tag Text
        if (SelectTagText) {
            SelectTagText.text = Type2Tag[VideoType.Background];
        }

        // 导入视频、缩略图生成成功后触发事件
        GetImage getImage = transform.Find("GetImage")?.GetComponent<GetImage>();
        if (getImage) {
            getImage.Completed += (s, e) => RefreshPanel();
        }
    }


    // 按钮事件 添加视频
    public void AddVideo() {
        StartCoroutine(ResAPI.Instance.ImportFile((VideoType)curPanelIndex));
        StartCoroutine(RefreshPanel(1.2f));
    }

    // 按钮事件 删除视频（图片）
    public void DeleteVideo() {
        // 删除图片调用 DeleteImage 单独处理
        if (video.CurrentVideoName == "" && video.CurrentImageName == "") {
            return;
        }
        else if (video.CurrentImageName != "") {
            DeleteImage();
            return;
        }

        string name = video.CurrentVideoName;
        string prefix = name.Split('_')[0];
        Debug.Log(name);
        video.Stop();
        StartCoroutine(StartDeleteVideo(name, prefix));
    }

    IEnumerator StartDeleteVideo(string name,string prefix)
    {
        //如果不延迟会删除失败
        yield return new WaitForSeconds(0.2f);
        ResAPI.Delete(Prefix2Type[prefix], name);
        Panels[(int)Prefix2Type[prefix]].UpdateAll();

        // video player stop
        video.Stop();
    }

    private void DeleteImage() {
        string name = video.CurrentImageName;
        string prefix = name.Split('_')[0];
        ResAPI.Delete(Prefix2Type[prefix], name);
        Panels[(int)Prefix2Type[prefix]].UpdateAll();
        StartCoroutine(RefreshPanel(1.5f));

        // video player stop
        video.Stop();
    }

    // 按钮事件 刷新所有 Panel，[增量刷新]
    public IEnumerator RefreshPanel(float delay=0) {
        yield return new WaitForSeconds(delay);
        foreach (var panel in Panels) {
            panel.UpdateAll();
        }
    }
}
