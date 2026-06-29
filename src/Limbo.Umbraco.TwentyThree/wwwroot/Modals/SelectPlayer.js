import { html, css, when, repeat } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";

import { TwentyThreeService } from "@limbo/twentythree/service";

export class LimboTwentyThreeSelectPlayerModalElement extends UmbModalBaseElement {

    #loading = false;
    #players;

    get #config() {
        return this.data?.config ?? {};
    }

    constructor() {
        super();
        this.#loading = true;
        this.title = "Select player";
    }

    connectedCallback() {
        super.connectedCallback();
        this.#loadPlayers();
    }

    async #loadPlayers() {

        if (!this.data?.credentials) {
            console.error("No credentials provided to select player modal.");
            return;
        }

        this.#players = await TwentyThreeService.getPlayers(this.data.credentials);
        this.#loading = false;
        this.requestUpdate();

    }

    #handleCancel() {
        this.modalContext?.reject();
    }

    #handleSubmit() {
        if (!this.selectedPlayer) return;
        this.value = this.selectedPlayer;
        this.modalContext?.submit();
    }

    #selectPlayer(player, submit = false) {
        this.selectedPlayer = player;
        this.requestUpdate();
        if (submit) this.#handleSubmit();
    }

    #renderPlayers() {

        if (!Array.isArray(this.#players)) return html``;

        if (this.#players.length === 0) return html`<p>No players found.</p>`;

        return html`
            <div class="players">
                <div class="item-list">
                    ${repeat(this.#players, player => player.key, player => html`
                        <button class="item-card ${player === this.selectedPlayer ? 'selected' : ''}" @click=${() => this.#selectPlayer(player)} @dblclick=${() => this.#selectPlayer(player, true)}>
                            <uui-icon name="icon-embed"></uui-icon>
                            <span class="item-content">
                                <strong>${player.name}</strong>
                            </span>
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
                    ${this.#renderPlayers()}
                </div>
                <div slot="actions">
                    <uui-button id="cancel" label="${this.localize.term("general_close")}" @click="${() => this.#handleCancel()}">
                        ${this.localize.term("general_close")}
                    </uui-button>
                    ${when(this.selectedPlayer, () => html`
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

        .grid {
            height: 100%;
            padding: 5px;
            margin: 0 -5px;
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(230px, 1fr));
            gap: var(--uui-size-space-5);
            align-content: start;
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

        .item-card.selected {
            outline-color: var(--uui-color-focus);
            outline-width: var(--uui-card-border-width);
            outline-style: solid;
            outline-offset: var(--uui-card-border-width);
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

customElements.define("limbo-twentythree-select-player", LimboTwentyThreeSelectPlayerModalElement);

export default LimboTwentyThreeSelectPlayerModalElement;