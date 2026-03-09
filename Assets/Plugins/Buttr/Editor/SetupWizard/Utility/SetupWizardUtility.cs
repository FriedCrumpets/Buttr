using System;
using UnityEngine.UIElements;

namespace Buttr.Editor.SetupWizard {
    internal static class SetupWizardUtility {
        public static string GetSetupDescription(this SetupMode setupMode) {
            return setupMode switch {
                SetupMode.QuickSetup => SetupWizardStrings.QuickSetupDescription,
                SetupMode.SkipConventions => SetupWizardStrings.SkipConventionsDescription,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public static (string leftButton, string rightButton) GetButtonText(this SetupMode mode) {
            return mode switch {
                SetupMode.SkipConventions => (SetupWizardStrings.Skip, SetupWizardStrings.Close),
                _ => (SetupWizardStrings.Skip, SetupWizardStrings.SetupProject)
            };
        }
    }
}