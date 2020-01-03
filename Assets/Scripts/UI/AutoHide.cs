using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


[RequireComponent(typeof(CanvasGroup))]
public class AutoHide : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public bool HideOnAwake = true;


    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        if (HideOnAwake) {
            Hide();
        }
    }

    public void Hide(float duration=0f) {
        canvasGroup.DOFade(0f, duration);
        canvasGroup.blocksRaycasts = false;
    }

    public void Show(float duration = 0f) {
        canvasGroup.DOFade(1f, duration);
        canvasGroup.blocksRaycasts = true;
    }
}
