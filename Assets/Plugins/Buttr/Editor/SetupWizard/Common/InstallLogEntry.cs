namespace Buttr.Editor.SetupWizard
{
    /// <summary>
    /// A single line in the installation console log.
    /// </summary>
    internal readonly struct InstallLogEntry {
        internal string Timestamp { get; }
        internal string Message { get; }
        internal InstallLogStatus Status { get; }

        internal InstallLogEntry(string timestamp, string message, InstallLogStatus status) {
            Timestamp = timestamp;
            Message = message;
            Status = status;
        }

        /// <summary>
        /// Returns a new entry with the status changed (e.g. InProgress → Success).
        /// </summary>
        internal InstallLogEntry WithStatus(InstallLogStatus status)
            => new(Timestamp, Message, status);
    }
}
