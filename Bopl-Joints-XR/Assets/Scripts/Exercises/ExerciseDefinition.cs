using UnityEngine;

namespace BoplJoints.Exercises
{
    /// <summary>
    /// One exercise of the game. Every exercise comes from the NHS "Meniscal Surgery - 6-16 Weeks" programme.
    /// </summary>
    [CreateAssetMenu(menuName = "Bopl Joints/Exercise Definition", fileName = "Exercise")]
    public class ExerciseDefinition : ScriptableObject
    {
        [Tooltip("Game level that uses this exercise (1, 2 or 3).")]
        [SerializeField] private int level = 1;

        [SerializeField] private string displayName = "Exercise";

        [Tooltip("Exercise number on the NHS meniscal surgery (6-16 weeks) page.")]
        [SerializeField] private int nhsExerciseNumber;

        [TextArea(2, 4)]
        [SerializeField] private string summary;

        [Tooltip("Step-by-step instructions shown by the trainer before the exercise.")]
        [TextArea(1, 3)]
        [SerializeField] private string[] steps;

        [Tooltip("True when the exercise works one leg: the affected knee setting decides which one.")]
        [SerializeField] private bool singleLeg;

        [SerializeField, Range(1, 20)] private int defaultRepetitions = 8;
        [SerializeField, Range(1, 6)] private int defaultSets = 3;

        public int Level => level;
        public string DisplayName => displayName;
        public int NhsExerciseNumber => nhsExerciseNumber;
        public string Summary => summary;
        public string[] Steps => steps;
        public bool SingleLeg => singleLeg;
        public int DefaultRepetitions => defaultRepetitions;
        public int DefaultSets => defaultSets;
    }
}
