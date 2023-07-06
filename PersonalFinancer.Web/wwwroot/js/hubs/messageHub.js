let messageHub = new signalR.HubConnectionBuilder().withUrl('/message').build();

let repliesDiv = document.getElementById('replies');

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

    repliesDiv.appendChild(replyDiv);
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

let sendReplyBtn = document.getElementById('sendReply');
sendReplyBtn.disabled = true;

let textArea = sendReplyBtn.parentElement.querySelector('textarea');

sendReplyBtn.addEventListener('click', () => {
    messageHub
        .invoke('SendMessage', params.messageId, textArea.value)
        .then((response) => {
            textArea.value = '';
            console.log(response);

            notificationsHub
                .invoke('SendNotification', params.authorId, params.messageId)
                .then((response) => {
                    console.log(response);
                });
        })
        .catch(() => {
            alert('Oops... Something goes wrong!');
            location.reload();
        });
});

messageHub
    .start()
    .then(() => {
        sendReplyBtn.disabled = false;
        messageHub.invoke('JoinGroup', params.messageId);
    })
    .catch((err) => console.error(err.toString()));