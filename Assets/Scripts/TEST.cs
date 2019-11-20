using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TEST : MonoBehaviour
{
    public static TEST Instance;

    public Text LogText;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void Log(string msg) {
        msg += (msg.EndsWith("\n") ? "" : "\n");
        LogText.text += msg;
    }

    public void ClearLog() {
        LogText.text = "";
    }

    public void Quit() {
        Application.Quit();
    }
}
