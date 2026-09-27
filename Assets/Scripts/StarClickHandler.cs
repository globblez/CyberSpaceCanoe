using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Put this on the star prefab alongside a Collider (3D) or Collider2D.
/// Fires OnClicked with this star's data when the player clicks/taps it,
/// unless the click is currently over UI.
/// </summary>
[RequireComponent(typeof(Collider))]
public class StarClickHandler : MonoBehaviour
{
    [HideInInspector] public StarInfo starInfo;
    public event System.Action<StarInfo> OnClicked;

    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return; // ignore clicks that land on UI

        if (starInfo != null) OnClicked?.Invoke(starInfo);
    }
}
