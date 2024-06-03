using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PergPipelineManager : MonoBehaviour
{

    /*
    @@@PergPanel
    Start:
    PanelSlotSpawner -> Her panelin slotları instantiate edilir, slots listesine slotlar eklenir                                                       /OTO\
    AddMe -> Her panel ItemDatabaseManager'e eklenir.                                                                                                  /OTO\
    Items -> Items listesi doldurulur.(Databaseden vb.)                                                                                                /MANUEL\
    FillInventorySlot -> Slots listesindeki slotlara items'deki itemler koyulur                                                                        /MANUEL\
    @@@PanelSlot
    SlotActions -> Slotlardaki itemler koyulduğu için artık slot olayları çağrılabilir. İtemler konmadan çağrılsaydı, slotlardaki itemleri göremezdik. /MANUEL\
    */
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
