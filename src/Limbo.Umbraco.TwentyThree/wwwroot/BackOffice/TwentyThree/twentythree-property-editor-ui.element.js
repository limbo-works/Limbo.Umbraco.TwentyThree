import { html, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event";
import { UmbFormControlMixin } from "@umbraco-cms/backoffice/validation";

import { TwentyThreeService } from "@limbo/twentythree/service";

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

class TwentyThreePropertyEditorUiElement extends UmbFormControlMixin(UmbLitElement, undefined) {
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

  #applyResponse(response, source) {
    const currentValue = this.#currentValue();
    if (response.type === "video") {
      this.#setValue(this.#buildVideoValue(response, source, currentValue));
      return;
    }
    if (response.type === "spot") {
      this.#setValue(this.#buildSpotValue(response, source));
    }
  }

  async #lookup(source) {
    const requestId = ++this.#requestToken;
    this._loading = true;
      this._error = "";


      TwentyThreeService.getVideo(source).then((response) => {

          console.log(response);

      });



      return;





    try {
        const response = await fetch(`limbo/twentythree/GetVideo?source=${encodeURIComponent(source)}`, {
        credentials: "same-origin",
        headers: {
          "X-Requested-With": "XMLHttpRequest"
        }
      });

      const text = await response.text();
      if (requestId !== this.#requestToken) return;

      if (!response.ok) {
        this._error = text || "Unable to resolve the source.";
        this._loading = false;
        this.requestUpdate();
        return;
      }

      const data = text ? JSON.parse(text) : null;
      if (data) {
        this.#applyResponse(data, source);
      } else {
        this._error = "The server returned no video data.";
      }
    } catch (error) {
      if (requestId !== this.#requestToken) return;
      this._error = error instanceof Error ? error.message : "Unable to load the video.";
    } finally {
      if (requestId === this.#requestToken) {
        this._loading = false;
        this.requestUpdate();
      }
    }
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

  #formatDuration(seconds) {
    if (seconds === null || seconds === undefined || Number.isNaN(Number(seconds))) {
      return null;
    }

    const totalSeconds = Math.max(0, Math.floor(Number(seconds)));
    const hours = Math.floor(totalSeconds / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    const remaining = totalSeconds % 60;

    if (hours > 0) {
      return [hours, String(minutes).padStart(2, "0"), String(remaining).padStart(2, "0")].join(":");
    }

    return [minutes, String(remaining).padStart(2, "0")].join(":");
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
    if (!value) {
      return html``;
    }

    const type = value.type ?? "video";
    const details = type === "spot"
      ? this.#parseJsonField(value.spot)
      : this.#parseJsonField(value.video);
    const title = type === "spot" ? details?.spot_name : details?.title;
    const id = type === "spot" ? details?.spot_id : details?.photo_id;
    const duration = type === "spot" ? null : this.#formatDuration(details?.video_length);

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
          ${duration ? html`
            <div class="field">
              <div class="label">Duration</div>
              <div class="value">${duration}</div>
            </div>
          ` : null}
          ${value.site?.secureDomain ? html`
            <div class="field">
              <div class="label">Site</div>
              <div class="value">${value.site.secureDomain}</div>
            </div>
          ` : null}
        </div>

        ${type === "video" && !this.#config.hidePlayer ? html`
          <div class="embed">
            <h5>Player</h5>
            <div class="value">${value.player?.name ?? "-"}</div>
          </div>
        ` : null}

        ${!this.#config.hideEmbed ? html`
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
        ` : null}

        ${type === "spot" && Array.isArray(value.thumbnails) ? html`
          <div class="embed">
            <h5>Thumbnails</h5>
            <div class="value">${value.thumbnails.length} available</div>
          </div>
        ` : null}
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

  render() {
    const source = this.#sourceValue();
    const currentValue = this.#currentValue();

    return html`
      <section class="shell">
        <div class="editor">
          <textarea
            class="source"
            .value=${source}
            ?disabled=${this.readonly}
            placeholder="Paste a TwentyThree URL or embed code"
            @input=${this.#onSourceInput}></textarea>

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

            ${this.#config.showUploadLink && currentValue?.credentials?.uploadUrl ? html`
              <uui-button
                color="default"
                look="secondary"
                label="Upload video"
                href=${currentValue.credentials.uploadUrl}
                target="_blank"
                rel="noopener noreferrer"></uui-button>
            ` : null}
          </div>
        </div>

        ${this.#renderStatus()}
        ${this.#renderPreview()}
      </section>
    `;
  }
}

customElements.define("umb-property-editor-ui-twentythree", TwentyThreePropertyEditorUiElement);

export { TwentyThreePropertyEditorUiElement as element };
