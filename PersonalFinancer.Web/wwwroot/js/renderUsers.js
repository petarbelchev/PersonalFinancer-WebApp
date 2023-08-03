let container = document.querySelector('tbody');

function render(model) {
    let innerHtml = '';

    for (let user of model.users) {
        let tr = `
            <tr role="button" onclick="location.href='${'/Admin/Users/Details/' + user.id}'">
				<th><img src="/icons/icons8-customer-64.png" class="smallIcon" alt="Icon of customer" /></th>
				<td>${user.firstName} ${user.lastName}</td>
				<td>${user.userName}</td>
				<td>${user.email}</td>
			</tr>
        `;

        innerHtml += tr;
    }

    container.innerHTML = innerHtml;
}