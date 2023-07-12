let tbody = document.querySelector('tbody');
let currentPage = 1;

async function get(page) {
    tbody.innerHTML = `
        <tr>
            <td colspan="5">
	            <div class="d-flex justify-content-center">
		            <div class="spinner-border" role="status">
			            <span class="visually-hidden">Loading...</span>
		            </div>
	            </div>
            </td>
        </tr>
	`;

    let response = await fetch(params.url + page);

    if (response.status == 200) {
        let model = await response.json();
        renderMessages(model.messages);
        setUpPagination(model.pagination);
        currentPage = page;
    } else {
        let error = await response.json();
        alert(`${error.status} ${error.title}`)
    }
}

function renderMessages(messages) {
    let innerHtml = '';

    if (messages.length == 0) {
        innerHtml = `
            <tr>
                <td colspan="2">
                    <p id="noMessagesNote" class="display-6 fs-4 text-center">You don't have any messages.</p>
                </td>
            </tr>
        `;
    }

    for (let message of messages) {
        let tr = `
			<tr role="button" onclick="location.href='/Messages/Details/${message.id}'">
				<td messageId="${message.id}">
					<img src="/icons/icons8-message-64.png" style="max-width: 30px;" />
					<span>	${message.subject}</span>
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

    tbody.innerHTML = innerHtml;
}