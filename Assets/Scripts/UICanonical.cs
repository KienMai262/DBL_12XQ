using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICanonical : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textCanonical;
    [SerializeField] private Button btnClose;

    private void Awake()
    {
        btnClose.onClick.RemoveAllListeners();
        btnClose.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            // Quan trọng: Trả lại quyền điều khiển cho game sau khi đóng UI
            GameManager.instance.status = GameManager.GameStatus.NONE; 
        });
    }

    public void SetTextCanonical(string canonical)
    {
        textCanonical.text = canonical;
    }
}