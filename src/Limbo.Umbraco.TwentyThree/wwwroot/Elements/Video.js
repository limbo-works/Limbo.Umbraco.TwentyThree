import { html, css, when, nothing } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event";
import { UmbFormControlMixin } from "@umbraco-cms/backoffice/validation";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT } from "@umbraco-cms/backoffice/notification";

import "@limbo/video/elements/duration";
import { TwentyThreeService } from "@limbo/twentythree/service";

import {
    LIMBO_TWENTYTHREE_SELECT_SPOT_MODAL,
    LIMBO_TWENTYTHREE_SELECT_VIDEO_MODAL,
    LIMBO_TWENTYTHREE_SELECT_PLAYER_MODAL,
    LIMBO_TWENTYTHREE_UPLOAD_VIDEO_MODAL
} from "@limbo/twentythree/modals/tokens";






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
    allowVideos: true,
    allowSpots: true,
    hideSite: false,
    hidePlayer: false,
    hideEmbed: false,
    showUploadLink: true,
    autoplay: "inherit",
    loop: "inherit",
    endOn: "inherit"
};

function parseJsonField(field) {
    if (!field || typeof field !== "object" || !field._data) return field;
    try {
        return JSON.parse(field._data);
    } catch {
        return null;
    }
}

class LimboTwentyThreeVideoElement extends UmbFormControlMixin(UmbLitElement, undefined) {

    static properties = {
        readonly: { type: Boolean, reflect: true },
        mandatory: { type: Boolean },
        mandatoryMessage: { type: String }
    };

    static styles = css`

        :host {
            display: block;
            position: relative;
        }

        .loading > div {
            opacity: 0.6;
            pointer-events: none
        }

        .loading uui-loader {
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translateX(-50%);
        }

        pre {
            white-space: pre-wrap;
            max-width: 650px;
        }

        .shell {
            display: grid;
            gap: var(--uui-size-layout-2);
            __padding: var(--uui-size-layout-1);
        }

        .editor {
            display: grid;
            gap: var(--uui-size-space-3);
        }

        .actions {
            display: flex;
            flex-wrap: wrap;
            gap: var(--uui-size-space-3);
            align-items: center;
            > div:first-child {
                flex: 1;
            }
            > div {
                white-space: nowrap;
                display: flex;
                gap: var(--uui-size-space-3);
            }
        }

        .source {
            width: 100%;
            min-height: 8rem;
            resize: vertical;
            box-sizing: border-box;
            padding: var(--uui-size-space-3);
            border: 1px solid var(--uui-color-border);
            border-radius: var(--uui-border-radius);
            background: var(--uui-color-surface);
            color: var(--uui-color-text);
            font: inherit;
        }

        .notice {
            color: var(--uui-color-text-alt);
            font-size: var(--uui-font-size-1);
        }

        .notice.--error {
            color: var(--uui-color-danger);
        }



        .block { margin-top: var(--uui-size-layout-1); }
        .block > h5 { margin: 0 0 var(--uui-size-space-2); }

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

        .tags { margin-top: 15px; display: flex; flex-wrap: wrap; gap: var(--uui-size-space-1); margin-top: var(--uui-size-space-3); }
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

        .property .label { font-weight: bold; }
        .property .label small { font-weight: normal; }

        .tag {
            background: #68676b;
            border-radius: 6px;
            display: inline-flex;
            padding: 2px 8px;
            color: #fff;
            font-size: 11px;
            line-height: 14px;
        }

  `;

    #video = null;
    #spot = null;
    #config = { ...DEFAULT_CONFIG };
    #loading = false;
    #error = "";
    #value = null;
    #sourceInput;
    #debounceTimer = 0;
    #requestToken = 0;

    set config(config) {
        if (!config) return;
        this.#config = {
            allowVideos: config.getValueByAlias("allowVideos") !== false,
            allowSpots: config.getValueByAlias("allowSpots") !== false,
            hideSite: config.getValueByAlias("hideSite") === true,
            hidePlayer: config.getValueByAlias("hidePlayer") === true,
            hideEmbed: config.getValueByAlias("hideEmbed") === true,
            showUploadLink: config.getValueByAlias("showUploadLink") !== false,
            autoplay: config.getValueByAlias("autoplay") ?? "inherit",
            loop: config.getValueByAlias("loop") ?? "inherit",
            endOn: config.getValueByAlias("endOn") ?? "inherit",
            descriptionMaxLength: config.getValueByAlias("descriptionMaxLength") ?? 0
        };
        this.requestUpdate();
    }

    get value() {
        return this.#value;
    }

    set value(value) {
        const oldValue = this.#value;
        this.#value = value ?? null;
        this.requestUpdate("value", oldValue);
    }

    constructor() {

        super();

        this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
            this._modalManagerContext = instance;
        });

        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (instance) => {
            this._notificationContext = instance;
        });

    }

    connectedCallback() {

        super.connectedCallback();

        const value = this.#currentValue();

        if (!value) return;

        if (!value.type && value.video) {
            this.#video = JSON.parse(JSON.stringify(value));
            this.#video.type = "video";
            this.#video.video = parseJsonField(value.video);
        } else if (value?.type === "video") {
            this.#video = JSON.parse(JSON.stringify(value));
            this.#video.video = parseJsonField(value.video);
        } else if (value?.type === "spot") {
            this.#spot = JSON.parse(JSON.stringify(value));
            this.#spot.spot = parseJsonField(value.spot);
        }

        console.log(value);

    }

    firstUpdated() {

        this.#sourceInput = this.renderRoot.querySelector(".source");

        if (this.#sourceInput) {
            this.addFormControlElement(this.#sourceInput);
        }

    }

    disconnectedCallback() {
        super.disconnectedCallback();
        window.clearTimeout(this.#debounceTimer);
    }

    #currentValue() {
        return this.value && typeof this.value === "object" ? this.value : null;
    }

    #sourceValue() {
        return this.#currentValue()?.source ?? "";
    }

    #serializeField(value) {
        return { _data: JSON.stringify(value) };
    }

    #setValue(nextValue) {
        this.value = nextValue;
        this.dispatchEvent(new UmbChangeEvent());
    }

    #clear() {
        this.#setValue(null);
        this.#video = null;
        this.#spot = null;
        this.#error = "";
        this.#loading = false;
    }

    #buildVideoValue(response, source, currentValue) {

        const embed = currentValue?.embed ?? {
            autoplay: this.#config.autoplay,
            loop: this.#config.loop,
            endOn: this.#config.endOn
        };

        return {
            type: "video",
            source,
            site: response.site,
            credentials: response.credentials,
            parameters: response.parameters,
            video: this.#serializeField(response.video),
            player: response.player,
            embed
        };

    }

    #buildSpotValue(response, source) {
        return {
            type: "spot",
            source,
            site: response.site,
            credentials: response.credentials,
            spot: this.#serializeField(response.spot),
            thumbnails: response.thumbnails
        };
    }

    #applyResponse(data, source) {
        const currentValue = this.#currentValue();
        if (data.type === "video") {
            this.#video = data;
            this.#setValue(this.#buildVideoValue(data, source, currentValue));
            this.requestUpdate();
            return;
        }
        if (data.type === "spot") {
            this.#spot = data;
            this.#setValue(this.#buildSpotValue(data, source));
            this.requestUpdate();
        }
    }

    async #lookup(source) {

        const requestId = ++this.#requestToken; // TODO: what is this used for?
        this.#loading = true;
        this.#error = "";
        this.requestUpdate();

        TwentyThreeService.getVideo(source).then((response) => {
            this.#loading = false;
            this.#applyResponse(response.data, source);
        }, (response) => {
            this.#error = response.textContent;
            this.#loading = false;
            this.requestUpdate();
        });

    }

    #scheduleLookup(source) {
        window.clearTimeout(this.#debounceTimer);
        const value = source.trim();
        if (!value) {
            this.#clear();
            return;
        }

        this.#loading = true;
        this.#error = "";
        this.#debounceTimer = window.setTimeout(() => {
            this.#lookup(value);
        }, 300);
    }

    #onSourceInput(event) {
        const source = event.target.value ?? "";
        const currentValue = this.#currentValue() ?? {};
        this.#setValue({ ...currentValue, source });
        this.#scheduleLookup(source);
    }

    #onRefresh() {
        const source = this.#sourceValue().trim();
        if (!source) {
            this.#clear();
            return;
        }

        this.#lookup(source);
    }

    #openSelectVideo() {

        const self = this;

        const modal = this._modalManagerContext?.open(this, LIMBO_TWENTYTHREE_SELECT_VIDEO_MODAL, { data: { config: this.#config } });

        modal.onSubmit().then(function (video) {
            self.#applyResponse(video, video.url);
        }, function () {
            // modal closed by the user
        });

    }

    #openSelectSpot() {

        const self = this;

        const modal = this._modalManagerContext?.open(this, LIMBO_TWENTYTHREE_SELECT_SPOT_MODAL, { data: { config: this.#config } });

        modal.onSubmit().then(function (spot) {
            self.#applyResponse(spot, spot.source);
        }, function () {
            // modal closed by the user
        });

    }

    #openSelectPlayer() {

        const credentials = this.#currentValue().credentials;

        const self = this;

        const modal = this._modalManagerContext?.open(this, LIMBO_TWENTYTHREE_SELECT_PLAYER_MODAL, {
            data: { config: self.#config, credentials }

        });

        modal.onSubmit().then(function (player) {
            self.#commit({ ...self.#value, player });
        }, function () {
            // modal closed by the user
        });

    }

    #openUploadVideo() {

        const self = this;

        const modal = this._modalManagerContext?.open(this, LIMBO_TWENTYTHREE_UPLOAD_VIDEO_MODAL, { data: { config: this.#config } });

    }

    #renderStatus() {

        if (this.#error) {
            return html`<p class="notice --error">${this.#error}</p>`;
        }

        return html``;

    }

    #renderVideo() {

        if (!this.#video) return html``;

        const value = this.#currentValue();

        const video = this.#video.video;

        // TODO: it is expensive to determine these for each render?
        const thumbnails = TwentyThreeService.getThumbnails(video);
        const appUrl = this.#video.site?.secureDomain ? "https://" + this.#video.site.secureDomain + "/manage/video/" + video.photo_id : null;

        return html`
            <div class="block">
                <h5>Video</h5>
                <div class="box">
                    ${when(appUrl, () => html`
                        <a class="app-url" href=${appUrl} target="_blank" rel="noopener noreferrer" title="Manage the video in the TwentyThree control panel"><uui-icon name="icon-out"></uui-icon></a>
                    `)}
                    <div class="card-row">
                        ${when(thumbnails.medium, () => html`
                            <div class="thumbnail"><img src=${thumbnails.medium.url ?? ""} alt=${video.title ?? ""} /></div>
                        `)}
                        <table>
                            <tr><th>ID</th><td><code>${video.photo_id}</code></td></tr>
                            <tr><th>Title</th><td>${video.title}</td></tr>
                            <tr><th>Duration</th><td><limbo-video-duration .value=${video.video_length}></limbo-video-duration></td></tr>
                            <tr><th>Total plays</th><td>${video.view_count ?? 0}</td></tr>
                        </table>
                    </div>
                    ${when(video.tags?.length, () => html`
                        <div class="tags">${video.tags.map((t) => html`<span class="tag">${t}</span>`)}</div>
                    `)}
                    ${when(video.content_text, () => html`
                        <div class="description">${video.content_text.trim()}</div>
                    `)}
                </div>
            </div>
            ${when(!this.#config.hideSite && this.#video.site, () => this.#renderSite(this.#video.site)) }
            ${when(!this.#config.hideEmbed, () => this.#renderEmbed())}
        `;
    }

    #renderSpot() {

        const spot = this.#spot?.spot;

        if (!spot) return html``;

        const thumbnail = Array.isArray(spot.__thumbnails) ? spot.__thumbnails.find((x) => x.alias === "medium") : null;

        return html`
            <div class="block">
                <h5>Spot</h5>
                <div class="box card-row">
                    ${when(thumbnail, () => html`
                        <div class="thumbnail">
                            <img src=${thumbnail?.url ?? ""} alt=${spot.spot_name ?? ""} />
                        </div>
                    `)}
                    <table>
                        <tr><th>ID</th><td><code>${spot.spot_id}</code></td></tr>
                        <tr><th>Title</th><td>${spot.spot_name}</td></tr>
                        <tr><th>Videos</th><td>${spot.video_count}</td></tr>
                    </table>
                </div>
            </div>
        `;

    }

    #commit(value) {
        this.value = value;
        this.dispatchEvent(new UmbChangeEvent());
    }

    #setEmbedOption(field, alias) {
        const embed = { autoplay: "inherit", loop: "inherit", endOn: "inherit", ...(this.#value?.embed ?? {}) };
        embed[field] = alias;
        this.#commit({ ...this.#value, embed });
    }

    #renderEmbed() {
        const value = this.#value;
        const embed = value?.embed ?? {};
        return html`
            <div class="block">
                <h5>Embed</h5>
                <div class="box embed">
                    ${when(!this.#config.hidePlayer, () => html`
                        <div class="property">
                            <div class="label">Player<br /><small>Select the player to be used when the video is embedded.</small></div>
                            <div class="value">
                                <uui-ref-node id="refNode" name="${value?.player?.name ?? "Default player"}" details="Howdy" standalone selectonly readonly>
                                  <div slot="icon">
                                    <uui-icon name="icon-embed"></uui-icon>
                                  </div>
                                  <uui-action-bar slot="actions">
                                    <uui-button label="Change">
                                      <uui-icon name="edit" aria-hidden="true" @click=${this.#openSelectPlayer}></uui-icon>
                                    </uui-button>
                                  </uui-action-bar>
                                </uui-ref-node>
                            </div>
                        </div>
                    `)}
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

    #renderSite(site) {
        if (!site) return html``;
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

    #renderEditor(source, currentValue) {
        return html`
            <div class="editor">
                <textarea class="source" .value=${source} ?disabled=${this.readonly} placeholder="Paste a TwentyThree URL or embed code" @input=${this.#onSourceInput}></textarea>
                <div class="actions">
                    <div>
                        <uui-button color="positive" look="primary" label="Select video" @click=${() => this.#openSelectVideo()}></uui-button>
                        <uui-button color="default" look="outline" label="Select spot" @click=${() => this.#openSelectSpot()}></uui-button>
                        <uui-button color="default" look="outline" label="Upload video" @click=${() => this.#openUploadVideo()}></uui-button>
                    </div>
                    <div>
                        <uui-button
                            color="outline"
                            look="outline"
                            label="Refresh"
                            ?disabled=${this.readonly || !source.trim()}
                            @click=${this.#onRefresh}></uui-button>

                        <uui-button
                            color="default"
                            look="outline"
                            label="Clear"
                            ?disabled=${this.readonly || !source.trim()}
                            @click=${this.#clear}></uui-button>
                    </div>
            </div>
        `;
    }

    render() {
        const source = this.#sourceValue();
        const currentValue = this.#currentValue();
        return html`
            <div class="shell ${this.#loading ? "loading" : ""}">
                <div>
                    ${this.#renderEditor(source, currentValue)}
                    ${this.#renderStatus()}
                    ${this.#renderVideo()}
                    ${this.#renderSpot()}
                </div>
                ${when(false, () => html`
                    <h3>Video</h3>
                    <pre>${JSON.stringify(this.#video, null, 2)}</pre>
                    <h3>Spot</h3>
                    <pre>${JSON.stringify(this.#spot, null, 2)}</pre>
                    <h3>Current value</h3>
                    <pre>${JSON.stringify(currentValue, null, 2)}</pre>
                `)}
                ${this.#loading ? html`<uui-loader></uui-loader>` : nothing}
            </section>
        `;
    }

}

customElements.define("limbo-twentythree-video", LimboTwentyThreeVideoElement);

export { LimboTwentyThreeVideoElement as element };