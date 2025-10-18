using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStory : MonoBehaviour
{
    [SerializeField] private ButtonStory buttonStoryPrefab;
    [SerializeField] private Transform contentPanel;
    [SerializeField] private Button btnClose;

    public void Show()
    {
        var listStory = GameManager.instance.Story;

        for (int i = contentPanel.childCount - 1; i >= 0; i--)
        {
            Destroy(contentPanel.GetChild(i).gameObject);
        }

        for (int i = 0; i < listStory.story.Count; i++)
        {
            var storyData = listStory.story[i];
            var buttonObj = Instantiate(buttonStoryPrefab, contentPanel);
            bool isLocked = i > PlayerPrefs.GetInt("MaxUnlockedChapter", 0);
            buttonObj.Init(storyData.nameStory, isLocked);
            int chapterNumber = i + 1;

            buttonObj.Button.onClick.AddListener(() =>
            {
                // 3. Gọi hàm GoToChapter trong GameManager với số chương tương ứng
                GameManager.instance.GoToChapter(chapterNumber);

                // 4. Ẩn panel chọn chapter đi
                gameObject.SetActive(false);
                UIBrain.instance.UIMainMenu.gameObject.SetActive(false);
                GameManager.instance.isPause = false;
            });
        }


    }
    private void Start()
    {   
        btnClose.onClick.AddListener(() => { gameObject.SetActive(false); });
    }
}
