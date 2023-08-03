let container = document.querySelector('tbody');

function render(model) {
    let innerHtml = '';

    if (model.messages.length == 0) {
        innerHtml = `
            <tr>
                <td colspan="3">
                    <p id="noMessagesNote" class="display-6 fs-4 text-center">You don't have any messages.</p>
                </td>
            </tr>
        `;
    }

    for (let message of model.messages) {
        let tr = `
			<tr role="button" onclick="location.href='/Messages/Details/${message.id}'">
				<td><img src="/icons/icons8-message-64.png" class="smallIcon" alt="Icon of message letter" /></td>
				<td messageId="${message.id}">
					<span>${message.subject}</span>
					${message.isSeen ? '' : '<span class="badge text-bg-danger">New messages</span>' }
				</td>
				<td>
                    ${new Date(message.createdOnUtc).toLocaleString('en-US', {
                        weekday: "long",
                        year: "numeric",
                        month: "long",
                        day: "numeric",
                        hour: "numeric",
                        minute: "numeric"
                    })}
                </td>
			</tr>
        `;

        innerHtml += tr;
    }

    container.innerHTML = innerHtml;
}