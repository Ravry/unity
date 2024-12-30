using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class RadialSelectionWheel : MonoBehaviour
{
    [Header("Wheel Settings")]
    [SerializeField] private GameObject wheel;
    [SerializeField] private List<Texture> itemIcon;
    [SerializeField] private RectTransform wheelCenter;
    [SerializeField] private float radius = 100f;
    [SerializeField] private Color[] sliceColors;
    
    [Header("Compute Settings")]
    [SerializeField] private ComputeShader computeShader;


    private List<(Image, Image)> slices = new List<(Image, Image)>();
    private float perItemAngle;


    void Start()
    {
        GenerateWheel();
    }

    void GenerateWheel()
    {
        int itemCount = itemIcon.Count;

        if (itemCount == 0)
        {
            Debug.LogError("No items to display in the radial selection wheel.");
            return;
        }

        perItemAngle = 360.0f / itemCount;

        Sprite sliceSprite = CreateSliceSprite(1024 / 32, 1024 / 32, perItemAngle, perItemAngle);

        for (int i = 0; i < itemCount; i++)
        {
            Image sliceImage = new GameObject("Slice " + i, typeof(Image)).GetComponent<Image>();
            sliceImage.transform.SetParent(wheelCenter, false);
            sliceImage.sprite = sliceSprite;
            sliceImage.color = sliceColors[i % sliceColors.Length];
            sliceImage.rectTransform.sizeDelta = new Vector2(radius * 2, radius * 2);
            sliceImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            float rotAngle = i * perItemAngle - perItemAngle;
            sliceImage.rectTransform.localRotation = Quaternion.Euler(0, 0, rotAngle);

            Image iconImage = new GameObject("Icon " + i, typeof(Image)).GetComponent<Image>();
            iconImage.transform.SetParent(sliceImage.transform, false);
            iconImage.sprite = TextureToSprite(itemIcon[i]);
            iconImage.rectTransform.sizeDelta = new Vector2(radius * .4f, radius * .4f);
            iconImage.rectTransform.localRotation = Quaternion.Euler(0, 0, -rotAngle);

            float angleOffset = (perItemAngle / 2) + perItemAngle;
            float angleInRadians = Mathf.Deg2Rad * angleOffset;

            Vector2 iconPosition = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)) * (radius * 0.7f);
            iconImage.rectTransform.localPosition = iconPosition;
             
            slices.Add((sliceImage, iconImage));
        }
    }

    Sprite TextureToSprite(Texture texture)
    {
        return Sprite.Create(texture as Texture2D, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    Sprite CreateSliceSprite(int textureWidth, int textureHeight, float sliceAngle, float startAngle)
    {
        Texture2D resultTexture = new Texture2D(textureWidth, textureHeight);

        ComputeBuffer buffer = new ComputeBuffer(textureWidth * textureHeight, sizeof(float) * 4);

        int kernelIndex = computeShader.FindKernel("CSMain");
        computeShader.SetBuffer(kernelIndex, "Result", buffer);

        computeShader.SetInt("_TextureResolution", textureWidth);
        float startAngleRad = Mathf.Deg2Rad * startAngle;
        float endAngleRad = Mathf.Deg2Rad * (startAngle + sliceAngle);
        computeShader.SetFloat("_StartAngle", startAngleRad);
        computeShader.SetFloat("_EndAngle", endAngleRad);

        int threadGroupsX = Mathf.CeilToInt(textureWidth / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(textureHeight / 8.0f);

        computeShader.Dispatch(kernelIndex, threadGroupsX, threadGroupsY, 1);
        
        Color[] colors = new Color[textureWidth * textureWidth];
        buffer.GetData(colors);

        resultTexture.SetPixels(colors);
        resultTexture.Apply();

        buffer.Release();

        return Sprite.Create(resultTexture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f));
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Cursor.lockState = CursorLockMode.Confined  ;
            Cursor.visible = true;
            wheel.SetActive(true);
            for (int i = 0; i < slices.Count; i++)
            {
                RectTransform _rectTransform = slices[i].Item2.rectTransform;
                _rectTransform.localScale = Vector3.zero;
                _rectTransform.DOScale(Vector3.one, 0.2f);
            }
        }
            
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            wheel.SetActive(false);
        }


        if (!Input.GetKey(KeyCode.Tab))
            return;


        Vector2 localPointerPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - (Vector2)wheelCenter.position;
        float angle = Mathf.Atan2(localPointerPosition.y, localPointerPosition.x) * Mathf.Rad2Deg;

        if (angle < 0) angle += 360f;

        int selectedSliceIndex = Mathf.FloorToInt(angle / perItemAngle);
        selectedSliceIndex = Mathf.Clamp(selectedSliceIndex, 0, slices.Count - 1);

        for (int i = 0; i < slices.Count; i++) {
            Color color = new Color(1, 1, 1, .4f);
            if (i == selectedSliceIndex) {
                color = new Color(0, 0, 0, .6f);
            }
            slices[i].Item1.color = color;
        }
    }
}