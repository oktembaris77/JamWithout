using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    public static Managers instance;
    public MainMenuManager menuManager;
    public SoundManager soundManager;
    public UIManager uiManager;
    public LanguageManager languageManager;
    public GameplayManager gameplayManager;
    public PrefabManager prefabManager;
    private void Awake()
    {
        instance = this;

        TryGetComponent(out menuManager);
        TryGetComponent(out soundManager);
        TryGetComponent(out uiManager);
        TryGetComponent(out languageManager);
        TryGetComponent(out gameplayManager);
        TryGetComponent(out prefabManager);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
