let paginations = document.getElementsByClassName('pagination');

for (let pagination of paginations) {
    pagination.addEventListener('click', async (e) => {
        if (e.target.tagName == 'A') {
            await getMessages(e.target.getAttribute('page'));
        }
    })
}

let tbody = document.querySelector('tbody');

async function getMessages(page) {
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
        renderMessages(model);
        setUpPagination(page, model.pagination);
    } else {
        let error = await response.json();
        alert(`${error.status} ${error.title}`)
    }
}

function renderMessages(model) {
    let innerHtml = '';

    for (let message of model.messages) {
        let tr = `
			<tr role="button" onclick="location.href='/Messages/MessageDetails/${message.id}'">
				<td messageId="${message.id}">
					<img src="/icons/icons8-message-64.png" style="max-width: 30px;" />
					<span>	${message.subject}</span>
					${message.isSeen ? '' : '<span class="badge text-bg-danger">New messages</span>' }
				</td>
				<td>
                    ${new Date(message.createdOnLocalTime).toLocaleString('en-US', {
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