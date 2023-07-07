let allMessagesHub = new signalR.HubConnectionBuilder().withUrl('/allMessages').build();

let tableContainer = document.getElementById('tableContainer');

allMessagesHub.on('ReceiveNotification', (messageId, subject, createdOn) => {
    let tbody = tableContainer.querySelector('tbody');
    let messageRow = tbody.querySelector(`[messageId="${messageId}"]`);

    // TODO: Check is the page already full before add new message.
    if (messageRow == null) {
        tbody.innerHTML += `
			<tr role="button" onclick='location.href="/Messages/MessageDetails/${messageId}"'>
				<td messageId="${messageId}">
					<img src="/icons/icons8-message-64.png" style="max-width: 30px;" />
					<span>	${subject}</span>
					<span class="badge text-bg-danger">New messages</span>
				</td>
				<td>
                    ${new Date(createdOn).toLocaleString('en-US', {
                        year: "numeric",
                        month: "long",
                        day: "numeric",
                        hour: "numeric",
                        minute: "numeric",
                        weekday: "long"
                    })}
                </td>
			</tr>
        `;

        let noMessagesNote = tableContainer.querySelector('#noMessagesNote');
        if (noMessagesNote != null) {
            tableContainer.removeChild(noMessagesNote);
        }
    }
    else if (messageRow.getElementsByTagName('span').length == 1) {
        messageRow.innerHTML += `
		    <span class="badge text-bg-danger">New messages</span>
	    `;
    }
});

allMessagesHub.on('DeleteMessage', (messageId) => {
    let tbody = tableContainer.querySelector('tbody');
    let messageRow = tbody.querySelector(`[messageId="${messageId}"]`);
    tbody.removeChild(messageRow.parentElement);

    // TODO: Reduce the number of messages when delete message.

    if (tbody.children.length == 0) {
        tableContainer.innerHTML += `
			<p id="noMessagesNote" class="display-6 fs-4 text-center">You don't have any messages.</p>
        `;
    }

    if (tbody.querySelector('.badge') == null) {
        navpanMessagesBtn.removeChild(navpanMessagesBtn.querySelector('span'));
    }
});

allMessagesHub
    .start()
    .catch((err) => console.error(err.toString()));