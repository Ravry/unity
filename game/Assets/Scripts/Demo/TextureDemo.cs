using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TextureDemo : MonoBehaviour
{
    [Header("Noise Parameters")]
    [SerializeField] private int octaves = 1;
    [SerializeField] private float scalar = .1f;
        
    [Header("Texture Parameters")]
    [SerializeField] private int size = 10;

    private Material mat;
    private Texture2D tex;

    void OnValidate()
    {
        Validate();
    }

    private void Validate() {   
        if (tex != null)
        {
            this.tex = null;
        }
    
        this.mat = this.GetComponent<MeshRenderer>().sharedMaterial;
        this.tex = new Texture2D(size, size);
        this.tex.filterMode = FilterMode.Bilinear;
        this.mat.mainTexture = tex;
        ApplyTexture();
    }

    public void ApplyTexture() {
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float perlinValue = 0;
                float scale = 1.0f;

                for (int o = 1; o < (octaves + 1); o++)
                {
                    perlinValue += scale * Perlin.Noise2D((x) * scalar * (float)o, (y) * scalar * (float)o); 
                    scale /= 2.0f;
                }
                perlinValue += 1.0f;
                perlinValue /= 2.0f;
                tex.SetPixel(x, y, new Color(perlinValue, perlinValue, perlinValue));
            }

        tex.Apply();
    }
}
