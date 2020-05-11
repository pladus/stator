namespace Stator.Enums
{
    /// <summary>
    /// Describes a type of ocurred error while handling
    /// </summary>
    public enum FailureTypes
    {
        None = 0,
        EventNotRegistered = 1,
        TransitionNotRegistered = 2,
        TransitionConditionFailed = 3
    }
}
