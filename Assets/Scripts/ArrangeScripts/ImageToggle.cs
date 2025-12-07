using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ImageToggle is a UI script that ensures an image is shown when the scene starts,
/// and hides it when a button is clicked.
/// </summary>
public class ImageToggle : MonoBehaviour
{
    /// <summary>
    /// The GameObject containing the image to be shown/hidden.
    /// </summary>
    public GameObject imageObject;

    /// <summary>
    /// The button used to hide the image.
    /// </summary>
    public Button hideButton;

    void Start()
    {
        // Activate the image GameObject at scene start
        if (imageObject != null)
        {
            imageObject.SetActive(true);
        }

        // Hook up the button
        if (hideButton != null)
        {
            hideButton.onClick.AddListener(HideImage);
        }
    }

    /// <summary>
    /// Hides the image when the button is clicked.
    /// </summary>
    void HideImage()
    {
        if (imageObject != null)
        {
            imageObject.SetActive(false);
        }
    }
}
