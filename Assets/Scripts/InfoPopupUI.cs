using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple info panel. Wire this to a Canvas with:
///   panelRoot   - the whole popup GameObject (toggled active/inactive)
///   titleText   - star name
///   bodyText    - infoText
///   image       - infoImage (optional, hidden if none)
///   closeButton - hides the panel
/// </summary>
public class InfoPopupUI : MonoBehaviour
{
    public GameObject panelRoot;
    public Text titleText;
    public Text bodyText;
    public Image image;
    public Button closeButton;

    private void Awake()
    {
        if (closeButton != null) closeButton.onClick.AddListener(Hide);
        Hide();
    }

    public void Show(StarPresenter star)
    {
        if (star == null) return;

        float earthDiameters = star.radiusInSolarRadii * 109f; // Sun's radius is ~109x Earth's
        string sizeLine = star.radiusInSolarRadii > 0f
            ? $"\nSize: ~{star.radiusInSolarRadii:0.#}x the Sun (about {earthDiameters:N0} Earths could fit across it)"
            : "";

        string body = $"{star.infoText}\n\nDistance: ~{star.distanceLightYears:N0} light years{sizeLine}";
        ShowRaw(star.starName, body, star.infoImage);
    }

    public void Show(string starName, string infoText, Sprite infoImage, float distanceLightYears)
    {
        string body = $"{infoText}\n\nDistance: ~{distanceLightYears:N0} light years";
        ShowRaw(starName, body, infoImage);
    }

    private void ShowRaw(string starName, string body, Sprite infoImage)
    {
        if (titleText != null) titleText.text = starName;
        if (bodyText != null) bodyText.text = body;

        if (image != null)
        {
            image.sprite = infoImage;
            image.enabled = infoImage != null;
        }

        if (panelRoot != null) panelRoot.SetActive(true);
    }

    public void Hide()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }
}
