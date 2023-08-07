let container = document.querySelector('tbody');

function render(model) {
    let innerHtml = '';

    if (model.users.length == 0) {
        innerHtml = `
            <tr>
                <td colspan="5">
                    <div class="d-flex justify-content-center">
                        <p>There are no users matching the specified criteria.</p>
                    </div>
                </td>
            </tr>
		`;
    }

    for (let user of model.users) {
        let role = user.isAdmin ? 'Admin' : 'User';

        let tr = `
            <tr role="button" onclick="location.href='${'/Admin/Users/Details/' + user.id}'">
				<th><img src="/icons/icons8-customer-64.png" class="smallIcon" alt="Icon of customer" /></th>
				<td>${user.firstName} ${user.lastName}</td>
				<td>${user.userName}</td>
				<td>${user.email}</td>
				<td>${role}</td>
			</tr>
        `;

        innerHtml += tr;
    }

    container.innerHTML = innerHtml;
}