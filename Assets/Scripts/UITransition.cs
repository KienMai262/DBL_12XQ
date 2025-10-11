using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UITransition : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textNameChapter;
    [SerializeField] private Button nextButton;
    [SerializeField] private Image backgroundImage;

    private void Awake()
    {
        // Gán listener MỘT LẦN DUY NHẤT ở đây để tránh lỗi
        nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    public void Init(string nameChapter)
    {
        textNameChapter.text = nameChapter;
        ShowTransition();
    }

    private void OnNextButtonClicked()
    {
        HideTransition();
        // Báo cho GameManager bắt đầu chapter
        GameManager.instance.startChapter = true; 
    }

    public void ShowTransition()
    {
        gameObject.SetActive(true);
        backgroundImage.DOFade(1, 1f);
    }

    public void HideTransition()
    {
        backgroundImage.DOFade(0.4f, 0.5f).OnComplete(() => {
            gameObject.SetActive(false);
        });
    }
}