using TMPro;
using UnityEngine;

/// <summary>
/// Simple helper for showing a short "not enough materials" message.
/// Hook ShowNotEnoughMaterials() to UnityEvents like Minigame2.onNotEnoughMaterials.
/// </summary>
public class NotEnoughUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private string defaultMessage = "Not enough materials";
    [SerializeField] private float showDuration = 2f;

    private float hideAtTime = -1f;

    private void Awake()
    {
        HideImmediate();
    }

    private void Update()
    {
        if (hideAtTime < 0f) return;
        if (Time.unscaledTime >= hideAtTime)
        {
            HideImmediate();
        }
    }

    public void ShowNotEnoughMaterials()
    {
        Show(defaultMessage);
    }

    public void Show(string message)
    {
        if (root != null)
            root.SetActive(true);

        if (messageText != null)
            messageText.text = string.IsNullOrWhiteSpace(message) ? defaultMessage : message;

        hideAtTime = Time.unscaledTime + Mathf.Max(0.1f, showDuration);
    }

    public void HideImmediate()
    {
        hideAtTime = -1f;
        if (root != null)
            root.SetActive(false);
    }
}
