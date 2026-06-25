import { html, css, when } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event";
import { UmbFormControlMixin } from "@umbraco-cms/backoffice/validation";

import { TwentyThreeService } from "@limbo/twentythree/service";

import "@limbo/video/elements/duration";

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

class LimboTwentyThreeVideoElement extends UmbFormControlMixin(UmbLitElement, undefined) {

    static properties = {
        readonly: { type: Boolean, reflect: true },
        mandatory: { type: Boolean },
        mandatoryMessage: { type: String },
        _loading: { state: true },
        _error: { state: true }
    };

    static styles = css`
    :host {
      display: block;
    }

    .shell {
      display: grid;
      gap: var(--uui-size-layout-2);
      padding: var(--uui-size-layout-1);
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

    .source:focus {
      outline: 2px solid var(--uui-color-border-emphasis);
      outline-offset: 1px;
    }

    .notice {
      color: var(--uui-color-text-alt);
      font-size: var(--uui-font-size-1);
    }

    .notice.--error {
      color: var(--uui-color-danger);
    }

    .preview {
      display: grid;
      gap: var(--uui-size-space-4);
      padding: var(--uui-size-layout-2);
      border: 1px solid var(--uui-color-border);
      border-radius: var(--uui-border-radius);
      background: var(--uui-color-surface-alt);
    }

    .preview h4,
    .preview h5,
    .preview p {
      margin: 0;
    }

    .grid {
      display: grid;
      gap: var(--uui-size-space-2);
      grid-template-columns: repeat(auto-fit, minmax(14rem, 1fr));
    }

    .field {
      display: grid;
      gap: var(--uui-size-space-1);
    }

    .label {
      font-size: var(--uui-font-size-1);
      color: var(--uui-color-text-alt);
      text-transform: uppercase;
      letter-spacing: 0.04em;
    }

    .value {
      overflow-wrap: anywhere;
    }

    .embed {
      display: grid;
      gap: var(--uui-size-space-2);
    }

    .meta-list {
      display: flex;
      flex-wrap: wrap;
      gap: var(--uui-size-space-2);
    }
  `;

    #config = { ...DEFAULT_CONFIG };
    _loading = false;
    _error = "";
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
            endOn: config.getValueByAlias("endOn") ?? "inherit"
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
        this._error = "";
        this._loading = false;
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
            this.#setValue(this.#buildVideoValue(data, source, currentValue));
            this.requestUpdate();
            return;
        }
        if (data.type === "spot") {
            this.#setValue(this.#buildSpotValue(data, source));
            this.requestUpdate();
        }
    }

    async #lookup(source) {

        const requestId = ++this.#requestToken; // TODO: what is this used for?
        this._loading = true;
        this._error = "";

        TwentyThreeService.getVideo(source).then((response) => {
            this._loading = false;
            this.#applyResponse(response.data, source);
        }, (response) => {
            this._error = response.text;
            this._loading = false;
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

        this._loading = true;
        this._error = "";
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

    #renderStatus() {

        if (this._loading) {
            return html`<p class="notice">Loading video data...</p>`;
        }

        if (this._error) {
            return html`<p class="notice --error">${this._error}</p>`;
        }

        return html`<p class="notice">Paste a TwentyThree URL or embed code to load the video metadata.</p>`;

    }

    #renderPreview() {

        const value = this.#currentValue();

        if (!value) return html``;

        const type = value.type ?? "video";
        const details = type === "spot" ? this.#parseJsonField(value.spot) : this.#parseJsonField(value.video);
        const title = type === "spot" ? details?.spot_name : details?.title;
        const id = type === "spot" ? details?.spot_id : details?.photo_id;
        const duration = type === "spot" ? null : details?.video_length;

        return html`
            <div class="preview">
                <div class="meta-list">
                    <uui-tag color="positive">${type}</uui-tag>
                    ${value.site?.secureDomain ? html`<uui-tag>${value.site.secureDomain}</uui-tag>` : null}
                    ${value.credentials?.name ? html`<uui-tag>${value.credentials.name}</uui-tag>` : null}
                </div>
                <div class="grid">
                    <div class="field">
                        <div class="label">Title</div>
                        <div class="value">${title ?? "Untitled"}</div>
                    </div>
                    <div class="field">
                        <div class="label">ID</div>
                        <div class="value">${id ?? "-"}</div>
                    </div>
                    ${when(duration, () => html`
                        <div class="field">
                            <div class="label">Duration</div>
                            <div class="value">
                                <limbo-video-duration .value=${details.video_length}></limbo-video-duration>
                            </div>
                        </div>
                    `)}
                    ${when(value.site?.secureDomain, () => html`
                        <div class="field">
                            <div class="label">Site</div>
                            <div class="value">${value.site.secureDomain}</div>
                        </div>
                    `)}
                </div>
                ${when(type === "video" && !this.#config.hidePlayer, () => html`
                    <div class="embed">
                        <h5>Player</h5>
                        <div class="value">${value.player?.name ?? "-"}</div>
                    </div>
                `)}
                ${when(!this.#config.hideEmbed, () => html`
                    <div class="embed">
                        <h5>Embed</h5>
                        <div class="grid">
                            <div class="field">
                                <div class="label">Autoplay</div>
                                <div class="value">${value.embed?.autoplay ?? this.#config.autoplay}</div>
                            </div>
                            <div class="field">
                                <div class="label">Loop</div>
                                <div class="value">${value.embed?.loop ?? this.#config.loop}</div>
                            </div>
                            <div class="field">
                                <div class="label">End on</div>
                                <div class="value">${value.embed?.endOn ?? this.#config.endOn}</div>
                            </div>
                        </div>
                    </div>
                `)}
                ${when(type === "spot" && Array.isArray(value.thumbnails), () => html`
                    <div class="embed">
                        <h5>Thumbnails</h5>
                        <div class="value">${value.thumbnails.length} available</div>
                    </div>
                `)}
            </div>
        `;

    }

    #parseJsonField(field) {
        if (!field || typeof field !== "object" || !field._data) return field;
        try {
            return JSON.parse(field._data);
        } catch {
            return null;
        }
    }

    #renderEditor(source, currentValue) {
        return html`
            <div class="editor">
                <textarea class="source" .value=${source} ?disabled=${this.readonly} placeholder="Paste a TwentyThree URL or embed code" @input=${this.#onSourceInput}></textarea>

                <div class="actions">
                    <uui-button
                    color="positive"
                    look="primary"
                    label="Refresh"
                    ?disabled=${this.readonly || !source.trim()}
                    @click=${this.#onRefresh}></uui-button>

                    <uui-button
                    color="default"
                    look="secondary"
                    label="Clear"
                    ?disabled=${this.readonly || !source.trim()}
                    @click=${this.#clear}></uui-button>

                    ${when(this.#config.showUploadLink && currentValue?.credentials?.uploadUrl, () => html`
                        <uui-button
                        color="default"
                        look="secondary"
                        label="Upload video"
                        href=${currentValue.credentials.uploadUrl}
                        target="_blank"
                        rel="noopener noreferrer"></uui-button>
                    `)}
            </div>
        `;
    }

    render() {
        const source = this.#sourceValue();
        const currentValue = this.#currentValue();
        return html`
            <section class="shell">
                ${this.#renderEditor(source, currentValue)}
                ${this.#renderStatus()}
                ${this.#renderPreview()}
            </section>
        `;
    }

}

customElements.define("limbo-twentythree-video", LimboTwentyThreeVideoElement);

export { LimboTwentyThreeVideoElement as element };