namespace Buttr.Editor.SetupWizard {
    internal sealed class ButtrWizardModel {
        private readonly ButtrWizardInstallationModel m_InstallationModel;
        private readonly ButtrWizardPageModel m_PageModel;
        private readonly ButtrWizardProjectModel m_ProjectModel;
        private readonly ButtrWizardSetupModel m_SetupModel;

        public ButtrWizardModel(string unityVersion, string projectName, string buttrVersion) {
            m_InstallationModel = new ButtrWizardInstallationModel();
            m_PageModel = new ButtrWizardPageModel();
            m_ProjectModel = new ButtrWizardProjectModel(unityVersion, projectName, buttrVersion);
            m_SetupModel = new ButtrWizardSetupModel();
        }
        
        public ButtrWizardInstallationModel InstallationModel => m_InstallationModel;
        public ButtrWizardPageModel PageModel => m_PageModel;
        public ButtrWizardProjectModel ProjectModel => m_ProjectModel;
        public ButtrWizardSetupModel SetupModel => m_SetupModel;
    }
}