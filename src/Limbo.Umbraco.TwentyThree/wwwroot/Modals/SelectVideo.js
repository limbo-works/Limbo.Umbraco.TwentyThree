import { html, css, when, repeat } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";

import { TwentyThreeService } from "@limbo/twentythree/service";

const NO_ALBUM = { id: "", title: "Select category" };

import "@limbo/twentythree/elements/pagination";

export class LimboTwentyThreeSelectVideoModalElement extends UmbModalBaseElement {

    #searchTimer = 0;
    #pagination = null;

    get #config() {
        return this.data?.config ?? {};
    }

    constructor() {

        super();

        this._loading = true;
        this._album = NO_ALBUM;

        this.title = "Select account";
        this.search = '';
        this.page = 1;
        this.pageSize = 20;

    }

    async connectedCallback() {
        super.connectedCallback();
        this.#loadAccounts();
    }

    async #loadAccounts() {

        this.accounts = await TwentyThreeService.getAccounts();
        this._loading = false;
        this.requestUpdate();

        if (this.accounts.length === 1) {
            this.#selectAccount(this.accounts[0]);
        }

    }

    async #loadAlbums() {
        const list = await TwentyThreeService.getAlbums(this.account);
        this._albums = [NO_ALBUM, ...list.albums];
        this._loading = true;
        this.requestUpdate();
    }

    #buildPagination(page, pages) {
        const pagination = { page, pages, items: [] };
        const from = Math.max(1, page - 7);
        const to = Math.min(pages, page + 7);
        for (let i = from; i <= to; i++) pagination.items.push({ page: i, active: page === i });
        return pagination;
    }

    async #loadVideos(page) {

        if (!this.account) return;

        const self = this;

        this._loading = true;
        this.title = "Select video";
        this.requestUpdate();

        const query = {
            page: page ?? this.page,
            text: this.search,
            limit: this.pageSize
        };

        if (this._album?.id) query.albumId = this._album.id;

        const response = await TwentyThreeService.getVideos(this.account, query);

        this.pagination = this.#buildPagination(response.page, response.pages);

        this.videoList = response;

        this.videoList.videos = this.videoList.videos.map(function (x) {

            const scheme = x.absolute_url.split(":")[0];
            const domain = x.absolute_url.split("/")[2];

            const item = {
                type: "video",
                credentials: self.account,
                parameters: { videoId: x.photo_id, token: x.token, playerId: null, autoplay: null, endOn: null },
                url: `${scheme}://${domain}/manage/video/${x.photo_id}`,
                site: self.videoList.site,
                video: x,
                videoId: x.photo_id,
                title: x.title,
                description: x.content_text,
                descriptionFull: x.content_text,
                duration: x.video_length,
                thumbnails: TwentyThreeService.getThumbnails(x),
                //player: this._player
            };

            if (item.description === item.title) item.description = null;

            if (self.#config.descriptionMaxLength && item.description?.length > self.#config.descriptionMaxLength) {
                item.description = item.description.substring(0, self.#config.descriptionMaxLength - 3) + "...";
            }

            return item;

        });

        this._loading = false;
        this.requestUpdate();

    }

    #selectAccount(account) {
        this.account = account;
        this.requestUpdate();
        this.#loadAlbums();
        this.#loadVideos();
    }

    #handleCancel() {
        this.modalContext?.reject();
    }

    #handleSubmit() {
        if (!this.selectedVideo) return;
        this.value = this.selectedVideo;
        this.modalContext?.submit();
    }

    #onSearch(event) {
        this.search = event.target.value ?? "";
        window.clearTimeout(this.#searchTimer);
        this.#searchTimer = window.setTimeout(() => this.#loadVideos(1), 300);
    }

    #onAlbumChange(event) {
        const id = event.target.value;
        this._album = this._albums.find((a) => a.id === id) ?? NO_ALBUM;
        this.#loadVideos(1);
    }

    #onPageChange(event) {
        this.page = event.detail.page;
        this.#loadVideos();
    }

    #selectVideo(video, submit = false) {
        this.selectedVideo = video;
        this.requestUpdate();
        if (submit) this.#handleSubmit();
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

    #renderVideos() {

        if (!Array.isArray(this.videoList?.videos)) return html``;

        return html`
            <div class="grid">
                ${repeat(this.videoList.videos, (video) => html`
                    <uui-card class="video-card" selectable .selected=${this.selectedVideo === video} @click=${() => this.#selectVideo(video)} @dblclick=${() => this.#selectVideo(video, true)}>
                        <div>
                            <img src=${video.thumbnails.medium.url} alt=${video.title} />
                            <div class="video-card-text">
                                <div class="video-card-title">${video.title}</div>
                                <div class="video-card-details">
                                    <limbo-video-duration .value=${video.duration}></limbo-video-duration>
                                </div>
                                <div class="video-card-description">
                                    ${video.description}
                                </div>
                            </div>
                        </div>
                    </uui-card>
                `)}
            </div>
        `;

    }

    render() {

        return html`
            <umb-body-layout headline=${this.title}>
                ${when(this._loading, () => html`<uui-loader-bar></uui-loader-bar>`)}
                ${when(this.videoList, () => html`
                    <div slot="action-menu">
                        <uui-input class="search" placeholder="Type to search..." .value=${this.search} @input=${this.#onSearch}></uui-input>
                        ${when(this._albums?.length > 0, () => html`
                            <uui-select
                                .options=${this._albums.map((a) => ({ name: a.title, value: a.id, selected: a.id === this._album.id }))}
                                @change=${this.#onAlbumChange}>
                            </uui-select>
                        `)}
                    </div>
                `)}
                <div class="content">
                    ${when(!this.account, () => this.#renderAccounts())}
                    ${when(this.account, () => this.#renderVideos())}
                </div>
                <div slot="footer-info">
                    ${when(this.pagination?.pages > 1, () => html`
                        <limbo-twentythree-pagination .pagination=${this.pagination} @change=${this.#onPageChange}></limbo-twentythree-pagination>
                    `)}
                </div>
                <div slot="actions">
                    <uui-button id="cancel" label="${this.localize.term("general_close")}" @click="${() => this.#handleCancel()}">
                        ${this.localize.term("general_close")}
                    </uui-button>
                    ${when(this.selectedVideo, () => html`
                        <uui-button id="submit" color="positive" look="primary" label="Select" @click="${() => this.#handleSubmit()}">
                            Select
                        </uui-button>
                    `)}
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

        .video-card img {
            width: 100%;
        }

        .video-card-text {
            padding: 2px 7px 7px 7px;
        }

        .video-card-title {
            font-weight: bold;
        }

        .video-card-details {
            margin-top: 2px;
            font-size: 12px;
        }

        .video-card-description {
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

        .video-card {
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

customElements.define("limbo-twentythree-select-video", LimboTwentyThreeSelectVideoModalElement);

export default LimboTwentyThreeSelectVideoModalElement;