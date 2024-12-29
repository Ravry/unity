using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UI;

public class RadialSelectionWheel : MonoBehaviour
{
    [SerializeField] private GameObject wheel;
    [SerializeField] private List<Texture> itemIcon;
    [SerializeField] private RectTransform wheelCenter;
    [SerializeField] private float radius = 100f;
    [SerializeField] private Color[] sliceColors;
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

        for (int i = 0; i < itemCount; i++)
        {
            // Create a new Image object for the slice
            Image sliceImage = new GameObject("Slice " + i, typeof(Image)).GetComponent<Image>();
            sliceImage.transform.SetParent(wheelCenter, false);
            sliceImage.sprite = CreateSliceSprite(perItemAngle, i * perItemAngle);
            sliceImage.color = sliceColors[i % sliceColors.Length];
            sliceImage.rectTransform.sizeDelta = new Vector2(radius * 2, radius * 2);
            sliceImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);

            // Create an Image for the item icon and assign it to the slice
            Image iconImage = new GameObject("Icon " + i, typeof(Image)).GetComponent<Image>();
            iconImage.transform.SetParent(sliceImage.transform, false); // Set the icon as a child of the slice
            iconImage.sprite = TextureToSprite(itemIcon[i]); // Convert Texture to Sprite and assign
            iconImage.rectTransform.sizeDelta = new Vector2(radius * .4f, radius * .4f); // Adjust size of icon

            // Calculate position for the icon within the slice
            float angleOffset = (perItemAngle * i) + (perItemAngle / 2); // Offset to place icon in the middle of the slice

            // Convert the angle to radians
            float angleInRadians = Mathf.Deg2Rad * angleOffset;

            // Calculate the position of the icon based on the angle
            Vector2 iconPosition = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)) * (radius * 0.7f); // Half radius offset for the icon
            iconImage.rectTransform.localPosition = iconPosition;

            
            slices.Add((sliceImage, iconImage));
        }
    }

    Sprite TextureToSprite(Texture texture)
    {
        return Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    Sprite CreateSliceSprite(float sliceAngle, float startAngle)
    {
        int textureWidth = 1024;
        int textureHeight = 1024;
        Texture2D texture = new Texture2D(textureWidth, textureHeight);

        Color[] transparentPixels = new Color[textureWidth * textureHeight];
        for (int i = 0; i < transparentPixels.Length; i++)
        {
            transparentPixels[i] = Color.clear;
        }
        texture.SetPixels(transparentPixels);

        Vector2 center = new Vector2(textureWidth / 2f, textureHeight / 2f);
        float outerRadius = textureWidth / 2f;
        float innerRadius = outerRadius * 0.4f;

        float startAngleRad = Mathf.Deg2Rad * startAngle;
        float endAngleRad = Mathf.Deg2Rad * (startAngle + sliceAngle);

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                Vector2 pixelPos = new Vector2(x, y) - center;
                float distance = pixelPos.magnitude;
                float angle = Mathf.Atan2(pixelPos.y, pixelPos.x);

                if (angle < 0)
                    angle += 2 * Mathf.PI;

                if (distance >= innerRadius && distance <= outerRadius && angle >= startAngleRad && angle < endAngleRad)
                {
                    texture.SetPixel(x, y, Color.white);
                }
            }
        }

        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f));
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
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