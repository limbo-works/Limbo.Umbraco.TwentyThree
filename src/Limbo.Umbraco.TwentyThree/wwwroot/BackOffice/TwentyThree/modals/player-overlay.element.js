import { html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";

import { TwentyThreeService } from "@limbo/twentythree/service";

import { overlayStyles } from "./shared.js";

// Player picker overlay — lists the players of an account and returns the selected one.
// Data in: { credentials, selected }. Value out: the selected player.
// [CHANGE: Umbraco 17 migration — ports PlayerOverlay.js / PlayerOverlay.html] Related: tokens.js

export class TwentyThreePlayerOverlayElement extends UmbModalBaseElement {

    static properties = {
        _players: { state: true },
        _loading: { state: true }
    };

    constructor() {
        super();
        this._players = [];
        this._loading = true;
    }

    connectedCallback() {
        super.connectedCallback();
        this.#loadPlayers();
    }

    async #loadPlayers() {
        const credentials = this.data?.credentials;
        if (!credentials?.id) {
            this._loading = false;
            return;
        }
        try {
            this._players = (await TwentyThreeService.getPlayers(credentials.id)) ?? [];
        } catch (error) {
            console.error("[TwentyThree]", error);
        } finally {
            this._loading = false;
        }
    }

    #select(player) {
        this.value = player;
        this._submitModal();
    }

    render() {
        const selectedId = this.data?.selected?.id;
        return html`
            <umb-body-layout headline="Select a player">
                <div class="content">
                    ${this._loading ? html`<uui-loader></uui-loader>` : nothing}
                    <div class="grid">
                        ${this._players.map((player) => html`
                            <button
                                type="button"
                                class="card account ${player.id === selectedId ? "selected" : ""}"
                                @click=${() => this.#select(player)}>
                                <uui-icon name="icon-application-window-alt"></uui-icon>
                                <div class="details">
                                    <div class="title">${player.name}</div>
                                    ${player.default ? html`<small>Default</small>` : nothing}
                                </div>
                            </button>
                        `)}
                    </div>
                </div>
                <uui-button slot="actions" label="Close" @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
    }

    static styles = [overlayStyles, css`
        .card.selected { border-color: var(--uui-color-selected); }
    `];

}

customElements.define("twentythree-player-overlay", TwentyThreePlayerOverlayElement);

export default TwentyThreePlayerOverlayElement;
