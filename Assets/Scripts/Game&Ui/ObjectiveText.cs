using UnityEngine;
using TMPro;



[RequireComponent(typeof(TextMeshProUGUI))]
public class ObjectiveText : MonoBehaviour
{
    public TextMeshProUGUI textComponent;

    // Awake is called when the script instance is being loaded.
    private void Awake()
    {
        // Automatically find the TextMeshPro component on this same GameObject
        // so you don't have to drag it in the Inspector.
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// A public function that the GameManager calls to change the displayed text.
    /// </summary>
    /// <param name="message">The new objective message to display.</param>
    public void SetObjectiveText(string message)
    {
        if (textComponent != null)
        {
            textComponent.text = message;
        }
    }

    /// <summary>
    /// A public function to clear the text, used when resetting the HUD.
    /// </summary>
    public void ClearText()
    {
        if (textComponent != null)
        {
            textComponent.text = "";
        }
    }
}