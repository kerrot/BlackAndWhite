using UnityEngine;
using UnityEngine.UI;

//detect UI transparent, for circle button
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UIRaycastMask : MonoBehaviour, ICanvasRaycastFilter
{
    private Sprite _sprite;

    void Start()
    {
        _sprite = GetComponent<Image>().sprite;
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        var rectTransform = (RectTransform)transform;
        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, sp, eventCamera, out local);
        // normalize local coordinates
        var normalized = new Vector2(
            (local.x + rectTransform.pivot.x * rectTransform.rect.width) / rectTransform.rect.width,
            (local.y + rectTransform.pivot.y * rectTransform.rect.height) / rectTransform.rect.height);

        var x = Mathf.FloorToInt(_sprite.rect.width * normalized.x);
        var y = Mathf.FloorToInt(_sprite.rect.height * normalized.y);

        try
        {
            return _sprite.texture.GetPixel(x, y).a == 1;
        }
        catch (UnityException e)
        {
            Debug.LogError(e.Message + " Mask texture not readable, set your sprite to Texture Type 'Advanced' and check 'Read/Write Enabled'");
            Destroy(this);
            return false;
        }
    }
}
