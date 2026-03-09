using UnityEngine.UIElements;

namespace Buttr.Editor.SetupWizard {
    internal sealed class ButtrWizard2View {
        private readonly VisualElement m_Root;
        
        // tabs
        private readonly RadioButton m_StructureTab;
        private readonly RadioButton m_BootTab;
        private readonly RadioButton m_PersistenceTab;
        
        // panels
        private readonly VisualElement m_FoldersPanel;
        private readonly VisualElement m_BootPanel;
        private readonly VisualElement m_RepositoryPanel;

        // Structure tab elements
        private readonly Toggle m_CreateRootFolderToggle;
        private readonly TextField m_RootFolderNameField;
        private readonly Toggle m_ScaffoldFoldersToggle;
        private readonly TextField m_ScaffoldFolderNameField;
        private readonly Button m_AddFolderButton;
        private readonly TreeView m_FoldersTreeView;

        // Boot tab elements
        private readonly Toggle m_CreateBootSceneToggle;

        // Persistence tab elements
        private readonly Toggle m_SQLiteToggle;
        private readonly Toggle m_PlayerPrefsToggle;
        private readonly Toggle m_CustomToggle;
        
        public ButtrWizard2View(VisualElement root) {
            m_Root = root;
            
            // tabs
            var tabContainer = root.Q<VisualElement>("Title__Section--additional");
            var tabs = tabContainer.Query<RadioButton>().ToList();
            m_StructureTab = tabs.Count > 0 ? tabs[0] : null;
            m_BootTab = tabs.Count > 1 ? tabs[1] : null;
            m_PersistenceTab = tabs.Count > 2 ? tabs[2] : null;

            // panels
            m_FoldersPanel = root.Q<VisualElement>(ButtrWizardElements.SetupFolders);
            m_BootPanel = root.Q<VisualElement>(ButtrWizardElements.SetupBoot);
            m_RepositoryPanel = root.Q<VisualElement>(ButtrWizardElements.SetupRepository);

            // Structure
            var rootFolderConfig = m_FoldersPanel.Q<VisualElement>("RootFolder__Configuration");
            m_CreateRootFolderToggle = rootFolderConfig?.Q<Toggle>();
            m_RootFolderNameField = rootFolderConfig?.Q<TextField>();

            var scaffoldConfig = m_FoldersPanel.Q<VisualElement>("Scaffold__Configuration");
            m_ScaffoldFoldersToggle = scaffoldConfig?.Q<Toggle>();
            m_ScaffoldFolderNameField = scaffoldConfig?.Q<TextField>();
            m_AddFolderButton = scaffoldConfig?.Q<Button>();

            m_FoldersTreeView = m_FoldersPanel.Q<TreeView>(ButtrWizardElements.FoldersTreeView);

            // Boot
            var bootConfig = m_BootPanel.Q<VisualElement>("RootFolder__Configuration");
            m_CreateBootSceneToggle = bootConfig?.Q<Toggle>();

            // Persistence
            m_SQLiteToggle = root.Q<Toggle>(ButtrWizardElements.SQLiteToggle);
            m_PlayerPrefsToggle = root.Q<Toggle>(ButtrWizardElements.PlayerPrefsToggle);
            m_CustomToggle = root.Q<Toggle>(ButtrWizardElements.CustomToggle);
        }
    }
    
    /*
todo: Mediator stuff removed to reduce complexity of Version 2, will delete or include depending on community feedback
     * // Window 2 – tab selection
            m_StructureTab?.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue) Events.RaiseCustomTabSelected(CustomSetupTab.Structure);
            });
            m_BootTab?.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue) Events.RaiseCustomTabSelected(CustomSetupTab.Boot);
            });
            m_PersistenceTab?.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue) Events.RaiseCustomTabSelected(CustomSetupTab.Persistence);
            });

            // Window 2 – Structure
            m_CreateRootFolderToggle?.RegisterValueChangedCallback(evt => {
                Events.RaiseCreateRootFolderToggled(evt.newValue);
                if(evt.newValue) {
                    m_CreateRootFolderToggle.AddToClassList("unity-toggle__checked");
                }
                else {
                    m_CreateRootFolderToggle.RemoveFromClassList("unity-toggle__checked");
                }           
            });
            
            m_RootFolderNameField?.RegisterValueChangedCallback(evt =>
                Events.RaiseRootFolderNameChanged(evt.newValue));
            
            m_ScaffoldFoldersToggle?.RegisterValueChangedCallback(evt => {
                Events.RaiseScaffoldFoldersToggled(evt.newValue);
                if(evt.newValue) {
                    m_ScaffoldFoldersToggle.AddToClassList("unity-toggle__checked");
                }
                else {
                    m_ScaffoldFoldersToggle.RemoveFromClassList("unity-toggle__checked");
                }           
            });
            
            m_AddFolderButton?.RegisterCallback<ClickEvent>(_ =>
            {
                var name = m_ScaffoldFolderNameField?.value;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    Events.RaiseScaffoldFolderAdded(name.Trim());
                    m_ScaffoldFolderNameField.value = string.Empty;
                }
            });

            // Window 2 – Boot
            m_CreateBootSceneToggle?.RegisterValueChangedCallback(evt => {
                Events.RaiseCreateBootSceneToggled(evt.newValue);
                if(evt.newValue) {
                    m_CreateBootSceneToggle.AddToClassList("unity-toggle__checked");
                }
                else {
                    m_CreateBootSceneToggle.RemoveFromClassList("unity-toggle__checked");
                }         
            });

            // Window 2 – Persistence
            m_SQLiteToggle?.RegisterValueChangedCallback(evt => {
                Events.RaiseSQLiteToggled(evt.newValue);
                if(evt.newValue) {
                    m_SQLiteToggle.AddToClassList("unity-toggle__checked");
                }
                else {
                    m_SQLiteToggle.RemoveFromClassList("unity-toggle__checked");
                }         
            });
            
            m_PlayerPrefsToggle?.RegisterValueChangedCallback(evt => {
                Events.RaisePlayerPrefsToggled(evt.newValue);
                if(evt.newValue) {
                    m_PlayerPrefsToggle.AddToClassList("unity-toggle__checked");
                }
                else {
                    m_PlayerPrefsToggle.RemoveFromClassList("unity-toggle__checked");
                }         
            });
            
            m_CustomToggle?.RegisterValueChangedCallback(evt => {
                Events.RaiseCustomPersistenceToggled(evt.newValue);
                if(evt.newValue) {
                    m_CustomToggle.AddToClassList("unity-toggle__checked");
                }
                else {
                    m_CustomToggle.RemoveFromClassList("unity-toggle__checked");
                }         
            });
            
            // internal void SetCustomTab(CustomSetupTab tab)
        // {
        //     if (m_StructureTab != null) m_StructureTab.value = tab == CustomSetupTab.Structure;
        //     if (m_BootTab != null) m_BootTab.value = tab == CustomSetupTab.Boot;
        //     if (m_PersistenceTab != null) m_PersistenceTab.value = tab == CustomSetupTab.Persistence;
        //
        //     var structureLeft = tab == CustomSetupTab.Structure ? 0 : -520;
        //     var bootLeft = tab == CustomSetupTab.Boot ? 0 : (tab < CustomSetupTab.Boot ? 520 : -520);
        //     var persistenceLeft = tab == CustomSetupTab.Persistence ? 0 : 520;
        //
        //     SetWindowPosition(m_FoldersPanel, structureLeft);
        //     SetWindowPosition(m_BootPanel, bootLeft);
        //     SetWindowPosition(m_RepositoryPanel, persistenceLeft);
        // }
        
        internal void RebuildFolderTree(bool hasRoot, string rootName, IReadOnlyList<string> folders)
        {
            if (m_FoldersTreeView == null) return;

            m_TreeHasRoot = hasRoot;

            var id = 0;
            var rootItems = new List<TreeViewItemData<string>>();

            if (hasRoot)
            {
                // Root folder exists — scaffold folders are children
                var children = new List<TreeViewItemData<string>>();
                foreach (var folder in folders)
                    children.Add(new TreeViewItemData<string>(++id, folder));

                rootItems.Add(new TreeViewItemData<string>(++id, rootName, children));
            }
            else
            {
                // No root folder — scaffold folders are top-level items
                foreach (var folder in folders)
                    rootItems.Add(new TreeViewItemData<string>(++id, folder));
            }

            m_FoldersTreeView.SetRootItems(rootItems);

            // Load the FolderItem.uxml template for makeItem.
            // Cache it once — if it fails, fall back to a plain label.
            m_FolderItemTemplate ??= UnityEditor.AssetDatabase
                .LoadAssetAtPath<VisualTreeAsset>(SetupWizardPaths.FolderItemUxml);

            if (m_FolderItemTemplate != null)
            {
                m_FoldersTreeView.makeItem = () => m_FolderItemTemplate.Instantiate();
                m_FoldersTreeView.bindItem = BindFolderItem;
            }
            else
            {
                // Fallback if template isn't found
                m_FoldersTreeView.makeItem = () => new Label();
                m_FoldersTreeView.bindItem = (element, index) =>
                {
                    if (element is Label label)
                        label.text = m_FoldersTreeView.GetItemDataForIndex<string>(index);
                };
            }

            m_FoldersTreeView.Rebuild();
        }

        private void BindFolderItem(VisualElement element, int index)
        {
            var folderName = m_FoldersTreeView.GetItemDataForIndex<string>(index);

            // The root folder item is always index 0 when a root exists.
            // Without a root, every item is a scaffold folder.
            var isRootItem = m_TreeHasRoot && index == 0;

            // Set label text — root shows as-is, scaffold folders show with "/" prefix
            var label = element.Q<Label>();
            if (label != null)
                label.text = isRootItem ? folderName : $"/{folderName}";

            // Configure delete button — hidden on root, visible on scaffold folders
            var deleteButton = element.Q<Button>();
            if (deleteButton == null) return;

            if (isRootItem)
            {
                deleteButton.style.display = DisplayStyle.None;
            }
            else
            {
                deleteButton.style.display = DisplayStyle.Flex;

                // Unregister any previous handler to avoid stacking
                deleteButton.userData = folderName;
                deleteButton.clickable = new Clickable(() =>
                {
                    var name = deleteButton.userData as string;
                    if (!string.IsNullOrEmpty(name))
                        Events.RaiseScaffoldFolderRemoved(name);
                });
            }
        }
     */
}