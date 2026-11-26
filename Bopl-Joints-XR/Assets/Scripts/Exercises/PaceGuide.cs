using System;
using UnityEngine;

namespace BoplJoints.Exercises
{
    /// <summary>
    /// Pace guide for the chair exercises: a bee goes down to the "touch the chair" height and up to the
    /// standing height. It never sets a tempo: at each end it waits until the player gets there.
    /// </summary>
    public class PaceGuide : MonoBehaviour
    {
        public enum Phase
        {
            Idle,
            GoingDown,
            WaitingDown,
            GoingUp,
            WaitingUp
        }

        [Header("Parts")]
        [SerializeField] private Transform bee;
        [SerializeField] private Transform[] wings;
        [SerializeField] private Transform topMarker;
        [SerializeField] private Transform bottomMarker;
        [SerializeField] private LineRenderer path;

        [Header("Heights (metres above the floor, set by calibration)")]
        [SerializeField] private float topHeight = 1.55f;
        [SerializeField] private float bottomHeight = 1.15f;

        [Header("Motion")]
        [Tooltip("Speed of the bee between the two heights, in m/s. Gentle on purpose.")]
        [SerializeField] private float moveSpeed = 0.25f;
        [SerializeField] private float hoverAmplitude = 0.012f;
        [SerializeField] private float hoverFrequency = 1.3f;
        [SerializeField] private float wingFlapAngle = 35f;
        [SerializeField] private float wingFlapFrequency = 12f;

        /// <summary>The bee has arrived at the bottom and waits for the player.</summary>
        public event Action ReachedBottom;
        /// <summary>The bee has arrived at the top and waits for the player.</summary>
        public event Action ReachedTop;
        /// <summary>The player went down and came back up: one repetition done.</summary>
        public event Action RepetitionCompleted;

        public Phase CurrentPhase { get; private set; } = Phase.Idle;
        public float TopHeight => topHeight;
        public float BottomHeight => bottomHeight;

        private bool playerIsDown;
        private bool playerIsUp = true;
        private float currentHeight;
        private Quaternion[] wingRest;

        private void Awake()
        {
            currentHeight = topHeight;
            wingRest = new Quaternion[wings.Length];
            for (int i = 0; i < wings.Length; i++) wingRest[i] = wings[i].localRotation;
            UpdateMarkers();
            PlaceBee(0f);
        }

        /// <summary>Sets the standing and "touch the chair" heights of the player.</summary>
        public void SetHeights(float standing, float touch)
        {
            topHeight = Mathf.Max(standing, touch + 0.05f);
            bottomHeight = Mathf.Min(touch, standing - 0.05f);
            UpdateMarkers();
        }

        /// <summary>Starts guiding repetitions from the standing position.</summary>
        public void Begin()
        {
            currentHeight = topHeight;
            playerIsDown = false;
            playerIsUp = true;
            CurrentPhase = Phase.GoingDown;
        }

        /// <summary>Stops guiding; the bee stays where it is.</summary>
        public void Stop() => CurrentPhase = Phase.Idle;

        /// <summary>Call when the player reaches the bottom of the movement.</summary>
        public void PlayerReachedBottom()
        {
            if (CurrentPhase == Phase.Idle) return;
            playerIsDown = true;
            playerIsUp = false;
            if (CurrentPhase == Phase.WaitingDown) CurrentPhase = Phase.GoingUp;
        }

        /// <summary>Call when the player is standing again.</summary>
        public void PlayerReachedTop()
        {
            if (CurrentPhase == Phase.Idle || !playerIsDown) return;
            playerIsUp = true;
            if (CurrentPhase == Phase.WaitingUp) CompleteRepetition();
        }

        private void Update()
        {
            switch (CurrentPhase)
            {
                case Phase.GoingDown:
                    if (MoveTowards(bottomHeight))
                    {
                        CurrentPhase = playerIsDown ? Phase.GoingUp : Phase.WaitingDown;
                        ReachedBottom?.Invoke();
                    }
                    break;
                case Phase.GoingUp:
                    if (MoveTowards(topHeight))
                    {
                        if (playerIsUp && playerIsDown) CompleteRepetition();
                        else CurrentPhase = Phase.WaitingUp;
                        ReachedTop?.Invoke();
                    }
                    break;
            }

            bool hovering = CurrentPhase != Phase.GoingDown && CurrentPhase != Phase.GoingUp;
            PlaceBee(hovering ? Mathf.Sin(Time.time * hoverFrequency * 2f * Mathf.PI) * hoverAmplitude : 0f);
            FlapWings();
        }

        private void CompleteRepetition()
        {
            playerIsDown = false;
            CurrentPhase = Phase.GoingDown;
            RepetitionCompleted?.Invoke();
        }

        private bool MoveTowards(float target)
        {
            currentHeight = Mathf.MoveTowards(currentHeight, target, moveSpeed * Time.deltaTime);
            return Mathf.Approximately(currentHeight, target);
        }

        private void PlaceBee(float hoverOffset)
        {
            if (bee == null) return;
            var p = bee.localPosition;
            bee.localPosition = new Vector3(p.x, currentHeight + hoverOffset, p.z);
        }

        private void FlapWings()
        {
            float angle = Mathf.Sin(Time.time * wingFlapFrequency * 2f * Mathf.PI) * wingFlapAngle;
            for (int i = 0; i < wings.Length; i++)
                wings[i].localRotation = wingRest[i] * Quaternion.Euler(0f, 0f, i % 2 == 0 ? angle : -angle);
        }

        private void UpdateMarkers()
        {
            if (topMarker != null) topMarker.localPosition = new Vector3(topMarker.localPosition.x, topHeight, topMarker.localPosition.z);
            if (bottomMarker != null) bottomMarker.localPosition = new Vector3(bottomMarker.localPosition.x, bottomHeight, bottomMarker.localPosition.z);
            if (path != null)
            {
                path.useWorldSpace = false;
                path.positionCount = 2;
                path.SetPosition(0, new Vector3(0f, bottomHeight, 0f));
                path.SetPosition(1, new Vector3(0f, topHeight, 0f));
            }
        }
    }
}
