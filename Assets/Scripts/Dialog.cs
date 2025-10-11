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

    private void EndCurrentConversation()
    {
        if (conver.choices != null && conver.choices.Count > 0)
        {
            isShowingChoice = true;
            ShowChoice();
        }
        else if (conver.nextConversation != null)
        {
            GameManager.instance.GoToConversation(conver.nextConversation);
        }
        else
        {
            ShowEndOfChapterUI();
        }
    }
    
    public void ShowChoice()
    {
        choiceContainer.gameObject.SetActive(true);
        foreach (Transform child in choiceContainer) Destroy(child.gameObject);

        foreach (var branch in conver.choices)
        {
            Choice choiceInstance = Instantiate(choicePrefab, choiceContainer);
            Conversation destination = branch.nextConversation;
            Parameter param = branch.parameterChange;
            choiceInstance.SetChoice(branch.choiceText, 0, param);

            choiceInstance.button.onClick.AddListener(() =>
            {
                if (param != null) PlayerManager.instance.UpdateParameter(param);

                choiceContainer.gameObject.SetActive(false);
                foreach (Transform child in choiceContainer) Destroy(child.gameObject);
                isShowingChoice = false;

                if (destination != null)
                {
                    GameManager.instance.GoToConversation(destination);
                }
                else
                {
                    ShowEndOfChapterUI();
                }
            });
        }
    }
    
    private void ShowEndOfChapterUI()
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


    #region Unchanged Code
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
        if (Input.GetMouseButtonDown(0))
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
    
    private void CloseDialogAndContinue()
    {
        dialogBox.SetActive(false);
        narratorBox.SetActive(false);
        isShowingChoice = false;
        OnCloseDialog?.Invoke();
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
                TMP_LineInfo firstLine = narratorText.textInfo.lineInfo[0];
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
    public void NextChapter()
    {
        GameManager.instance.GoToNextChapter();
    }
    #endregion
}