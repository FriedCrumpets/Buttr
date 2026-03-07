using System;
using UnityEngine;

namespace Buttr.Editor.SetupWizard {

    /// <summary>
    /// Provides explicit operations on <see cref="ButtrWizardModel"/>.
    /// This is the only type that mutates model state.
    /// The <see cref="ButtrWizardMediator"/> calls into these methods
    /// in response to view events.
    /// </summary>
    internal sealed class ButtrWizardPresenter : IDisposable {
        private readonly ButtrWizardModel m_Model;
        private readonly PhraseCycler m_PhraseCycler;

        public ButtrWizardPresenter(ButtrWizardModel model) {
            m_Model = model;
            m_PhraseCycler = new(model);
        }

        public void ChangeSetupMode(SetupMode mode) {
            m_Model.SetupModel.SetupMode = mode;
        }

        public void NavigateForward() {
            switch (m_Model.PageModel.CurrentPage) {
                case WizardPage.Selection:
                    BeginInstallation();
                    break;

                // case WizardPage.CustomSetup:
                //     HandleCustomSetupForward();
                //     break;
            }
        }

        public void NavigateBack() {
            switch (m_Model.PageModel.CurrentPage) {
                case WizardPage.Installing:
                    m_Model.PageModel.CurrentPage = WizardPage.CustomSetup;
                    break;
            }
        }

        public void BeginInstallation() {
            m_Model.PageModel.CurrentPage = WizardPage.Installing;
            m_PhraseCycler.Reset();
            m_PhraseCycler.Phrases = m_Model.SetupModel.SetupMode.GetPhrasesForMode();

            AdvancePhrase();
            try {
                Debug.Log("Starting Buttr Scaffolding...");
                new ButtrProjectScaffolder(
                    m_Model.ProjectModel.ProjectName,
                    m_Model.ProjectModel.ButtrVersion
                ).ExecuteQuickSetup();

                m_Model.InstallationModel.InstallComplete = true;
            } catch (Exception) {
                Debug.Log("Buttr Installation Interrupted");
            }
        }

        public void AdvancePhrase() {
            m_PhraseCycler.AdvancePhrase();
        }

        // ── Private helpers ──────────────────────────────────────────

        private void HandleSelectionForward() {
            switch (m_Model.SetupModel.SetupMode) {
                case SetupMode.QuickSetup:
                    BeginInstallation();
                    break;

                case SetupMode.CustomSetup:
                    break;

                case SetupMode.SkipConventions:
                    // Handled by the Mediator — raises CloseRequested.
                    // This case should never be reached because the Mediator
                    // intercepts it, but it's here for completeness.
                    break;
            }
        }

        public void Dispose() {
            m_PhraseCycler.Cycling = false;
        }
    }
}
