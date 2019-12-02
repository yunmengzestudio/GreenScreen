using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Qiniu.Util;
using Qiniu.IO;
using Qiniu.IO.Model;
using Qiniu.Http;
using UnityEngine.UI;
using System;
using RenderHeads.Media.AVProMovieCapture;
public class InternetTest : MonoBehaviour
{
    public GameObject LoadingText;
    public GameObject PicturePanel;
    public string saveKey;
    public string _filePath;
    public Image Picture;
    private static InternetTest _instance;
    public static InternetTest Instance { get { return _instance; } }
    public string IconData;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        Qiniu.Common.ZoneID zoneId = Qiniu.Common.ZoneID.CN_South;
        Qiniu.Common.Config.SetZone(zoneId, false);
    }
    private void Update()
    {
       
        Shift();
    }
    void Shift()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            IconData = "";
            for (int i = 0; i < 8; i++)
            {
                IconData += UnityEngine.Random.Range(0, 10);
            }
            StartCoroutine(Post());
        }

    }
    public void TestGet()
    {
        Debug.Log("FilePath=" + CaptureBase.LastFileSaved);
        _filePath = CaptureBase.LastFileSaved;

        StartCoroutine(Get());
    }
    IEnumerator Get()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("http://lmsc.dominikyang.vip:8080/lmscfw/service/upload");
        //webRequest.timeout = 10;
        yield return webRequest.SendWebRequest();
        //异常处理，

        if (webRequest.isHttpError || webRequest.isNetworkError)
            Debug.Log(webRequest.error);
        else
        {    
            string token =System.Text.Encoding.Default.GetString(webRequest.downloadHandler.data);
            Debug.Log(token);
            token = token.Substring(0, token.Length - 2);
            int x = token.LastIndexOf('\"');
            //Debug.Log("x=" + x);
            token =token.Substring(x,token.Length-x);
            //Debug.Log("token=" + token);
            token=token.Substring(1, token.Length-1);
            Debug.Log("token=" + token);
            UpVidio(token);
        }

    }
    public void UpVidio(string token)
    {
        saveKey = _filePath.Substring(_filePath.LastIndexOf('\\') + 1, _filePath.Length - _filePath.LastIndexOf('\\') - 1);
        string localFile = _filePath;
        Debug.Log("saveKey=" + saveKey);
        Debug.Log("localFile=" + localFile);
        // 上传策略，参见 
        UploadManager um = new UploadManager();
        LoadingText.SetActive(true);
       HttpResult result = um.UploadFile(localFile, saveKey, token);
        Debug.Log(result);
        saveKey = saveKey.Substring(0, saveKey.Length - 4);
        Debug.Log("fileName" + saveKey);
        LoadingText.SetActive(false);
    }
    public void TestPost()
    {
        StartCoroutine(Post());
    }

    IEnumerator Post()
    {
        WWWForm form = new WWWForm();
        //键值对
        form.AddField("fileName", saveKey);
        form.AddField("suffix", "mp4");
        Debug.Log("send message");
        UnityWebRequest webRequest = UnityWebRequest.Post("http://lmsc.dominikyang.vip:8080/lmscfw/service/download", form);
        yield return webRequest.SendWebRequest();
        //异常处理
        if (webRequest.isHttpError || webRequest.isNetworkError)
            Debug.Log(webRequest.error);
        else
        {
            Debug.Log(webRequest.downloadHandler.data.ToString());
            Base64ToImg(Picture, webRequest.downloadHandler.data);
            PicturePanel.SetActive(true);
        }
    }
    public void Base64ToImg(Image imgComponent,byte[] recordBase64String)
    {
        //string base64 = recordBase64String;
        //byte[] bytes = Convert.FromBase64String(base64);
        Texture2D tex2D = new Texture2D(100, 100);
        tex2D.LoadImage(recordBase64String);
        Sprite s = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f));
        imgComponent.sprite = s;
        Resources.UnloadUnusedAssets();
    }

}
