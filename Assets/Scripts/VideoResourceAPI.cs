using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine.Video;
using System.Linq;

public class VideoResourceAPI : MonoBehaviour
{
    public enum VideoType {
        Background = 0,
        Effect = 1,
        Product = 2,
        BackgroundImage = 3
    }

    /// <summary>
    ///     判断该类型是否为视频
    /// </summary>
    public static bool TypeIsVideo(VideoType type) {
        return (int)type <= 2;
    }

    /// <summary>
    ///     根据类型，返回所属文件夹名 (Background,Effect,Product)
    /// </summary>
    public static string TypeToDir(VideoType type) {
        if (TypeIsVideo(type)) {
            return type.ToString("g");
        }
        switch (type) {
            case VideoType.BackgroundImage:
                return "Background";
            default:
                return "VideoTypeError";
        }
    }

    public static Dictionary<string, VideoType> Prefix2Type = new Dictionary<string, VideoType>() {
        { "BG", VideoType.Background },
        { "EF", VideoType.Effect },
        { "PRO", VideoType.Product },
        { "BGI", VideoType.BackgroundImage }
    };
    public static Dictionary<VideoType, string> Type2Tag = new Dictionary<VideoType, string>() {
        { VideoType.Background, "背景" },
        { VideoType.Effect, "特效" },
        { VideoType.Product, "成品" },
        { VideoType.BackgroundImage, "背景图" }
    };

    /// <summary>
    ///     根据类型，导入视频（图片）
    /// </summary>
    public static IEnumerator ImportFile(VideoType type) {
        string outDir = TypeToDir(type);
        string prefix = Prefix2Type.FirstOrDefault(q => q.Value == type).Key;
        if (TypeIsVideo(type)) {
            OpenFile(outDir, prefix);
        }
        else {
            ImportImageFile(outDir, prefix);
        }
        yield return null;
    }

    private static void ImportImageFile(string outDir, string prefix) {
        FileOpenDialog dialog = new FileOpenDialog();
        dialog.structSize = Marshal.SizeOf(dialog);
        dialog.filter = ".png\0*.png";
        dialog.file = new string(new char[256]);
        dialog.maxFile = dialog.file.Length;
        dialog.fileTitle = new string(new char[64]);
        dialog.maxFileTitle = dialog.fileTitle.Length;
        dialog.initialDir = Application.dataPath;  //默认路径
        dialog.title = "Open File Dialog";
        dialog.defExt = "png";  //显示文件的类型
        dialog.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;  //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR

        if (DialogShow.GetOpenFileName(dialog)) {
            string destDir = Path.Combine(Application.dataPath, "Resources", outDir, "Images/");
            string name = prefix + "_" + dialog.fileTitle;
            string fullPath = Path.Combine(destDir, name);
            File.Copy(dialog.file, fullPath, true);
            Debug.Log("复制成功：" + fullPath);
        }
    }

    // outPath: Resource 下的路径
    private static void OpenFile(string outDir,string prefix)
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
    public static void Delete(VideoType type, string name) {
        string dir = Path.Combine(Application.dataPath, "Resources", TypeToDir(type));
        string[] paths;
        
        if (TypeIsVideo(type)) {
            paths = new string[] {
                Path.Combine(dir, "Videos", name) + ".mp4",
                Path.Combine(dir, "Videos", name) + ".mp4.meta",
                Path.Combine(dir, "Thumbnails", name) + ".png",
                Path.Combine(dir, "Thumbnails", name) + ".png.meta"
            };
        }
        else {
            paths = new string[] {
                Path.Combine(dir, "Images", name) + ".png",
                Path.Combine(dir, "Images", name) + ".png.meta"
            };
        }

        foreach (string path in paths) {
            File.Delete(path);
        }
        Debug.Log("成功删除：" + paths[0]);
    }

    /// <summary>
    /// 根据视频名称补全其绝对路径，包括视频名及后缀.mp4；图片补全.png
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string FillVideoPath(string name) {
        string prefix = name.Split('_')[0];
        if (TypeIsVideo(Prefix2Type[prefix])) {
            name += (name.EndsWith(".mp4") ? "" : ".mp4");
        }
        else {
            name += (name.EndsWith(".png") ? "" : ".png");
        }
        switch (prefix) {
            case "BG":
                return Application.dataPath + "/Resources/Background/Videos/" + name;
            case "EF":
                return Application.dataPath + "/Resources/Effect/Videos/" + name;
            case "PRO":
                return Application.dataPath + "/Resources/Product/Videos/" + name;
            case "BGI":
                return Application.dataPath + "/Resources/Background/Images/" + name;
            default:
                return "[VideoResourceAPI.FillVideoPath] Error: Video Name Error";
        }
    }
}
    

   

