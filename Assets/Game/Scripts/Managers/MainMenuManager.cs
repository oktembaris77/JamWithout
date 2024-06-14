using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject sectionsPanel;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void StartGameButton()
    {
        SceneManager.LoadScene(1);
    }
    public void PanelToggle(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }
    public void ExitButton()
    {
        Application.Quit();
    }
}
