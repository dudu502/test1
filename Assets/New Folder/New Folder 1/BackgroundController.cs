using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct PathNode
{
    public int Id;
    public Vector2 Position;
    public Vector2 Size;
    public Vector2 Radius;
}
public class BackgroundController : MonoBehaviour
{
    public Image image; // ����ʹ���Զ�����ʵ�Image���
    public Vector4[] rectPositions; // ÿ�����ε�λ��
    public Vector4[] rectSizes; // ÿ�����εĴ�С
    public Vector4[] rectRadii; // ÿ�����ε�Բ�ǰ뾶


    void Start()
    {
        if (image == null)
        {
            Debug.LogError("Image component is not assigned.");
            return;
        }

        Material material = image.material;
        if (material == null)
        {
            Debug.LogError("Material is not set on the Image component.");
            return;
        }

        //material.SetInt("_RectCount", rectPositions.Length);
        //material.SetVectorArray("_RectPositions", rectPositions);
        //material.SetVectorArray("_RectSizes", rectSizes);
        //material.SetVectorArray("_RectRadii", rectRadii);
    }
    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            UpdateRectangles22(rectPositions, rectSizes, rectRadii);
        }

    }
    public void UpdateRectangles22(Vector4[] positions, Vector4[] sizes, Vector4[] radii)
    {
        Material material = image.material;
        if (material == null)
        {
            Debug.LogError("Material is not set on the Image component.");
            return;
        }
        Debug.LogWarning("pos:" + positions.Length);
        Debug.LogWarning("size:" + sizes.Length);
        Debug.LogWarning("radii:" + radii.Length);

        material.SetInt("_RectCount", positions.Length);
        //material.SetVectorArray("_RectPositions", positions);
        //material.SetVectorArray("_RectSizes", sizes);
        //material.SetVectorArray("_RectRadii", radii);
        material.SetVector("_RectPosition0", positions[0]);
        material.SetVector("_RectSize0", sizes[0]);
        material.SetVector("_RectRadii0", radii[0]);

        material.SetVector("_RectPosition1", positions[1]);
        material.SetVector("_RectSize1", sizes[1]);
        material.SetVector("_RectRadii1", radii[1]);
    }
    public void UpdateRectangles(Vector4[] positions, Vector4[] sizes, Vector4[] radii)
    {
        Material material = image.material;
        if (material == null)
        {
            Debug.LogError("Material is not set on the Image component.");
            return;
        }
        Debug.LogWarning("pos:"+positions.Length);
        Debug.LogWarning("size:"+sizes.Length);
        Debug.LogWarning("radii:"+radii.Length);
   
        material.SetInt("_RectCount", positions.Length);
        material.SetVectorArray("_RectPositions", positions);
        material.SetVectorArray("_RectSizes", sizes);
        material.SetVectorArray("_RectRadii", radii);
    }
}
