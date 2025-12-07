using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MysteryUIManager : MonoBehaviour
{
    public static MysteryUIManager Instance;

    [Header("Tutorial UI")]
    public GameObject tutorialPanel; // Drag your TutorialPanel here
    
    [Header("Notebook UI")]
    public RectTransform notebookRect; 
    public Button openButton;
    public Transform evidenceListContent; 
    public GameObject evidenceButtonPrefab; 

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject continueIcon; // <--- NEW: The "More Text" Triangle

    [Header("Argument UI")]
    public GameObject choicePanel;       // The new panel for 3 buttons
    public TextMeshProUGUI[] choiceTexts; // CHANGED: We only need the text objects now, not buttons
    private EvidenceClue currentActiveClue; // Track which clue we are arguing about

    [Header("Animation")]
    public Vector2 hiddenPos = new Vector2(0, -500);
    public Vector2 showPos = new Vector2(0, 0);
    public float slideSpeed = 5f;

    // Helper to check if any UI is blocking the player
    private bool isNotebookOpen = false;
    private Queue<string> dialogueQueue = new Queue<string>();

    // Helper to check if any UI is blocking the player
    public bool isUIOpen => isNotebookOpen || dialoguePanel.activeSelf || (choicePanel != null && choicePanel.activeSelf);

    void Awake()
    {
        Instance = this;
        if (openButton != null) openButton.onClick.AddListener(ToggleNotebook);

        // Force Dialogue Box OFF initially (Critical Fix)
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (continueIcon != null) continueIcon.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);
        
        // Show Tutorial immediately
        if (tutorialPanel != null) 
        {
            tutorialPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None; // Unlock mouse to click "Begin"
            Cursor.visible = true;
            Time.timeScale = 0f; // PAUSE the game so they can read
        }
        else
        {
            // If you forgot to assign a tutorial panel, start the game normally
            StartGameDialogue();
        }
    }

    void Update()
    {
        // 1. Argument Choice Input (Keys 1-3) -- HIGHEST PRIORITY
        if (choicePanel != null && choicePanel.activeSelf)
        {
            CheckArgumentInput();
            return; // Block other inputs while choosing
        }

        // 2. Notebook Toggle
        if (Input.GetKeyDown(KeyCode.Tab)) ToggleNotebook();

        // 3. Advance Dialogue (Space)
        if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextLine();
        }
        
        // 4. Evidence Selection (Number Keys 1-9) - ONLY when Notebook is OPEN
        if (isNotebookOpen)
        {
            CheckEvidenceInput();
        }
        
        // 5. Slide Animation
        if (notebookRect != null)
        {
            Vector2 targetPos = isNotebookOpen ? showPos : hiddenPos;
            notebookRect.anchoredPosition = Vector2.Lerp(notebookRect.anchoredPosition, targetPos, Time.deltaTime * slideSpeed);
        }
    }

    // --- NEW INPUT SYSTEM ---
    void CheckEvidenceInput()
    {
        if (MysteryWorldManager.Instance == null) return;
        
        // Check keys 1 through 9
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) // Alpha1 is '1', Alpha2 is '2', etc.
            {
                int index = i;
                if (index < MysteryWorldManager.Instance.collectedEvidence.Count)
                {
                    EvidenceClue clue = MysteryWorldManager.Instance.collectedEvidence[index];
                    
                    // Close notebook immediately
                    ToggleNotebook();
                    
                    // Trigger the Presentation Logic
                    MysteryWorldManager.Instance.PresentEvidence(clue);
                }
            }
        }
    }

    // -- ARGUMENT CHOICE SYSTEM ---
    void CheckArgumentInput()
    {
        // Check keys 1, 2, 3
        for (int i = 0; i < 3; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                // Send choice to Manager
                if (currentActiveClue != null)
                {
                    OnArgumentSelected(currentActiveClue, i);
                }
            }
        }
    }

    public void ShowArgumentChoices(EvidenceClue clue)
    {
        currentActiveClue = clue;
        choicePanel.SetActive(true);
        dialoguePanel.SetActive(false); // Hide Chief while we think

        // --- Hide the HUD button ---
        if (openButton != null) openButton.gameObject.SetActive(false);

        // Setup Text Only
        for (int i = 0; i < choiceTexts.Length; i++)
        {
            if (i < clue.arguments.Length)
            {
                choiceTexts[i].gameObject.SetActive(true);
                // Add number prefix so player knows what key to press
                choiceTexts[i].text = $"[{i + 1}]  {clue.arguments[i].optionText}";
            }
            else
            {
                choiceTexts[i].gameObject.SetActive(false);
            }
        }
    }

    void OnArgumentSelected(EvidenceClue clue, int index)
    {
        choicePanel.SetActive(false);
        currentActiveClue = null;

        // --- Restore the HUD button ---
        if (openButton != null) openButton.gameObject.SetActive(true);
        
        // Send choice back to Logic
        if (MysteryWorldManager.Instance != null)
            MysteryWorldManager.Instance.ResolveArgument(clue, index);
    }

    void PresentEvidenceByIndex(int index)
    {
        if (MysteryWorldManager.Instance != null)
        {
            // Check if we actually have that much evidence
            if (index < MysteryWorldManager.Instance.collectedEvidence.Count)
            {
                EvidenceClue clue = MysteryWorldManager.Instance.collectedEvidence[index];
                
                // Call the Presentation Logic
                MysteryWorldManager.Instance.PresentEvidence(clue);
                
                // Close the notebook to show the drama
                ToggleNotebook(); 
            }
        }
    }

    // --- DIALOGUE SYSTEM ---

    // Wrapper for single lines (keeps your existing code working)
    public void ShowDialogue(string text)
    {
        ShowDialogue(new string[] { text });
    }

    // New overload for multiple lines
    public void ShowDialogue(string[] lines)
    {
        dialogueQueue.Clear();
        foreach (string line in lines)
        {
            dialogueQueue.Enqueue(line);
        }

        dialoguePanel.SetActive(true);
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            CloseDialogue();
            return;
        }

        string line = dialogueQueue.Dequeue();
        dialogueText.text = line;

        // VISUAL LOGIC: Show Icon ONLY if more lines are waiting
        if (continueIcon != null)
        {
            continueIcon.SetActive(dialogueQueue.Count > 0);
        }
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    // --- NOTEBOOK SYSTEM ---

    public void ToggleNotebook()
    {
        isNotebookOpen = !isNotebookOpen;

        if (openButton != null) 
            openButton.gameObject.SetActive(!isNotebookOpen);

        if (isNotebookOpen)
        {
            RefreshContent(); 
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void RefreshContent()
    {
        // Clear old buttons
        foreach (Transform child in evidenceListContent)
        {
            Destroy(child.gameObject);
        }

        // Spawn new buttons from the Manager's list
        if (MysteryWorldManager.Instance != null)
        {
            List<EvidenceClue> evidence = MysteryWorldManager.Instance.collectedEvidence;
            
            for (int i = 0; i < evidence.Count; i++)
            {
                if (evidenceButtonPrefab != null)
                {
                    GameObject btnObj = Instantiate(evidenceButtonPrefab, evidenceListContent);

                    // --- FORCE Z POSITION TO 0 ---
                    btnObj.transform.localPosition = new Vector3(btnObj.transform.localPosition.x, btnObj.transform.localPosition.y, 0);
                    btnObj.transform.localScale = Vector3.one;
                    // -----------------------------
                    
                    // Set Text
                    TextMeshProUGUI txt = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                    
                    // NEW VISUALS: Show "1. Clue Name"
                    if (txt != null) {
                        txt.text = $"[{i + 1}] {evidence[i].clueName}";
                    }

                    // Set Click Logic
                    Button btn = btnObj.GetComponent<Button>();
                    if (btn != null) btn.interactable = false; // Optional: make it look static
                }
            }
        }
    }

    void OnEvidenceClicked(EvidenceClue clue)
    {
        if (MysteryWorldManager.Instance != null)
        {
            MysteryWorldManager.Instance.PresentEvidence(clue);
            ToggleNotebook(); // Close book to see reaction
        }
    }

    public void CloseTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);

            // Resume Game
            Time.timeScale = 1f; 
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            StartGameDialogue();
        }
    }

    void StartGameDialogue()
    {
        ShowDialogue("Chief: 'Look, the door was locked. He clearly jumped. I'm closing the case.'");
    }
}