import { html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";

import { TwentyThreeService } from "@limbo/twentythree/service";

import { overlayStyles } from "./shared.js";

// External upload overlay — lists accounts that allow upload and opens the upload URL in a new tab.
// [CHANGE: Umbraco 17 migration — ports UploadExternal.js / UploadExternal.html] Related: tokens.js

export class TwentyThreeUploadOverlayElement extends UmbModalBaseElement {

    static properties = {
        _accounts: { state: true },
        _loading: { state: true },
        _error: { state: true }
    };

    constructor() {
        super();
        this._accounts = [];
        this._loading = true;
        this._error = "";
    }

    connectedCallback() {
        super.connectedCallback();
        this.#loadAccounts();
    }

    async #loadAccounts() {
        try {
            const accounts = await TwentyThreeService.getAccounts();
            this._accounts = (accounts ?? []).filter((x) => !!x.uploadUrl);
        } catch (error) {
            this._error = error instanceof Error ? error.message : "Failed loading TwentyThree accounts.";
            console.error("[TwentyThree]", error);
        } finally {
            this._loading = false;
        }
    }

    #select(account) {
        window.open(account.uploadUrl, "_blank", "noopener,noreferrer");
        this._rejectModal();
    }

    render() {
        return html`
            <umb-body-layout headline="Upload video">
                <div class="content">
                    ${this._loading ? html`<uui-loader></uui-loader>` : nothing}
                    ${this._error ? html`<div class="error"><uui-icon name="icon-alert"></uui-icon> ${this._error}</div>` : nothing}
                    ${!this._loading && !this._error && this._accounts.length === 0
                        ? html`<umb-empty-state position="center">None of the configured accounts allows video upload.</umb-empty-state>`
                        : nothing}
                    <div class="grid">
                        ${this._accounts.map((account) => html`
                            <button type="button" class="card account" @click=${() => this.#select(account)}>
                                <uui-icon name=${account.icon ?? "icon-application-window-alt"}></uui-icon>
                                <div class="details">
                                    <div class="title">${account.name}</div>
                                    ${account.domains?.[0] ? html`<small>${account.domains[0]}</small>` : nothing}
                                </div>
                            </button>
                        `)}
                    </div>
                </div>
                <uui-button slot="actions" label="Close" @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
    }

    static styles = [overlayStyles, css``];

}

customElements.define("twentythree-upload-overlay", TwentyThreeUploadOverlayElement);

export default TwentyThreeUploadOverlayElement;
