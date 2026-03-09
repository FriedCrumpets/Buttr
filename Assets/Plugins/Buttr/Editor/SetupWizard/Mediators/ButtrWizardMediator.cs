using System;

namespace Buttr.Editor.SetupWizard {

    /// <summary>
    /// Listens to events from <see cref="ButtrWizardView"/> and routes them
    /// to <see cref="ButtrWizardPresenter"/>. Listens to events from
    /// <see cref="ButtrWizardInstallationModel"/> and pushes state into
    /// <see cref="ButtrWizardView"/>.
    ///
    /// The Mediator is the only type that knows about both the View and Presenter.
    /// Neither the View nor the Presenter reference each other.
    /// </summary>
    internal sealed class ButtrWizardMediator : IDisposable {
        private readonly ButtrWizardModel m_Model;
        private readonly ButtrWizardView m_View;

        /// <summary>
        /// Raised when the wizard should close.
        /// The EditorWindow subscribes to this.
        /// </summary>
        public event Action CloseRequested;
        
        private readonly ButtrWizardViewMediator m_ViewMediator;
        private readonly ButtrWizardModelMediator m_ModelMediator;

        private IDisposable m_PhraseTimer;

        internal ButtrWizardMediator(ButtrWizardModel model, ButtrWizardView view, ButtrWizardPresenter presenter) {
            m_Model = model;
            m_View = view;
            
            m_ViewMediator = new ButtrWizardViewMediator(model, view, presenter);
            m_ModelMediator = new ButtrWizardModelMediator(model, view);

            m_ViewMediator.CloseRequested += HandleCloseRequested;
        }

        private void HandleCloseRequested() {
            CloseRequested?.Invoke();
        }

        /// <summary>
        /// Pushes the model's initial state into the view.
        /// Call once after construction to sync UI with defaults.
        /// </summary>
        internal void Initialize() {
            m_View.Window.ButtrVersion = m_Model.ProjectModel.ButtrVersion;
            m_View.Window.UnityVersion = m_Model.ProjectModel.UnityVersion;
            m_View.Window.Project = m_Model.ProjectModel.ProjectName;
            
            m_View.Window.SetupRadioGroup.value = (int)(m_Model.SetupModel.SetupMode);
            m_View.Window.Description = m_Model.SetupModel.SetupMode.GetSetupDescription();

            m_View.Footer.LeftButtonText = SetupWizardStrings.Skip;
            m_View.Footer.LeftButtonEnabled = true;
            m_View.Footer.LeftButtonVisible = true;
            
            m_View.Footer.RightButtonText = SetupWizardStrings.SetupProject;
            m_View.Footer.RightButtonEnabled = true;
            m_View.Footer.RightButtonVisible = true;
        }

        public void Dispose() {
            m_ViewMediator.CloseRequested -= HandleCloseRequested;
            
            m_ModelMediator?.Dispose();
            m_ViewMediator?.Dispose();
        }
    }

}