using System;
using UnityEngine;

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
            
            view.Window.OnSetupModeChanged += OnSetupModeSelected;
            
            view.Footer.OnLeftButtonClicked += OnLeftFooterButtonClicked;
            view.Footer.OnRightButtonClicked += OnRightFooterButtonClicked;
        }
        
        private void OnSetupModeSelected(int previousValue, int newValue) {
            if (newValue is >= 0 and <= 2) {
                m_Presenter.ChangeSetupMode((SetupMode)newValue);
            }
        }
        
        private void OnLeftFooterButtonClicked() {
            CloseRequested?.Invoke();
        }
        
        private void OnRightFooterButtonClicked() {
            if (m_Model.SetupModel.SetupMode == SetupMode.QuickSetup) {
                try {
                    Debug.Log("Starting Buttr Scaffolding...");
                    new ButtrProjectScaffolder(
                        m_Model.ProjectModel.ProjectName,
                        m_Model.ProjectModel.ButtrVersion
                    ).ExecuteQuickSetup();
                } catch (Exception) {
                    Debug.Log("Buttr Installation Interrupted");
                }
            }

            if (m_Model.SetupModel.SetupMode == SetupMode.SkipConventions) {
                try {
                    Debug.Log("skipping setup...");
                    new ButtrProjectScaffolder(
                        m_Model.ProjectModel.ProjectName,
                        m_Model.ProjectModel.ButtrVersion
                    ).ExecuteSkipConventions();
                } catch (Exception) {
                    Debug.Log("Buttr Interrupted");
                }
            }
            
            CloseRequested?.Invoke();
        }
        
        public void Dispose() {
            m_View.Window.OnSetupModeChanged -= OnSetupModeSelected;
            
            m_View.Footer.OnLeftButtonClicked -= OnLeftFooterButtonClicked;
            m_View.Footer.OnRightButtonClicked -= OnRightFooterButtonClicked;
        }
    }
}