let paginations = document.getElementsByClassName('pagination');

for (let pagination of paginations) {
    pagination.addEventListener('click', async (e) => {
        if (e.target.tagName == 'A') {
            await getAccounts(e.target.getAttribute('page'));
        }
    })
}

let accountsDiv = document.getElementById('accounts');

async function getAccounts(page) {
    accountsDiv.innerHTML = `
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
        renderAccounts(model);
        setUpPagination(page, model.pagination);
    } else {
        let error = await response.json();
        alert(`${error.status} ${error.title}`)
    }
}

function renderAccounts(model) {
    let innerHtml = '';

    for (let account of model.accountsCards) {
        let div = `
			<div class="col-sm-6 col-lg-4 col-xl-3">
				<div class="card align-items-center shadow mb-4">
					<div class="d-flex align-items-center">
						<div class="p-2">
							<img src="/icons/icons8-wallet-64.png" style="width: 50px;">
						</div>
						<div class="card-body p-2 flex-grow-1">
							<h5 class="card-title display-6 fs-5"><b>${account.name}</b></h5>
							<p class="card-text display-6 fs-6">${account.balance.toFixed(2)} ${account.currencyName}</p>
						</div>
					</div>
					<div class="d-flex">
						<a href = '/Admin/Accounts/AccountDetails/${account.id}' class="btn btn-secondary m-2">Details</a>
						<a href = '/Admin/Users/Details/${account.ownerId}' class="btn btn-secondary m-2">Owner</a>
					</div>
				</div>
			</div>
        `;

        innerHtml += div;
    }

    accountsDiv.innerHTML = innerHtml;
}