using System;
using UnityEngine.UIElements;

namespace Buttr.Editor.SetupWizard {
    internal static class SetupWizardUtility {
        public static Label CreateLogLine(this InstallLogEntry entry) {
            var status = entry.Status.GetStatusString();

            return new Label($"<color=#888888>[{entry.Timestamp}]</color> {status} {entry.Message}") {
                enableRichText = true,
                style = {
                    unityFontStyleAndWeight = UnityEngine.FontStyle.Normal,
                    fontSize = 11,
                    marginLeft = 5,
                    marginTop = 2,
                    marginBottom = 2,
                    color = new UnityEngine.Color(0.77f, 0.77f, 0.77f)
                }
            };
        }

        public static string GetSetupDescription(this SetupMode setupMode) {
            return setupMode switch {
                SetupMode.QuickSetup => SetupWizardStrings.QuickSetupDescription,
                SetupMode.SkipConventions => SetupWizardStrings.SkipConventionsDescription,
                SetupMode.CustomSetup => SetupWizardStrings.CustomSetupDescription,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public static Label UpdateLogLine(this Label label, InstallLogStatus status) {
            var text = label.text;
            var split = text.Split(' ');
            split[1] = status.GetStatusString();
            label.text = string.Join(" ", split);
            return label;
        }

        private static string GetStatusString(this InstallLogStatus status) {
            (var prefix, var color) = status switch {
                InstallLogStatus.Success => ("\u2713", "#4EC959"), // ✓ green
                InstallLogStatus.InProgress => ("\u25CF", "#CBAA59"), // ● gold
                InstallLogStatus.Error => ("\u2717", "#E05252"), // ✗ red
                _ => ("\u25CF", "#CBAA59")
            };

            return $"<color={color}>{prefix}</color>";
        }

        public static string[] GetPhrasesForMode(this SetupMode mode) => mode switch {
            SetupMode.QuickSetup => SetupWizardStrings.QuickSetupPhrases,
            SetupMode.CustomSetup => SetupWizardStrings.CustomSetupPhrases,
            SetupMode.SkipConventions => SetupWizardStrings.SkipConventionsPhrases,
            _ => SetupWizardStrings.QuickSetupPhrases
        };
        
        public static (string leftButton, string rightButton) GetButtonText(this WizardPage page, SetupMode mode) {
            return page switch
            {
                WizardPage.Selection => mode switch
                {
                    SetupMode.SkipConventions => (SetupWizardStrings.Skip, SetupWizardStrings.Close),
                    SetupMode.CustomSetup => (SetupWizardStrings.Skip, SetupWizardStrings.Next),
                    _ => (SetupWizardStrings.Skip, SetupWizardStrings.SetupProject)
                },

                WizardPage.Installing => (string.Empty, SetupWizardStrings.Done),

                _ => (SetupWizardStrings.Skip, SetupWizardStrings.SetupProject)
            };
        }
    }
}