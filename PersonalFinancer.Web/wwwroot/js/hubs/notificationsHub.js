let notificationsHub = new signalR.HubConnectionBuilder().withUrl('/notifications').build();
let navpanMessagesBtn = document.querySelector('.navpanMessagesBtn');

notificationsHub.on('ReceiveNotification', () => {
    if (navpanMessagesBtn.querySelector('.badge') == null) {
        navpanMessagesBtn.innerHTML += `
		    <span class="badge text-bg-danger">New messages</span>
	    `;
    }
});

notificationsHub.on('RemoveNotification', () => {
    let notificationBadge = navpanMessagesBtn.querySelector('.badge');

    if (notificationBadge != null) {
        navpanMessagesBtn.removeChild(notificationBadge);
    }
});

notificationsHub
    .start()
    .catch((err) => console.error(err.toString()));