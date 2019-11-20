using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine.Video;

public class VideoResourceAPI : MonoBehaviour
{
    // outPath: Resource 下的路径
    public static void OpenFile(string outDir,string prefix)
    {
        FileOpenDialog dialog = new FileOpenDialog();

        dialog.structSize = Marshal.SizeOf(dialog);

        dialog.filter = ".mp4\0*.mp4";

        dialog.file = new string(new char[256]);

        dialog.maxFile = dialog.file.Length;

        dialog.fileTitle = new string(new char[64]);

        dialog.maxFileTitle = dialog.fileTitle.Length;

        dialog.initialDir = UnityEngine.Application.dataPath;  //默认路径

        dialog.title = "Open File Dialog";

        dialog.defExt = "mp4";//显示文件的类型
        //注意一下项目不一定要全选 但是0x00000008项不要缺少
        dialog.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;  //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR

        if (DialogShow.GetOpenFileName(dialog))
        {
            string destDir = Application.dataPath + "/Resources/" + outDir + "/Videos/";
            string videoName = prefix + "_" + dialog.fileTitle;
            File.Copy(dialog.file, destDir + videoName, true);
            Debug.Log("复制成功：" + destDir + videoName);

            GameObject.Find("GetImage").GetComponent<GetImage>().GeneratePreviewImage(
                VideoResourceAPI.FillVideoPath(videoName),
                Application.dataPath + "/Resources/" + outDir + "/Thumbnails/"
                );
        }
    }


    // videoType: Resource 下的目录（包含 Videos/ 和 Thumbnails/）
    public static void Delete(string videoType, string name) {
        string destVideoPath = Application.dataPath + "/Resources/" + videoType + "/Videos/" + name + ".mp4";
        string destVideoPath1 = Application.dataPath + "/Resources/" + videoType + "/Videos/" + name + ".mp4.meta";
        string destPngPath = Application.dataPath + "/Resources/" + videoType + "/Thumbnails/" + name + ".png";
        string destPngPath1 = Application.dataPath + "/Resources/" + videoType + "/Thumbnails/" + name + ".png.meta";

        File.Delete(destVideoPath);
        File.Delete(destVideoPath1);
        File.Delete(destPngPath);
        File.Delete(destPngPath1);
        Debug.Log("成功删除："+destVideoPath);
    }

    /// <summary>
    /// 根据视频名称补全其绝对路径，包括视频名及后缀.mp4
    /// </summary>
    /// <param name="videoName"></param>
    /// <returns></returns>
    public static string FillVideoPath(string videoName) {
        string prefix = videoName.Split('_')[0];
        videoName += (videoName.EndsWith(".mp4") ? "" : ".mp4");
        switch (prefix) {
            case "BG":
                return Application.dataPath + "/Resources/Background/Videos/" + videoName;
            case "EF":
                return Application.dataPath + "/Resources/Effect/Videos/" + videoName;
            case "PRO":
                return Application.dataPath + "/Resources/Product/Videos/" + videoName;
            default:
                return "[VideoResourceAPI.FillVideoPath] Error: Video Name Error";
        }
    }
}
    

   

