using UnityEngine;

public class SettingsController : MonoBehaviour
{
    public void CloseSettings()
    {
        Transform menuButtons = transform.root.Find("MainMenuUI/MenuButtons");
        if (menuButtons != null)
        {
            menuButtons.gameObject.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}