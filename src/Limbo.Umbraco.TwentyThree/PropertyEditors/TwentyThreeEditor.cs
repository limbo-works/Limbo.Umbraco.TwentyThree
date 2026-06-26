using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable 1591

namespace Limbo.Umbraco.TwentyThree.PropertyEditors;

/// <summary>
/// Represents the server-side schema for the TwentyThree property editor. The editor UI lives client-side
/// (see <c>wwwroot/umbraco-package.json</c>), so this only defines the alias, value type and configuration.
/// </summary>
[DataEditor(EditorAlias, ValueType = ValueTypes.Json)]
public class TwentyThreeEditor : DataEditor {

    private readonly IIOHelper _ioHelper;

    #region Constants

    public const string EditorAlias = "Limbo.Umbraco.TwentyThree";

    public const string EditorName = "Limbo TwentyThree Video";

    #endregion

    #region Constructors

    public TwentyThreeEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper) : base(dataValueEditorFactory) {
        _ioHelper = ioHelper;
    }

    #endregion

    #region Member methods

    protected override IConfigurationEditor CreateConfigurationEditor() {
        return new TwentyThreeConfigurationEditor(_ioHelper);
    }

    #endregion

}