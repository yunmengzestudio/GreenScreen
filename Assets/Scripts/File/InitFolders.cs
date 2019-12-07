using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InitFolders : MonoBehaviour
{
    public bool InitOnAwake = true;

    private void Awake() {
        if (InitOnAwake)
            Init();
    }

    public void Init() {
        string resourcePath = Path.Combine(Application.dataPath, "Resources");
        string[] paths = new string[] {
            resourcePath,
            Path.Combine(resourcePath, "Background"),
            Path.Combine(resourcePath, "Effect"),
            Path.Combine(resourcePath, "Product"),
            Path.Combine(resourcePath, "Background", "Videos"),
            Path.Combine(resourcePath, "Background", "Thumbnails"),
            Path.Combine(resourcePath, "Background", "Images"),
            Path.Combine(resourcePath, "Effect", "Videos"),
            Path.Combine(resourcePath, "Effect", "Thumbnails"),
            Path.Combine(resourcePath, "Product", "Videos"),
            Path.Combine(resourcePath, "Product", "Thumbnails"),
        };
        foreach (string path in paths) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
        }
    }
}
