let messageHub = new signalR.HubConnectionBuilder().withUrl('/message').build();
let repliesDiv = document.getElementById('replies');

messageHub.on('ReceiveReply', (reply) => {
    let replyDiv = document.createElement('div');
    replyDiv.className = 'card shadow mb-4';

    replyDiv.innerHTML = `
		<div class="card-body">
			<blockquote class="blockquote mb-0">
				<p>${reply.content}</p>
				<footer class="blockquote-footer mt-1">
					Writed from ${reply.authorName} on
					<cite title="Source Title">
                        ${new Date(reply.createdOnUtc).toLocaleString('en-US', {
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

let requestVerificationToken = document.querySelector('[name="__RequestVerificationToken"]').value;

messageHub.on('MarkAsSeen', async () => {
    let response = await fetch(params.url + params.messageId, {
        method: 'PATCH',
        headers: {
            'RequestVerificationToken': requestVerificationToken
        }
    });

    if (response.status == 400) {
        return console.error(response.statusText);
    }
});

let sendReplyBtn = document.getElementById('sendReply');
sendReplyBtn.disabled = true;
let textArea = sendReplyBtn.parentElement.querySelector('textarea');

document.getElementById('replyForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    let replyContent = textArea.value;

    if (replyContent.length < params.replyMinLenght || replyContent.length > params.replyMaxLenght) {
        return;
    }

    await sendReply(replyContent);
});

async function sendReply(replyContent) {
    let response = await fetch(params.url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': requestVerificationToken
        },
        body: JSON.stringify({
            messageId: params.messageId,
            replyContent: replyContent
        })
    });

    if (response.status == 200) {
        let reply = await response.json();

        messageHub
            .invoke('SendReply', params.messageId, reply)
            .then((response) => {
                textArea.value = '';
                console.log(response);

                notificationsHub
                    .invoke('SendNotification', params.authorId)
                    .then((response) => {
                        console.log(response);
                    })
                    .catch(() => {
                        console.error('Sending the notification failed!');
                    });
            })
            .catch(() => {
                alert('Oops... Something goes wrong!');
                location.reload();
            });
    } else {
        let error = await response.json();
        alert(`${error.status} ${error.title}`);
    }
};

messageHub
    .start()
    .then(() => {
        sendReplyBtn.disabled = false;
        messageHub.invoke('JoinGroup', params.messageId);
    })
    .catch((err) => console.error(err.toString()));