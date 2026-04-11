using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CutsceneDialogue", menuName = "Dialogue/Cutscene Dialogue")]
public class CutsceneDialogue : ScriptableObject
{
    [System.Serializable]
    public struct Line
    {
        public string characterName;
        [TextArea(3, 5)]
        public string text;
    }

    public Line[] lines;
}