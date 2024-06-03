using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject settingsPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
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
}
