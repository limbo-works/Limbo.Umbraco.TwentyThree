import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

class LimboTwentyThreePaginationElement extends UmbElementMixin(LitElement) {

    static properties = {
        pagination: { type: String },
        change: { type: Function }
    };

    constructor() {
        super();
    }

    #onPageChange(page, event) {

        this.pagination.page = page;
        this.requestUpdate();

        this.dispatchEvent(new CustomEvent("change", {
            detail: {
                page: this.pagination.page,
                pagination: this.pagination
            },
            bubbles: true,
            composed: true,
        }));

    }

    render() {
        if (!this.pagination) return html``;
        return html`
            <div class="pagination">
                <uui-button label="${this.localize.term("general_previous")}" ?disabled=${this.pagination.page <= 1} @click=${(e) => this.#onPageChange(this.pagination.page - 1, e)}></uui-button>
                ${this.pagination.items.map((item) => html`
                    <uui-button look=${item.active ? "primary" : "default"} label=${String(item.page)} @click=${(e) => this.#onPageChange(item.page)}></uui-button>
                `)}
                <uui-button label="${this.localize.term("general_next")}" ?disabled=${this.pagination.page >= this.pagination.pages} @click=${(e) => this.#onPageChange(this.pagination.page + 1, e)}></uui-button>
            </div>
        `;
    }

    static styles = css`

        .pagination {
            display: flex;
            flex-wrap: wrap;
            gap: var(--uui-size-space-1);
            align-items: center;
            padding: 0 var(--uui-size-layout-1);
        }

    `;

}

customElements.define("limbo-twentythree-pagination", LimboTwentyThreePaginationElement);

export default LimboTwentyThreePaginationElement;