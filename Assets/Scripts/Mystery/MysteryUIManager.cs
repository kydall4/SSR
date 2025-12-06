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

    [Header("Animation")]
    public Vector2 hiddenPos = new Vector2(0, -500);
    public Vector2 showPos = new Vector2(0, 0);
    public float slideSpeed = 5f;

    // Helper to check if any UI is blocking the player
    private bool isNotebookOpen = false;
    private Queue<string> dialogueQueue = new Queue<string>();

    // Helper to check if any UI is blocking the player
    private bool isUIOpen => isNotebookOpen || dialoguePanel.activeSelf;

    void Awake()
    {
        Instance = this;
        if (openButton != null) openButton.onClick.AddListener(ToggleNotebook);

        // Force Dialogue Box OFF initially (Critical Fix)
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (continueIcon != null) continueIcon.SetActive(false);
        
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
        // 1. Notebook Toggle
        if (Input.GetKeyDown(KeyCode.Tab)) ToggleNotebook();

        // 2. Advance Dialogue (Space)
        if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextLine();
        }
        
        // 3. Slide Animation
        if (notebookRect != null)
        {
            Vector2 targetPos = isNotebookOpen ? showPos : hiddenPos;
            notebookRect.anchoredPosition = Vector2.Lerp(notebookRect.anchoredPosition, targetPos, Time.deltaTime * slideSpeed);
        }
    }

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
            Cursor.lockState = CursorLockMode.None;
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
            foreach (var clue in MysteryWorldManager.Instance.collectedEvidence)
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
                    if (txt != null) txt.text = clue.clueName;

                    // Set Click Logic
                    Button btn = btnObj.GetComponent<Button>();
                    if (btn != null) btn.onClick.AddListener(() => OnEvidenceClicked(clue));
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