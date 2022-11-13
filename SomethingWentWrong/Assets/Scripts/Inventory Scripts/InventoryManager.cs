using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class InventoryManager : MonoBehaviour
{

    public static InventoryManager instance { get; private set; }
    [SerializeField] private GameObject[] inventoryCells;
    [SerializeField] private GameObject ContextMenu;
    [SerializeField] private GameObject SurvivalManager;

    public GameObject InventoryPanel;
    private GameObject AlreadyChosenCell = null;
    private GameObject ChosenCellExtra = null;
    private bool isOpened;
    private GameObject onMouseObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            return;
        }

        Destroy(this.gameObject);
    }

    private void Start()
    {
        isOpened = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isOpened = !isOpened;
            InventoryPanel.SetActive(isOpened);
            IsometricPlayerMovementController.IsAbleToMove = !IsometricPlayerMovementController.IsAbleToMove;
            AlreadyChosenCell = null;
            if (onMouseObject != null)
            {
                Destroy(onMouseObject);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (ContextMenu.activeSelf == true)
            {
                StartCoroutine(CorountineClosingContextMenu());
            }
        }
    }

    IEnumerator CorountineClosingContextMenu()
    {
        yield return new WaitForSeconds(0.2f);
        ChosenCellExtra = null;
        ContextMenu.SetActive(false);
    }

    //����������� ��������� �� ���������
    public void OnCellClick(GameObject CurrentCell)
    {
        //���� ������ ������ (������) ��� Swap-� ��� �� �������
        if (AlreadyChosenCell == null)
        {
            if (CurrentCell.GetComponent<InventoryCell>().item.TypeOfThisItem != ItemType.NoItem)
            {
                //��� ������� �� ������ ������ ���� ������������� ����������� ����
                if (Input.GetMouseButtonUp(1))
                {
                    ChosenCellExtra = CurrentCell;
                    Debug.Log("ContextMenuHere");
                    ShowContextMenu(CurrentCell);
                }
                else
                {
                    AlreadyChosenCell = CurrentCell;
                    onMouseObject = Instantiate(AlreadyChosenCell.GetComponent<InventoryCell>().dragAndDropElement, transform);
                    onMouseObject.transform.position = Input.mousePosition;
                }
            }
        }
        //���� ������ ������ (������) ��� Swap-� ��� �������
        else
        {
            GameObject temporary = Instantiate(AlreadyChosenCell);

            AlreadyChosenCell.GetComponent<InventoryCell>().item = CurrentCell.GetComponent<InventoryCell>().item;
            AlreadyChosenCell.GetComponent<InventoryCell>().amount = CurrentCell.GetComponent<InventoryCell>().amount;
            AlreadyChosenCell.GetComponent<InventoryCell>().icon.GetComponent<Image>().sprite = CurrentCell.GetComponent<InventoryCell>().item.image;
            AlreadyChosenCell.GetComponent<InventoryCell>().dragAndDropElement = CurrentCell.GetComponent<InventoryCell>().dragAndDropElement;

            CurrentCell.GetComponent<InventoryCell>().item = temporary.GetComponent<InventoryCell>().item;
            CurrentCell.GetComponent<InventoryCell>().amount = temporary.GetComponent<InventoryCell>().amount;
            CurrentCell.GetComponent<InventoryCell>().icon.GetComponent<Image>().sprite = temporary.GetComponent<InventoryCell>().item.image;
            CurrentCell.GetComponent<InventoryCell>().dragAndDropElement = temporary.GetComponent<InventoryCell>().dragAndDropElement;

            Destroy(temporary);
            Destroy(onMouseObject);
            onMouseObject = null;
            AlreadyChosenCell = null;
        }
    }

    //������������ ��������� �� ���������
    public void OnDropZoneSpaceClick()
    {
        if (AlreadyChosenCell != null)
        {
            AlreadyChosenCell.GetComponent<InventoryCell>().item = AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/Items/EmtyCell.asset", typeof(ItemsBase)) as ItemsBase;
            AlreadyChosenCell.GetComponent<InventoryCell>().amount = 0;
            AlreadyChosenCell.GetComponent<InventoryCell>().icon.GetComponent<Image>().sprite = AlreadyChosenCell.GetComponent<InventoryCell>().item.image;
            Destroy(onMouseObject);
            AlreadyChosenCell = null;
        }
    }

    public void AddItem(ItemsBase newItem)
    {
        foreach (GameObject cell in inventoryCells)
        {
            if (cell.GetComponent<InventoryCell>().item.TypeOfThisItem == ItemType.NoItem)
            {
                cell.GetComponent<InventoryCell>().item = newItem;
                cell.GetComponent<InventoryCell>().amount = 0;
                cell.GetComponent<InventoryCell>().icon.GetComponent<Image>().sprite = newItem.image;
                cell.GetComponent<InventoryCell>().dragAndDropElement = newItem.dragAndDropElement;
                return;
            }
        }
    }

    public bool IsInventoryFull()
    {
        foreach (GameObject cell in inventoryCells)
        {
            if (cell.GetComponent<InventoryCell>().item.TypeOfThisItem == ItemType.NoItem)
            {
                return false;
            }
        }
        return true;
    }

    private void ShowContextMenu(GameObject CurrentCell)
    {
        ContextMenu.transform.position = new Vector2(Input.mousePosition.x + ContextMenu.GetComponent<RectTransform>().rect.width / 2, Input.mousePosition.y - ContextMenu.GetComponent<RectTransform>().rect.height / 2);
        ContextMenu.SetActive(true);
    }

    public void UseItem() /*(GameObject CurrentCell)*/
    {
        if(ChosenCellExtra.GetComponent<InventoryCell>().item.TypeOfThisItem == ItemType.Food)
        {
            ItemTypeFood temporary = ChosenCellExtra.GetComponent<InventoryCell>().item as ItemTypeFood;
            SurvivalManager.GetComponent<SurvivalManager>().ReplenishHunger(temporary.satiationEffect);
            SurvivalManager.GetComponent<SurvivalManager>().ReplenishThirst(temporary.slakingOfThirstEffect);
            DeleteItem();
        }
    }

    public void DeleteItem()/*(GameObject CurrentCell)*/
    {
        ChosenCellExtra.GetComponent<InventoryCell>().item = AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/Items/EmtyCell.asset", typeof(ItemsBase)) as ItemsBase;
        ChosenCellExtra.GetComponent<InventoryCell>().amount = 0;
        ChosenCellExtra.GetComponent<InventoryCell>().icon.GetComponent<Image>().sprite = ChosenCellExtra.GetComponent<InventoryCell>().item.image;
    }
}
