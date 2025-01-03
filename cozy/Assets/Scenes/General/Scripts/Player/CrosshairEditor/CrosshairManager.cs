using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    public ComputeShader crosshairShader;
    public RawImage crosshairImage;

    public Vector2Int resolution = new Vector2Int(256, 256);
    public Color crosshairColor = Color.white;
    [Range(0f, 1f)]public float thickness = .2f;
    public Vector2 size = new Vector2(0.1f, 0.1f);
    public int shape = 0;

    private RenderTexture renderTexture;

    void Start()
    {
        renderTexture = new RenderTexture(resolution.x, resolution.y, 0);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
        renderTexture.filterMode = FilterMode.Point; 
        crosshairImage.texture = renderTexture;
    }

    void Update()
    {
        int kernel = crosshairShader.FindKernel("CSMain");

        crosshairShader.SetTexture(kernel, "Result", renderTexture);
        crosshairShader.SetFloats("resolution", resolution.x, resolution.y);
        crosshairShader.SetFloats("color", crosshairColor.r, crosshairColor.g, crosshairColor.b, crosshairColor.a);
        crosshairShader.SetFloat("thickness", thickness);
        crosshairShader.SetFloats("size", size.x, size.y);
        crosshairShader.SetInt("shape", shape);

        crosshairShader.Dispatch(kernel, resolution.x / 8, resolution.y / 8, 1);
    }

    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
        }
    }
}