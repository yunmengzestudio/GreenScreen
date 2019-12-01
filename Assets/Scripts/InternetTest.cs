using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Qiniu.Util;
using Qiniu.IO;
using Qiniu.IO.Model;
using Qiniu.Http;
public class InternetTest : MonoBehaviour
{
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
    IEnumerator Get()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("http://129.204.80.144::8080/service/upload");

        yield return webRequest.SendWebRequest();
        //异常处理，
        if (webRequest.isHttpError || webRequest.isNetworkError)
            Debug.Log(webRequest.error);
        else
        {
            Debug.Log(webRequest.downloadHandler.text);
            //UpVidio(webRequest.downloadHandler.text);
        }

    }
    /*
        public void UpVidio(string token)
        {
              /// <summary>
            /// 简单上传-上传小文件
            /// </summary>
        
                // 生成(上传)凭证时需要使用此Mac
                // 这个示例单独使用了一个Settings类，其中包含AccessKey和SecretKey
                // 实际应用中，请自行设置您的AccessKey和SecretKey
                Mac mac = new Mac(Settings.AccessKey, Settings.SecretKey);
                string bucket = "test";
                string saveKey = "1.png";
                string localFile = "D:\\QFL\\1.png";
                // 上传策略，参见 
                // https://developer.qiniu.com/kodo/manual/put-policy
                PutPolicy putPolicy = new PutPolicy();
                // 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
                // putPolicy.Scope = bucket + ":" + saveKey;
                putPolicy.Scope = bucket;
                // 上传策略有效期(对应于生成的凭证的有效期)          
                putPolicy.SetExpires(3600);
                // 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除
                putPolicy.DeleteAfterDays = 1;
                // 生成上传凭证，参见
                // https://developer.qiniu.com/kodo/manual/upload-token            
                string jstr = putPolicy.ToJsonString();
                string token = Auth.CreateUploadToken(mac, jstr);
                UploadManager um = new UploadManager();
                HttpResult result = um.UploadFile(localFile, saveKey, token);
                Console.WriteLine(result);

        }*/
    public void TestPost()
    {
        StartCoroutine(Post());
    }

    IEnumerator Post()
    {
        WWWForm form = new WWWForm();
        //键值对
        form.AddField("address", "0xde5de661bec2d4dd3769b5cf6bb81f9852cc22bb");
        form.AddField("equipId", IconData);
        Debug.Log("send message");
        //UnityWebRequest webRequest = UnityWebRequest.Post("http://192.168.1.100:8080", form);
        UnityWebRequest webRequest = UnityWebRequest.Post("http://192.168.43.43:8080/game/add", form);
        yield return webRequest.SendWebRequest();
        //异常处理
        if (webRequest.isHttpError || webRequest.isNetworkError)
            Debug.Log(webRequest.error);
        else
        {
            Debug.Log(webRequest.downloadHandler.text);
        }
    }

}
