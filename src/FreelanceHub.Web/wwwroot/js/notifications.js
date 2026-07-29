(() => {
    const root = document.getElementById("notification-root");
    if (!root) {
        return;
    }

    const badge = document.getElementById("notification-badge");
    const list = document.getElementById("notification-dropdown-list");
    const token = document.querySelector("#notification-antiforgery input[name='__RequestVerificationToken']")?.value;
    const baseUrl = root.dataset.baseUrl.replace(/\/$/, "");
    let refreshPromise = null;
    let refreshQueued = false;

    const post = async (url) => {
        const response = await fetch(url, {
            method: "POST",
            credentials: "same-origin",
            headers: token ? { RequestVerificationToken: token } : {}
        });

        if (!response.ok) {
            throw new Error("Notification request failed.");
        }
    };

    const updateBadge = (unreadCount) => {
        badge.hidden = unreadCount === 0;
        badge.textContent = unreadCount > 99 ? "99+" : String(unreadCount);
    };

    const createNotificationItem = (notification) => {
        const link = document.createElement("a");
        link.href = notification.targetUrl;
        link.className = `notification-dropdown-item${notification.isRead ? "" : " is-unread"}`;
        link.dataset.notificationLink = "";
        link.dataset.notificationId = notification.notificationId;

        const avatar = document.createElement("img");
        avatar.src = notification.actorProfileImageUrl || root.dataset.defaultAvatarUrl;
        avatar.alt = "";

        const content = document.createElement("span");
        content.className = "notification-dropdown-content";

        const title = document.createElement("strong");
        title.textContent = notification.title;

        const message = document.createElement("span");
        message.textContent = notification.message;

        const meta = document.createElement("small");
        const timestamp = new Date(notification.createdAt).toLocaleString();
        meta.textContent = notification.actorDisplayName
            ? `${notification.actorDisplayName} · ${timestamp}`
            : timestamp;

        content.append(title, message, meta);
        link.append(avatar, content);
        return link;
    };

    const loadSummary = async () => {
        try {
            const response = await fetch(root.dataset.summaryUrl, { credentials: "same-origin" });
            if (!response.ok) {
                return;
            }

            const summary = await response.json();
            updateBadge(summary.unreadCount);
            list.replaceChildren();

            if (summary.notifications.length === 0) {
                const empty = document.createElement("span");
                empty.className = "notification-loading";
                empty.textContent = "No notifications yet.";
                list.appendChild(empty);
                return;
            }

            summary.notifications.forEach(notification => {
                list.appendChild(createNotificationItem(notification));
            });
        } catch {
            const error = document.createElement("span");
            error.className = "notification-loading";
            error.textContent = "Unable to load notifications.";
            list.replaceChildren(error);
        }
    };

    const markActiveChatRead = async () => {
        const thread = document.getElementById("chat-thread");
        if (!thread || document.visibilityState !== "visible") {
            return;
        }

        const applicationId = Number(thread.dataset.applicationId);
        if (Number.isInteger(applicationId) && applicationId > 0) {
            await post(`${baseUrl}/chat/${applicationId}/read`);
        }
    };

    const refresh = () => {
        if (refreshPromise) {
            refreshQueued = true;
            return refreshPromise;
        }

        refreshPromise = (async () => {
            try {
                await markActiveChatRead();
            } catch {
                // A failed read update should not prevent the summary from loading.
            }

            await loadSummary();
        })().finally(() => {
            refreshPromise = null;
            if (refreshQueued) {
                refreshQueued = false;
                refresh();
            }
        });

        return refreshPromise;
    };

    document.addEventListener("click", async (event) => {
        const target = event.target instanceof Element ? event.target : null;
        const link = target?.closest("[data-notification-link]");
        if (link && !event.ctrlKey && !event.metaKey && !event.shiftKey && !event.altKey) {
            event.preventDefault();
            try {
                await post(`${baseUrl}/${link.dataset.notificationId}/read`);
            } catch {
                // Navigation should still work if the read update fails.
            } finally {
                window.location.assign(link.href);
            }
            return;
        }

        const markAllButton = target?.closest("[data-mark-all-read]");
        if (!markAllButton) {
            return;
        }

        markAllButton.disabled = true;
        try {
            await post(`${baseUrl}/read-all`);
            if (document.querySelector(".notifications-page")) {
                window.location.reload();
            } else {
                await refresh();
            }
        } catch {
            // Keep the current UI when the update cannot be saved.
        } finally {
            markAllButton.disabled = false;
        }
    });

    root.addEventListener("show.bs.dropdown", refresh);
    document.addEventListener("visibilitychange", () => {
        if (document.visibilityState === "visible") {
            refresh();
        }
    });

    if (typeof signalR !== "undefined") {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(root.dataset.hubUrl)
            .withAutomaticReconnect()
            .build();

        connection.on("NotificationsChanged", refresh);
        connection.onreconnected(refresh);

        const startConnection = async () => {
            if (connection.state !== signalR.HubConnectionState.Disconnected) {
                return;
            }

            try {
                await connection.start();
                await refresh();
            } catch {
                window.setTimeout(startConnection, 5000);
            }
        };

        connection.onclose(() => {
            window.setTimeout(startConnection, 5000);
        });

        startConnection();
    }

    refresh();
})();
