using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Video;

public class GetImage : MonoBehaviour
{
    VideoPlayer vp;
    Texture2D videoFrameTexture;
    RenderTexture renderTexture;
    public string outpath;
    public string imagename;
    void Start()
    {
        videoFrameTexture = new Texture2D(2, 2);
        vp = GetComponent<VideoPlayer>();
        vp.waitForFirstFrame = true;
        vp.sendFrameReadyEvents = true;
        vp.frameReady += OnNewFrame;
    }
    void OnNewFrame(VideoPlayer source, long frameIdx)
    {
        renderTexture = source.texture as RenderTexture;
        if (videoFrameTexture.width != renderTexture.width || videoFrameTexture.height != renderTexture.height)
        {
            videoFrameTexture.Resize(renderTexture.width, renderTexture.height);
        }
        RenderTexture.active = renderTexture;
        videoFrameTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        videoFrameTexture.Apply();
        RenderTexture.active = null;
        vp.frameReady -= OnNewFrame;
        vp.sendFrameReadyEvents = false;
        ScaleTexture(videoFrameTexture, 800, 400, outpath + imagename);
        Debug.Log("111"+outpath + imagename);
    }

    //生成缩略图
    void ScaleTexture(Texture2D source, int targetWidth, int targetHeight, string savePath)
    {

        Texture2D result = new Texture2D(targetWidth, targetHeight, TextureFormat.ARGB32, false);

        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }
        result.Apply();
        File.WriteAllBytes(savePath, result.EncodeToPNG());
    }

    public  void GeneratePreviewImage(string videoPath, string outImagePath)
    {
        Debug.Log("111");
        var splitArray = Regex.Split(videoPath,"Resources/", RegexOptions.IgnoreCase);
        var ImageName = Regex.Split(videoPath, "Videos", RegexOptions.IgnoreCase);
        Debug.Log("222");
        string imageName = ImageName[1].TrimEnd(".mp4".ToCharArray())+ ".png";
        string LoadPath = splitArray[1].TrimEnd(".mp4".ToCharArray());

        this.imagename = imageName;
        this.outpath = outImagePath;
      
        Debug.Log(LoadPath);
        VideoClip clip = Resources.Load<VideoClip>(LoadPath);
        Debug.Log("333");
        vp.clip = clip;
        vp.Play();  
    }



}
