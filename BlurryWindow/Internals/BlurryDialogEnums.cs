using System;

namespace BlurryControls.Internals
{
    /// <summary>
    /// provides a preset argument list which is provided when an action is performed
    /// </summary>
    public class BlurryDialogResultArgs : EventArgs
    {
        public BlurryDialogResult Result { get; set; }
    }

    /// <summary>
    /// enum which enlists every possible icon value
    /// </summary>
    public enum BlurryDialogIcon
    {
        None,
        Question,
        Information,
        Warning,
        Error
    }

    /// <summary>
    /// enum which enlists every possible combination set of result values
    /// </summary>
    public enum BlurryDialogButton
    {
        Ok,
        OkCancel,
        YesNo,
        YesNoCancel,
        None
    }

    /// <summary>
    /// enum which enlists every possible result value
    /// </summary>
    public enum BlurryDialogResult
    {
        Ok,
        Yes,
        No,
        Cancel,
        None
    }
}