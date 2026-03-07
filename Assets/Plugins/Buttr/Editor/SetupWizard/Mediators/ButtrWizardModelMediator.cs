using System;

namespace Buttr.Editor.SetupWizard {
    internal sealed class ButtrWizardModelMediator : IDisposable {
        private readonly ButtrWizardModel m_Model;
        private readonly ButtrWizardView m_View;
        private readonly ButtrWizardPresenter m_Presenter;
        
        private ScheduledPhraseTimer m_PhraseTimer;

        public ButtrWizardModelMediator(ButtrWizardModel model, ButtrWizardView view, ButtrWizardPresenter presenter) {
            m_Model = model;
            m_View = view;
            m_Presenter = presenter;
            
            model.SetupModel.OnSetupModeChanged += OnSetupModeChanged;
            
            model.PageModel.OnCurrentPageChanged += OnPageChanged;

            model.InstallationModel.OnInstallProgressChanged += OnInstallProgressChanged; 
            model.InstallationModel.OnInstallPhraseChanged += OnInstallPhraseChanged; 
            model.InstallationModel.OnInstallCompleted += OnInstallCompleted; 
            model.InstallationModel.OnInstallLogAdded += OnInstallLogAdded; 
            model.InstallationModel.OnInstallLogUpdated += OnInstallLogUpdated; 
        }
        
        private void OnSetupModeChanged(SetupMode mode) {
            m_View.Window1.SetupRadioGroup.SetValueWithoutNotify((int)mode);
            m_View.Window1.Description = mode.GetSetupDescription();

            var page = m_Model.PageModel.CurrentPage;
            (var leftButton, var rightButton) =  page.GetButtonText(mode);
            m_View.Footer.LeftButtonText = leftButton;
            m_View.Footer.RightButtonText = rightButton;
        }

        private void OnPageChanged(WizardPage page) {
            // m_View.NavigateToPage(page);
            //
            // if (page == WizardPage.Installing) {
            //     StartPhraseTimer();
            //     
            //     (var leftButton, var rightButton) = WizardPage.Installing.GetButtonText(m_Model.SetupModel.SetupMode);
            //     m_View.Footer.LeftButtonText = leftButton;
            //     m_View.Footer.RightButtonText = rightButton;
            //     
            //     m_View.Footer.RightButtonEnabled = false;
            // } else {
            //     (var leftButton, var rightButton) = WizardPage.Installing.GetButtonText(m_Model.SetupModel.SetupMode);
            //     m_View.Footer.LeftButtonText = leftButton;
            //     m_View.Footer.RightButtonText = rightButton;
            //     
            //     StopPhraseTimer();
            //     m_View.Footer.RightButtonEnabled = true;
            // }
        }

        private void OnInstallProgressChanged(float progress) {
            // does nothing right now 👀
        }

        private void OnInstallPhraseChanged(string phrase) {
            m_View.Window3.InstallingHeader = phrase;
        }

        private void OnInstallCompleted() {
            StopPhraseTimer();
            
            m_View.Footer.LeftButtonVisible = false;
            m_View.Footer.RightButtonVisible = true;
            m_View.Footer.RightButtonEnabled = true;
            m_View.Footer.RightButtonText = SetupWizardStrings.Done;
            
        }

        private void OnInstallLogAdded(InstallLogEntry entry) {
            m_View.Window3.AppendLogEntry(entry);
        }
        
        private void OnInstallLogUpdated(int index, InstallLogStatus installLogStatus) {
            m_View.Window3.UpdateLogEntry(index, installLogStatus);
        }

        private void StartPhraseTimer() {
            StopPhraseTimer();

            m_PhraseTimer = new ScheduledPhraseTimer(m_Presenter);
        }

        private void StopPhraseTimer() {
            m_PhraseTimer?.Dispose();
            m_PhraseTimer = null;
        }
        
        public void Dispose() {
            m_Model.SetupModel.OnSetupModeChanged -= OnSetupModeChanged;
            
            m_Model.PageModel.OnCurrentPageChanged -= OnPageChanged;

            m_Model.InstallationModel.OnInstallProgressChanged -= OnInstallProgressChanged; 
            m_Model.InstallationModel.OnInstallPhraseChanged -= OnInstallPhraseChanged; 
            m_Model.InstallationModel.OnInstallCompleted -= OnInstallCompleted; 
            m_Model.InstallationModel.OnInstallLogAdded -= OnInstallLogAdded; 
            m_Model.InstallationModel.OnInstallLogUpdated -= OnInstallLogUpdated; 
        }
    }
}