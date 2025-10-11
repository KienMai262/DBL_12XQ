using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Core References")]
    [SerializeField] private Story story;
    [SerializeField] private Dialog dialog;

    [Header("Chapter State")]
    public int indexChapter = 0;
    private Conversation currentConversation;
    private string nameChapter;

    [Header("Game Flow Flags")]
    public bool startChapter = false;
    private bool isTransitioning = false;
    private bool hasChapterBeenStarted = false;

    public enum GameStatus { UI, NONE }
    public GameStatus status = GameStatus.NONE;

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
        
        Init();
    }

    public void Init()
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

    void Update()
    {
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

    // Hàm này bây giờ CHỈ CÓ NHIỆM VỤ hiển thị hội thoại
    public void GoToConversation(Conversation nextConv)
    {
        currentConversation = nextConv;
        if (currentConversation.type == DialogType.Narrator)
        {
            StartCoroutine(dialog.ShowDialogNarrator(currentConversation));
        }
        else
        {
            StartCoroutine(dialog.ShowDialogChat(currentConversation, currentConversation.sentences.nameNPC));
        }
    }
    
    // HÀM MỚI: Dùng để chuyển chapter, sẽ được gọi từ UI
    public void GoToNextChapter()
    {
        if (indexChapter + 1 < story.story.Count)
        {
            indexChapter++;
            Init();
        }
        else
        {
            Debug.Log("End of the game! No more chapters.");
        }
    }

    public Conversation GetConversation()
    {
        return currentConversation;
    }
}