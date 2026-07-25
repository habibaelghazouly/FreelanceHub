(() => {
    const thread = document.getElementById("chat-thread");
    if (!thread || typeof signalR === "undefined") {
        return;
    }

    const applicationId = Number(thread.dataset.applicationId);
    const currentUserId = Number(thread.dataset.currentUserId);
    const messageList = document.getElementById("chat-message-list");
    const emptyMessage = document.getElementById("chat-empty-message");
    const form = document.getElementById("chat-message-form");
    const input = document.getElementById("chat-message-input");
    const sendButton = document.getElementById("chat-send-button");
    const status = document.getElementById("chat-connection-status");
    const error = document.getElementById("chat-error");
    let isConnected = false;

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/chat")
        .withAutomaticReconnect()
        .build();

    const setConnectionState = (connected, message) => {
        isConnected = connected;
        sendButton.disabled = !connected;
        status.textContent = message;
        status.classList.toggle("is-connected", connected);
    };

    const showError = (message) => {
        error.textContent = message;
        error.hidden = !message;
    };

    const appendMessage = (message) => {
        if (document.querySelector(`[data-message-id="${message.chatMessageId}"]`)) {
            return;
        }

        emptyMessage.hidden = true;

        const article = document.createElement("article");
        article.className = `chat-message ${message.senderUserId === currentUserId ? "is-own" : "is-other"}`;
        article.dataset.messageId = message.chatMessageId;

        const bubble = document.createElement("div");
        bubble.className = "chat-message-bubble";

        const sender = document.createElement("strong");
        sender.textContent = message.senderDisplayName;

        const content = document.createElement("p");
        content.textContent = message.content;

        const sentAt = document.createElement("time");
        sentAt.dateTime = message.sentAt;
        sentAt.textContent = new Date(message.sentAt).toLocaleString();

        bubble.append(sender, content, sentAt);
        article.appendChild(bubble);
        messageList.appendChild(article);
        messageList.scrollTop = messageList.scrollHeight;
    };

    connection.on("ReceiveMessage", appendMessage);

    connection.onreconnecting(() => {
        setConnectionState(false, "Reconnecting...");
    });

    connection.onreconnected(async () => {
        try {
            await connection.invoke("JoinApplication", applicationId);
            setConnectionState(true, "Connected");
            showError("");
        } catch {
            setConnectionState(false, "Unable to rejoin this conversation");
        }
    });

    connection.onclose(() => {
        setConnectionState(false, "Disconnected");
    });

    form.addEventListener("submit", async (event) => {
        event.preventDefault();
        const content = input.value.trim();
        if (!content || !isConnected) {
            return;
        }

        sendButton.disabled = true;
        showError("");

        try {
            await connection.invoke("SendMessage", applicationId, content);
            input.value = "";
            input.focus();
        } catch (sendError) {
            showError(sendError.message || "Unable to send your message.");
        } finally {
            sendButton.disabled = !isConnected;
        }
    });

    const start = async () => {
        messageList.scrollTop = messageList.scrollHeight;

        try {
            await connection.start();
            await connection.invoke("JoinApplication", applicationId);
            setConnectionState(true, "Connected");
            input.focus();
        } catch {
            setConnectionState(false, "Unable to connect");
            showError("Real-time messaging is unavailable. Refresh the page to try again.");
        }
    };

    start();
})();
