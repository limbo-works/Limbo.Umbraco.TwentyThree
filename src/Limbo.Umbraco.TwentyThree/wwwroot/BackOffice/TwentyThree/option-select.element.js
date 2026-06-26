import { html, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event";

// Small button-list property editor UI used for the autoplay/loop/endOn data type configuration fields.
// It stores the selected option alias as a plain string so the value stays byte-compatible with the
// TwentyThreeConfiguration model. The available options are supplied via the "items" config.
// [CHANGE: Umbraco 17 migration — replaces the AngularJS ButtonList.html config view] Related: twentythree.bundle.js

export class TwentyThreeOptionSelectElement extends UmbLitElement {

    static properties = {
        value: { type: String },
        _items: { state: true }
    };

    constructor() {
        super();
        this.value = "";
        this._items = [];
    }

    set config(config) {
        if (!config) return;
        const items = config.getValueByAlias("items") ?? [];
        this._items = items.map((item) =>
            typeof item === "string" ? { alias: item, label: this.#labelFor(item) } : item
        );
        if (!this.value && this._items.length) this.value = this._items[0].alias;
    }

    #labelFor(alias) {
        return alias.charAt(0).toUpperCase() + alias.slice(1);
    }

    #select(alias) {
        if (this.value === alias) return;
        this.value = alias;
        this.dispatchEvent(new UmbChangeEvent());
    }

    render() {
        return html`
            <div class="button-list">
                ${this._items.map((item) => html`
                    <button
                        type="button"
                        class=${item.alias === this.value ? "active" : ""}
                        title=${item.title ?? ""}
                        @click=${() => this.#select(item.alias)}>${item.label}</button>
                `)}
            </div>
        `;
    }

    static styles = css`
        :host { display: block; }
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
    `;

}

customElements.define("twentythree-option-select", TwentyThreeOptionSelectElement);

export default TwentyThreeOptionSelectElement;
