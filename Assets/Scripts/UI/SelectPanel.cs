using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class SelectPanel : MonoBehaviour
{
    private List<string> imageNames = new List<string>();
    private Dictionary<string, Image> images = new Dictionary<string, Image>();

    public GameObject ImagePrefab;
    public Transform ImageRoot;

    [Header("Conf")]
    public string ResourcePath = "Background/Thumbnails/";
    public string ImageSuffix = ".png";


    private void Start() {
        UpdateAll();
    }


    public void UpdateAll() {
        List<string> newNames = new List<string>();
        string fullPath = "Assets/Resources/" + ResourcePath + (ResourcePath.EndsWith("/") ? "" : "/");

        // 生成 newNames:List 当前文件夹下所有 FileSuffix 后缀的文件名
        if (Directory.Exists(fullPath)) {
            DirectoryInfo direction = new DirectoryInfo(fullPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++) {
                if (files[i].Name.EndsWith(ImageSuffix))
                    newNames.Add(files[i].Name.TrimEnd(ImageSuffix.ToArray()));
            }
        }

        // 删除：原List有 新List没有，需要删除
        IEnumerable<string> deleteList = imageNames.Except(newNames);
        foreach (string name in deleteList) {
            Destroy(images[name].gameObject);
        }


        // 添加：新List有 原List没有，需要添加
        IEnumerable<string> newList = newNames.Except(imageNames);
        foreach (string name in newList) {
            GameObject go = Instantiate(ImagePrefab, ImageRoot);
            go.GetComponent<Button>().onClick.AddListener(Click);
            go.name = name;

            Sprite sprite = Resources.Load<Sprite>(ResourcePath + name);
            Image image = go.GetComponent<Image>();
            image.sprite = sprite;
            images.Add(name, image);

            if (sprite == null) {
                Debug.Log("Fail to Load Sprite 【" + ResourcePath + name + "】 sprite->" + sprite);
            }
        }

        // imageNames 列表更新
        imageNames = newNames;
    }


    private void Click() {
        var clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        string selectName = clickedButton.name;
        Debug.Log("[SelectPanel] 选择 Image：" + selectName);
        // ..Play(selectName);
    }

}
