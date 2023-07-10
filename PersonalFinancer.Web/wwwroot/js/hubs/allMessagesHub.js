let allMessagesHub = new signalR.HubConnectionBuilder().withUrl('/allMessages').build();

allMessagesHub.on('ReceiveNewReplyNotification', async (messageId) => {
    let messageWithNewReply = document.querySelector(`[messageId="${messageId}"]`);

    if (messageWithNewReply == null)
        return;

    if (messageWithNewReply.getElementsByTagName('span').length == 1) {
        messageWithNewReply.innerHTML += `<span class="badge text-bg-danger">New messages</span>`;
    }
});

allMessagesHub.on('RefreshMessages', async () => {
    await get(currentPage);
});

allMessagesHub
    .start()
    .catch((err) => console.error(err.toString()));