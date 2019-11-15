using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using System;
namespace UnityEngine.Recorder.Examples
{
    public enum RecorderControllerState
    {
        Video,
        Animation,
        ImageSequence
    }
    /// <summary>
    /// 录制
    /// </summary>
    public class RecordScreen : MonoBehaviour
    {
        RecorderController m_RecorderController;
        [Header("第一个模式，下面两个单纯观看数据，不用管")]
        public RecorderControllerState controllerState = RecorderControllerState.Video;
        public RecorderControllerSettings controllerSettings;
        public MovieRecorderSettings videoRecorder;
        private string animationOutputFolder;
        private string mediaOutputFolder;
        private void Start()
        {
            controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            m_RecorderController = new RecorderController(controllerSettings);
            animationOutputFolder = Application.dataPath + "/SampleRecordings";
            mediaOutputFolder = Application.dataPath + "../SampleRecordings";
        }
        private void Update()
        {
        }
        #region 记录视频-结束视频 位置在项目根目录RecordingScreen
        /// <summary>
        /// 开始记录视频
        /// </summary>
        /// <param name="state">默认视频</param>
        /// 开始录屏的接口 三种模式 视频、动画和截图 目前先只用视频 该函数暂时用不到
        public void StartRecorder()
        {
            //var outputFolder = Application.dataPath + "/SampleRecordings";
            switch (controllerState)
            {
                case RecorderControllerState.Video:
                    // Video
                    RecorderVideo();
                    break;
                case RecorderControllerState.Animation:
                    // Animation
                    RecorderAnimation();
                    break;
                case RecorderControllerState.ImageSequence:
                    // Image Sequence
                    RecorderImageSequence();
                    break;
                default:
                    break;

            }
            // Setup Recording
            controllerSettings.SetRecordModeToManual();
            controllerSettings.frameRate = 60.0f;
            Options.verboseMode = false;
            m_RecorderController.StartRecording();

        }
        /// <summary>
        /// 录制视频
        /// </summary>
        /// 目前只用这个
        public void RecorderVideo()
        {
            videoRecorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();
            videoRecorder.name = "My Video Recorder";
            videoRecorder.enabled = true;
            videoRecorder.outputFormat = VideoRecorderOutputFormat.MP4;
            videoRecorder.videoBitRateMode = VideoBitrateMode.Low;
            // videoRecorder.SetOutput_720p_HD(); GameViewInputSettings 修改屏幕分辨率
            videoRecorder.imageInputSettings = new CameraInputSettings
            {
                source = ImageSource.TaggedCamera,
                outputWidth = 1920,
                outputHeight = 1080,
                captureUI = false
            };
            videoRecorder.audioInputSettings.preserveAudio = true;
            string str = DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second;
            videoRecorder.outputFile = mediaOutputFolder + "/Magic_" + str;
            controllerSettings.AddRecorderSettings(videoRecorder);
        }
        ///<summary>
        /// 动画
        /// </summary>
        /// 暂时不用
        private void RecorderAnimation()
        {
            var animationRecorder = ScriptableObject.CreateInstance<AnimationRecorderSettings>();
            animationRecorder.name = "My Animation Recorder";
            animationRecorder.enabled = true;
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            animationRecorder.animationInputSettings = new AnimationInputSettings
            {
                gameObject = sphere,
                recursive = true,
            };
            animationRecorder.animationInputSettings.AddComponentToRecord(typeof(Transform));
            animationRecorder.outputFile = animationOutputFolder + "/animation_" + DefaultWildcard.GeneratePattern("GameObject") + "_" + DefaultWildcard.Take;
            controllerSettings.AddRecorderSettings(animationRecorder);
        }
        /// <summary>
        /// 图像序列
        /// </summary>
        /// 暂时不用
        private void RecorderImageSequence()
        {
            var imageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
            imageRecorder.name = "My Image Recorder";
            imageRecorder.enabled = true;
            imageRecorder.outputFormat = ImageRecorderOutputFormat.PNG;
            imageRecorder.captureAlpha = true;
            imageRecorder.outputFile = mediaOutputFolder + "/image_" + DefaultWildcard.Frame + "_" + DefaultWildcard.Take;
            imageRecorder.imageInputSettings = new CameraInputSettings
            {
                source = ImageSource.TaggedCamera,
                outputWidth = 1920,
                outputHeight = 1080,
                captureUI = true
            };
            controllerSettings.AddRecorderSettings(imageRecorder);
        }
        /// <summary>
        /// 停止录制
        /// </summary>
        public void StopRecorder()
        {
            Debug.Log("停止录制");
            m_RecorderController.StopRecording();
            controllerSettings.RemoveRecorder(videoRecorder);
        }
        #endregion
        void OnDisable()
        {
            StopRecorder();
        }
    }

}


