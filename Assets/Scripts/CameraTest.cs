using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CameraTest : MonoBehaviour
{
    WebCamTexture tex;
    public float aspect = 9f / 16f;
    public MeshRenderer ma;
 
    void Start()
    {
        StartCoroutine(OpenCamera());
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator OpenCamera()
    {
        //等待用户允许访问
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        //如果用户允许访问，开始获取图像        
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            //先获取设备
            WebCamDevice[] device = WebCamTexture.devices;

            string deviceName = device[0].name;
            //然后获取图像
            tex = new WebCamTexture(deviceName);
            //将获取的图像赋值
            ma.material.mainTexture = tex;
            //开始实施获取
            tex.Play();

        }
    }
   
    
   
}
