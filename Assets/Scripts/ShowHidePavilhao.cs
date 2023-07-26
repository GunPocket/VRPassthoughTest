using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHidePavilhao : MonoBehaviour
{
    private bool show = true;
    public GameObject pavilhao;

    public void ShowHide() {
        pavilhao.gameObject.SetActive(show);
        show = !show;
        print(show);
    }
}
