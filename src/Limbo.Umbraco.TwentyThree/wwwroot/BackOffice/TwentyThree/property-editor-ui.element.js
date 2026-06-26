import { html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";

import { TwentyThreeService } from "@limbo/twentythree/service";

import {
    TWENTYTHREE_VIDEO_PICKER_MODAL,
    TWENTYTHREE_SPOT_PICKER_MODAL,
    TWENTYTHREE_PLAYER_PICKER_MODAL,
    TWENTYTHREE_UPLOAD_MODAL
} from "./modals/tokens.js";

// Main TwentyThree property editor — full parity with the AngularJS Video.js/Video.html editor.
// [CHANGE: Umbraco 17 migration] Related: twentythree.bundle.js, Service.js, modals/*

const AUTOPLAY_OPTIONS = [
    { alias: "inherit", label: "Inherit" },
    { alias: "enabled", label: "Enabled" },
    { alias: "disabled", label: "Disabled" }
];

const LOOP_OPTIONS = AUTOPLAY_OPTIONS;

const ENDON_OPTIONS = [
    { alias: "inherit", label: "Inherit", title: "Inherit from player or embed code" },
    { alias: "share", label: "Share", title: "Show share" },
    { alias: "browse", label: "Browse", title: "Show recommended videos" },
    { alias: "loop", label: "Loop", title: "Loop through recommendations" },
    { alias: "thumbnail", label: "Thumbnail", title: "Show thumbnail" }
];

const DEFAULT_CONFIG = {
    autoplay: "inherit",
    loop: "inherit",
    endOn: "inherit",
    hideSite: false,
    hideEmbed: false,
    hidePlayer: false,
    allowVideos: true,
    allowSpots: true,
    showUploadLink: true,
    descriptionMaxLength: 0
};

export class TwentyThreePropertyEditorUiElement extends UmbLitElement {

    // NOTE: "value" intentionally uses manual accessors (below) rather than a reactive property, so Lit does
    // not replace the get/set that hydrate the display state.
    static properties = {
        readonly: { type: Boolean, reflect: true },
        _loading: { state: true },
        _embed: { state: true }
    };

    #config = { ...DEFAULT_CONFIG };
    #value = null;
    #video = null;
    #spot = null;
    #thumbnails = null;
    #debounce = 0;

    constructor() {
        super();
        this._loading = false;
        this._embed = false;
    }

    set config(config) {
        if (!config) return;
        this.#config = {
            autoplay: config.getValueByAlias("autoplay") ?? "inherit",
            loop: config.getValueByAlias("loop") ?? "inherit",
            endOn: config.getValueByAlias("endOn") ?? "inherit",
            hideSite: config.getValueByAlias("hideSite") === true,
            hideEmbed: config.getValueByAlias("hideEmbed") === true,
            hidePlayer: config.getValueByAlias("hidePlayer") === true,
            allowVideos: config.getValueByAlias("allowVideos") !== false,
            allowSpots: config.getValueByAlias("allowSpots") !== false,
            showUploadLink: config.getValueByAlias("showUploadLink") !== false,
            descriptionMaxLength: config.getValueByAlias("descriptionMaxLength") ?? 0
        };
        this.requestUpdate();
    }

    get value() {
        return this.#value;
    }

    set value(value) {
        const old = this.#value;
        this.#value = value && typeof value === "object" ? value : null;
        this.#hydrate();
        this.requestUpdate("value", old);
    }

    // Parses the stored video/spot payload into display state.
    #hydrate() {
        const value = this.#value;
        this.#video = null;
        this.#spot = null;
        this.#thumbnails = null;

        if (!value) return;

        if (value.video?._data) {
            this.#video = this.#parse(value.video._data);
            this.#thumbnails = TwentyThreeService.getThumbnails(this.#video);
            this._embed = !!(value.source && value.source.indexOf("<") >= 0);
        } else if (value.spot?._data) {
            this.#spot = this.#parse(value.spot._data);
            this.#thumbnails = value.thumbnails ?? null;
        }
    }

    #parse(data) {
        try {
            return typeof data === "string" ? JSON.parse(data) : data;
        } catch {
            return null;
        }
    }

    #commit(value) {
        this.value = value;
        this.dispatchEvent(new UmbChangeEvent());
    }

    #source() {
        return this.#value?.source ?? "";
    }

    // Mirrors the AngularJS vm.setVideo: builds the persisted value from a picked/looked-up item.
    #setVideo(item, source, refresh) {
        if (!item) {
            this.#commit(null);
            return;
        }

        const next = { ...(this.#value ?? {}) };
        if (source) next.source = source;

        next.site = item.site;
        next.credentials = item.credentials;

        if (item.video) {
            delete next.spot;
            delete next.thumbnails;

            next.parameters = item.parameters;
            next.video = { _data: JSON.stringify(item.video) };

            if (!next.player) {
                next.player = item.player;
            } else if (!refresh && item.parameters?.playerId) {
                next.player = item.player;
            }

            if (!next.embed) next.embed = { autoplay: "inherit", loop: "inherit", endOn: "inherit" };
        } else if (item.spot) {
            delete next.parameters;
            delete next.video;
            delete next.player;
            delete next.embed;

            next.spot = { _data: JSON.stringify(item.spot) };
            next.thumbnails = item.thumbnails;
        }

        this._loading = false;
        this.#commit(next);
    }

    async #getVideo(source, refresh) {
        const value = (source ?? this.#source()).trim();
        if (!value) {
            this.#setVideo(null);
            return;
        }

        this._loading = true;
        try {
            const response = await TwentyThreeService.getVideo(value);
            this.#setVideo(response, null, refresh);
        } catch (error) {
            const message = error instanceof Error ? error.message : "An unknown error occured.";
            this.#notifyError(message);
        } finally {
            this._loading = false;
        }
    }

    #notifyError(message) {
        // Surfaces the server message; the backoffice notification context could be consumed here if desired.
        console.error("[TwentyThree]", message);
        this.dispatchEvent(new CustomEvent("error", { detail: message, bubbles: true, composed: true }));
    }

    #onSourceInput(event) {
        const source = event.target.value ?? "";
        this.#value = { ...(this.#value ?? {}), source };
        this._embed = source.indexOf("<") >= 0;
        this.dispatchEvent(new UmbChangeEvent());
        window.clearTimeout(this.#debounce);
        this.#debounce = window.setTimeout(() => this.#getVideo(source), 250);
    }

    #onRefresh() {
        this.#getVideo(undefined, true);
    }

    async #openModal(token, data) {
        const modalManager = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);
        const modal = modalManager.open(this, token, data ? { data } : undefined);
        return modal.onSubmit().catch(() => undefined);
    }

    async #addVideo() {
        const item = await this.#openModal(TWENTYTHREE_VIDEO_PICKER_MODAL, { config: this.#config });
        if (item) this.#setVideo(item, item.url);
    }

    async #addSpot() {
        const item = await this.#openModal(TWENTYTHREE_SPOT_PICKER_MODAL, { config: this.#config });
        if (item) this.#setVideo(item, item.source);
    }

    async #uploadExternal() {
        await this.#openModal(TWENTYTHREE_UPLOAD_MODAL);
    }

    async #selectPlayer() {
        const player = await this.#openModal(TWENTYTHREE_PLAYER_PICKER_MODAL, {
            credentials: this.#value?.credentials,
            selected: this.#value?.player
        });
        if (player) this.#commit({ ...this.#value, player });
    }

    #setEmbedOption(field, alias) {
        const embed = { autoplay: "inherit", loop: "inherit", endOn: "inherit", ...(this.#value?.embed ?? {}) };
        embed[field] = alias;
        this.#commit({ ...this.#value, embed });
    }

    #formatDuration(seconds) {
        if (seconds === null || seconds === undefined || Number.isNaN(Number(seconds))) return null;
        const total = Math.max(0, Math.floor(Number(seconds)));
        const h = Math.floor(total / 3600);
        const m = Math.floor((total % 3600) / 60);
        const s = total % 60;
        const pad = (n) => String(n).padStart(2, "0");
        return h > 0 ? `${h}:${pad(m)}:${pad(s)}` : `${m}:${pad(s)}`;
    }

    render() {
        return html`
            <div class="editor ${this._loading ? "is-loading" : ""}">
                ${this.#renderInput()}
                ${this.#spot ? this.#renderSpot() : nothing}
                ${this.#video ? this.#renderVideo() : nothing}
                ${this._loading ? html`<uui-loader-bar></uui-loader-bar>` : nothing}
            </div>
        `;
    }

    #renderInput() {
        const source = this.#source();
        return html`
            <div class="block input">
                <h5>URL or embed code</h5>
                ${this._embed
                    ? html`<uui-textarea
                          class="source"
                          rows="5"
                          .value=${source}
                          ?disabled=${this.readonly}
                          placeholder="Enter the URL or embed code of the video here..."
                          @input=${this.#onSourceInput}></uui-textarea>`
                    : html`<uui-input
                          class="source"
                          .value=${source}
                          ?disabled=${this.readonly}
                          placeholder="Enter the URL of the video here..."
                          @input=${this.#onSourceInput}></uui-input>`}
                <div class="actions">
                    ${this.#config.allowVideos
                        ? html`<uui-button look="secondary" label="Select video" ?disabled=${this.readonly} @click=${this.#addVideo}></uui-button>`
                        : nothing}
                    ${this.#config.allowSpots
                        ? html`<uui-button look="secondary" label="Select spot" ?disabled=${this.readonly} @click=${this.#addSpot}></uui-button>`
                        : nothing}
                    ${this.#config.showUploadLink
                        ? html`<uui-button look="secondary" label="Upload video" ?disabled=${this.readonly} @click=${this.#uploadExternal}></uui-button>`
                        : nothing}
                    ${this.#video
                        ? html`<uui-button look="secondary" label="Refresh current video" ?disabled=${this.readonly} @click=${this.#onRefresh}></uui-button>`
                        : nothing}
                    ${this.#spot
                        ? html`<uui-button look="secondary" label="Refresh current spot" ?disabled=${this.readonly} @click=${this.#onRefresh}></uui-button>`
                        : nothing}
                </div>
            </div>
        `;
    }

    #renderSpot() {
        const spot = this.#spot;
        const thumbnail = Array.isArray(this.#thumbnails) ? this.#thumbnails.find((x) => x.alias === "medium") : null;
        return html`
            <div class="block">
                <h5>Spot</h5>
                <div class="box card-row">
                    <div class="thumbnail"><img src=${thumbnail?.url ?? ""} alt=${spot.spot_name ?? ""} /></div>
                    <table>
                        <tr><th>ID</th><td><code>${spot.spot_id}</code></td></tr>
                        <tr><th>Title</th><td>${spot.spot_name}</td></tr>
                        <tr><th>Videos</th><td>${spot.video_count}</td></tr>
                    </table>
                </div>
            </div>
        `;
    }

    #renderVideo() {
        const video = this.#video;
        const value = this.#value;
        const thumbnail = this.#thumbnails?.medium;
        const duration = video.video_length;
        const appUrl = TwentyThreeService.getAppUrl(value?.site, video.photo_id);
        const avg = duration && video.avg_playtime ? this.#formatDuration(duration * (video.avg_playtime / 100)) : null;

        return html`
            <div class="block">
                <h5>Video</h5>
                <div class="box">
                    ${appUrl
                        ? html`<a class="app-url" href=${appUrl} target="_blank" rel="noopener noreferrer" title="Manage the video in the TwentyThree control panel"><uui-icon name="icon-out"></uui-icon></a>`
                        : nothing}
                    <div class="card-row">
                        <div class="thumbnail"><img src=${thumbnail?.url ?? ""} alt=${video.title ?? ""} /></div>
                        <table>
                            <tr><th>ID</th><td><code>${video.photo_id}</code></td></tr>
                            <tr><th>Title</th><td>${video.title}</td></tr>
                            <tr><th>Duration</th><td>${this.#formatDuration(duration) ?? ""}</td></tr>
                            <tr><th>Total plays</th><td>${video.view_count ?? 0}</td></tr>
                            <tr><th>Avg. play time</th><td>${avg ?? ""}</td></tr>
                        </table>
                    </div>
                    ${video.tags?.length
                        ? html`<div class="tags">${video.tags.map((t) => html`<span class="tag">${t}</span>`)}</div>`
                        : nothing}
                    ${video.content_text ? html`<div class="description">${video.content_text.trim()}</div>` : nothing}
                </div>
            </div>

            ${!this.#config.hideSite && value?.site ? this.#renderSite(value.site) : nothing}
            ${!this.#config.hideEmbed ? this.#renderEmbed() : nothing}
        `;
    }

    #renderSite(site) {
        return html`
            <div class="block">
                <h5>Account</h5>
                <div class="box">
                    <table>
                        <tr><th>ID</th><td><code>${site.id}</code></td></tr>
                        <tr><th>Name</th><td>${site.name}</td></tr>
                        <tr><th>Domain</th><td>${site.secureDomain}</td></tr>
                    </table>
                </div>
            </div>
        `;
    }

    #renderEmbed() {
        const value = this.#value;
        const embed = value?.embed ?? {};
        return html`
            <div class="block">
                <h5>Embed</h5>
                <div class="box embed">
                    ${!this.#config.hidePlayer ? html`
                        <div class="property">
                            <div class="label">Player<br /><small>Select the player to be used when the video is embedded.</small></div>
                            <div class="value player">
                                <span>${value?.player?.name ?? "Default player"}</span>
                                <uui-button look="secondary" compact label="Change" ?disabled=${this.readonly} @click=${this.#selectPlayer}></uui-button>
                            </div>
                        </div>
                    ` : nothing}
                    ${this.#renderOption("Autoplay", "Select whether the video should automatically start playing.", "autoplay", AUTOPLAY_OPTIONS, embed.autoplay)}
                    ${this.#renderOption("Loop", "Select whether the video should loop.", "loop", LOOP_OPTIONS, embed.loop)}
                    ${this.#renderOption("When video ends", "Select what happens when the video ends.", "endOn", ENDON_OPTIONS, embed.endOn)}
                </div>
            </div>
        `;
    }

    #renderOption(label, description, field, options, current) {
        const overridden = this.#config[field] !== "inherit";
        const activeAlias = overridden ? this.#config[field] : (current ?? "inherit");
        const activeLabel = options.find((o) => o.alias === activeAlias)?.label ?? activeAlias;
        return html`
            <div class="property">
                <div class="label">${label}<br /><small>${description}</small></div>
                <div class="value">
                    ${overridden
                        ? html`<div class="overridden"><strong>${activeLabel}</strong><small>Overridden by data type.</small></div>`
                        : html`<div class="button-list">
                              ${options.map((o) => html`
                                  <button
                                      type="button"
                                      class=${o.alias === (current ?? "inherit") ? "active" : ""}
                                      title=${o.title ?? ""}
                                      ?disabled=${this.readonly}
                                      @click=${() => this.#setEmbedOption(field, o.alias)}>${o.label}</button>
                              `)}
                          </div>`}
                </div>
            </div>
        `;
    }

    static styles = css`
        :host { display: block; }

        .block { margin-bottom: var(--uui-size-layout-1); }
        .block > h5 { margin: 0 0 var(--uui-size-space-2); }

        .source { width: 100%; }
        .actions { display: flex; flex-wrap: wrap; gap: var(--uui-size-space-2); margin-top: var(--uui-size-space-3); }

        .box {
            position: relative;
            padding: var(--uui-size-space-4);
            border: 1px solid var(--uui-color-border);
            border-radius: var(--uui-border-radius);
            background: var(--uui-color-surface-alt);
        }

        .card-row { display: flex; gap: var(--uui-size-space-4); align-items: flex-start; }
        .thumbnail { flex: 0 0 270px; max-width: 270px; aspect-ratio: 16 / 9; border-radius: var(--uui-border-radius); overflow: hidden; background: var(--uui-color-surface); }
        .thumbnail img { width: 100%; height: 100%; object-fit: cover; display: block; }

        table { border-collapse: collapse; }
        th { text-align: left; padding: 2px var(--uui-size-space-4) 2px 0; color: var(--uui-color-text-alt); font-weight: 600; vertical-align: top; white-space: nowrap; }
        td { padding: 2px 0; }

        .app-url { position: absolute; top: var(--uui-size-space-3); right: var(--uui-size-space-3); color: var(--uui-color-text-alt); }

        .tags { display: flex; flex-wrap: wrap; gap: var(--uui-size-space-1); margin-top: var(--uui-size-space-3); }
        .tag { padding: 2px 8px; border-radius: 1rem; background: var(--uui-color-surface); font-size: var(--uui-font-size-1); }
        .description { margin-top: var(--uui-size-space-3); color: var(--uui-color-text-alt); white-space: pre-wrap; }

        .embed { display: grid; gap: var(--uui-size-space-4); }
        .property { display: grid; grid-template-columns: minmax(160px, 1fr) 2fr; gap: var(--uui-size-space-4); align-items: center; }
        .property .label small { color: var(--uui-color-text-alt); }
        .player { display: flex; gap: var(--uui-size-space-3); align-items: center; }

        .button-list { display: inline-flex; border: 1px solid var(--uui-color-border); border-radius: var(--uui-border-radius); overflow: hidden; }
        .button-list button {
            border: 0;
            border-right: 1px solid var(--uui-color-border);
            padding: var(--uui-size-space-2) var(--uui-size-space-4);
            background: var(--uui-color-surface);
            color: inherit;
            font: inherit;
            cursor: pointer;
        }
        .button-list button:last-child { border-right: 0; }
        .button-list button.active { background: var(--uui-color-selected); color: var(--uui-color-selected-contrast); }
        .button-list button:disabled { cursor: not-allowed; opacity: 0.6; }

        .overridden { display: flex; flex-direction: column; }
        .overridden small { color: var(--uui-color-text-alt); }

        .is-loading { opacity: 0.6; pointer-events: none; }
    `;

}

customElements.define("twentythree-property-editor-ui", TwentyThreePropertyEditorUiElement);

export default TwentyThreePropertyEditorUiElement;
