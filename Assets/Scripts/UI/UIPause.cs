using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
    [SerializeField] private Button btnHome;
    [SerializeField] private Button btnSetting;
    [SerializeField] private Button btnClose;

    private void Start()
    {
        btnHome.onClick.AddListener(GoMainMenu);
        btnSetting.onClick.AddListener(OnSetting);
        btnClose.onClick.AddListener(OnClosse);
    }

    private void GoMainMenu()
    {
        gameObject.SetActive(false);
        GameManager.instance.StopStoryAndReturnToMenu();
    }

    private void OnSetting()
    {
        // Mở giao diện cài đặt
        Debug.Log("Setting button clicked");
    }
    private void OnClosse()
    {
        gameObject.SetActive(false);
        GameManager.instance.isPause = false;
    }
}
