namespace Buttr.Editor.SetupWizard
{
    /// <summary>
    /// UXML element name constants for the Setup Wizard.
    /// </summary>
    internal static class SetupWizardElements
    {
        // ── Core Window ──────────────────────────────────────────────
        internal const string CoreWindow = "CoreWindow";
        internal const string LogoImage = "Logo__Image";

        // ── Footer ───────────────────────────────────────────────────
        internal const string Footer = "Footer";
        internal const string FooterLeftButton = "Footer__LeftButton";
        internal const string FooterRightButton = "Footer__RightButton";

        // ── Window 1 – Setup Selection ───────────────────────────────
        internal const string Window1 = "Window-1";
        internal const string Window1UnityVersion = "Title__Section--unity-version";
        internal const string Window1Project = "Title__Section--project";
        internal const string Window1ButtrVersion = "Title__Section--buttr-version";
        internal const string Window1Header = "Title__Section--header";
        internal const string SetupRadioButtonGroup = "Setup__RadioButtonGroup";
        internal const string SetupDescriptionLabel = "SetupDescription__Label";

        // ── Window 2 – Custom Setup ──────────────────────────────────

        // Structure tab
        internal const string Window2 = "Window-2";
        internal const string SetupFolders = "Setup__Folders";
        internal const string FoldersTreeView = "Folders";

        // Boot tab
        internal const string SetupBoot = "Setup__Boot";

        // Persistence tab
        internal const string SetupRepository = "Setup__Repository";
        internal const string SQLiteToggle = "SQLite__Toggle";
        internal const string PlayerPrefsToggle = "PlayerPrefs__Toggle";
        internal const string CustomToggle = "Custom__Toggle";

        // ── Folder Item Template ─────────────────────────────────────
        internal const string FolderItemRoot = "Folder";

        // ── Window 3 – Installing ────────────────────────────────────
        internal const string Window3 = "Window-3";
        internal const string Installer = "Installer";
        internal const string InstallerHeader = "Title__Section--header";
        internal const string InstallerAdditional = "Title__Section--additional";
        
    }
}