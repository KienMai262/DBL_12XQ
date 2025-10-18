using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    [Header("Dialog Chat")]
    [SerializeField] protected GameObject dialogBox;
    [SerializeField] protected TextMeshProUGUI dialogText;
    [SerializeField] protected TextMeshProUGUI nameText;

    [Header("Choice Box")]
    [SerializeField] protected Transform choiceContainer;
    [SerializeField] protected Choice choicePrefab;

    [Header("Dialog Narrator")]
    [SerializeField] protected GameObject narratorBox;
    [SerializeField] protected TextMeshProUGUI narratorText;

    [Header("Settings")]
    [SerializeField] protected float letterPerSecond;
    [SerializeField] protected Image imageBg;
    public int maxLines = 3;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    private Conversation conver;
    private int currentLine = 0;
    private bool isTyping;
    private bool isShowingChoice = false;
    private DialogType dialogType;
    private bool hasShownEndOfChapterUI = false;

    // HÀM QUAN TRỌNG: XỬ LÝ KHI HỘI THOẠI KẾT THÚC
    private void EndCurrentConversation()
    {
        // Ưu tiên 1: Nếu có lựa chọn, hiển thị chúng
        if (conver.choices != null && conver.choices.Count > 0)
        {
            isShowingChoice = true;
            ShowChoice();
            return; // Dừng lại ở đây
        }

        // Áp dụng phần thưởng/phạt của hội thoại vừa kết thúc (NẾU CÓ)
        if (conver.onCompletionParameterChange != null)
        {
            PlayerManager.instance.UpdateParameter(conver.onCompletionParameterChange);
        }

        // Ưu tiên 2: Nếu là điểm kích hoạt trận đánh
        if (conver.triggersBattleResolution)
        {
            GameManager.instance.ResolveBattle();
        }
        // Ưu tiên 3: Nếu có hội thoại tiếp theo
        else if (conver.nextConversation != null)
        {
            GameManager.instance.GoToConversation(conver.nextConversation);
        }
        // Cuối cùng: Nếu không còn gì cả (kết thúc nhánh/chương)
        else
        {
            if (GameManager.instance.isWarlordChapterActive)
            {
                GameManager.instance.EnterBattleSelection();
            }
            else
            {
                ShowEndOfChapterUI();
            }
        }
    }

    public void ForceCloseDialog()
    {
        // Dừng tất cả các Coroutine đang chạy trên component này 
        // (quan trọng nhất là dừng hiệu ứng TypeDialog)
        StopAllCoroutines();

        // Ẩn tất cả các UI liên quan đến hội thoại
        if (dialogBox != null) dialogBox.SetActive(false);
        if (narratorBox != null) narratorBox.SetActive(false);
        if (choiceContainer != null) choiceContainer.gameObject.SetActive(false);

        // Reset lại các cờ trạng thái nội bộ
        isTyping = false;
        isShowingChoice = false;
        OnCloseDialog?.Invoke(); // Gửi sự kiện để các hệ thống khác (nếu có) biết
    }

    public void ShowChoice()
    {
        choiceContainer.gameObject.SetActive(true);
        foreach (Transform child in choiceContainer) Destroy(child.gameObject);

        foreach (var branch in conver.choices)
        {
            Choice choiceInstance = Instantiate(choicePrefab, choiceContainer);

            var currentBranch = branch;

            choiceInstance.SetChoice(currentBranch.choiceText, 0, null);

            choiceInstance.button.onClick.AddListener(() =>
            {
                if (currentBranch.parameterChange != null)
                {
                    PlayerManager.instance.UpdateParameter(currentBranch.parameterChange);
                }

                choiceContainer.gameObject.SetActive(false);
                foreach (Transform child in choiceContainer) Destroy(child.gameObject);
                isShowingChoice = false;

                Conversation destination = null;

                // 3. KIỂM TRA ĐIỀU KIỆN VÀ QUYẾT ĐỊNH KỊCH BẢN
                // Nếu có yêu cầu chỉ số (requiredStats)
                if (currentBranch.failureConversation != null)
                {
                    if (currentBranch.requiredStats != null)
                    {
                        // Gọi PlayerManager để kiểm tra xem người chơi có đủ chỉ số không
                        bool success = PlayerManager.instance.CheckACParameter(currentBranch.requiredStats);

                        if (success)
                        {
                            // Nếu thành công, đi đến kịch bản successConversation
                            destination = currentBranch.successConversation;
                        }
                        else
                        {
                            // Nếu thất bại, đi đến kịch bản failureConversation
                            destination = currentBranch.failureConversation;
                        }
                    }
                    else
                    {
                        destination = currentBranch.successConversation;
                    }
                }
                else
                {
                    destination = currentBranch.successConversation;
                }

                // 4. Chuyển đến hội thoại tiếp theo
                if (destination != null)
                {
                    GameManager.instance.GoToConversation(destination);
                }
                else // Nếu không có hội thoại nào được gán (kết thúc nhánh)
                {
                    // Quay về bản đồ chiến thuật
                    if (GameManager.instance.isWarlordChapterActive)
                    {
                        GameManager.instance.EnterBattleSelection();
                    }
                    else // Hoặc kết thúc chương như bình thường
                    {
                        ShowEndOfChapterUI();
                    }
                }
            });
        }
    }

    #region Unchanged Code
    public void ShowEndOfChapterUI()
    {
        if (hasShownEndOfChapterUI) return;
        hasShownEndOfChapterUI = true;
        isShowingChoice = true;

        dialogBox.SetActive(false);
        narratorBox.SetActive(false);

        UIBrain.instance.BtnNextChapter.gameObject.SetActive(true);
        UIBrain.instance.BtnNextChapter.onClick.RemoveAllListeners();
        UIBrain.instance.BtnNextChapter.onClick.AddListener(() =>
        {
            UIBrain.instance.UICanonical.gameObject.SetActive(false);
            UIBrain.instance.BtnCanonical.gameObject.SetActive(false);
            UIBrain.instance.BtnNextChapter.gameObject.SetActive(false);
            GameManager.instance.status = GameManager.GameStatus.NONE;

            GameManager.instance.GoToNextChapter();
        });

        if (conver.isCanonical)
        {
            UIBrain.instance.BtnCanonical.gameObject.SetActive(true);
            UIBrain.instance.BtnCanonical.onClick.RemoveAllListeners();
            UIBrain.instance.BtnCanonical.onClick.AddListener(() =>
            {
                GameManager.instance.status = GameManager.GameStatus.UI;
                UIBrain.instance.UICanonical.gameObject.SetActive(true);
                UIBrain.instance.UICanonical.SetTextCanonical(GameManager.instance.GetConversation().canonical);
            });
        }
    }

    public IEnumerator ShowDialogChat(Conversation conv, string nameNPC)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();
        this.conver = conv;
        narratorBox.SetActive(false);
        dialogBox.SetActive(true);
        nameText.text = nameNPC;
        dialogType = DialogType.Chat;
        ResetDialogState();
        StartCoroutine(TypeDialogChat(conver.sentences.Lines[0], 0));
    }

    public IEnumerator ShowDialogNarrator(Conversation conv)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();
        this.conver = conv;
        dialogBox.SetActive(false);
        narratorBox.SetActive(true);
        dialogType = DialogType.Narrator;
        ResetDialogState();
        StartCoroutine(TypeDialogNarrator(conver.sentences.Lines[0], 0));
    }

    private void ResetDialogState()
    {
        currentLine = 0;
        isTyping = false;
        isShowingChoice = false;
        hasShownEndOfChapterUI = false;
        choiceContainer.gameObject.SetActive(false);
    }

    public void Update()
    {
        if (isTyping || isShowingChoice || GameManager.instance.status != GameManager.GameStatus.NONE) return;
        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButtonDown(0) && !GameManager.instance.isPause)
        {
            ++currentLine;
            if (currentLine < conver.sentences.Lines.Count)
            {
                if (dialogType == DialogType.Narrator)
                    StartCoroutine(TypeDialogNarrator(conver.sentences.Lines[currentLine], currentLine));
                else
                    StartCoroutine(TypeDialogChat(conver.sentences.Lines[currentLine], currentLine));
            }
            else
            {
                EndCurrentConversation();
            }
        }
    }

    public IEnumerator TypeDialogChat(string lines, int index)
    {
        TransImage(conver.sentences.images[index]);
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in lines.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }
        isTyping = false;
    }
    public IEnumerator TypeDialogNarrator(string lines, int index)
    {
        TransImage(conver.sentences.images[index]);
        isTyping = true;
        narratorText.text = "";
        string currentTextToShow = "";
        foreach (var letter in lines.ToCharArray())
        {
            currentTextToShow += letter;
            narratorText.text = currentTextToShow;
            narratorText.ForceMeshUpdate();
            if (narratorText.textInfo.lineCount > maxLines)
            {
                var firstLine = narratorText.textInfo.lineInfo[0];
                int firstLineEndIndex = firstLine.lastCharacterIndex + 1;
                currentTextToShow = currentTextToShow.Substring(firstLineEndIndex);
                narratorText.text = currentTextToShow;
            }
            yield return new WaitForSeconds(1f / letterPerSecond);
        }
        isTyping = false;
    }
    public void TransImage(Sprite sprite, bool isAnim = false)
    {
        if (sprite == null) return;
        if (isAnim) { /*Chuyển ảnh có hiệu ứng*/ }
        else { imageBg.sprite = sprite; }
    }
    #endregion
}