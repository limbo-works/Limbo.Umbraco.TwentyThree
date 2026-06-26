import { UmbModalToken } from "@umbraco-cms/backoffice/modal";

// Modal tokens for the TwentyThree picker overlays. The alias of each token must match the modal extension
// alias registered in twentythree.bundle.js.
// [CHANGE: Umbraco 17 migration — replaces the AngularJS editorService overlays] Related: twentythree.bundle.js

export const TWENTYTHREE_VIDEO_PICKER_MODAL = new UmbModalToken("Limbo.Umbraco.TwentyThree.Modal.VideoPicker", {
    modal: { type: "sidebar", size: "large" }
});

export const TWENTYTHREE_SPOT_PICKER_MODAL = new UmbModalToken("Limbo.Umbraco.TwentyThree.Modal.SpotPicker", {
    modal: { type: "sidebar", size: "large" }
});

export const TWENTYTHREE_PLAYER_PICKER_MODAL = new UmbModalToken("Limbo.Umbraco.TwentyThree.Modal.PlayerPicker", {
    modal: { type: "sidebar", size: "small" }
});

export const TWENTYTHREE_UPLOAD_MODAL = new UmbModalToken("Limbo.Umbraco.TwentyThree.Modal.Upload", {
    modal: { type: "sidebar", size: "small" }
});
