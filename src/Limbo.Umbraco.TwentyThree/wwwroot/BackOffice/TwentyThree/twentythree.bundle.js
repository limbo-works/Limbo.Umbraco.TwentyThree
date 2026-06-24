export const manifests = [
  {
    type: "propertyEditorUi",
    alias: "Limbo.PropertyEditorUi.TwentyThree",
    name: "TwentyThree Property Editor UI",
    js: () => import("./twentythree-property-editor-ui.element.js"),
    elementName: "umb-property-editor-ui-twentythree",
    meta: {
      label: "TwentyThree",
      propertyEditorSchemaAlias: "Limbo.Umbraco.TwentyThree",
      icon: "icon-television",
      group: "media",
      supportsReadOnly: true
    }
  }
];
