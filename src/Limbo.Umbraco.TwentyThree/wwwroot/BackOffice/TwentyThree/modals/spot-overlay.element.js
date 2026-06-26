import { html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";

import { TwentyThreeService } from "@limbo/twentythree/service";

import { overlayStyles, renderAccountGrid, renderPagination } from "./shared.js";

// Spot picker overlay — account grid → searchable, paginated spot grid.
// [CHANGE: Umbraco 17 migration — ports SpotOverlay.js / SpotOverlay.html] Related: tokens.js, shared.js

export class TwentyThreeSpotOverlayElement extends UmbModalBaseElement {

    static properties = {
        _accounts: { state: true },
        _account: { state: true },
        _spots: { state: true },
        _pagination: { state: true },
        _loading: { state: true },
        _loaded: { state: true },
        _text: { state: true },
        _error: { state: true }
    };

    constructor() {
        super();
        this._accounts = [];
        this._account = null;
        this._spots = [];
        this._pagination = null;
        this._loading = true;
        this._loaded = false;
        this._text = "";
        this._error = "";
        this.#searchTimer = 0;
    }

    #searchTimer;

    connectedCallback() {
        super.connectedCallback();
        this.#loadAccounts();
    }

    disconnectedCallback() {
        super.disconnectedCallback();
        window.clearTimeout(this.#searchTimer);
    }

    async #loadAccounts() {
        this._loading = true;
        try {
            const accounts = await TwentyThreeService.getAccounts();
            this._accounts = accounts ?? [];
            if (this._accounts.length === 1) {
                this.#selectAccount(this._accounts[0]);
            } else {
                this._loading = false;
            }
        } catch (error) {
            this._loading = false;
            this._error = error instanceof Error ? error.message : "Failed loading TwentyThree accounts.";
            console.error("[TwentyThree]", error);
        }
    }

    #selectAccount(account) {
        this._account = account;
        this.#getSpots();
    }

    async #getSpots(page) {
        this._loading = true;
        try {
            const response = await TwentyThreeService.getSpots(this._account.id, {
                text: this._text || undefined,
                limit: 20,
                page
            });

            this._pagination = this.#buildPagination(response.page, response.pages);

            this._spots = (response.spots ?? []).map((x) => {
                const thumbnails = x.__thumbnails ?? [];
                thumbnails.forEach((y) => { thumbnails[y.alias] = y; });
                return {
                    source: x.include_html,
                    credentials: this._account,
                    site: response.site,
                    spot: x,
                    title: x.spot_name,
                    description: `${x.video_count} ${x.video_count === "1" || x.video_count === 1 ? "video" : "videos"}`,
                    thumbnails
                };
            });

            this._loaded = true;
        } catch (error) {
            console.error("[TwentyThree]", error);
        } finally {
            this._loading = false;
        }
    }

    #buildPagination(page, pages) {
        const pagination = { page, pages, items: [] };
        const from = Math.max(1, page - 7);
        const to = Math.min(pages, page + 7);
        for (let i = from; i <= to; i++) pagination.items.push({ page: i, active: page === i });
        return pagination;
    }

    #onSearch(event) {
        this._text = event.target.value ?? "";
        window.clearTimeout(this.#searchTimer);
        this.#searchTimer = window.setTimeout(() => this.#getSpots(1), 300);
    }

    #select(item) {
        this.value = item;
        this._submitModal();
    }

    render() {
        return html`
            <umb-body-layout headline=${this._account ? "Select a spot" : "Select an account"}>
                ${this._account && this._loaded
                    ? html`<div slot="header" class="search">
                          <uui-input type="search" placeholder="Type to search..." .value=${this._text} @input=${this.#onSearch}></uui-input>
                      </div>`
                    : nothing}
                <div class="content">
                    ${this._loading && !this._loaded ? html`<uui-loader></uui-loader>` : nothing}
                    ${this._error ? html`<div class="error"><uui-icon name="icon-alert"></uui-icon> ${this._error}</div>` : nothing}
                    ${!this._account ? renderAccountGrid(this._accounts, (a) => this.#selectAccount(a)) : this.#renderSpots()}
                </div>
                ${this._account && this._pagination
                    ? html`<div slot="footer-info">${renderPagination(this._pagination, (p) => this.#getSpots(p))}</div>`
                    : nothing}
                <uui-button slot="actions" label="Close" @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
    }

    #renderSpots() {
        if (this._loaded && this._spots.length === 0) {
            return html`<umb-empty-state position="center">Your search did not match any spots.</umb-empty-state>`;
        }
        return html`
            <div class="grid">
                ${this._spots.map((spot) => html`
                    <button type="button" class="card" @click=${() => this.#select(spot)}>
                        <div class="thumbnail">
                            <img loading="lazy" src=${spot.thumbnails?.medium?.url ?? ""} alt=${spot.title ?? ""} />
                        </div>
                        <div class="details">
                            <div class="title">${spot.title}</div>
                            <div class="stats">${spot.description}</div>
                        </div>
                    </button>
                `)}
            </div>
        `;
    }

    static styles = [overlayStyles, css`
        .search {
            display: flex;
            width: 100%;
            padding: var(--uui-size-space-3) var(--uui-size-layout-1);
            box-sizing: border-box;
        }
        .search uui-input { flex: 1; }
    `];

}

customElements.define("twentythree-spot-overlay", TwentyThreeSpotOverlayElement);

export default TwentyThreeSpotOverlayElement;
