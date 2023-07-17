let container = document.querySelector('tbody');

function render(model) {
    let innerHtml = '';
    let row = model.pagination.firstElement;

    for (let user of model.users) {
        let tr = `
            <tr role="button" onclick="location.href='${'/Admin/Users/Details/' + user.id}'">
				<th scope="row">${row++}.</th>
				<td><img src="/icons/icons8-budget-64.png" style="max-width: 30px;">${user.firstName} ${user.lastName}</td>
				<td>${user.userName}</td>
				<td>${user.email}</td>
			</tr>
        `;

        innerHtml += tr;
    }

    container.innerHTML = innerHtml;
}