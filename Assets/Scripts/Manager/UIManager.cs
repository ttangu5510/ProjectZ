using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [Header("Set Save Panel")]
    [SerializeField] Canvas UI_Canvas;
    [SerializeField] GameObject savePanel;
    [SerializeField] Button save1Btn;
    [SerializeField] Button save2Btn;
    [SerializeField] Button save3Btn;
    [SerializeField] GameObject confirmPanel;
    [SerializeField] Button saveBtn;
    [SerializeField] Button loadBtn;
    [SerializeField] TMP_Text fileName;
    [SerializeField] TMP_Text fileData;

    void Start()
    {
        savePanel.SetActive(false);
        confirmPanel.SetActive(false);
        save1Btn.onClick.AddListener(SaveFileSelect1);
        save2Btn.onClick.AddListener(SaveFileSelect2);
        save3Btn.onClick.AddListener(SaveFileSelect3);
    }

    void Update()
    {
        
    }
    public void OpenMenu()
    {
        savePanel.SetActive(true);
        save1Btn.Select();
    }
    public void OpenSelect()
    {
        savePanel.SetActive(false);
        confirmPanel.SetActive(true);
        saveBtn.onClick.RemoveAllListeners();
        if (!SaveManager.Instance.CheckSaveNull())
        {
            loadBtn.enabled = true;
            loadBtn.onClick.RemoveAllListeners();
        }
        else
        {
            loadBtn.enabled = false;
        }
    }
    public void SaveFileSelect1()
    {
        OpenSelect();
        saveBtn.onClick.AddListener(SaveManager.Instance.SaveJson1);
        if (!SaveManager.Instance.CheckSaveNull())
        {
            loadBtn.onClick.AddListener(SaveManager.Instance.LoadJson1);
            loadBtn.onClick.AddListener(SaveManager.Instance.PlayerDataLoad);
        }
    }    
    public void SaveFileSelect2()
    {
        OpenSelect();
        saveBtn.onClick.AddListener(SaveManager.Instance.SaveJson2);
        if (!SaveManager.Instance.CheckSaveNull())
        {
            loadBtn.onClick.AddListener(SaveManager.Instance.LoadJson2);
            loadBtn.onClick.AddListener(SaveManager.Instance.PlayerDataLoad);
        }
    }
    public void SaveFileSelect3()
    {
        OpenSelect();
        saveBtn.onClick.AddListener(SaveManager.Instance.SaveJson3);
        if (!SaveManager.Instance.CheckSaveNull())
        {
            loadBtn.onClick.AddListener(SaveManager.Instance.LoadJson3);
            loadBtn.onClick.AddListener(SaveManager.Instance.PlayerDataLoad);
        }
    }
}
