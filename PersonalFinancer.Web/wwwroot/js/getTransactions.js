const tbody = document.querySelector('tbody');
let paginations = document.getElementsByClassName('pagination');
let currentPage = 1;

for (let pagination of paginations) {
    pagination.addEventListener('click', async (e) => {
        if (e.target.tagName == 'A') {
            currentPage = e.target.getAttribute('page');
            await getTransactions(currentPage);
        }
    })
}

async function getTransactions(page) {
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

    let response = await fetch(params.allTransactionsEndpoint, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify({
            id: params.id,
            page: page,
            startDate: params.startDate,
            endDate: params.endDate,
            accountId: params.accountId == '' ? null : params.accountId,
            accountTypeId: params.accountTypeId == '' ? null : params.accountTypeId,
            currencyId: params.currencyId == '' ? null : params.currencyId,
            categoryId: params.categoryId == '' ? null : params.categoryId,
            ownerId: params.ownerId
        })
    });

    if (response.status == 200) {
        let model = await response.json();
        renderTransactions(model);
        setUpPagination(page, model.pagination);
    } else {
        alert('Oops! Something was wrong!')
    }
}

function renderTransactions(model) {
    let innerHtml = '';

    for (let transaction of model.transactions) {
        let tr = `
                <tr role="button" data-bs-toggle="modal" data-bs-target="#staticBackdrop" transactionId="${transaction.id}">
					<td><img src="${transaction.transactionType == 'Income' ? '/icons/greenArrow.png' : '/icons/redArrow.png'}" style="max-width: 25px;"></td>
				    <td>${new Date(transaction.createdOn).toLocaleString('en-US', {
                        year: "numeric",
                        month: "numeric",
                        day: "numeric",
                        hour: "numeric",
                        minute: "numeric"
                    })}
                    </td>
				    <td>${transaction.categoryName}</td>
				    <td>${transaction.amount.toFixed(2)} ${transaction.accountCurrencyName}</td>
				    <td>${transaction.reference}</td>
			    </tr>
            `;

        innerHtml += tr;
    }

    tbody.innerHTML = innerHtml;
}