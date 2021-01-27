using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryPopup : Popup
{
    public Transform detailsContainer;
    public TextMeshProUGUI txtDetailItemName;
    public TextMeshProUGUI txtDetailItemDescription;

    // for going throug the inventory slots up and down
    GridLayoutGroup panelLayout;
    RectTransform panelRectTransform;

    InventorySlot selectedSlot;

    public Sprite lockImage;

    PlayerController player;
    Inventory inventory;
    InventorySlot[] lstInventorySlots;
    int iSelectedSlot;
    bool bItemMenuOpen;
    private void Start()
    {
        open();
        player = PlayerController.Instance;

        inventory = player.GetInventory();

        panelLayout = GetComponentInChildren<GridLayoutGroup>();
        panelRectTransform = panelLayout.gameObject.GetComponent<RectTransform>();

        lstInventorySlots = GetComponentsInChildren<InventorySlot>();

        InitializeInventoryUI();

        close();
    }
    private void Update()
    {
        if (container.gameObject.activeSelf)
        {
            if(!bItemMenuOpen)
                MenuKeysInput();
        }
    }

    public override void open()
    {
        //TODO: On loading game without opening inventory, the items stack do not load properly
        base.open();
        SetItemMenuOpenBool(false);
        if(lstInventorySlots != null) 
        {
            SetSelected(lstInventorySlots[iSelectedSlot]);
        }
        PopupUIManager.Instance.menuBarPopup.open();
    }
    public override void close()
    {
        base.close();
        PopupUIManager.Instance.menuBarPopup.close();
    }

    public void UpdateInventoryUI(Inventory _updatedInventory)
    {
        inventory = _updatedInventory;
        InitializeInventoryUI();
    }
    public void InitializeInventoryUI()
    {
        // foreach (InventorySlot inSlot in inventorySlots)
        for (int i = 0; i < lstInventorySlots.Length; i++)
        {
            lstInventorySlots[i].EmptySlot();
        }

        LockSlots();

        if (inventory.lstItems.Count <= inventory.iInventorySize)
        {
            for (int i = 0; i < inventory.lstItems.Count; i++)
            {
                lstInventorySlots[i].UpdateSlot(inventory.lstItems[i]);
            }
        }
    }
    public Inventory GetInventory()
    {
        return inventory;
    }

    public void LockSlots()
    {
        for (int i = inventory.iInventorySize; i < lstInventorySlots.Length; i++)
        {
            lstInventorySlots[i].icon.gameObject.SetActive(true);
            lstInventorySlots[i].icon.sprite = lockImage;
        }
    }
    public void SetSelected(InventorySlot _iSlot)
    {
        if (selectedSlot != null)
            selectedSlot.SetSelectedElement(false);

        selectedSlot = _iSlot;

        for (int i = 0; i < lstInventorySlots.Length; i++)
        {
            lstInventorySlots[i].SetSelectedElement(false);
            if (lstInventorySlots[i] == selectedSlot)
            {
                iSelectedSlot = i;
            }
        }

        if (selectedSlot)
        {
            selectedSlot.SetSelectedElement(true);
        }
        
    }

    public override void MenuKeysInput()
    {
        if (Input.GetAxisRaw("Horizontal") < 0f && Input.anyKeyDown) // going left
        {
            if (iSelectedSlot > 0)
                SetSelected(lstInventorySlots[iSelectedSlot - 1]);
            else if (iSelectedSlot == 0)
                SetSelected(lstInventorySlots[lstInventorySlots.Length - 1]);
        }
        else if (Input.GetAxisRaw("Horizontal") > 0f && Input.anyKeyDown) // going right
        {
            if (iSelectedSlot < lstInventorySlots.Length - 1)
                SetSelected(lstInventorySlots[iSelectedSlot + 1]);
            else if (iSelectedSlot == lstInventorySlots.Length - 1)
                SetSelected(lstInventorySlots[0]);
            else if (iSelectedSlot == 0)
                SetSelected(lstInventorySlots[1]);
        }
        else if (Input.GetAxisRaw("Vertical") < 0 && Input.anyKeyDown) // going down
        {
            int _iSlotsInOneRow = (int) panelRectTransform.rect.width / ((int) panelLayout.cellSize.x + (int) panelLayout.spacing.x);
            if (iSelectedSlot + _iSlotsInOneRow  < lstInventorySlots.Length)
                SetSelected(lstInventorySlots[iSelectedSlot + _iSlotsInOneRow]);
        }
        else if (Input.GetAxisRaw("Vertical") > 0 && Input.anyKeyDown) // going up
        {
            int _iSlotsInOneRow = (int)panelRectTransform.rect.width / ((int)panelLayout.cellSize.x + (int)panelLayout.spacing.x);
            if(iSelectedSlot - _iSlotsInOneRow >= 0)
                SetSelected(lstInventorySlots[iSelectedSlot - _iSlotsInOneRow]);
        }
        else if (Input.GetButtonDown("Interact"))
        {
            if (selectedSlot != null)
            {
                selectedSlot.OpenItemMenu();
                selectedSlot = null;
            }
        }
    }
    public void SetItemMenuOpenBool(bool _setBool)
    {
        bItemMenuOpen = _setBool;
    }
}
