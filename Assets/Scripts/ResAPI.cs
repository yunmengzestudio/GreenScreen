using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine.Video;
using System.Linq;
using System.Text.RegularExpressions;

public class ResAPI
{
    public enum VideoType {
        Background = 0,
        Effect = 1,
        Product = 2,
        BackgroundImage = 3
    }

    public static string[] VideoSuffix = new string[] {
        ".mp4", ".webm"
    };
    public static string[] ImageSuffix = new string[] {
        ".png", ".jpg"
    };
    private string[] FolderPaths = new string[] {
        Application.dataPath + "/Resources/Background/Videos/" ,
        Application.dataPath + "/Resources/Effect/Videos/",
        Application.dataPath + "/Resources/Product/Videos/",
        Application.dataPath + "/Resources/Background/Images/"
    };


    /// <summary>
    ///     判断 name 是否含有 video 或者 image 的后缀
    /// </summary>
    /// <param name="video">
    ///     true -> video, false -> image
    /// </param>
    public static bool HasSuffix(string name, bool video=true) {
        string[] Suffixs = video ? VideoSuffix : ImageSuffix;

        foreach (string suffix in Suffixs) {
            if (name.EndsWith(suffix))
                return true;
        }
        return false;
    }


    // 删除后缀并返回
    public static string RemoveSuffix(string name) {
        return Regex.Replace(name, @"\.\w+$", "");
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

    private static ResAPI instance = null;
    public static ResAPI Instance
    {
        get
        {
            if (instance == null)
                instance = new ResAPI();
            return instance;
        }
    }
    

    /// <summary>
    ///     根据类型，导入视频（图片）
    /// </summary>
    public IEnumerator ImportFile(VideoType type) {
        string outDir = TypeToDir(type);
        string prefix = Prefix2Type.FirstOrDefault(q => q.Value == type).Key;
        if (TypeIsVideo(type)) {
            ImportVideoFile(outDir, prefix);
        }
        else {
            ImportImageFile(outDir, prefix);
        }
        yield return null;
    }

    private static void ImportImageFile(string outDir, string prefix) {
        FileOpenDialog dialog = new FileOpenDialog();
        dialog.structSize = Marshal.SizeOf(dialog);
        dialog.filter = "png\0*.png\0jpg\0*.jpg";
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
            string name = prefix + "_" + dialog.fileTitle /*+ ".png"*/;
            string fullPath = Path.Combine(destDir, name);
            File.Copy(dialog.file, fullPath, true);
            Debug.Log("复制成功：" + fullPath);
        }
    }

    // outPath: Resource 下的路径
    private static void ImportVideoFile(string outDir, string prefix) {
        FileOpenDialog dialog = new FileOpenDialog();

        dialog.structSize = Marshal.SizeOf(dialog);

        dialog.filter = ".mp4\0*.mp4\0webm文件\0*.webm";

        dialog.file = new string(new char[256]);

        dialog.maxFile = dialog.file.Length;

        dialog.fileTitle = new string(new char[64]);

        dialog.maxFileTitle = dialog.fileTitle.Length;

        dialog.initialDir = UnityEngine.Application.dataPath;  //默认路径

        dialog.title = "Open File Dialog";

        //dialog.defExt = "mp4";//显示文件的类型
        //注意一下项目不一定要全选 但是0x00000008项不要缺少
        dialog.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;  //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR

        if (DialogShow.GetOpenFileName(dialog)) {
            string destDir = Application.dataPath + "/Resources/" + outDir + "/Videos/";
            string videoName = prefix + "_" + dialog.fileTitle;
            File.Copy(dialog.file, destDir + videoName, true);
            Debug.Log("复制成功：" + destDir + videoName);

            GameObject.Find("GetImage").GetComponent<GetImage>().GeneratePreviewImage(
                ResAPI.Instance.FillVideoPath(videoName),
                Application.dataPath + "/Resources/" + outDir + "/Thumbnails/"
                );
        }
    }


    // videoType: Resource 下的目录（包含 Videos/ 和 Thumbnails/）
    public static void Delete(VideoType type, string name) {
        string dir = Path.Combine(Application.dataPath, "Resources", TypeToDir(type));
        List<string> paths = new List<string>();

        if (TypeIsVideo(type)) {
            paths.Add(Path.Combine(dir, "Thumbnails", name) + ".png");
            paths.Add(Path.Combine(dir, "Thumbnails", name) + ".png.meta");
            foreach (string suf in VideoSuffix) {
                paths.Add(Path.Combine(dir, "Videos", name) + suf);
                paths.Add(Path.Combine(dir, "Videos", name) + suf + ".meta");
            }
        }
        else {
            foreach (string suf in ImageSuffix) {
                paths.Add(Path.Combine(dir, "Images", name) + suf);
                paths.Add(Path.Combine(dir, "Images", name) + suf + ".meta");
            }
        }

        foreach (string path in paths) {
            File.Delete(path);
        }
        Debug.Log("成功删除：" + RemoveSuffix(paths[0]));
    }
    

    /// <summary>
    ///     根据视频/图片名称补全其绝对路径，包括补全后缀
    /// </summary>
    public string FillVideoPath(string name) {
        VideoType type = Prefix2Type[name.Split('_')[0]];
        string dir = FolderPaths[(int)type];

        if (HasSuffix(name))
            name = RemoveSuffix(name);

        return searchFile(dir, name);
    }

    /// <summary>
    ///     根据 path (路径 + 文件名) 补全后缀
    /// </summary>
    public static string FillSuffix(string path) {
        Regex reg = new Regex(@"(.*)[\\/]([\w ]+)$");
        GroupCollection res = reg.Match(path).Groups;
        return searchFile(res[1].Value, res[2].Value);
    }

    // 在文件夹 dir 中查找文件名为 name 的文件全名
    private static string searchFile(string dir, string name) {
        if (Directory.Exists(dir)) {
            DirectoryInfo directory = new DirectoryInfo(dir);
            FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
            foreach (FileInfo file in files) {
                if (RemoveSuffix(file.Name) == name) {
                    return Path.Combine(dir, file.Name);
                }
            }
            return "[VideoResourceAPI.searchFile] Error: No Such File!";
        }
        return "[VideoResourceAPI.searchFile] Error: Directory Don't Exist!";
    }
}
    

   

