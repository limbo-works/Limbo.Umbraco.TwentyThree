import { html, css, when, repeat } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";

import { TwentyThreeService } from "@limbo/twentythree/service";

export class LimboTwentyThreeUploadVideoModalElement extends UmbModalBaseElement {

    #searchTimer;

    get #config() {
        return this.data?.config ?? {};
    }

    constructor() {
        super();
        this._loading = true;
        this.title = "Select account";
    }

    async connectedCallback() {
        super.connectedCallback();
        this.#loadAccounts();
    }

    async #loadAccounts() {

        this.accounts = (await TwentyThreeService.getAccounts()).filter(x => x.uploadUrl);
        this._loading = false;
        this.requestUpdate();

        if (this.accounts.length === 1) {
            this.#selectAccount(this.accounts[0]);
        }

    }

    #selectAccount(account) {
        window.open(account.uploadUrl, "_blank");
        this.modalContext?.submit();
    }

    #handleCancel() {
        this.modalContext?.reject();
    }

    #renderAccounts() {

        if (!Array.isArray(this.accounts)) return html``;

        if (this.accounts.length === 0) return html`<p>No accounts found.</p>`;

        return html`
            <div class="accounts">
                <div class="item-list">
                    ${repeat(this.accounts, account => account.key, account => html`
                        <button class="item-card" @click=${() => this.#selectAccount(account)}>
                            <uui-icon name=${account.icon}></uui-icon>
                            <span class="item-content">
                                <strong>${account.name}</strong>
                                <small>${account.domains.join(", ")}</small>
                            </span>
                            <uui-icon class="chevron" name="icon-navigation-right"></uui-icon>
                        </button>
                    `)}
                </div>
            </div>
        `;

    }

    render() {

        return html`
            <umb-body-layout headline=${this.title}>
                ${when(this._loading, () => html`<uui-loader-bar></uui-loader-bar>`)}
                <div class="content">
                    ${this.#renderAccounts()}
                </div>
                <div slot="actions">
                    <uui-button id="cancel" label="${this.localize.term("general_close")}" @click="${() => this.#handleCancel()}">
                        ${this.localize.term("general_close")}
                    </uui-button>
                </div>
            </umb-body-layout>
        `;

    }

    static styles = css`

        uui-loader-bar {
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            z-index: 9001;
        }

        div[slot='action-menu'] {
            padding-right: 20px;
            display: flex;
            gap: 10px;
        }

        .search {
            flex: 1;
            width: 300px;
        }

        div[slot='footer-info'] {
            width: 50%;
        }

        uui-pagination {
            margin-left: 10px;
        }

        .spot-card img {
            width: 100%;
        }

        .spot-card-text {
            padding: 2px 7px 7px 7px;
        }

        .spot-card-title {
            font-weight: bold;
        }

        .spot-card-details {
            margin-top: 2px;
            font-size: 12px;
        }

        .spot-card-description {
            margin-top: 5px;
            font-size: 12px;
            line-height: 16px;
        }

        .grid {
            height: 100%;
            padding: 5px;
            margin: 0 -5px;
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(230px, 1fr));
            gap: var(--uui-size-space-5);
            align-content: start;
        }

        .spot-card {
          cursor: pointer;
        }

        .item-list {
            display: grid;
            gap: var(--uui-size-space-3);
        }

        .item-card {
            width: 100%;
            display: grid;
            grid-template-columns: auto 1fr auto;
            gap: var(--uui-size-space-4);
            align-items: center;
            text-align: left;
            padding: var(--uui-size-space-4);
            border: 1px solid var(--uui-color-border);
            border-radius: var(--uui-border-radius);
            background: var(--uui-color-surface);
            color: inherit;
            cursor: pointer;
        }

        .item-card:hover,
        .item-card:focus-visible {
            border-color: var(--uui-color-selected);
            background: var(--uui-color-surface-emphasis);
            outline: none;
        }

        .item-card uui-icon:first-child {
            font-size: 24px;
            color: var(--uui-color-interactive);
        }

        .item-content {
            display: grid;
            gap: var(--uui-size-space-1);
        }

        .item-content small {
            color: var(--uui-color-text-alt);
        }

        .chevron {
            color: var(--uui-color-text-alt);
        }

    `;


}

customElements.define("limbo-twentythree-upload-video", LimboTwentyThreeUploadVideoModalElement);

export default LimboTwentyThreeUploadVideoModalElement;