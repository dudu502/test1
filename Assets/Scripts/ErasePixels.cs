using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErasePixels : MonoBehaviour
{
    public Texture2D texture; // Assign your Texture2D in the inspector  
    private Color[] originalPixels; // Store the original pixels for restoration  
    private Color[] erasedPixels; // Store the erased pixels  
    public RectInt[] MaskRects;
    void Start()
    {
        texture = GetComponent<Image>().mainTexture as Texture2D;
        // Ensure the texture is readable  
        if (texture != null)
        {
            originalPixels = texture.GetPixels();
            erasedPixels = new Color[originalPixels.Length];

            // Example area to erase  
            
            foreach(var r in MaskRects)
            {
                EraseArea(r.x, r.y, r.width, r.height);

            }
            
        }
    }

    void EraseArea(int startX, int startY, int width, int height)
    {
        for (int y = startY; y < startY + height; y++)
        {
            for (int x = startX; x < startX + width; x++)
            {
                if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
                {
                    // Store the original pixel color  
                    int index = y * texture.width + x;
                    erasedPixels[index] = originalPixels[index];

                    // Set pixel to transparent  
                    originalPixels[index] = Color.clear; // Erasing by setting to transparent  
                }
            }
        }
        texture.SetPixels(originalPixels);
        texture.Apply();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            foreach (var r in MaskRects)
                RestoreArea(r.x, r.y,r.width,r.height);
        }   
    }
    public void RestoreArea(int startX, int startY, int width, int height)
    {
        for (int y = startY; y < startY + height; y++)
        {
            for (int x = startX; x < startX + width; x++)
            {
                if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
                {
                    int index = y * texture.width + x;
                    // Restore the original pixel color  
                    originalPixels[index] = erasedPixels[index];
                }
            }
        }
        texture.SetPixels(originalPixels);
        texture.Apply();
    }
}
