using BoplJoints.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BoplJoints.Exercises
{
    /// <summary>
    /// Test driver for the pace guide until repetition tracking exists: starting an exercise from the menu
    /// shows the guide, and buttons tell it where the player is.
    /// Controllers: A = I am down, B = I am up, Menu = back to the menu.
    /// Keyboard (Editor): Down arrow, Up arrow, Escape.
    /// </summary>
    public class PaceGuideTester : MonoBehaviour
    {
        [SerializeField] private PaceGuide guide;
        [SerializeField] private MenuController menu;

        private int repetitions;

        private void OnEnable()
        {
            menu.ExerciseStarted += OnExerciseStarted;
            guide.RepetitionCompleted += OnRepetitionCompleted;
        }

        private void OnDisable()
        {
            menu.ExerciseStarted -= OnExerciseStarted;
            guide.RepetitionCompleted -= OnRepetitionCompleted;
        }

        private void Start() => guide.gameObject.SetActive(false);

        private void OnExerciseStarted(ExerciseRequest request)
        {
            repetitions = 0;
            guide.gameObject.SetActive(true);
            guide.Begin();
            Debug.Log($"Pace guide test: {request.Exercise.DisplayName}, {request.Sets} x {request.Repetitions}");
        }

        private void OnRepetitionCompleted()
        {
            repetitions++;
            Debug.Log($"Pace guide test: repetition {repetitions} done");
        }

        private void Update()
        {
            if (guide.CurrentPhase == PaceGuide.Phase.Idle) return;
            var keyboard = Keyboard.current;

            if (OVRInput.GetDown(OVRInput.Button.One) || (keyboard != null && keyboard.downArrowKey.wasPressedThisFrame))
                guide.PlayerReachedBottom();
            if (OVRInput.GetDown(OVRInput.Button.Two) || (keyboard != null && keyboard.upArrowKey.wasPressedThisFrame))
                guide.PlayerReachedTop();
            if (OVRInput.GetDown(OVRInput.Button.Start) || (keyboard != null && keyboard.escapeKey.wasPressedThisFrame))
            {
                guide.Stop();
                guide.gameObject.SetActive(false);
                menu.ShowMain();
            }
        }
    }
}
