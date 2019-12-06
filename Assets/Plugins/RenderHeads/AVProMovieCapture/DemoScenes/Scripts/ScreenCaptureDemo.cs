using UnityEngine;

//-----------------------------------------------------------------------------
// Copyright 2012-2017 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture.Demos
{
	public class ScreenCaptureDemo : MonoBehaviour
	{
		[SerializeField]
		private AudioClip _audioBG;

		[SerializeField]
		private AudioClip _audioHit;

		[SerializeField]
		private float _speed = 1.0f;

		[SerializeField]
		private CaptureBase _capture;

		[SerializeField]
		private GUISkin _guiSkin;

		[SerializeField]
		private bool _spinCamera = true;

        public Camera RecordCamera;
        public Rect RecordCameraRect;

		// State
		private float _timer;

		private void Start()
		{
			// Play music track
			if (_audioBG != null)
			{
				AudioSource.PlayClipAtPoint(_audioBG, Vector3.zero);
			}
		}

		private void Update()
		{
			// Press the S key to trigger audio and background color change - useful for testing A/V sync
			if (Input.GetKeyDown(KeyCode.S))
			{
				if (_audioHit != null && _capture.IsCapturing())
				{
					AudioSource.PlayClipAtPoint(_audioHit, Vector3.zero);
					Camera.main.backgroundColor = new Color(Random.value, Random.value, Random.value, 0);
				}
			}

			// ESC to stop capture and quit
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (_capture != null && _capture.IsCapturing())
				{
					_capture.StopCapture();
				}
				else
				{
					Application.Quit();
				}
			}

			// Spin the camera around
			if (_spinCamera && Camera.main != null)
			{
				Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, 20f * Time.deltaTime * _speed);
			}
		}

		private void OnGUI()
		{
			GUI.skin = _guiSkin;
			Rect r = new Rect(Screen.width - 108, 64, 128, 28);
			GUI.Label(r, "Frame " + Time.frameCount);
		}
        //private Texture2D CaptureCamera(Camera camera, Rect rect)
        //{
        //    // 创建一个RenderTexture对象
        //    RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
        //    // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机
        //    camera.targetTexture = rt;
        //    camera.Render();
        //    //ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。
        //    //ps: camera2.targetTexture = rt;
        //    //ps: camera2.Render();
        //    //ps: -------------------------------------------------------------------

        //    // 激活这个rt, 并从中中读取像素。
        //    RenderTexture.active = rt;
        //    Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        //    screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素
        //    screenShot.Apply();

        //    // 重置相关参数，以使用camera继续在屏幕上显示
        //    camera.targetTexture = null;
        //    //ps: camera2.targetTexture = null;
        //    RenderTexture.active = null; // JC: added to avoid errors
        //    GameObject.Destroy(rt);
        //    // 最后将这些纹理数据，成一个png图片文件
        //    byte[] bytes = screenShot.EncodeToPNG();
        //    string filename = Application.dataPath + "/Screenshot.png";
        //    System.IO.File.WriteAllBytes(filename, bytes);
        //    Debug.Log(string.Format("截屏了一张照片: {0}", filename));

        //    return screenShot;
        //}

        //public void CapturePreviewImage()
        //{
        //    //RecordCameraRect = GetComponent<Camera>().rect;
        //    CaptureCamera(RecordCamera, RecordCameraRect);
        //}
	}
}