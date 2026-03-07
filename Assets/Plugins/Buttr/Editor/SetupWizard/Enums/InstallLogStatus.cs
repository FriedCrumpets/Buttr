namespace Buttr.Editor.SetupWizard {
    /// <summary>
    /// Status of an individual installation log entry.
    /// </summary>
    internal enum InstallLogStatus
    {
        /// <summary>Completed successfully — rendered with ✓ in green.</summary>
        Success,
        /// <summary>Currently in progress — rendered with ● in gold.</summary>
        InProgress,
        /// <summary>Failed — rendered with ✗ in red.</summary>
        Error
    }
}