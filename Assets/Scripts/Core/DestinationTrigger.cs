namespace UTS.Core
{
    public class DestinationTrigger : Trigger
    {
        #region Public Methods

        public static void ToggleTrigger(bool enabled)
        {
            foreach (var trigger in FindObjectsOfType<DestinationTrigger>(true))
                trigger.SetEnabled(enabled);
        }

        #endregion
    }
}
