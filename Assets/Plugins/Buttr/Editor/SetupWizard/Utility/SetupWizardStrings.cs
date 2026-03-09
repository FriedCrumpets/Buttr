namespace Buttr.Editor.SetupWizard
{
    /// <summary>
    /// Display strings for the Setup Wizard UI.
    /// </summary>
    internal static class SetupWizardStrings {
        // ── Setup Option Descriptions ────────────────────────────────
        internal const string QuickSetupDescription =
            "Creates the <b>_Project/</b> folder with <b>Core/</b>, <b>Features/</b>, <br>" +
            "<b>Shared/</b>, and <b>Catalog/</b> subfolders. Generates a <b>Main.unity</b> <br>" +
            "boot scene and adds it to build settings at index 0. Scaffolds <b>Program.cs</b> <br>" +
            "and <b>ProgramLoader.cs</b> for application composition.";

        internal const string CustomSetupDescription =
            "Walk through each option step by step. Choose your root folder name, <br>" +
            "pick which subfolders to create, configure the boot scene and code generation, <br>" +
            "and select your persistence backend. Everything defaults to the recommended <br>" +
            "convention — change only what you need.";

        internal const string SkipConventionsDescription =
            "Installs Buttr as a standalone dependency injection framework with no folder <br>" +
            "structure, boot scene, or code generation. Use <b>ApplicationBuilder</b>, <br>" +
            "<b>ScopeBuilder</b>, and <b>[Inject]</b> on your own terms. You can run the <br>" +
            "setup wizard again later to adopt conventions.";

        // ── Footer Buttons ───────────────────────────────────────────
        internal const string Skip = "Skip";
        internal const string SetupProject = "Setup Project ➡";
        internal const string Next = "Next ➡";
        internal const string Close = "Close";
        internal const string Done = "Done";

        internal static readonly string[] QuickSetupPhrases =
        {
            "Spreading the butter...",
            "Buttering all the things...",
            "Laying down the bread...",
            "Achieving peak smoothness...",
            "Applying a generous layer...",
            "Melting into your project..."
        };

        internal static readonly string[] CustomSetupPhrases =
        {
            "Spreading artisan butter...",
            "Hand-crafting your spread...",
            "Carefully buttering each corner...",
            "Applying with precision...",
            "A bespoke buttering experience...",
            "Measured. Spread. Perfection."
        };

        internal static readonly string[] SkipConventionsPhrases =
        {
            "Just the butter, hold the bread...",
            "Butter, neat...",
            "No bread? Bold choice...",
            "Pure, uncut butter...",
            "Straight from the churn...",
            "Minimalist buttering in progress..."
        };
    }
}
