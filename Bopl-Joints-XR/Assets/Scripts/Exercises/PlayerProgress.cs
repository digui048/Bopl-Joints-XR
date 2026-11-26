using UnityEngine;

namespace BoplJoints.Exercises
{
    /// <summary>
    /// Progress and preferences saved on the headset between sessions.
    /// Levels unlock one by one; nothing raises difficulty automatically.
    /// </summary>
    public static class PlayerProgress
    {
        public const int MaxLevel = 3;

        const string UnlockedLevelKey = "bj.unlockedLevel";
        const string KneeSideKey = "bj.kneeSide";

        public static int UnlockedLevel => Mathf.Clamp(PlayerPrefs.GetInt(UnlockedLevelKey, 1), 1, MaxLevel);

        public static bool IsUnlocked(int level) => level <= UnlockedLevel;

        /// <summary>Unlocks the given level (and the ones before it). Never locks a level again.</summary>
        public static void UnlockLevel(int level)
        {
            if (level <= UnlockedLevel) return;
            PlayerPrefs.SetInt(UnlockedLevelKey, Mathf.Clamp(level, 1, MaxLevel));
            PlayerPrefs.Save();
        }

        public static void UnlockAll() => UnlockLevel(MaxLevel);

        public static KneeSide AffectedKnee
        {
            get => (KneeSide)PlayerPrefs.GetInt(KneeSideKey, (int)KneeSide.Right);
            set
            {
                PlayerPrefs.SetInt(KneeSideKey, (int)value);
                PlayerPrefs.Save();
            }
        }

        /// <summary>Locks levels 2 and 3 again (the knee setting is kept).</summary>
        public static void ResetProgress()
        {
            PlayerPrefs.DeleteKey(UnlockedLevelKey);
            PlayerPrefs.Save();
        }
    }
}
