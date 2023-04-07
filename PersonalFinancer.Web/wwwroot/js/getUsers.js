let paginations = document.getElementsByClassName('pagination');

for (let pagination of paginations) {
    pagination.addEventListener('click', (e) => {
        if (e.target.tagName == 'A') {
            getUsers(e.target.getAttribute('page'));
        }
    })
}

async function getUsers(page) {
    let response = await fetch(params.url + page);

    if (response.status == 200) {
        let model = await response.json();
        let innerHtml = '';
        let row = model.pagination.firstElement;

        for (let user of model.users) {
            let tr = `
                <tr role="button" onclick="location.href='${'/Admin/Users/Details/' + user.id}'">
					<th scope="row">${row++}</th>
					<td><img src="/icons/icons8-budget-64.png" style="max-width: 50px;"></td>
					<td>${user.firstName} ${user.lastName}</td>
					<td>${user.email}</td>
					<td>${user.phoneNumber}</td>
				</tr>
            `;

            innerHtml += tr;
        }

        document.querySelector('tbody').innerHTML = innerHtml;

        setUpPagination(page, model.pagination);

    } else {
        let error = await response.json();
        alert(`${error.status} ${error.title}`)
    }
}