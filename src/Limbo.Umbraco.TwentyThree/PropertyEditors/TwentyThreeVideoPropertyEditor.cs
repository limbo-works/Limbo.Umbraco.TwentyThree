using Limbo.Umbraco.TwentyThree.Constants;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable 1591

namespace Limbo.Umbraco.TwentyThree.PropertyEditors;

/// <summary>
/// Represents a block list property editor.
/// </summary>
[DataEditor(EditorAlias, ValueType = ValueTypes.Json)]
public class TwentyThreeVideoPropertyEditor : DataEditor {

    private readonly IIOHelper _ioHelper;

    #region Constants

    public const string EditorAlias = TwentyThreePropertyEditorSchemaAliases.Video;

    public const string EditorUiAlias = TwentyThreePropertyEditorUiAliases.Video;

    public const string EditorName = "Limbo TwentyThree Video";

    public const string EditorIcon = "limbo-twentythree-alt";

    public const string EditorGroup = "Limbo";

    public const string EditorValueType = ValueTypes.Json;

    #endregion

    public TwentyThreeVideoPropertyEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper) : base(dataValueEditorFactory) {
        _ioHelper = ioHelper;
    }

    #region Member methods

    protected override IConfigurationEditor CreateConfigurationEditor() {
        return new TwentyThreeVideoConfigurationEditor(_ioHelper);
    }

    #endregion

}