using System;
using BoplJoints.Exercises;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BoplJoints.UI
{
    /// <summary>
    /// Main menu, exercise selection, exercise setup (repetitions and sets) and settings.
    /// When the player presses Start it raises <see cref="ExerciseStarted"/> with the chosen exercise.
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        [Header("Exercises (levels 1 to 3)")]
        [SerializeField] private ExerciseDefinition[] exercises;

        [Header("Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject selectPanel;
        [SerializeField] private GameObject setupPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject readyPanel;

        [Header("Main menu")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button calibrateButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;

        [Header("Exercise selection")]
        [SerializeField] private ExerciseCard[] cards;
        [SerializeField] private Button selectBackButton;

        [Header("Exercise setup")]
        [SerializeField] private TMP_Text setupTitle;
        [SerializeField] private TMP_Text setupSummary;
        [SerializeField] private TMP_Text setupKneeNote;
        [SerializeField] private TMP_Text repetitionsValue;
        [SerializeField] private TMP_Text setsValue;
        [SerializeField] private Button repetitionsMinus;
        [SerializeField] private Button repetitionsPlus;
        [SerializeField] private Button setsMinus;
        [SerializeField] private Button setsPlus;
        [SerializeField] private Button setupBackButton;
        [SerializeField] private Button setupStartButton;

        [Header("Settings")]
        [SerializeField] private Button kneeLeftButton;
        [SerializeField] private Button kneeRightButton;
        [SerializeField] private Button unlockAllButton;
        [SerializeField] private Button resetProgressButton;
        [SerializeField] private Button settingsBackButton;
        [SerializeField] private TMP_Text settingsStatus;
        [SerializeField] private Color choiceSelectedColor = new Color(0.36f, 0.68f, 0.29f);
        [SerializeField] private Color choiceNormalColor = new Color(0.75f, 0.75f, 0.72f);

        [Header("Ready (shown when nothing handles the start yet)")]
        [SerializeField] private TMP_Text readyDetails;
        [SerializeField] private Button readyBackButton;

        [Header("Exercise length limits (NHS dosage: 8-12 repetitions, 3-4 sets)")]
        [SerializeField] private int minRepetitions = 5;
        [SerializeField] private int maxRepetitions = 12;
        [SerializeField] private int minSets = 1;
        [SerializeField] private int maxSets = 4;

        [Header("Events")]
        [SerializeField] private ExerciseRequestEvent onStartExercise = new ExerciseRequestEvent();

        /// <summary>Raised when the player starts an exercise from the setup screen.</summary>
        public event Action<ExerciseRequest> ExerciseStarted;

        public ExerciseRequestEvent OnStartExercise => onStartExercise;

        private ExerciseDefinition selected;
        private int repetitions;
        private int sets;

        private void Awake()
        {
            startButton.onClick.AddListener(ShowSelection);
            settingsButton.onClick.AddListener(ShowSettings);
            exitButton.onClick.AddListener(ExitGame);
            calibrateButton.interactable = false; // calibration comes with repetition tracking

            foreach (var card in cards)
            {
                var c = card;
                c.Button.onClick.AddListener(() => SelectExercise(c.Exercise));
            }
            selectBackButton.onClick.AddListener(ShowMain);

            repetitionsMinus.onClick.AddListener(() => ChangeRepetitions(-1));
            repetitionsPlus.onClick.AddListener(() => ChangeRepetitions(1));
            setsMinus.onClick.AddListener(() => ChangeSets(-1));
            setsPlus.onClick.AddListener(() => ChangeSets(1));
            setupBackButton.onClick.AddListener(ShowSelection);
            setupStartButton.onClick.AddListener(StartExercise);

            kneeLeftButton.onClick.AddListener(() => SetKnee(KneeSide.Left));
            kneeRightButton.onClick.AddListener(() => SetKnee(KneeSide.Right));
            unlockAllButton.onClick.AddListener(UnlockAllLevels);
            resetProgressButton.onClick.AddListener(ResetProgress);
            settingsBackButton.onClick.AddListener(ShowMain);

            readyBackButton.onClick.AddListener(ShowMain);
        }

        private void Start() => ShowMain();

        public void ShowMain() => ShowOnly(mainPanel);

        public void HideAll() => ShowOnly(null);

        private void ShowSelection()
        {
            for (int i = 0; i < cards.Length; i++)
            {
                bool hasExercise = i < exercises.Length && exercises[i] != null;
                cards[i].gameObject.SetActive(hasExercise);
                if (hasExercise) cards[i].Bind(exercises[i], PlayerProgress.IsUnlocked(exercises[i].Level));
            }
            ShowOnly(selectPanel);
        }

        private void SelectExercise(ExerciseDefinition exercise)
        {
            if (exercise == null || !PlayerProgress.IsUnlocked(exercise.Level)) return;
            selected = exercise;
            repetitions = Mathf.Clamp(exercise.DefaultRepetitions, minRepetitions, maxRepetitions);
            sets = Mathf.Clamp(exercise.DefaultSets, minSets, maxSets);
            RefreshSetup();
            ShowOnly(setupPanel);
        }

        private void ChangeRepetitions(int delta)
        {
            repetitions = Mathf.Clamp(repetitions + delta, minRepetitions, maxRepetitions);
            RefreshSetup();
        }

        private void ChangeSets(int delta)
        {
            sets = Mathf.Clamp(sets + delta, minSets, maxSets);
            RefreshSetup();
        }

        private void RefreshSetup()
        {
            setupTitle.text = $"Level {selected.Level}: {selected.DisplayName}";
            setupSummary.text = selected.Summary;
            setupKneeNote.text = selected.SingleLeg
                ? $"Working leg: {(PlayerProgress.AffectedKnee == KneeSide.Left ? "left" : "right")} (change it in Settings)"
                : "Both legs";
            repetitionsValue.text = repetitions.ToString();
            setsValue.text = sets.ToString();
            repetitionsMinus.interactable = repetitions > minRepetitions;
            repetitionsPlus.interactable = repetitions < maxRepetitions;
            setsMinus.interactable = sets > minSets;
            setsPlus.interactable = sets < maxSets;
        }

        private void StartExercise()
        {
            var request = new ExerciseRequest(selected, repetitions, sets, PlayerProgress.AffectedKnee);
            bool handled = ExerciseStarted != null || onStartExercise.GetPersistentEventCount() > 0;
            ExerciseStarted?.Invoke(request);
            onStartExercise.Invoke(request);

            if (handled)
            {
                HideAll();
                return;
            }

            readyDetails.text = $"{selected.DisplayName}\n{sets} sets of {repetitions} repetitions\n\nStand in front of the chair.";
            ShowOnly(readyPanel);
        }

        private void ShowSettings()
        {
            RefreshSettings(string.Empty);
            ShowOnly(settingsPanel);
        }

        private void SetKnee(KneeSide side)
        {
            PlayerProgress.AffectedKnee = side;
            RefreshSettings(string.Empty);
        }

        private void UnlockAllLevels()
        {
            PlayerProgress.UnlockAll();
            RefreshSettings("All levels unlocked.");
        }

        private void ResetProgress()
        {
            PlayerProgress.ResetProgress();
            RefreshSettings("Progress reset: only Level 1 is unlocked.");
        }

        private void RefreshSettings(string status)
        {
            bool left = PlayerProgress.AffectedKnee == KneeSide.Left;
            kneeLeftButton.image.color = left ? choiceSelectedColor : choiceNormalColor;
            kneeRightButton.image.color = left ? choiceNormalColor : choiceSelectedColor;
            settingsStatus.text = status;
        }

        private void ShowOnly(GameObject panel)
        {
            foreach (var p in new[] { mainPanel, selectPanel, setupPanel, settingsPanel, readyPanel })
                if (p != null) p.SetActive(p == panel);
        }

        private static void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
