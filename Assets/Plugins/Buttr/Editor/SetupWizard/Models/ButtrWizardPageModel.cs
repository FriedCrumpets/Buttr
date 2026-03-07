using System;

namespace Buttr.Editor.SetupWizard {
    internal sealed class ButtrWizardPageModel {
        public event Action<WizardPage> OnCurrentPageChanged;
        
        private WizardPage m_CurrentPage = WizardPage.Selection;
        
        internal WizardPage CurrentPage {
            get => m_CurrentPage;
            set {
                if (m_CurrentPage == value) return;
                
                m_CurrentPage = value;
                OnCurrentPageChanged?.Invoke(value);
            }
        }
    }
}