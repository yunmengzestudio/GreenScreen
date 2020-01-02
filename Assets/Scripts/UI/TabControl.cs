using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabControl : MonoBehaviour
{
    public GameObject[] Contents;
    public Button[] Buttons;
    public Color SelectBtnColor;
    public Color NotSelectBtnColor;

    private List<CanvasGroup> canvasGroups = new List<CanvasGroup>();
    private List<Image> ButtonImages = new List<Image>();
    public int Index { get; private set; } = 0;
    public event EventHandler IndexChanged;


    private void Start() {
        InitCanvasGroups();
        InitButtons();
    }

    private void InitCanvasGroups() {
        // CanvasGroup 初始化
        foreach (GameObject go in Contents) {
            CanvasGroup canvas = go.GetComponent<CanvasGroup>();
            if (canvas == null) {
                canvas = go.AddComponent<CanvasGroup>();
            }
            canvasGroups.Add(canvas);
        }

        // Content 隐藏
        if (canvasGroups.Count > 0) {
            canvasGroups[0].alpha = 1f;
            canvasGroups[0].blocksRaycasts = true;
        }
        for(int i = 1; i < canvasGroups.Count; i++) {
            canvasGroups[i].alpha = 0f;
            canvasGroups[i].blocksRaycasts = false;
        }
    }

    private void InitButtons() {
        // Button AddListener
        for (int i = 0; i < Buttons.Length; i++) {
            int tmpIndex = i;   // 在 delegate 中用, Reference: https://answers.unity.com/questions/912202/buttononclicaddlistenermethodobject-wrong-object-s.html
            Buttons[i].onClick.AddListener(delegate { SwitchTab(tmpIndex); });
            ButtonImages.Add(Buttons[i].GetComponent<Image>());
        }
        
        // Button 隐藏
        if (ButtonImages.Count > 0) {
            ButtonImages[0].color = SelectBtnColor;
        }
        for (int i = 1; i < ButtonImages.Count; i++) {
            ButtonImages[i].color = NotSelectBtnColor;
        }
    }

    public void SwitchTab(int index) {
        if (index == this.Index || index > canvasGroups.Count || index < 0)
            return;

        // Content
        canvasGroups[this.Index].alpha = 0f;
        canvasGroups[this.Index].blocksRaycasts = false;
        canvasGroups[index].alpha = 1f;
        canvasGroups[index].blocksRaycasts = true;

        // 按钮
        if (index < ButtonImages.Count) {
            ButtonImages[this.Index].color = NotSelectBtnColor;
            ButtonImages[index].color = SelectBtnColor;
        }
        
        this.Index = index;
        IndexChanged?.Invoke(this, null);
    }

}
