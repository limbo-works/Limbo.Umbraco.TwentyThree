using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

#pragma warning disable 1591

namespace Limbo.Umbraco.TwentyThree.PropertyEditors;

/// <summary>
/// Represents a block list property editor.
/// </summary>
[DataEditor(EditorAlias, ValueType = ValueTypes.Json)]
public class TwentyThreeEditor : DataEditor {

    private readonly IIOHelper _ioHelper;

    #region Constants

    public const string EditorAlias = "Limbo.Umbraco.TwentyThree";

    #endregion

    public TwentyThreeEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper) : base(dataValueEditorFactory) {
        _ioHelper = ioHelper;
    }

    #region Member methods

    protected override IConfigurationEditor CreateConfigurationEditor() {
        return new TwentyThreeConfigurationEditor(_ioHelper);
    }

    #endregion

}
