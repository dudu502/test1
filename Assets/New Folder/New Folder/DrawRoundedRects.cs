using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawRoundedRects : MonoBehaviour
{
    public Material material;
    public Image imgA;
    public Image imgB;
    // Start is called before the first frame update
    void Start()
    {
        // 示例：创建与设置多个矩形  
        SetRoundedRect(0.3f, 0.5f, 0.2f, 0.4f, 0.05f); // 调整参数以绘制您需要的矩形  
        SetRoundedRect(0.7f, 0.5f, 0.2f, 0.4f, 0.05f);
    }

    void SetRoundedRect(float xPos, float yPos, float width, float height, float radius)
    {
        // 定义矩形的中心位置和大小  
        Vector4 position = new Vector4(xPos, yPos, 0, 0);
        Vector4 size = new Vector4(width, height, 0, 0);

        // 设置材质属性  
        material.SetVector("_RectPosition", position);
        material.SetVector("_RectSize", size);
        material.SetFloat("_Radius", radius);

        // 更新材质以应用变化  
        material.SetTexture("_BackgroundA", imgA.mainTexture/* 你的背景图A */);
        material.SetTexture("_BackgroundB", imgB.mainTexture/* 你的背景图B */);

        // 这里可以选择设置材质的触发条件，比如Refresh  
    }
}
