using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using Newtonsoft.Json;

public class LanguageManager : MonoBehaviour
{
    [System.Serializable]
    public class Langs
    {
        public int textId = 0;      //Metin id
        public int[] textIndex;   //Text de�i�keni dizi indexi
        public int[] sceneIndex;  //Textin bulundu�u scene index
        public Dictionary<string, string> langDic = new Dictionary<string, string>();

        public Langs()
        {

        }
        public Langs(int textId, int[] textIndex, int[] sceneIndex, Dictionary<string, string> langDic)
        {
            this.textId = textId;
            this.textIndex = textIndex;
            this.sceneIndex = sceneIndex;
            this.langDic = langDic;
        }
    }
    public enum Languages
    {
        bg,     // Bulgarca                 iso-8859-5 
        cs,     // �ek�e                    iso-8859-2 
        da,     // Danca                    iso-8859-1 
        de,     // Almanca                  iso-8859-1 
        en,     // �ngilizce                iso-8859-1 
        es,     // Kastilya �spanyolcas�    iso-8859-1 
        el,     // Yunanca                  iso-8859-7 
        fr,     // Frans�zca                iso-8859-1 
        it,     // �talyanca                iso-8859-1 
        hu,     // Macarca                  iso-8859-2 
        nl,     // Felemenk�e               iso-8859-1
        no,     // Norve��e                 iso-8859-1 
        pl,     // Leh�e                    ISO-8859-16
        pt,     // Portekizce               iso-8859-1 
        pt_BR,  // Brazilya Portekizcesi    iso-8859-1 
        ro,     // Rumence                  ISO-8859-16
        ru,     // Rus�a                    iso-8859-5 
        fi,     // Fince                    iso-8859-1 
        sv,     // �sve��e                  iso-8859-1
        tr,     // T�rk�e                   iso-8859-9
        uk,     // Ukraynaca                iso-8859-5
        zh_CN,  // Basitle�tirilmi� �ince   x-cp50227       
        zh_TW,  // Geleneksel �ince         big5
        ja,     // Japonca                  iso-2022-jp     
        ko,     // Korece                   iso-2022-kr     
    }

    public string isoName = "ISO-8859-1";
    public string rootPath = "";

    [SerializeField] string path = "/Game/Languages/";
    [SerializeField] string jsonTextName = "languages.json";

    public Languages lan = Languages.en;
    [SerializeField] private List<Langs> langs = new List<Langs>();

    public TextMeshProUGUI[] texts;

    private void Awake()
    {
        Managers.instance.languageManager.ReadJsonLangTexts();

	}
	// Start is called before the first frame update
	void Start()
    {
#if UNITY_EDITOR
        rootPath = "";
#elif UNITY_STANDALONE
        rootPath = Application.dataPath;
#endif

    }

    // Update is called once per frame
    void Update()
    {
 
    }
    // Dil se�imi yapt�ktan sonra buras� �a�r�l�r.
    public void SelectIsoNameWithLanCode(Languages lang)
    {
        //Languages tipi g�ncellenir
        Managers.instance.languageManager.lan = lang;

        // Text dizisindeki textlere tekrar �eviri uygulan�r.
        Managers.instance.languageManager.ReadAllTextUI(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReadJsonLangTexts()
    {
        string fullPath = Application.dataPath + path + jsonTextName;
        string jsonDatas = "";
        jsonDatas = File.ReadAllText(fullPath/*, isoName*/);
        Managers.instance.languageManager.langs.Clear();
        Managers.instance.languageManager.langs = JsonConvert.DeserializeObject<List<Langs>>(jsonDatas);
        Managers.instance.languageManager.ReadAllTextUI(SceneManager.GetActiveScene().buildIndex);

    }
    private void ReadAllTextUI(int sceneId)
    {
        bool sceneCheck = false;
        for (int i = 0; i < Managers.instance.languageManager.langs.Count; i++)
        {
            sceneCheck = false;
            if (Managers.instance.languageManager.langs[i].sceneIndex.Length == 0) continue;
            for (int k = 0; k < Managers.instance.languageManager.langs[i].sceneIndex.Length; k++)
            {
                if (Managers.instance.languageManager.langs[i].sceneIndex[k] != sceneId)
                {
                    sceneCheck = true;
                    continue; //break
                }
                else //Yok
                {
                    sceneCheck = false;
                    break;
                }
            }
            if (sceneCheck)
            {
                sceneCheck = false;
                continue;
            }
            if (Managers.instance.languageManager.langs[i].textIndex[0] == -1 ||
                !CheckAllText(i))
            {
                continue;
            }
            //Managers.instance.languageManager.texts[Managers.instance.languageManager.langs[i].textIndex].text = Managers.instance.languageManager.langs[i].langDic[lan.ToString()];
            WriteTexts(i);
        }
    }
    private void WriteTexts(int langId)
    {
        for (int i = 0; i < Managers.instance.languageManager.texts.Length; i++)
        {
            foreach (int textIndex in Managers.instance.languageManager.langs[langId].textIndex)
            {
                if (i == textIndex)
                {
                    Managers.instance.languageManager.texts[textIndex].text = Managers.instance.languageManager.langs[langId].langDic[lan.ToString()];

                }
            }
        }
        
    }
    /// <summary>
    /// Text dizisindeki b�t�n textlere bakar. E�er hepsi bo�sa false, En az 1 tane dolu ise true d�nd�r�r.
    /// </summary>
    /// <param name="langId"></param>
    /// <returns></returns>
    private bool CheckAllText(int langId)
    {
        foreach(int textIndex in Managers.instance.languageManager.langs[langId].textIndex)
        {
            if(Managers.instance.languageManager.texts[textIndex] != null)
            {
                return true;
            }
            
        }

        return false;
    }
    public bool CheckLangIndex(int langIndex)
    {
        return langs.Count >= langIndex + 1;
    }
    // Her textte bu �a�r�l�r.
    // Her Text de�i�keni bir dizide kaydedilmeli ve dil de�i�ikli�i yap�ld���nda �eviri tekrar yap�lmal�.
    public string WriteLang(int textId)
    {
        //Debug.Log(Managers.instance.languageManager.langs[textId].langDic[lan.ToString()]);
        return Managers.instance.languageManager.langs[textId].langDic[lan.ToString()];
    }
}