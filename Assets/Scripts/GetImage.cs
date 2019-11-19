using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Video;

public class GetImage : MonoBehaviour
{
    private VideoPlayer vp;
    private Texture2D videoFrameTexture;
    private RenderTexture renderTexture;
    public string outpath;
    public string imagename;


    void Start() {
        videoFrameTexture = new Texture2D(2, 2);
        vp = GetComponent<VideoPlayer>();
        vp.waitForFirstFrame = true;
        vp.sendFrameReadyEvents = true;
        vp.frameReady += OnNewFrame;
    }

    private void OnNewFrame(VideoPlayer source, long frameIdx) {
        renderTexture = source.texture as RenderTexture;
        if (videoFrameTexture.width != renderTexture.width || videoFrameTexture.height != renderTexture.height) {
            videoFrameTexture.Resize(renderTexture.width, renderTexture.height);
        }
        RenderTexture.active = renderTexture;
        videoFrameTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        videoFrameTexture.Apply();
        RenderTexture.active = null;
        vp.frameReady -= OnNewFrame;
        vp.sendFrameReadyEvents = false;
        ScaleTexture(videoFrameTexture, 800, 400, outpath + imagename);
        Debug.Log("成功生成缩略图：" + outpath + imagename);
    }

    //生成缩略图
    private void ScaleTexture(Texture2D source, int targetWidth, int targetHeight, string savePath) {

        Texture2D result = new Texture2D(targetWidth, targetHeight, TextureFormat.ARGB32, false);

        for (int i = 0; i < result.height; ++i) {
            for (int j = 0; j < result.width; ++j) {
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }
        result.Apply();
        File.WriteAllBytes(savePath, result.EncodeToPNG());
    }

    public void GeneratePreviewImage(string videoPath, string outImagePath) {
        var splitArray = Regex.Split(videoPath, "Resources/", RegexOptions.IgnoreCase);
        var ImageName = Regex.Split(videoPath, "Videos", RegexOptions.IgnoreCase);
        string imageName = ImageName[1].TrimEnd(".mp4".ToCharArray()) + ".png";
        string LoadPath = splitArray[1].TrimEnd(".mp4".ToCharArray());

        this.imagename = imageName;
        this.outpath = outImagePath;
        StartCoroutine(LoadClipAndPlay(LoadPath));
    }

    private IEnumerator LoadClipAndPlay(string path) {
        VideoClip clip = null;
        while (clip == null) {
            clip = Resources.Load<VideoClip>(path);
            yield return new WaitForSeconds(1);
        }
        Debug.Log("成功加载 Clip:" + path);
        vp.clip = clip;
        vp.Play();
    }

}
