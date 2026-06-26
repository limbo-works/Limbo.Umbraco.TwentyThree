import { html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";

import { TwentyThreeService } from "@limbo/twentythree/service";

import { overlayStyles, renderAccountGrid, renderPagination } from "./shared.js";

// Video picker overlay — account grid → searchable, album-filtered, paginated video grid.
// [CHANGE: Umbraco 17 migration — ports VideoOverlay.js / VideoOverlay.html] Related: tokens.js, shared.js

const NO_ALBUM = { id: "", title: "Select category" };

export class TwentyThreeVideoOverlayElement extends UmbModalBaseElement {

    static properties = {
        _accounts: { state: true },
        _account: { state: true },
        _albums: { state: true },
        _album: { state: true },
        _videos: { state: true },
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
        this._albums = [];
        this._album = NO_ALBUM;
        this._videos = [];
        this._pagination = null;
        this._loading = true;
        this._loaded = false;
        this._text = "";
        this._error = "";
        this._player = null;
        this._limit = 0;
        this.#searchTimer = 0;
    }

    #searchTimer;

    get #config() {
        return this.data?.config ?? {};
    }

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
        this._error = "";
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
            this.#error(error);
        }
    }

    async #selectAccount(account) {
        this._account = account;
        this._loading = true;

        try {
            const players = await TwentyThreeService.getPlayers(account.id);
            this._player = (players ?? []).find((x) => x.default) ?? null;
        } catch {
            this._player = null;
        }

        this.#loadAlbums();
        this.#getVideos();
    }

    async #loadAlbums() {
        try {
            const result = await TwentyThreeService.getAlbums(this._account.id);
            this._albums = [NO_ALBUM, ...(result?.albums ?? [])];
        } catch {
            this._albums = [];
        }
    }

    #computeLimit() {
        if (this._limit) return this._limit;
        const container = this.renderRoot.querySelector(".video-list");
        if (!container) {
            this._limit = 20;
        } else {
            const x = Math.max(1, Math.floor(container.clientWidth / 290));
            const y = Math.max(1, Math.floor((container.clientHeight || 600) / 230));
            this._limit = Math.floor(Math.min(x * y, 50) / x) * x;
        }
        return this._limit;
    }

    async #getVideos(page) {
        this._loading = true;

        try {
            const response = await TwentyThreeService.getVideos(this._account.id, {
                text: this._text || undefined,
                limit: Math.max(10, this.#computeLimit()),
                page,
                albumId: this._album?.id || undefined
            });

            this._pagination = this.#buildPagination(response.page, response.pages);

            const maxLength = this.#config.descriptionMaxLength ?? 0;

            this._videos = (response.videos ?? []).map((x) => {
                const scheme = x.absolute_url.split(":")[0];
                const domain = x.absolute_url.split("/")[2];

                const item = {
                    credentials: this._account,
                    parameters: { videoId: x.photo_id, token: x.token, playerId: null, autoplay: null, endOn: null },
                    url: `${scheme}://${domain}/manage/video/${x.photo_id}`,
                    site: response.site,
                    video: x,
                    videoId: x.photo_id,
                    title: x.title,
                    description: x.content_text,
                    descriptionFull: x.content_text,
                    duration: x.video_length,
                    thumbnails: TwentyThreeService.getThumbnails(x),
                    player: this._player
                };

                if (item.description === item.title) item.description = null;
                if (maxLength > 3 && item.description && item.description.length > maxLength) {
                    item.description = `${item.description.substring(0, maxLength - 3)}...`;
                }
                if (maxLength < 0) item.description = null;

                return item;
            });

            this._loaded = true;
        } catch (error) {
            this.#error(error);
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
        this.#searchTimer = window.setTimeout(() => this.#getVideos(1), 300);
    }

    #onAlbumChange(event) {
        const id = event.target.value;
        this._album = this._albums.find((a) => a.id === id) ?? NO_ALBUM;
        this.#getVideos(1);
    }

    #select(item) {
        this.value = item;
        this._submitModal();
    }

    #error(error) {
        const message = error instanceof Error ? error.message : "Failed getting list of videos from the TwentyThree API.";
        this.dispatchEvent(new CustomEvent("error", { detail: message }));
        console.error("[TwentyThree]", message);
    }

    render() {
        return html`
            <umb-body-layout headline=${this._account ? "Select a video" : "Select an account"}>
                ${this._account ? this.#renderHeader() : nothing}
                <div class="content">
                    ${this._loading && !this._loaded ? html`<uui-loader></uui-loader>` : nothing}
                    ${this._error ? html`<div class="error"><uui-icon name="icon-alert"></uui-icon> ${this._error}</div>` : nothing}
                    ${!this._account
                        ? renderAccountGrid(this._accounts, (a) => this.#selectAccount(a))
                        : this.#renderVideos()}
                </div>
                ${this._account && this._pagination
                    ? html`<div slot="footer-info">${renderPagination(this._pagination, (p) => this.#getVideos(p))}</div>`
                    : nothing}
                <uui-button slot="actions" label="Close" @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
    }

    #renderHeader() {
        if (!this._loaded) return nothing;
        return html`
            <div slot="header" class="search">
                <uui-input
                    type="search"
                    placeholder="Type to search..."
                    .value=${this._text}
                    @input=${this.#onSearch}></uui-input>
                ${this._albums.length > 0
                    ? html`<uui-select
                          .options=${this._albums.map((a) => ({ name: a.title, value: a.id, selected: a.id === this._album.id }))}
                          @change=${this.#onAlbumChange}></uui-select>`
                    : nothing}
            </div>
        `;
    }

    #renderVideos() {
        if (this._loaded && this._videos.length === 0) {
            return html`<umb-empty-state position="center">Your search did not match any videos.</umb-empty-state>`;
        }
        return html`
            <div class="video-list grid">
                ${this._videos.map((video) => html`
                    <button type="button" class="card" @click=${() => this.#select(video)}>
                        <div class="thumbnail">
                            <img loading="lazy" src=${video.thumbnails?.medium?.url ?? ""} alt=${video.title ?? ""} />
                        </div>
                        <div class="details">
                            <div class="title">${video.title}</div>
                            <div class="stats">
                                ${this.#formatDuration(video.duration)} &mdash; ${video.video.view_count ?? 0} plays
                            </div>
                            ${video.description
                                ? html`<div class="description" title=${video.descriptionFull ?? ""}>${video.description}</div>`
                                : nothing}
                        </div>
                    </button>
                `)}
            </div>
        `;
    }

    #formatDuration(seconds) {
        const total = Math.max(0, Math.floor(Number(seconds) || 0));
        const h = Math.floor(total / 3600);
        const m = Math.floor((total % 3600) / 60);
        const s = total % 60;
        const pad = (n) => String(n).padStart(2, "0");
        return h > 0 ? `${h}:${pad(m)}:${pad(s)}` : `${m}:${pad(s)}`;
    }

    static styles = [overlayStyles, css`
        .search {
            display: flex;
            gap: var(--uui-size-space-3);
            align-items: center;
            width: 100%;
            padding: var(--uui-size-space-3) var(--uui-size-layout-1);
            box-sizing: border-box;
        }
        .search uui-input { flex: 1; }
    `];

}

customElements.define("twentythree-video-overlay", TwentyThreeVideoOverlayElement);

export default TwentyThreeVideoOverlayElement;
