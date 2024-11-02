using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Easer : MonoBehaviour
{
    public Image maskedImage;
    public Texture2D originalTexture;

    public Rect[] masks;

    public Vector2 soft;
    void Start()
    {
      


    }

    void Update()
    {
        Vector2 mouse = Input.mousePosition;
        foreach(var r in masks)
        {
            maskedImage.SetClipRect(r, true);
            maskedImage.SetAllDirty();
        }
        maskedImage.SetClipSoftness(soft);

    }

   
}
