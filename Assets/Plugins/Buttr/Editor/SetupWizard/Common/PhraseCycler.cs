namespace Buttr.Editor.SetupWizard {
    internal sealed class PhraseCycler {
        private readonly ButtrWizardModel m_Model;
        
        private int m_Index;
        
        private bool m_Cycling;
        private string[] m_Phrases;

        public PhraseCycler(ButtrWizardModel model) {
            m_Model = model;
        }

        public bool Cycling {
            set => m_Cycling = value;
        }
        
        public string[] Phrases {
            set => m_Phrases = value;
        }
        
        public void AdvancePhrase()
        {
            if (false == m_Cycling) return;
            if (m_Phrases.Length == 0) return;

            m_Model.InstallationModel.CurrentPhrase = m_Phrases[m_Index % m_Phrases.Length];
            m_Index++;
        }

        public void Reset() {
            m_Index = 0;
            m_Cycling = true;
        }
    }
}