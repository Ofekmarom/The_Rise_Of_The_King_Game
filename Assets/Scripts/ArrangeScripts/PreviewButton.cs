using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This class manages the preview functionality, allowing users to show or hide a preview image
/// when interacting with a button or clicking elsewhere on the screen.
/// </summary>
public class PreviewManager : MonoBehaviour
{
    // Reference to the button that toggles the preview image visibility
    public Button previewButton;

    // Reference to the preview image that will be shown or hidden
    public Image previewImage;

    // Boolean flag to track the current visibility state of the preview image
    private bool isPreviewVisible = false;

    /// <summary>
    /// Called when the script is initialized.
    /// Ensures the preview image is hidden by default and adds a listener to the button.
    /// </summary>
    void Start()
    {
        // Set the preview image to be fully transparent (hidden) by default
        previewImage.color = new Color(previewImage.color.r, previewImage.color.g, previewImage.color.b, 0);
        Debug.Log("PreviewManager initialized: Preview image hidden by default. isPreviewVisible = " + isPreviewVisible);
    }

    /// <summary>
    /// Called every frame to check for user interactions.
    /// If the preview image is visible and the user clicks anywhere on the screen,
    /// the preview image will be hidden.
    /// </summary>
    void Update()
    {
        // Check if the preview image is visible and if the user clicked
        if (isPreviewVisible && Input.GetMouseButtonDown(0))
        {
            // Verify that the click was not on the specific button
            if (!IsPointerOverSpecificButton(previewButton))
            {
                Debug.Log("Screen clicked: Hiding preview image. isPreviewVisible = " + isPreviewVisible);
                HidePreview();
            }
        }
    }

    /// <summary>
    /// Checks if the user's click is on the specific button.
    /// </summary>
    /// <param name="button">The button to check for interaction.</param>
    /// <returns>True if the click is on the specified button, false otherwise.</returns>
    private bool IsPointerOverSpecificButton(Button button)
    {
        // Check if the pointer is over any UI object
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // Get the currently selected UI object
            GameObject currentObject = EventSystem.current.currentSelectedGameObject;

            // Verify if the current object is the specified button's GameObject
            if (currentObject != null && currentObject == button.gameObject)
            {
                return true; // The click was on the specified button
            }
        }
        return false; // The click was not on the specified button
    }

    /// <summary>
    /// Toggles(החלף) the visibility of the preview image when the button is clicked.
    /// </summary>
    public void TogglePreview()
    {
        if (isPreviewVisible)
        {
            Debug.Log("Button clicked: Hiding preview image.");
            HidePreview(); // Hide the preview image if it is currently visible
        }
        else
        {
            Debug.Log("Button clicked: Showing preview image.");
            ShowPreview(); // Show the preview image if it is currently hidden
        }
    }

    /// <summary>
    /// Makes the preview image fully visible by setting its alpha value to 1.
    /// </summary>
    public void ShowPreview()
    {
        // Set the preview image to be fully visible (opaque)
        previewImage.color = new Color(previewImage.color.r, previewImage.color.g, previewImage.color.b, 1f);
        isPreviewVisible = true;

        Debug.Log("Preview image shown: Fully visible. isPreviewVisible = " + isPreviewVisible);
        Debug.Log("Color set to: " + previewImage.color);
    }

    /// <summary>
    /// Hides the preview image by setting its alpha value to 0.
    /// </summary>
    void HidePreview()
    {
        // Set the preview image to be fully transparent (hidden)
        previewImage.color = new Color(previewImage.color.r, previewImage.color.g, previewImage.color.b, 0);
        isPreviewVisible = false;

        Debug.Log("Preview image hidden: Fully transparent. isPreviewVisible = " + isPreviewVisible);
    }
}
