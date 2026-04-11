using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
public class CutsceneCharacterController : MonoBehaviour
{
    [SerializeField] GameObject[] allCharacters;

    private static CutsceneCharacterController instance;
    private Coroutine slideCoroutine;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void OnEnable()
    {
        DialogueManager.OnLineStart += OnLineChanged;
        DialogueManager.OnDialogueEnd += OnCutsceneEnd;
    }

    void OnDisable()
    {
        DialogueManager.OnLineStart -= OnLineChanged;
        DialogueManager.OnDialogueEnd -= OnCutsceneEnd;
    }

    void OnLineChanged(CutsceneDialogue.Line line)
    {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideInElements(line.characterName));
    }

    IEnumerator SlideInElements(string speakerName)
    {
        foreach (GameObject characterObj in allCharacters)
        {
            RectTransform rect = characterObj.GetComponent<RectTransform>();
            if (rect == null) continue;

            Vector2 originalPos = rect.anchoredPosition;

            if (characterObj.name == speakerName)
            {
                characterObj.SetActive(true);
                Vector2 startPos = originalPos + Vector2.right * 500;
                rect.anchoredPosition = startPos;

                float timer = 0;
                float slideDuration = 0.1f;
                while (timer < slideDuration)
                {
                    timer += Time.deltaTime;
                    float t = timer / slideDuration;
                    rect.anchoredPosition = Vector2.Lerp(startPos, originalPos, t);
                    yield return null;
                }
                rect.anchoredPosition = originalPos;
            }
            else
            {
                characterObj.SetActive(false);
            }
        }
    }

    void OnCutsceneEnd()
    {
        foreach (GameObject character in allCharacters)
        {
            character.SetActive(false);
        }
    }
}