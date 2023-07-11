let allMessagesHub = new signalR.HubConnectionBuilder().withUrl('/allMessages').build();

allMessagesHub.on('RefreshMessages', async () => {
    await get(currentPage);
});

allMessagesHub
    .start()
    .catch((err) => console.error(err.toString()));