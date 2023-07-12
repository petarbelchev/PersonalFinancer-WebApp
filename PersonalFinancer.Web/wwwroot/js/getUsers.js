let tbody = document.querySelector('tbody');

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
        renderUsers(model);
        setUpPagination(model.pagination);
    } else {
        let error = await response.json();
        alert(`${error.status} ${error.title}`)
    }
}

function renderUsers(model) {
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

    tbody.innerHTML = innerHtml;
}