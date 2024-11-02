using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrushSampleController : MonoBehaviour
{
    public Material brushMaterial;
    public Vector2 brushSize = new Vector2(0.1f, 0.1f);
    public Texture2D baseTexture;
    public Texture2D brushTexture;

    void Start()
    {
        baseTexture = GetComponent<Image>().mainTexture as Texture2D;
        brushMaterial.SetTexture("_BaseTex", baseTexture);
        brushMaterial.SetTexture("_BrushTex", brushTexture);
    }

    void Update()
    {
        // 计算当前鼠标在屏幕上的位置  
        Vector2 mousePos = Input.mousePosition;
        mousePos.x /= Screen.width;
        mousePos.y /= Screen.height;

        // 将鼠标位置调整为纹理坐标  
        Vector4 brushPosition = new Vector4(mousePos.x, mousePos.y, 0, 0);
        brushMaterial.SetVector("_BrushPosition", brushPosition);

        // 更新笔刷大小  
        brushMaterial.SetFloat("_BrushSize", brushSize.x);
    }
}
