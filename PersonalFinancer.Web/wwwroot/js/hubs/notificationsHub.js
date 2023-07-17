let notificationsHub = new signalR.HubConnectionBuilder().withUrl('/notifications').build();
let navpanMessagesBtn = document.getElementById('navpanMessagesBtn');

notificationsHub.on('ReceiveNotification', () => {
    if (navpanMessagesBtn.querySelector('.badge') == null) {
        navpanMessagesBtn.innerHTML += `
			<span class="position-absolute top-40 start-80 translate-middle p-2 bg-danger rounded-circle"></span>
	    `;
    }
});

notificationsHub.on('RemoveNotification', () => {
    let notificationBadge = navpanMessagesBtn.querySelector('span');

    if (notificationBadge != null) {
        navpanMessagesBtn.removeChild(notificationBadge);
    }
});

notificationsHub
    .start()
    .catch((err) => console.error(err.toString()));