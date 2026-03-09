using System;

namespace Buttr.Editor.SetupWizard {
    internal sealed class ButtrWizardModelMediator : IDisposable {
        private readonly ButtrWizardModel m_Model;
        private readonly ButtrWizardView m_View;
        
        public ButtrWizardModelMediator(ButtrWizardModel model, ButtrWizardView view) {
            m_Model = model;
            m_View = view;
            
            model.SetupModel.OnSetupModeChanged += OnSetupModeChanged;
        }
        
        private void OnSetupModeChanged(SetupMode mode) {
            m_View.Window.SetupRadioGroup.SetValueWithoutNotify((int)mode);
            m_View.Window.Description = mode.GetSetupDescription();

            (var leftButton, var rightButton) =  m_Model.SetupModel.SetupMode.GetButtonText();
            m_View.Footer.LeftButtonText = leftButton;
            m_View.Footer.RightButtonText = rightButton;
        }
        
        public void Dispose() {
            m_Model.SetupModel.OnSetupModeChanged -= OnSetupModeChanged;
        }
    }
}