let messageHub = new signalR.HubConnectionBuilder().withUrl('/message').build();

const sendReply = document.getElementById('sendReply');
sendReply.disabled = true;

messageHub.on('ReceiveMessage', (reply) => {
    let replyDiv = document.createElement('div');

    replyDiv.className = 'card formField shadow mb-4';

    replyDiv.innerHTML = `
		<div class="card-body">
			<blockquote class="blockquote mb-0">
				<p>${reply.content}</p>
				<footer class="blockquote-footer" style="margin-top: 10px;">
					Writed from ${reply.authorName} on
					<cite title="Source Title">${new Date(reply.createdOn).toLocaleString('en-US', {
                            year: "numeric",
                            month: "long",
                            day: "numeric",
                            hour: "numeric",
                            minute: "numeric"
                        })}
					</cite>
				</footer>
			</blockquote>
		</div>
    `;

    document.getElementById('replies').appendChild(replyDiv);
});

messageHub.on('MarkAsSeen', async () => {
    let response = await fetch(params.url + params.messageId, {
        method: 'PATCH',
        headers: {
            'RequestVerificationToken': document.querySelector('[name="__RequestVerificationToken"]').value
        }
    });

    if (response.status == 400) {
        return console.error(response.statusText);
    }
});

messageHub
    .start()
    .then(() => {
        sendReply.disabled = false;
        messageHub.invoke('JoinGroup', params.messageId);
    })
    .catch((err) => console.error(err.toString()));

sendReply.addEventListener('click', (e) => {
    let textArea = e.target.parentElement.querySelector('textarea');
    let replyContent = textArea.value;

    messageHub
        .invoke('SendMessage', params.messageId, replyContent)
        .then(() => {
            textArea.value = '';
            notificationsHub.invoke('SendNotification', params.authorId);
        })
        .catch(() => {
            alert("Oops... Something goes wrong!");
            location.reload();
        });
});