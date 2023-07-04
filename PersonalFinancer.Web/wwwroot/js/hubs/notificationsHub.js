let notificationsHub = new signalR.HubConnectionBuilder().withUrl('/notifications').build();

notificationsHub.on('ReceiveNotification', () => {
    let navbarDropdown = document.querySelector('.notification');

    if (navbarDropdown.getElementsByTagName('span').length == 0) {
        navbarDropdown.innerHTML += `
		    <span class="badge text-bg-danger">New messages</span>
	    `;
    }
});

notificationsHub
    .start()
    .catch((err) => console.error(err.toString()));