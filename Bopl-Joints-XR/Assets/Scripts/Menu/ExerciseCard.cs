using BoplJoints.Exercises;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BoplJoints.UI
{
    /// <summary>
    /// One selectable exercise in the exercise selection screen.
    /// </summary>
    public class ExerciseCard : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image background;
        [SerializeField] private TMP_Text levelLabel;
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private TMP_Text sourceLabel;
        [SerializeField] private TMP_Text detailLabel;
        [SerializeField] private Color unlockedColor = new Color(0.98f, 0.93f, 0.80f);
        [SerializeField] private Color lockedColor = new Color(0.82f, 0.82f, 0.80f);

        public Button Button => button;
        public ExerciseDefinition Exercise { get; private set; }

        public void Bind(ExerciseDefinition exercise, bool unlocked)
        {
            Exercise = exercise;
            levelLabel.text = $"Level {exercise.Level}";
            nameLabel.text = exercise.DisplayName;
            sourceLabel.text = $"NHS exercise {exercise.NhsExerciseNumber}";
            detailLabel.text = unlocked
                ? exercise.Summary
                : $"Locked. Complete Level {exercise.Level - 1} to unlock it.";
            button.interactable = unlocked;
            background.color = unlocked ? unlockedColor : lockedColor;
        }
    }
}
