using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Core References")]
    [SerializeField] private Story story;
    public Story Story => story;
    [SerializeField] private Dialog dialog;

    [Header("Chapter State")]
    public int indexChapter = 0;
    private Conversation currentConversation;
    private string nameChapter;

    [Header("Game Flow Flags")]
    public bool startChapter = false;
    private bool isTransitioning = false;
    private bool hasChapterBeenStarted = false;

    [Header("Warlord Battle System")]
    [SerializeField] private WorldDataManager worldData;
    [SerializeField] private int warlordChapterIndex = 8; // Đặt index của chương hỗn chiến (ví dụ: chương 3 thì điền 2)
    private Warlord currentWarlord;
    public bool isWarlordChapterActive { get; private set; } = false;

    public enum GameStatus { UI, NONE }
    public GameStatus status = GameStatus.NONE;

    public bool isPause = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Khởi tạo game ở chapter đầu tiên
        GoToChapter(1);
    }

    void Update()
    {
        // Không xử lý Update nếu đang ở chương hỗn chiến và chờ người chơi chọn trên bản đồ
        if (isWarlordChapterActive) return;

        if (!startChapter && !isTransitioning)
        {
            isTransitioning = true;
            UIBrain.instance.UITransition.Init(nameChapter);
            return;
        }

        if (!startChapter) return;

        if (!hasChapterBeenStarted)
        {
            hasChapterBeenStarted = true;
            GoToConversation(currentConversation);
        }
    }

    // Hàm khởi tạo cho một chương tuyến tính
    public void InitLinearChapter()
    {
        var currentChapterData = story.story[indexChapter];
        nameChapter = currentChapterData.nameList;

        if (currentChapterData.conversations != null && currentChapterData.conversations.Count > 0)
        {
            currentConversation = currentChapterData.conversations[0];
        }
        else
        {
            Debug.LogError($"Chapter {indexChapter} is empty or has no starting conversation!");
            currentConversation = null;
        }

        startChapter = false;
        isTransitioning = false;
        hasChapterBeenStarted = false;
    }

    // Hàm hiển thị hội thoại
    public void GoToConversation(Conversation nextConv)
    {
        if (nextConv == null)
        {
            Debug.LogError("Cannot go to a null conversation!");
            return;
        }
        currentConversation = nextConv;
        dialog.gameObject.SetActive(true);

        if (currentConversation.type == DialogType.Narrator)
        {
            StartCoroutine(dialog.ShowDialogNarrator(currentConversation));
        }
        else
        {
            StartCoroutine(dialog.ShowDialogChat(currentConversation, currentConversation.sentences.nameNPC));
        }
    }

    // Chuyển sang chương tiếp theo
    public void GoToNextChapter()
    {
        if (indexChapter + 1 < story.story.Count)
        {
            GoToChapter(indexChapter + 2); // Chuyển đến chapter tiếp theo (số thứ tự + 1)
        }
        else
        {
            Debug.Log("End of the game! No more chapters.");
            // Có thể thêm logic hiển thị màn hình kết thúc game ở đây
        }
    }

    public Conversation GetConversation()
    {
        return currentConversation;
    }

    public void EnterBattleSelection()
    {
        dialog.gameObject.SetActive(false);
        // UIBrain.instance.HideTransition(); // Ẩn màn hình chuyển chương nếu có

        if (worldData.GetAttackableWarlords().Count == 0 && worldData.warlords.All(w => w.status == WarlordStatus.Defeated))
        {
            Debug.Log("Đã dẹp loạn 12 sứ quân! Chuyển sang chương tiếp theo.");
            isWarlordChapterActive = false;
            GoToNextChapter();
        }
        else
        {
            UIBrain.instance.ShowBattleMap(worldData.GetAttackableWarlords());
        }
    }

    // Bắt đầu một trận đánh với sứ quân được chọn
    public void StartBattle(Warlord warlord)
    {
        currentWarlord = warlord;
        UIBrain.instance.HideBattleMap();
        GoToConversation(warlord.battleStartConversation);
    }

    // Xử lý kết quả trận đánh (khi conversation có cờ triggersBattleResolution)
    public void ResolveBattle()
    {
        // (Hàm này giữ nguyên như bạn đã viết, rất tốt!)
        if (currentWarlord == null) return;
        bool playerWins = PlayerManager.instance.CheckACParameter(currentWarlord.stats);
        if (playerWins)
        {
            currentWarlord.status = WarlordStatus.Defeated;
            GoToConversation(currentWarlord.victoryConversation);
        }
        else
        {
            GoToConversation(currentWarlord.defeatConversation);
        }
    }

    // --- HÀM MỚI THEO YÊU CẦU ---

    /// <summary>
    /// Chuyển trực tiếp đến một chương cụ thể.
    /// Dùng cho việc chọn chapter từ menu hoặc để test game.
    /// </summary>
    /// <param name="chapterNumber">Số thứ tự của chương (bắt đầu từ 1).</param>
    public void GoToChapter(int chapterNumber)
    {
        int chapterIndex = chapterNumber - 1;

        if (chapterIndex < 0 || chapterIndex >= story.story.Count)
        {
            Debug.LogError($"Invalid chapter number: {chapterNumber}. Game has only {story.story.Count} chapters.");
            return;
        }

        indexChapter = chapterIndex;

        // Lưu lại tiến trình của người chơi
        PlayerPrefs.SetInt("MaxUnlockedChapter", Mathf.Max(PlayerPrefs.GetInt("MaxUnlockedChapter", 0), indexChapter));

        if (indexChapter == warlordChapterIndex)
        {
            isWarlordChapterActive = true;
            worldData.ResetAllWarlords(); // Reset trạng thái các sứ quân khi bắt đầu chương
            EnterBattleSelection();
        }
        else
        {
            isWarlordChapterActive = false;
            InitLinearChapter();
        }

    }

    public void StopStoryAndReturnToMenu()
    {
        Debug.Log("Stopping story and returning to main menu...");

        // 1. Yêu cầu Dialog dọn dẹp UI và dừng các coroutine
        if (dialog != null)
        {
            dialog.ForceCloseDialog();
        }

        // 2. Reset lại toàn bộ trạng thái của GameManager
        startChapter = false;
        isTransitioning = false;
        hasChapterBeenStarted = false;
        isWarlordChapterActive = false;
        currentConversation = null;
        currentWarlord = null;
        status = GameStatus.NONE;

        // Ẩn tất cả các UI liên quan đến game play
        UIBrain.instance.HideBattleMap();
        UIBrain.instance.UITransition.gameObject.SetActive(false);
        // Thêm các lệnh ẩn UI khác nếu cần...

        // 3. Gọi UIBrain để bật Main Menu lên
        UIBrain.instance.ShowMainMenu();
    }
}