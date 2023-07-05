let notificationsHub = new signalR.HubConnectionBuilder().withUrl('/notifications').build();

let navpanMessagesBtn = document.querySelector('.navpanMessagesBtn');

notificationsHub.on('ReceiveNotification', () => {

    if (navpanMessagesBtn.getElementsByTagName('span').length == 0) {
        navpanMessagesBtn.innerHTML += `
		    <span class="badge text-bg-danger">New messages</span>
	    `;
    }
});

notificationsHub
    .start()
    .catch((err) => console.error(err.toString()));