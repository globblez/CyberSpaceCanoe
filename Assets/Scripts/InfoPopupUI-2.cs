using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A SMALL floating panel that appears right next to whatever was clicked
/// (a constellation's name label, or a star's info) and follows it on screen
/// if the camera moves or the stars shift in free roam. Stays hidden until
/// something is clicked.
///
/// Needs a Canvas set to "Screen Space - Overlay" (the default).
/// Fields: panelRoot (the small panel), titleText, bodyText.
/// image and closeButton are optional and can stay empty.
/// </summary>
public class InfoPopupUI : MonoBehaviour
{
    public GameObject panelRoot;
    public Text titleText;
    public Text bodyText;
    public Image image;          // optional, unused by the small panel
    public Button closeButton;   // optional

    [Header("Placement")]
    [Tooltip("Leave empty to use Camera.main.")]
    public Camera worldCamera;
    [Tooltip("Pixels between the star and the panel's edge.")]
    public Vector2 screenOffset = new Vector2(30f, 0f);

    private RectTransform _rect;
    private System.Func<Vector3> _getAnchor;

    private void Awake()
    {
        EnsureRect();
        if (closeButton != null) closeButton.onClick.AddListener(Hide);
        Hide();
    }

    /// <summary>Small info panel next to a star.</summary>
    public void Show(StarPresenter star)
    {
        if (star == null) return;

        string size = star.radiusInSolarRadii > 0f
            ? $"\nSize: ~{star.radiusInSolarRadii:0.#}x the Sun (about {star.radiusInSolarRadii * 109f:N0} Earths could fit across it)"
            : "";

        Display(() => star.AnchorWorldPosition,
                star.starName,
                $"{star.infoText}\nDistance: ~{star.distanceLightYears:N0} light years{size}");
    }

    /// <summary>Small name label next to a constellation.</summary>
    public void ShowConstellation(ConstellationRoot constellation)
    {
        if (constellation == null) return;
        Display(() => constellation.CenterWorld, constellation.DisplayName, constellation.description);
    }

    public void Hide()
    {
        _getAnchor = null;
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    private void Display(System.Func<Vector3> getAnchor, string title, string body)
    {
        _getAnchor = getAnchor;
        if (titleText != null) titleText.text = title;
        if (bodyText != null) bodyText.text = body;
        if (image != null) image.enabled = false;
        if (panelRoot != null) panelRoot.SetActive(true);
        Reposition();
    }

    private void LateUpdate() => Reposition();

    private void EnsureRect()
    {
        if (_rect == null && panelRoot != null)
        {
            _rect = panelRoot.GetComponent<RectTransform>();
            if (_rect != null) _rect.pivot = new Vector2(0f, 0.5f); // left edge sits at the star
        }
    }

    private void Reposition()
    {
        if (_getAnchor == null || panelRoot == null || !panelRoot.activeSelf) return;

        EnsureRect();
        var cam = worldCamera != null ? worldCamera : Camera.main;
        if (_rect == null || cam == null) return;

        Vector3 screen = cam.WorldToScreenPoint(_getAnchor());
        if (screen.z < 0f) return; // behind the camera

        Vector2 size = Vector2.Scale(_rect.rect.size, _rect.lossyScale); // size in pixels

        float x = screen.x + screenOffset.x;
        if (x + size.x > Screen.width)                       // no room on the right: flip to the left
            x = screen.x - screenOffset.x - size.x;
        x = Mathf.Clamp(x, 0f, Mathf.Max(0f, Screen.width - size.x));

        float halfH = size.y * 0.5f;
        float y = Mathf.Clamp(screen.y + screenOffset.y, halfH, Mathf.Max(halfH, Screen.height - halfH));

        _rect.position = new Vector3(x, y, 0f);
    }
}
