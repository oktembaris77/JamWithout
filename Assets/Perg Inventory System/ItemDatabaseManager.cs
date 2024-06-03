using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDatabaseManager : MonoBehaviour
{
  /*  public class Loots
    {
        public string lootName;
        public string lootDes;
        public int icon;
        public ItemType itemType;
        public int maxStack;

        public Loots(string lootName, string lootDes, int icon, ItemType itemType, int maxStack)
        {
            this.lootName = lootName;
            this.lootDes = lootDes;
            this.icon = icon;
            this.itemType = itemType;
            this.maxStack = maxStack;
        }
    }
    public struct ItemMetaDatas
    {
        public int itemId;
        public int itemValue;
        public ItemMetaDatas(int itemId, int itemValue)
        {
            this.itemId = itemId;
            this.itemValue = itemValue;
        }
    }*/

    public static ItemDatabaseManager instance;
    private GameObject panelSpawner;
    public Dictionary<int, PanelCreator> panels = new Dictionary<int, PanelCreator>();

    public Dictionary<int, Item> ItemDatabaseList = new Dictionary<int, Item>();
    public Sprite[] sprites;

    public bool writedItemDb = true;

    private void Awake()
    {
        instance = this;
        writedItemDb = true;
        panelSpawner = GameObject.Find("PergPanels");
    }
    // Start is called before the first frame update
    void Start()
    {
        AddPanels();
    }

    // Update is called once per frame
    void Update()
    {

		if (Input.GetKeyDown(KeyCode.K))
        {
            for (int i = 0; i < ItemDatabaseManager.instance.panels[0].panelObject.GetComponent<PergPanel>().items.Count; i++)
            {
                Debug.Log(ItemDatabaseManager.instance.panels[0].panelObject.GetComponent<PergPanel>().items[i].itemId + " slotId: " + ItemDatabaseManager.instance.panels[0].panelObject.GetComponent<PergPanel>().items[i].slotId
                  + " itemvalue: " + ItemDatabaseManager.instance.panels[0].panelObject.GetComponent<PergPanel>().items[i].itemValue);
            }

		}

		if (writedItemDb)
        {
            WriteItemDb();
            writedItemDb = false;
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            FillPanel(0, new Item().NewItem(ItemDatabaseList[0]));
            FillPanel(0, new Item().NewItem(ItemDatabaseList[1]));

			FillPanel(1, new Item().NewItem(ItemDatabaseList[0]));
			FillPanel(1, new Item().NewItem(ItemDatabaseList[1]));

			FillPanel(2, new Item().NewItem(ItemDatabaseList[0]));
			FillPanel(2, new Item().NewItem(ItemDatabaseList[1]));

			panels[0].panelObject.GetComponent<PergPanel>().FillInventorySlot();
            panels[1].panelObject.GetComponent<PergPanel>().FillInventorySlot();
            panels[2].panelObject.GetComponent<PergPanel>().FillInventorySlot();
        }
    }
    private void WriteItemDb()
    {
        ItemDatabaseList.Add(0, new Item(0, 0, 0, "Name1", "Des1", 5, 5, sprites[0], null, ItemType.Piece));
        ItemDatabaseList.Add(1, new Item(1, 1, 1, "Name2", "Des2", 5, 5, sprites[1], null, ItemType.Piece));



        // SaveSystem.instance.LoadPermaChest(true, true, true);
    }
    //Veritabanına yeni item ekle
    public void AddItemDb()
    {
        int newId = ItemDatabaseList.Count;

	}
    private void AddPanels()
    {
        for (int k = 0; k < panelSpawner.transform.childCount; k++)
        {
            for (int m = 0; m < panelSpawner.transform.GetChild(k).childCount; m++)
            {
                panelSpawner.transform.GetChild(k).GetChild(m).GetComponent<PergPanel>().AddMe();
                panelSpawner.transform.GetChild(k).GetChild(m).GetComponent<PergPanel>().PanelSlotSpawner();
            }
        }
    }
    public void RemoveItem(int panelId, Item item)
    {
		if (panels.TryGetValue(panelId, out PanelCreator v))
		{
			if (v.panelObject != null)
			{
                for(int i = 0; i< v.panelObject.GetComponent<PergPanel>().items.Count;i++)
                {
                    if (v.panelObject.GetComponent<PergPanel>().items[i].itemId == item.itemId)
                    {
                        v.panelObject.GetComponent<PergPanel>().items.RemoveAt(i);
					}
                }
			}
			else
			{
				Debug.LogError("Panel object not found. Try removing the panel and adding it again.");
			}
		}
		else
		{
			Debug.LogError("There is no panel with this panelId.");
		}
	}

    /// <summary>
    /// Bu panelId ye item ekle
    /// </summary>
    /// <param name="panelId"></param>
    /// <param name="item"></param>
    public void FillPanel(int panelId, Item item)
    {
        if (panels.TryGetValue(panelId, out PanelCreator v))
        {
            if (v.panelObject != null)
            {
                v.panelObject.GetComponent<PergPanel>().items.Add(item);
            }
            else
            {
                Debug.LogError("Panel object not found. Try removing the panel and adding it again.");
            }
        }
        else
        {
            Debug.LogError("There is no panel with this panelId.");
        }
    }
	/// <summary>
    /// Bu panelId ye birden fazla item ekle
    /// </summary>
    /// <param name="panelId"></param>
    /// <param name="items"></param>
	public void FillPanel(int panelId, List<Item> items)
    {
        if (panels.TryGetValue(panelId, out PanelCreator v))
        {
            v.panelObject.GetComponent<PergPanel>().items.AddRange(items);
        }
        else
        {
            Debug.LogError("There is no panel with this panelId.");
        }
    }
}
