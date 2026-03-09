using System;
using UnityEngine;

namespace Buttr.Editor.SetupWizard {

    /// <summary>
    /// Provides explicit operations on <see cref="ButtrWizardModel"/>.
    /// This is the only type that mutates model state.
    /// The <see cref="ButtrWizardMediator"/> calls into these methods
    /// in response to view events.
    /// </summary>
    internal sealed class ButtrWizardPresenter {
        private readonly ButtrWizardModel m_Model;

        public ButtrWizardPresenter(ButtrWizardModel model) {
            m_Model = model;
        }

        public void ChangeSetupMode(SetupMode mode) {
            m_Model.SetupModel.SetupMode = mode;
        }
    }
}
