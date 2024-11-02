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
        // ʾ�������������ö������  
        SetRoundedRect(0.3f, 0.5f, 0.2f, 0.4f, 0.05f); // ���������Ի�������Ҫ�ľ���  
        SetRoundedRect(0.7f, 0.5f, 0.2f, 0.4f, 0.05f);
    }

    void SetRoundedRect(float xPos, float yPos, float width, float height, float radius)
    {
        // ������ε�����λ�úʹ�С  
        Vector4 position = new Vector4(xPos, yPos, 0, 0);
        Vector4 size = new Vector4(width, height, 0, 0);

        // ���ò�������  
        material.SetVector("_RectPosition", position);
        material.SetVector("_RectSize", size);
        material.SetFloat("_Radius", radius);

        // ���²�����Ӧ�ñ仯  
        material.SetTexture("_BackgroundA", imgA.mainTexture/* ��ı���ͼA */);
        material.SetTexture("_BackgroundB", imgB.mainTexture/* ��ı���ͼB */);

        // �������ѡ�����ò��ʵĴ�������������Refresh  
    }
}
