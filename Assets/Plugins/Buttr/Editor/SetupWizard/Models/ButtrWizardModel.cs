namespace Buttr.Editor.SetupWizard {
    internal sealed class ButtrWizardModel {
        private readonly ButtrWizardProjectModel m_ProjectModel;
        private readonly ButtrWizardSetupModel m_SetupModel;

        public ButtrWizardModel(string unityVersion, string projectName, string buttrVersion) {
            m_ProjectModel = new ButtrWizardProjectModel(unityVersion, projectName, buttrVersion);
            m_SetupModel = new ButtrWizardSetupModel();
        }
        
        public ButtrWizardProjectModel ProjectModel => m_ProjectModel;
        public ButtrWizardSetupModel SetupModel => m_SetupModel;
    }
}