using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
    /// <summary>
    ///     Resourcrs/ 目录的绝对位置（Windows）
    /// </summary>
    public static readonly string ResourcePath = Path.Combine(Application.dataPath, "Resources");


    /// <summary>
    ///     用 IO 方式从外部加载图片到 Sprite
    /// </summary>
    /// <param name="path">
    ///     可以是 Resources/ 下的路径也可以是绝对路径，需要加后缀
    /// </param>
    public static Sprite LoadSprite(string path) {
        Texture2D texture = LoadTexture(path);

        // 创建 Sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }

    /// <summary>
    ///     用 IO 方式从外部加载图片到 Texture2D
    /// </summary>
    /// <param name="path">
    ///     可以是 Resources/ 下的路径也可以是绝对路径，需要加后缀
    /// </param>
    public static Texture2D LoadTexture(string path) {
        // 路径格式处理
        if (!path.Contains(ResourcePath)) {
            path = Path.Combine(ResourcePath, path);
        }

        //创建文件读取流
        FileStream fileStream;
        try {
            fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        }
        catch (System.Exception) {
            return null;
        }
        fileStream.Seek(0, SeekOrigin.Begin);

        byte[] bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, (int)fileStream.Length);

        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        // 创建 Texture
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        return texture;
    }
}
