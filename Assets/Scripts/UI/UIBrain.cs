using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class UIBrain : MonoBehaviour
{
    public static UIBrain instance;

    [Header("Core UI")]
    [SerializeField] private UICanonical uiCanonical;
    [SerializeField] private UITransition uiTransition;
    [SerializeField] private UIStory uIStory;
    [SerializeField] private UIMainMenu uIMainMenu;
    [SerializeField] private UIPause uiPause;
    public UICanonical UICanonical => uiCanonical;
    public UITransition UITransition => uiTransition;
    public UIStory UIStory => uIStory;
    public UIMainMenu UIMainMenu => uIMainMenu;
    public UIPause UIPause => uiPause;

    [SerializeField] private Button btnCanonical;
    [SerializeField] private Button btnNextChapter;
    [SerializeField] private Button btnPause;
    public Button BtnCanonical => btnCanonical;
    public Button BtnNextChapter => btnNextChapter;

    [Header("Battle Map")]
    // Sửa lại thành GameObject để linh hoạt hơn
    [SerializeField] private GameObject battleMapPanel;

    private Dictionary<string, Button> warlordButtons = new Dictionary<string, Button>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (battleMapPanel != null)
        {
            // Lấy tất cả button con của panel và lưu vào Dictionary
            foreach (Button button in battleMapPanel.GetComponentsInChildren<Button>(true))
            {
                // Chỉ thêm nếu key chưa tồn tại để tránh lỗi
                if (!warlordButtons.ContainsKey(button.gameObject.name))
                {
                    warlordButtons.Add(button.gameObject.name, button);
                }
            }
        }
    }

    private void Start()
    {
        btnPause.onClick.AddListener(() =>
        {
            uiPause.gameObject.SetActive(!uiPause.gameObject.activeSelf);
            GameManager.instance.isPause = true;
        });
    }

    public void ShowBattleMap(List<Warlord> attackableWarlords)
    {
        if (battleMapPanel == null) return;
        battleMapPanel.SetActive(true);

        foreach (var button in warlordButtons.Values)
        {
            button.interactable = false;
        }

        foreach (Warlord warlord in attackableWarlords)
        {
            if (warlordButtons.ContainsKey(warlord.warlordName))
            {
                Button button = warlordButtons[warlord.warlordName];
                button.interactable = true;
                button.onClick.RemoveAllListeners();

                Warlord currentWarlord = warlord;
                // Khi bấm nút, chỉ gọi StartBattle. GameManager sẽ tự ẩn map.
                button.onClick.AddListener(() => GameManager.instance.StartBattle(currentWarlord));
            }
            else
            {
                warlordButtons[warlord.warlordName].gameObject.SetActive(false);
            }
        }
    }

    public void HideBattleMap()
    {
        if (battleMapPanel != null)
        {
            battleMapPanel.SetActive(false);
        }
    }

    public void HideTransition()
    {
        if (uiTransition != null && uiTransition.gameObject.activeSelf)
        {
            uiTransition.HideTransition();
        }
    }
    public void ShowMainMenu()
    {
        if (uIMainMenu != null)
        {
            uIMainMenu.gameObject.SetActive(true);
        }
    }
}