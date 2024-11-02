using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErasePixels2 : MonoBehaviour
{
    public Texture2D texture; // Assign your Texture2D in the inspector  
    private Color[] originalPixels; // Store the original pixels for restoration  
    private Color[] erasedPixels; // Store the erased pixels  
    public float slowMotionDelay = 0.05f; // Delay in seconds for slow-motion effect  

    void Start()
    {
        texture = GetComponent<Image>().mainTexture as Texture2D;
        // Ensure the texture is readable  
        if (texture != null)
        {
            originalPixels = texture.GetPixels();
            erasedPixels = new Color[originalPixels.Length];
        }
    }

    [ContextMenu("E")]
    public void OnEraseButtonClicked()
    {
        // Example: Erase a circular area at (100, 100) with a radius of 20  
        StartEraseArea(100, 100, 30);
    }
    [ContextMenu("R")]
    public void OnRestoreButtonClicked()
    {
        // Example: Restore the same area  
        StartRestoreArea(100, 100, 30);
    }
    public void StartEraseArea(int centerX, int centerY, int radius)
    {
        //tartCoroutine(EraseAreaCoroutine(centerX, centerY, radius));
        EraseAreaCoroutine(centerX, centerY, radius);
    }

    private void EraseAreaCoroutine(int centerX, int centerY, int radius)
    {
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y <= radius * radius) // Keep within the circular shape  
                {
                    int px = centerX + x;
                    int py = centerY + y;

                    if (px >= 0 && px < texture.width && py >= 0 && py < texture.height)
                    {
                        int index = py * texture.width + px;
                        // Store original pixel color  
                        erasedPixels[index] = originalPixels[index];

                        // Set pixel to transparent (erase it)  
                        originalPixels[index] = Color.clear;

                        texture.SetPixels(originalPixels);
                        texture.Apply();
                    }
                }
            }
            //yield return null; // Slow-motion effect  
        }
    }

    public void StartRestoreArea(int centerX, int centerY, int radius)
    {
        //StartCoroutine(RestoreAreaCoroutine(centerX, centerY, radius));
        RestoreAreaCoroutine(centerX, centerY, radius);
    }

    private void RestoreAreaCoroutine(int centerX, int centerY, int radius)
    {
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y <= radius * radius) // Keep within the circular shape  
                {
                    int px = centerX + x;
                    int py = centerY + y;

                    if (px >= 0 && px < texture.width && py >= 0 && py < texture.height)
                    {
                        int index = py * texture.width + px;
                        // Restore the original pixel color  
                        originalPixels[index] = erasedPixels[index];

                        texture.SetPixels(originalPixels);
                        texture.Apply();
                    }
                }
            }
            //yield return null; // Slow-motion effect  
        }
    }
}
