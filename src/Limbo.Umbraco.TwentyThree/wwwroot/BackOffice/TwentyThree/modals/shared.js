import { html, css, nothing } from "@umbraco-cms/backoffice/external/lit";

// Shared rendering helpers and styles for the TwentyThree picker overlays.
// [CHANGE: Umbraco 17 migration] Related: video-overlay.element.js, spot-overlay.element.js, upload-overlay.element.js

/** Renders the account selection grid shown before an account is picked. */
export function renderAccountGrid(accounts, onSelect) {
    if (!accounts || accounts.length === 0) {
        return html`<umb-empty-state position="center">No TwentyThree accounts are configured.</umb-empty-state>`;
    }
    return html`
        <div class="grid account-grid">
            ${accounts.map((account) => html`
                <button type="button" class="card account" @click=${() => onSelect(account)}>
                    <uui-icon name=${account.icon ?? "icon-application-window-alt"}></uui-icon>
                    <div class="details">
                        <div class="title">${account.name}</div>
                        ${account.domains?.[0] ? html`<small>${account.domains[0]}</small>` : nothing}
                    </div>
                </button>
            `)}
        </div>
    `;
}

/** Renders the pagination control for a paginated overlay. */
export function renderPagination(pagination, onPage) {
    if (!pagination || pagination.pages <= 1) return nothing;
    return html`
        <div class="pagination">
            <uui-button
                label="Previous"
                ?disabled=${pagination.page <= 1}
                @click=${() => onPage(pagination.page - 1)}></uui-button>
            ${pagination.items.map((item) => html`
                <uui-button
                    look=${item.active ? "primary" : "default"}
                    label=${String(item.page)}
                    @click=${() => onPage(item.page)}></uui-button>
            `)}
            <uui-button
                label="Next"
                ?disabled=${pagination.page >= pagination.pages}
                @click=${() => onPage(pagination.page + 1)}></uui-button>
        </div>
    `;
}

export const overlayStyles = css`
    .content {
        padding: var(--uui-size-layout-1);
        min-height: 200px;
    }

    .grid {
        display: grid;
        gap: var(--uui-size-space-4);
        grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
    }

    .card {
        display: flex;
        flex-direction: column;
        text-align: left;
        gap: var(--uui-size-space-2);
        padding: var(--uui-size-space-3);
        border: 1px solid var(--uui-color-border);
        border-radius: var(--uui-border-radius);
        background: var(--uui-color-surface);
        color: inherit;
        font: inherit;
        cursor: pointer;
        transition: border-color 120ms ease, box-shadow 120ms ease;
    }

    .card:hover {
        border-color: var(--uui-color-selected);
        box-shadow: var(--uui-shadow-depth-1);
    }

    .account-grid .card.account {
        flex-direction: row;
        align-items: center;
        gap: var(--uui-size-space-4);
    }

    .account uui-icon {
        font-size: 1.6rem;
        flex: 0 0 auto;
    }

    .thumbnail {
        aspect-ratio: 16 / 9;
        overflow: hidden;
        border-radius: var(--uui-border-radius);
        background: var(--uui-color-surface-alt);
    }

    .thumbnail img {
        width: 100%;
        height: 100%;
        object-fit: cover;
        display: block;
    }

    .details .title {
        font-weight: 700;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
    }

    .details .stats {
        color: var(--uui-color-text-alt);
        font-size: var(--uui-font-size-1);
    }

    .details .description {
        margin-top: var(--uui-size-space-1);
        color: var(--uui-color-text-alt);
        font-size: var(--uui-font-size-1);
        display: -webkit-box;
        -webkit-line-clamp: 2;
        -webkit-box-orient: vertical;
        overflow: hidden;
    }

    .pagination {
        display: flex;
        flex-wrap: wrap;
        gap: var(--uui-size-space-1);
        align-items: center;
        padding: 0 var(--uui-size-layout-1);
    }

    .error {
        display: flex;
        gap: var(--uui-size-space-2);
        align-items: center;
        margin-bottom: var(--uui-size-space-4);
        padding: var(--uui-size-space-4);
        border-radius: var(--uui-border-radius);
        background: var(--uui-color-danger-standalone, #fce4e4);
        color: var(--uui-color-danger, #b21d1d);
    }
`;
