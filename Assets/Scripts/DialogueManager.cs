using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
public enum DialogueType
{
    Cutscene,
    Victory
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [SerializeField]
    private DialogueType dialogueType;

    [SerializeField] TextMeshProUGUI nameText, dialogueText;
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] RectTransform textBoxRect;
    [SerializeField] RectTransform spriteRect;
    [SerializeField] float slideDuration = 0.3f;

    public static event Action<CutsceneDialogue.Line> OnLineStart;
    public static event Action OnDialogueEnd;

    private CutsceneDialogue currentDialogue;
    private int currentLineIndex;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string lastSpeaker = "";



    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (dialoguePanel.activeInHierarchy == false)
            {
                StartCutscene();
            }
            else
            {
                AdvanceDialogue();
            }
        }
    }

    public void AdvanceDialogue()
    {
        if (isTyping)
        {
            isTyping = false;
            dialogueText.text = currentDialogue.lines[currentLineIndex].text;
        }
        currentLineIndex++;
        ShowLine();
    }

    public void StartCutscene()
    {
        string resourceName = string.Empty;

        switch (dialogueType)
        {
            case DialogueType.Cutscene:
                resourceName = "CutsceneDialogue";
                break;
            case DialogueType.Victory:
                resourceName = "VictoryDialogue";
                break;
        }

        currentDialogue = Resources.Load<CutsceneDialogue>(resourceName);
        if (currentDialogue == null)
        {
            Debug.LogError($"Could not load {resourceName}");
            return;
        }

        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        ShowLine();
    }

    void ShowLine()
    {
        if (currentLineIndex >= currentDialogue.lines.Length)
        {
            EndDialogue();
            return;
        }

        var line = currentDialogue.lines[currentLineIndex];
        nameText.text = line.characterName;
        if (line.characterName == "Mel") nameText.color = Color.cyan;
        else if (line.characterName == "Ziv") nameText.color = Color.purple;
        else if (line.characterName == "Kip") nameText.color = Color.green;

        if (line.characterName != lastSpeaker)
        {
            lastSpeaker = line.characterName;
            StartCoroutine(SlideInElements());
        }
        else
        {
            StartCoroutine(TypeText(line.text));
            OnLineStart?.Invoke(line);
        }
    }

    IEnumerator SlideInElements()
    {
        Vector2 originalTextPos = textBoxRect.anchoredPosition;

        Vector2 textStartPos = originalTextPos + Vector2.left * 500;
        textBoxRect.anchoredPosition = textStartPos;

        float timer = 0;
        while (timer < slideDuration)
        {
            timer += Time.deltaTime;
            float t = timer / slideDuration;
            textBoxRect.anchoredPosition = Vector2.Lerp(textStartPos, originalTextPos, t);
            yield return null;
        }

        textBoxRect.anchoredPosition = originalTextPos;

        StartCoroutine(TypeText(currentDialogue.lines[currentLineIndex].text));
        OnLineStart?.Invoke(currentDialogue.lines[currentLineIndex]);
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in text)
        {
            if (!isTyping) yield break;
            dialogueText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        isTyping = false;
    }

    private bool canAdvance => !isTyping;

   

    void EndDialogue()
    {
        OnDialogueEnd?.Invoke();
       
        FindObjectOfType<MainMenuManager>().PlayGame();
    }
}

