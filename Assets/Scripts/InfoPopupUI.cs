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
        Show(star.starName, star.infoText, star.infoImage, star.distanceLightYears);
    }

    public void Show(string starName, string infoText, Sprite infoImage, float distanceLightYears)
    {
        if (titleText != null) titleText.text = starName;
        if (bodyText != null)
            bodyText.text = $"{infoText}\n\nDistance: ~{distanceLightYears:0} light years";

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
