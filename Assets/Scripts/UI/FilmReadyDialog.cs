using RenderHeads.Media.AVProMovieCapture;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class FilmReadyDialog : MonoBehaviour
{
    public AutoHide QRCode;
    public AutoHide Loading;

    public Image QRCodeImage;
    public RawImage VideoRawImage;
    public VideoPlayer Video;

    private string videoPath;
    private string videoName;
    

    // 录屏结束调用
    public void FilmReady() {
        videoPath = GameObject.FindObjectOfType<CaptureBase>().LastFilePath;
        Video.url = "file://" + videoPath;
        Video.Prepare();
        Video.prepareCompleted += Video_prepareCompleted;

        Regex reg = new Regex(@".*[\\/](.+).mp4$");
        GroupCollection res = reg.Match(videoPath).Groups;
        videoName = res[1].Value;
        GetComponent<AutoHide>().Show();
    }


    public void Download() {
        InternetTest.Instance.TestGet();
        InternetTest.Instance.complteUp += UploadComplete;
        Loading.Show();
    }
    

    // 取消上传
    public void Cancel() {
        Close();
        ResAPI.Delete(ResAPI.VideoType.Product, videoName);
    }

    // 隐藏界面
    public void Close() {
        QRCode.Hide();
        Loading.Hide();
        GetComponent<AutoHide>().Hide();
    }


    private void UploadComplete(object sender, EventArgs eventArgs) {
        InternetTest.Instance.complteUp -= UploadComplete;
        InternetTest.Instance.GetQRcode(videoName, QRCodeImage);
        QRCode.Show();
        Loading.Hide();
    }

    private void Video_prepareCompleted(VideoPlayer source) {
        // RenderTexture
        RenderTexture texture = RenderTexture.GetTemporary(source.texture.width, source.texture.height);
        source.targetTexture = texture;
        VideoRawImage.texture = texture;
        source.Play();
    }
}
