using UnityEngine;
using System;

public static class ImageUtils
{
    /// <summary>
    /// Resizes an RGB24 image byte array to fit within maxWidth and maxHeight while maintaining aspect ratio.
    /// </summary>
    /// <param name="inputBytes">Input image bytes in RGB24 format (width * height * 3 bytes)</param>
    /// <param name="originalWidth">Original width of the image</param>
    /// <param name="originalHeight">Original height of the image</param>
    /// <param name="maxWidth">Max allowed width for output</param>
    /// <param name="maxHeight">Max allowed height for output</param>
    /// <returns>Resized image bytes in RGB24 format</returns>
    public static Byte[] ResizeRGBImage(Byte[] inputBytes, int originalWidth, int originalHeight, int maxWidth, int maxHeight)
    {
        // Calculate scale ratio while preserving aspect ratio
        float widthRatio = (float)maxWidth / originalWidth;
        float heightRatio = (float)maxHeight / originalHeight;
        float scale = Mathf.Min(widthRatio, heightRatio, 1f); // Never upscale, only downscale if needed

        int newWidth = Mathf.FloorToInt(originalWidth * scale);
        int newHeight = Mathf.FloorToInt(originalHeight * scale);

        // Create a Texture2D from raw RGB bytes
        Texture2D originalTex = new Texture2D(originalWidth, originalHeight, TextureFormat.RGB24, false);
        originalTex.LoadRawTextureData(inputBytes);
        originalTex.Apply();

        // Resize texture
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(originalTex, rt);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D resizedTex = new Texture2D(newWidth, newHeight, TextureFormat.RGB24, false);
        resizedTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        resizedTex.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);
        UnityEngine.Object.DestroyImmediate(originalTex);

        // Get resized image bytes
        byte[] resizedBytes = resizedTex.GetRawTextureData();

        UnityEngine.Object.DestroyImmediate(resizedTex);

        return resizedBytes;
    }
}
