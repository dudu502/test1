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
        // ���㵱ǰ�������Ļ�ϵ�λ��  
        Vector2 mousePos = Input.mousePosition;
        mousePos.x /= Screen.width;
        mousePos.y /= Screen.height;

        // �����λ�õ���Ϊ��������  
        Vector4 brushPosition = new Vector4(mousePos.x, mousePos.y, 0, 0);
        brushMaterial.SetVector("_BrushPosition", brushPosition);

        // ���±�ˢ��С  
        brushMaterial.SetFloat("_BrushSize", brushSize.x);
    }
}
