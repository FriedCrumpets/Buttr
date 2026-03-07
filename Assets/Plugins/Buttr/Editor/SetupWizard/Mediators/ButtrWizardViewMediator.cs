using System;

namespace Buttr.Editor.SetupWizard {
    internal sealed class ButtrWizardViewMediator : IDisposable {
        public event Action CloseRequested;
        
        private readonly ButtrWizardModel m_Model;
        private readonly ButtrWizardView m_View;
        private readonly ButtrWizardPresenter m_Presenter;
        
        public ButtrWizardViewMediator(ButtrWizardModel model, ButtrWizardView view, ButtrWizardPresenter presenter) {
            m_Model = model;
            m_View = view;
            m_Presenter = presenter;
            
            view.Window1.OnSetupModeChanged += OnSetupModeSelected;
            
            view.Footer.OnLeftButtonClicked += OnLeftFooterButtonClicked;
            view.Footer.OnRightButtonClicked += OnRightFooterButtonClicked;
        }
        
        private void OnSetupModeSelected(int previousValue, int newValue) {
            if (newValue is >= 0 and <= 2) {
                m_Presenter.ChangeSetupMode((SetupMode)newValue);
            }
        }
        
        private void OnLeftFooterButtonClicked() {
            if (m_Model.PageModel.CurrentPage == WizardPage.Selection) {
                CloseRequested?.Invoke();
                return;
            }

            m_Presenter.NavigateBack();
        }
        
        private void OnRightFooterButtonClicked() {
            var closeRequested =
                m_Model.InstallationModel.InstallComplete
                || m_Model.PageModel.CurrentPage == WizardPage.Selection
                && m_Model.SetupModel.SetupMode == SetupMode.SkipConventions;
            
            if (closeRequested) {
                CloseRequested?.Invoke();
                return;
            }

            m_Presenter.NavigateForward();
            CloseRequested?.Invoke();
        }
        
        public void Dispose() {
            m_View.Window1.OnSetupModeChanged -= OnSetupModeSelected;
            
            m_View.Footer.OnLeftButtonClicked -= OnLeftFooterButtonClicked;
            m_View.Footer.OnRightButtonClicked -= OnRightFooterButtonClicked;
        }
    }
}