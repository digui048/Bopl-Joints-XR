using System;
using UnityEngine.Events;

namespace BoplJoints.Exercises
{
    public enum KneeSide
    {
        Left,
        Right
    }

    /// <summary>
    /// What the player chose in the menu: the exercise, its length (repetitions and sets) and the affected knee.
    /// </summary>
    [Serializable]
    public struct ExerciseRequest
    {
        public ExerciseDefinition Exercise;
        public int Repetitions;
        public int Sets;
        public KneeSide AffectedKnee;

        public ExerciseRequest(ExerciseDefinition exercise, int repetitions, int sets, KneeSide affectedKnee)
        {
            Exercise = exercise;
            Repetitions = repetitions;
            Sets = sets;
            AffectedKnee = affectedKnee;
        }
    }

    [Serializable]
    public class ExerciseRequestEvent : UnityEvent<ExerciseRequest> { }
}
