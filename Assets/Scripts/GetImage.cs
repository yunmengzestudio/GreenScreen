using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Video;
using RenderHeads.Media.AVProMovieCapture;

public class GetImage : MonoBehaviour
{
    private VideoPlayer vp;
    private Texture2D videoFrameTexture;
    private RenderTexture renderTexture;
    public string outpath;
    public string imagename;
    public event EventHandler Completed;


    void Start() {
        videoFrameTexture = new Texture2D(2, 2);
        vp = GetComponent<VideoPlayer>();
        vp.waitForFirstFrame = true;
        vp.sendFrameReadyEvents = true;
        vp.frameReady += OnNewFrame;
        vp.source = VideoSource.Url;
        vp.prepareCompleted += Vp_prepareCompleted;
    }

    private void Vp_prepareCompleted(VideoPlayer source) {
        source.Play();
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
        vp.Stop();

        vp.frameReady += OnNewFrame;
        vp.sendFrameReadyEvents = true;

        Completed?.Invoke(this, null);
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
        this.imagename = ImageName[1].TrimEnd(".mp4".ToCharArray()) + ".png";
        this.outpath = outImagePath; 

        UrlLoadAndPlay(videoPath);
    }
    

    private void UrlLoadAndPlay(string path) {
        vp.url = "file://" + path;
        vp.Prepare();
    }

    public void GenerateVideoPreviewImage()
    {
        string videopath = CaptureBase.LastFileSaved;
        string outimagepath = "Assets/Resources/Product/Thumbnails";
        GeneratePreviewImage(videopath, outimagepath);
    }
}
