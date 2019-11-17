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
            string destPath = Application.dataPath + "/Resources/" + outDir + "/Videos/" + prefix + "_" + dialog.fileTitle;
            Debug.Log(destPath);
            File.Copy(dialog.file, destPath, true);
        }

        /// 生成缩略图
        /// ...
    }


    // path: Resource 下的目录（包含 Videos/ 和 Thumbnails/）
    public static void Delete(string path, string name) {
        Debug.Log(Application.dataPath);
        string destVideoPath = Application.dataPath + "Resource/" + path + "/Videos/" + name + ".mp4";
        string destVideoPath1 = Application.dataPath + "/Resources/" + path + "/Videos/" + name + ".mp4.meta";
        string destPngPath = Application.dataPath + "/Resources/" + path + "/Thumbnails/" + name + ".png";
        string destPngPath1 = Application.dataPath + "/Resources/" + path + "/Thumbnails/" + name + ".png.meta";

        File.Delete(destVideoPath);
        File.Delete(destVideoPath1);
        File.Delete(destPngPath);
        File.Delete(destPngPath1);
    }

}
    

   

