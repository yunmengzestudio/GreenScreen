using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
[RequireComponent(typeof(AutoHide))]
public class SwitchButton : MonoBehaviour
{
    private AutoHide autoHide;
    public AutoHide Other;
    public bool ShowFirst = false;
    

    private void Start() {
        autoHide = GetComponent<AutoHide>();
        GetComponent<Button>().onClick.AddListener(Switch);
        if (ShowFirst) {
            autoHide.Show();
        }
        else {
            autoHide.Hide();
        }
    }

    public void Switch() {
        autoHide.Hide();
        Other.Show();
    }
}
