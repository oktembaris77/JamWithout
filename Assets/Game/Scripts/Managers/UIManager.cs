using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public Slider mouseSensSlider;
    public Slider playerHealthBar;
    // Start is called before the first frame update
    void Start()
    {
		if (PlayerPrefs.GetFloat("mouseSen") == 0) PlayerPrefs.SetFloat("mouseSen", 130);

		mouseSensSlider.value = PlayerPrefs.GetFloat("mouseSen");

	}

    // Update is called once per frame
    void Update()
    {
        Inputs();

        if (settingsPanel.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
    }
    private void Inputs()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            PanelToggle(settingsPanel);
        }
    }
    public void GoMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void PanelToggle(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }
    public void UpdateMouseSensSlider()
    {
        PlayerPrefs.SetFloat("mouseSen", mouseSensSlider.value);

        Managers.instance.gameplayManager.player.GetComponent<Character>().mouseXSpeed = mouseSensSlider.value;
		Managers.instance.gameplayManager.player.GetComponent<Character>().mouseYSpeed = mouseSensSlider.value;
	}
}
