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
    public event EventHandler<string> SelectEvent;

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
        string fullPath = Path.Combine(Application.dataPath, "Resources", ResourcePath);

        // 生成 newNames:List 当前文件夹下所有 FileSuffix 后缀的文件名
        if (Directory.Exists(fullPath)) {
            DirectoryInfo direction = new DirectoryInfo(fullPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++) {
                if (files[i].Name.EndsWith(ImageSuffix)) {
                    newNames.Add(files[i].Name.TrimEnd(ImageSuffix.ToArray()));
                }
            }
        }

        // 删除：原List有 新List没有，需要删除
        IEnumerable<string> deleteList = imageNames.Except(newNames);
        // 添加：新List有 原List没有，需要添加
        IEnumerable<string> newList = newNames.Except(imageNames);
        // imageNames 列表更新，需及时更新，否则重复点击刷新可能会造成重复添加
        imageNames = newNames;

        // 添加的处理
        foreach (string name in newList) {
            StartCoroutine(AddNewImage(name));
        }

        // 删除的处理
        foreach (string name in deleteList) {
            Destroy(images[name].gameObject);
            images.Remove(name);
        }
    }

    private IEnumerator AddNewImage(string name) {
        Sprite sprite = null;
        sprite = ResourceLoader.LoadSprite(ResourcePath + name + ImageSuffix);

        GameObject go = Instantiate(ImagePrefab, ImageRoot);
        go.GetComponent<Button>().onClick.AddListener(delegate () { Click(name); });
        go.name = name;

        Image image = go.GetComponent<Image>();
        image.sprite = sprite;
        images.Add(name, image);
        
        yield return null;
    }

    private void Click(string name = "") {
        if (name == "") {
            var clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            name = clickedButton.name;
        }
        if (SelectEvent != null) {
            SelectEvent.Invoke(this, name);
        }
    }

}
