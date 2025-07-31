using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine.Rendering.UI;

public class DialogueScriptManager : MonoBehaviour
{
    public GameObject Parent_Dialogue;
    public GameObject Dialogue_Text; // TMP
    public GameObject Dialogue_Button; // Button

    private string[] currentContent;
    private int currentContentIndex = 0;
    private bool isInDialogueSequence = false;

    [System.Serializable]
    public class DialogueLine
    {
        public string name;
        public string content;
    }

    void LoadDialogueBlock(string target)
    {
        // Clear existing dialogue elements
        foreach (Transform child in Parent_Dialogue.transform)
        {
            Destroy(child.gameObject);
        }

        if (!ValidateDialogueFile(target, out TextAsset textAsset, out string[] blocks, out int blockIndex))
        {
            return;
        }

        string[] content = blocks[blockIndex].Split('\n');
        ValidateBlockContent(target, content[0]);
        
        // Disable Vertical Layout Group before adding buttons
        VerticalLayoutGroup layoutGroup = Parent_Dialogue.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup)
        {
            layoutGroup.enabled = false;
        }
        
        if (string.Equals(content[1].Trim(), "스탠딩"))

        // Create buttons
        for (int i = 1; i < content.Length; i++)
        {
            string line = content[i].Trim();
            if (string.Equals(line, "스탠딩"))
            {
                GameEvents.TriggerStanding(content[2].Trim());
                string[] names = content[3].Trim().Split(", ");

                for (int j = 0; j < names.Length; j++)
                {
                    GameEvents.TriggerDisplay(names[j]);
                }

                currentContent = content;
                currentContentIndex = 4;
                isInDialogueSequence = true;
                ProcessNextDialogueLine();
                break;
            }

            if (string.IsNullOrEmpty(line) || !line.StartsWith("-"))
            {
                // Create dialogue text
                GameObject textObj = Instantiate(Dialogue_Text);
                textObj.transform.SetParent(Parent_Dialogue.transform, false);
                TextMeshProUGUI textComponent = textObj.GetComponent<TextMeshProUGUI>();
                if (textComponent)
                {
                    textComponent.text = content[i];
                }
            }

            string[] buttonData = line.Split('(');
            if (!ValidateButtonData(buttonData)) continue;

            string buttonText = buttonData[0].Substring(1).Trim(); // Remove '-' prefix
            string buttonTarget = buttonData[1].Split(')')[0].Trim();

            // Check for HP/TP changes in button text
            CheckAndTriggerStatChanges(buttonText);

            GameObject buttonObj = Instantiate(Dialogue_Button);
            buttonObj.transform.SetParent(Parent_Dialogue.transform, false);

            Button buttonComponent = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonTextComponent = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonTextComponent)
            {
                buttonTextComponent.text = buttonText;
            }

            if (buttonComponent)
            {
                string targetCopy = buttonTarget; // Capture for closure
                buttonComponent.onClick.AddListener(() =>
                {
                    Debug.Log("Button clicked! Target: " + targetCopy);
                    LoadDialogueBlock(targetCopy);
                });
            }
        }

        // Re-enable Vertical Layout Group and force rebuild
        if (layoutGroup)
        {
            layoutGroup.enabled = true;
            
            // Also check and refresh Content Size Fitter if present
            ContentSizeFitter sizeFitter = Parent_Dialogue.GetComponent<ContentSizeFitter>();
            if (sizeFitter)
            {
                sizeFitter.enabled = false;
                sizeFitter.enabled = true;
            }
            
            // Force layout rebuild
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(Parent_Dialogue.GetComponent<RectTransform>());
        }
    }

    void Start()
    {
        StartDialogue("T-1");
    }

    void Update()
    {
        if (isInDialogueSequence && (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
        {
            AdvanceDialogue();
        }
    }
    
    public void StartDialogue(string initialTarget)
    {
        Parent_Dialogue.SetActive(true);
        LoadDialogueBlock(initialTarget);
    }
    
    private void CheckAndTriggerStatChanges(string buttonText)
    {
        // Check for HP changes (HP-1, HP+2, etc.)
        Match hpMatch = Regex.Match(buttonText, @"HP([+-]\d+)");
        if (hpMatch.Success)
        {
            int hpChange = int.Parse(hpMatch.Groups[1].Value);
            GameEvents.TriggerHPChange(hpChange);
        }
        
        // Check for TP changes (TP-1, TP+2, etc.)
        Match tpMatch = Regex.Match(buttonText, @"TP([+-]\d+)");
        if (tpMatch.Success)
        {
            int tpChange = int.Parse(tpMatch.Groups[1].Value);
            GameEvents.TriggerTPChange(tpChange);
        }
    }

    private bool ValidateDialogueFile(string target, out TextAsset textAsset, out string[] blocks, out int blockIndex)
    {
        textAsset = Resources.Load<TextAsset>(target.Split('-')[0]);
        blocks = null;
        blockIndex = -1;

        if (textAsset == null)
        {
            Debug.LogError("Could not find dialogue file: " + target.Split('-')[0]);
            return false;
        }

        blocks = textAsset.text.Split(new string[] { "###" }, System.StringSplitOptions.RemoveEmptyEntries);
        blockIndex = int.Parse(target.Split('-')[1]) - 1;

        if (blockIndex < 0 || blockIndex >= blocks.Length)
        {
            Debug.LogError("Block index out of range: " + (blockIndex + 1));
            return false;
        }

        return true;
    }

    private void ValidateBlockContent(string target, string actualContent)
    {
        if (!string.Equals(target, actualContent.Trim()))
        {
            Debug.Log("Dialogue Load Warning! " + target + " != " + actualContent.Trim());
        }
    }

    private bool ValidateButtonData(string[] buttonData)
    {
        return buttonData.Length >= 2;
    }

    private void ProcessNextDialogueLine()
    {
        while (currentContentIndex < currentContent.Length)
        {
            string line = currentContent[currentContentIndex].Trim();
            Debug.Log(line);
            currentContentIndex++;
            
            if (!string.IsNullOrEmpty(line))
            {
                string name = line.Split(": ")[0];
                string cont = line.Split(": ")[1];
                GameEvents.TriggerCameraMove(name);
                GameEvents.TriggerName(name);
                GameEvents.TriggerNameSpace(!string.Equals(name, "나"));
                GameEvents.TriggerText(cont);
                return;
            }
        }
        
        isInDialogueSequence = false;
    }

    private void AdvanceDialogue()
    {
        ProcessNextDialogueLine();
    }
}