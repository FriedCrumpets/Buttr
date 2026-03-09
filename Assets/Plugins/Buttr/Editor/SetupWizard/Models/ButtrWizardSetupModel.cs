using System;

namespace Buttr.Editor.SetupWizard {
    internal sealed class ButtrWizardSetupModel {
        public event Action<SetupMode> OnSetupModeChanged;
        
        private SetupMode m_SetupMode = SetupMode.QuickSetup;

        internal SetupMode SetupMode {
            get => m_SetupMode;
            set {
                if (m_SetupMode == value) return;
                
                m_SetupMode = value;
                OnSetupModeChanged?.Invoke(value);
            }
        }
    }
}