import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event";

class LimboTwentyThreeButtonListElement extends UmbElementMixin(LitElement) {

    static properties = {
        value: { type: String },
        _items: { state: true }
    };

    #config = { items: [] };

    set config(config) {
        if (!config) return;
        this.#config = {
            items: config.getValueByAlias("items") || []
        };
        this.requestUpdate();
    }

    constructor() {
        super();
        this.value = "";
        this.items = [];
    }

    select(item) {
        this.value = item ? item.alias : null;
        this.dispatchEvent(new UmbChangeEvent());
        this.requestUpdate();
    }

    getClassList(item) {
        const temp = [];
        if (item && item.alias == this.value) temp.push("--active");
        return temp;
    }

    render() {

        return html`
            <div class="items">
                ${repeat(this.#config.items, item => item.alias, item => html`
                    <uui-button class="${this.getClassList(item).join(" ")}" @click=${() => this.select(item)} label=${item.name ?? item.label}>
                        ${item.name ?? item.label}
                    </button>
                `)}
            </div>
        `;

    }

    static styles = css`

        div.items {
            display: flex;
            flex-wrap: wrap;
            gap: 7px;
        }

        uui-button {
            --uui-button-background-color: rgba(216,215,217, .5);
            --uui-button-background-color-hover: rgba(216,215,217, .3);
            --uui-button-font-weight: bold;
            --uui-button-font-size: 13px;
        }

        uui-button.--active {
            --uui-button-background-color: #F5C1BC;
            --uui-button-background-color-hover: #F5C1BC;
        }

        button {
            appearance: none;
            display: inline-block;
            -webkit-appearance: none;
            border: 0;
            font-weight: bold;
            color: #1A2650;
            line-height: 1;
            background-color: rgba(216,215,217, .5);
            font-size: 13px;
            padding: 10px 20px;
            border-radius: 4px;
            transition: all .2s ease;
            position: relative;
            cursor: pointer;
            &:hover {
                background-color: rgba(216,215,217, .3);
                color: #2152a3;
            }
            &.--active {
                background-color: #F5C1BC;
                color: #1A2650;
            }
        }

    `;

}

customElements.define("limbo-twentythree-button-list", LimboTwentyThreeButtonListElement);

export default LimboTwentyThreeButtonListElement;