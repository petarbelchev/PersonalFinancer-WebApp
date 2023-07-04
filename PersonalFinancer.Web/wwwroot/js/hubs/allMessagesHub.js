let allMessagesHub = new signalR.HubConnectionBuilder().withUrl('/allMessages').build();

const tbody = document.querySelector('tbody');

allMessagesHub.on('ReceiveNotification', (messageId, subject, createdOn) => {
    let messageRow = document.querySelector(`[messageId="${messageId}"]`);

    if (messageRow == null) {
        tbody.innerHTML += `
			<tr role="button" href = '/MessageDetails/Messages/${messageId}'>
				<td messageId="@message.Id">
					<img src="/icons/icons8-message-64.png" style="max-width: 45px;" />
					<span>	${subject}</span>
					<span class="badge text-bg-danger">New messages</span>
				</td>
				<td>
                    <p>
                        ${new Date(createdOn).toLocaleString('en-US', {
                            year: "numeric",
                            month: "long",
                            day: "numeric",
                            hour: "numeric",
                            minute: "numeric",
                            weekday: "long"
                        })}
                    </p>
                </td>
			</tr>
        `;
    }
    else if (messageRow.getElementsByTagName('span').length == 1) {
        messageRow.innerHTML += `
		    <span class="badge text-bg-danger">New messages</span>
	    `;
    }
});

allMessagesHub.on('DeleteMessage', (messageId) => {
    let messageRow = document.querySelector(`[messageId="${messageId}"]`);
    tbody.removeChild(messageRow.parentElement);
});

allMessagesHub
    .start()
    .catch((err) => console.error(err.toString()));