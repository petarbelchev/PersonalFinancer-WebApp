let paginations = document.getElementsByClassName('pagination');

for (let pagination of paginations) {
    pagination.addEventListener('click', (e) => {
        if (e.target.tagName == 'A') {
            getAccounts(e.target.getAttribute('page'));
        }
    })
}

async function getAccounts(page) {
    let response = await fetch(params.url + page);

    if (response.status == 200) {
        let model = await response.json();
        let innerHtml = '';

        for (let account of model.accounts) {
            let div = `
                <div class="col-xl-3 col-lg-4 col-sm-6">
				    <div class="card formField align-items-center">
					    <div class="d-flex flex-row align-items-center mb-2">
						    <div class="p-2">
							    <img src="/icons/icons8-wallet-64.png">
						    </div>
						    <div class="card-body p-2 flex-grow-1">
							    <h5 class="card-title">${account.name}</h5>
							    <p class="card-text">${account.balance} ${account.currencyName}</p>
						    </div>
					    </div>
					    <a href="${'Accounts/AccountDetails/' + account.id}" class="btn btn-secondary col-md-8" style="margin-bottom: 10px;">Details</a>
					    <a href="${'Users/Details/' + account.ownerId}" class="btn btn-secondary col-md-8" style="margin-bottom: 10px;">Owner</a>
				    </div>
			    </div>
            `;

            innerHtml += div;
        }

        document.querySelector('section').innerHTML = innerHtml;

        setUpPagination(page, model.pagination);

    } else {
        let error = await response.json();
        alert(`${error.status} ${error.title}`)
    }
}