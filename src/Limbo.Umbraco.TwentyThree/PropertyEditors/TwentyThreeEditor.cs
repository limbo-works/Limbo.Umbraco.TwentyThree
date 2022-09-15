﻿using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

#pragma warning disable 1591

namespace Limbo.Umbraco.TwentyThree.PropertyEditors {

    /// <summary>
    /// Represents a block list property editor.
    /// </summary>
    [DataEditor(EditorAlias, EditorName, EditorView, ValueType = ValueTypes.Json, Group = "Limbo", Icon = EditorIcon)]
    public class TwentyThreeEditor : DataEditor {
        
        private readonly IIOHelper _ioHelper;
        private readonly IEditorConfigurationParser _editorConfigurationParser;

        #region Constants

        internal const string EditorAlias = "Limbo.Umbraco.TwentyThree";

        internal const string EditorName = "Limbo TwentyThree Video";

        internal const string EditorView = "/App_Plugins/Limbo.Umbraco.TwentyThree/Views/Video.html";

        internal const string EditorIcon = "icon-limbo-twentythree color-limbo";

        #endregion

        #region Constructors

        public TwentyThreeEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser) : base(dataValueEditorFactory) {
            _ioHelper = ioHelper;
            _editorConfigurationParser = editorConfigurationParser;
        }

        #endregion

        #region Member methods

        protected override IConfigurationEditor CreateConfigurationEditor() {
            return new TwentyThreeConfigurationEditor(_ioHelper, _editorConfigurationParser);
        }

        #endregion

    }

}