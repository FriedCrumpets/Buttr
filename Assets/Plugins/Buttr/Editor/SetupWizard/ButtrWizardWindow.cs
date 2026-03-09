using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Buttr.Editor.SetupWizard {
    /// <summary>
    /// Composition root for the Setup Wizard. Creates the Model, View,
    /// Presenter, and Mediator, wires them together, and manages the
    /// EditorWindow lifecycle.
    /// </summary>
    internal sealed class ButtrWizardWindow : EditorWindow {
        private const string WindowTitle = "Buttr Setup Wizard";
        private const float WindowWidth = 520f;
        private const float WindowHeight = 580f;

        private ButtrWizardMediator m_Mediator;

        [InitializeOnLoadMethod]
        private static void ShowIfNotSetup() {
            if (ButtrProjectScaffolder.HasBeenSetUp) return;

            EditorApplication.delayCall += ShowWindow;
        }

        [MenuItem("Tools/Buttr/Setup Wizard")]
        internal static void ShowWindow() {
            var window = GetWindow<ButtrWizardWindow>(true, "Buttr Setup Wizard");
            window.titleContent = new GUIContent(WindowTitle);
            window.minSize = new Vector2(WindowWidth, WindowHeight);
            window.maxSize = new Vector2(WindowWidth, WindowHeight);

            window.Show();
            
            EditorApplication.delayCall -= ShowWindow;
        }

        private void CreateGUI() {
            var uxmlPath = SetupWizardPaths.SetupWizardUxml;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            if (visualTree == null) {
                Debug.LogError($"[Buttr] Setup Wizard UXML not found at: {uxmlPath}");
                return;
            }

            visualTree.CloneTree(rootVisualElement);

            var model = new ButtrWizardModel(
                unityVersion: Application.unityVersion,
                projectName: Application.productName,
                buttrVersion: "2.0.0" // TODO: Read from package.json
            );

            var view = new ButtrWizardView(rootVisualElement);
            var presenter = new ButtrWizardPresenter(model);
            m_Mediator = new ButtrWizardMediator(model, view, presenter);

            m_Mediator.CloseRequested += Close;
            m_Mediator.Initialize();
        }

        private void OnDestroy() {
            if (m_Mediator != null) {
                m_Mediator.CloseRequested -= Close;
                m_Mediator.Dispose();
                m_Mediator = null;
            }
        }
    }
}