namespace Buttr.Editor.SetupWizard {
    internal sealed class ButtrWizardProjectModel {
        private readonly string m_UnityVersion;
        private readonly string m_ProjectName;
        private readonly string m_ButtrVersion;

        internal ButtrWizardProjectModel(string unityVersion, string projectName, string buttrVersion) {
            m_UnityVersion = unityVersion;
            m_ProjectName = projectName;
            m_ButtrVersion = buttrVersion;
        }

        internal string UnityVersion => m_UnityVersion;
        internal string ProjectName => m_ProjectName;
        internal string ButtrVersion => m_ButtrVersion;
    }
}