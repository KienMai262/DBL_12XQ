using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private Button tapToStory;
    [SerializeField] private Button btnSetting;
    [SerializeField] private Button btnQuit;


    private void Start()
    {
        tapToStory.onClick.AddListener(OnTapToStoryClicked);
        btnSetting.onClick.AddListener(OnSettingClicked);
        btnQuit.onClick.AddListener(OnQuitClicked);
    }

    private void OnTapToStoryClicked()
    {
        // Chuyển sang giao diện câu chuyện
        UIBrain.instance.UIStory.gameObject.SetActive(true);    
        UIBrain.instance.UIStory.Show();
    }
    private void OnSettingClicked()
    {
        // Mở giao diện cài đặt
        Debug.Log("Setting button clicked");
    }
    private void OnQuitClicked()
    {
        // Thoát trò chơi
        Debug.Log("Quit button clicked");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
