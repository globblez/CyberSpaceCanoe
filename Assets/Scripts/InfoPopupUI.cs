using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple info panel. Wire this to a Canvas with:
///   panelRoot   - the whole popup GameObject (toggled active/inactive)
///   titleText   - star name
///   bodyText    - infoText
///   image       - infoImage (optional, hidden if none)
///   closeButton - hides the panel
/// Subscribe ConstellationController.OnStarClicked to Show(StarInfo).
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

    public void Show(StarInfo star)
    {
        if (star == null) return;

        if (titleText != null) titleText.text = star.starName;
        if (bodyText != null)
            bodyText.text = $"{star.infoText}\n\nDistance: ~{star.distanceLightYears:0} light years";

        if (image != null)
        {
            image.sprite = star.infoImage;
            image.enabled = star.infoImage != null;
        }

        if (panelRoot != null) panelRoot.SetActive(true);
    }

    public void Hide()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }
}
