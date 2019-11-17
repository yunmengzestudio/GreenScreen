using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine.Video;

public class OpenFileByWin32 : MonoBehaviour
{
    public SelectPanel selectPanal;
    
 
    public void OpenFile()
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
            Debug.Log(dialog.file);

            string destPath = Application.dataPath + "/Resources/Background/Videos/" + dialog.fileTitle;
            Debug.Log(destPath);
            File.Copy(dialog.file, destPath, true);
           

        }
    }

    public void Delete()
    {
        Debug.Log(Application.dataPath); 
        string destVideoPath =Application.dataPath+"/Resources/Background/Videos/"+selectPanal.selectName+".mp4";
        string destVideoPath1 = Application.dataPath + "/Resources/Background/Videos/" + selectPanal.selectName + ".mp4.meta";
        string destPngPath = Application.dataPath + "/Resources/Background/Thumbnails/" + selectPanal.selectName + ".png";
        string destPngPath1 = Application.dataPath + "/Resources/Background/Thumbnails/" + selectPanal.selectName + ".png.meta";
       
       File.Delete(destVideoPath);
        File.Delete(destVideoPath1);
        File.Delete(destPngPath);
        File.Delete(destPngPath1);
        selectPanal.UpdateAll();
    }
  


}
    

   

