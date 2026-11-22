using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private static readonly Instruction[] Instructions =
    {
        new Instruction("MOVE & RUN", "Explore the ruins. Keep moving to stay ahead of the flood.", "WASD + SHIFT"),
        new Instruction("JUMP ACROSS PLATFORMS", "Leap between platforms and climb toward higher ground.", "SPACE"),
        new Instruction("SOLVE MATH CHALLENGES", "Read each gate's question and work out the answer.", "THINK FAST"),
        new Instruction("SHOOT THE ANSWER", "Aim your bow at the correct floating target, then release.", "HOLD & RELEASE LMB"),
        new Instruction("ESCAPE THE RISING WATER", "Watch the water level. Reach the temple before it catches you!", "KEEP CLIMBING")
    };

    [SerializeField] private GameObject menuRoot;
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button helpButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button howToPlayBackButton;
    [SerializeField] private Button settingsBackButton;
    [SerializeField] private Text instructionTitle;
    [SerializeField] private Text instructionDescription;
    [SerializeField] private Text instructionControl;
    [SerializeField] private Text pageNumber;
    [SerializeField] private Image instructionIcon;
    [SerializeField] private Sprite[] instructionIcons;
    [SerializeField] private Slider masterVolumeSlider;

    private int currentPage;

    private void Awake()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(StartGame);
        }

        if (settingsButton != null && settingsPanel != null)
        {
            settingsButton.onClick.AddListener(OpenSettings);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(QuitGame);
        }

        if (helpButton != null)
        {
            helpButton.onClick.AddListener(OpenHowToPlay);
        }

        if (previousButton != null)
        {
            previousButton.onClick.AddListener(PreviousPage);
        }

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(NextPage);
        }

        if (howToPlayBackButton != null)
        {
            howToPlayBackButton.onClick.AddListener(ShowMenu);
        }

        if (settingsBackButton != null && settingsPanel != null)
        {
            settingsBackButton.onClick.AddListener(ShowMenu);
        }

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            masterVolumeSlider.SetValueWithoutNotify(AudioListener.volume);
        }

        ShowPage(0);
    }

    public void ConfigureMenu(GameObject menu, GameObject howToPlay, GameObject settings)
    {
        menuRoot = menu;
        howToPlayPanel = howToPlay;
        settingsPanel = settings;
    }

    public void ConfigureButtons(Button play, Button openSettings, Button exit, Button help, Button previous, Button next, Button howToPlayBack, Button settingsBack)
    {
        playButton = play;
        settingsButton = openSettings;
        exitButton = exit;
        helpButton = help;
        previousButton = previous;
        nextButton = next;
        howToPlayBackButton = howToPlayBack;
        settingsBackButton = settingsBack;
    }

    public void ConfigureHowToPlay(Text title, Text description, Text control, Text page, GameObject panel)
    {
        instructionTitle = title;
        instructionDescription = description;
        instructionControl = control;
        pageNumber = page;
        howToPlayPanel = panel;
    }

    public void ConfigureSettings(Slider volume, GameObject panel)
    {
        masterVolumeSlider = volume;
        settingsPanel = panel;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GetStarted_Scene");
    }

    public void OpenHowToPlay()
    {
        if (menuRoot != null)
        {
            menuRoot.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        howToPlayPanel.SetActive(true);
        ShowPage(0);
    }

    public void OpenSettings()
    {
        if (settingsPanel == null)
        {
            return;
        }

        menuRoot.SetActive(false);
        howToPlayPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ShowMenu()
    {
        howToPlayPanel.SetActive(false);
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        if (menuRoot != null)
        {
            menuRoot.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void PreviousPage()
    {
        ShowPage(currentPage - 1);
    }

    private void NextPage()
    {
        ShowPage(currentPage + 1);
    }

    private void ShowPage(int page)
    {
        currentPage = Mathf.Clamp(page, 0, Instructions.Length - 1);
        Instruction instruction = Instructions[currentPage];

        instructionTitle.text = instruction.title;
        instructionDescription.text = instruction.description;
        instructionControl.text = instruction.control;
        pageNumber.text = (currentPage + 1) + " / " + Instructions.Length;
        if (instructionIcon != null && instructionIcons != null && currentPage < instructionIcons.Length)
        {
            instructionIcon.sprite = instructionIcons[currentPage];
        }

        if (previousButton != null)
        {
            previousButton.interactable = currentPage > 0;
        }

        if (nextButton != null)
        {
            nextButton.interactable = currentPage < Instructions.Length - 1;
        }
    }

    private void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    private readonly struct Instruction
    {
        public readonly string title;
        public readonly string description;
        public readonly string control;

        public Instruction(string title, string description, string control)
        {
            this.title = title;
            this.description = description;
            this.control = control;
        }
    }
}
