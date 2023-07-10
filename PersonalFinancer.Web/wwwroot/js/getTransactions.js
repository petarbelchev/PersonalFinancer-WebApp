const tbody = document.querySelector('tbody');
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

    let response = await fetch(params.allTransactionsEndpoint, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify({
            id: params.id,
            page: page,
            fromLocalTime: params.fromLocalTime,
            toLocalTime: params.toLocalTime,
            accountId: params.accountId == '' ? null : params.accountId,
            accountTypeId: params.accountTypeId == '' ? null : params.accountTypeId,
            currencyId: params.currencyId == '' ? null : params.currencyId,
            categoryId: params.categoryId == '' ? null : params.categoryId,
            ownerId: params.ownerId
        })
    });

    if (response.status == 200) {
        let model = await response.json();
        renderTransactions(model.transactions);
        setUpPagination(model.pagination);
        currentPage = model.pagination.page;
    } else {
        alert('Oops! Something was wrong!')
    }
}

function renderTransactions(transactions) {
    let innerHtml = '';

    for (let transaction of transactions) {
        let tr = `
                <tr role="button" data-bs-toggle="modal" data-bs-target="#staticBackdrop" transactionId="${transaction.id}">
					<td><img src="${transaction.transactionType == 'Income' ? '/icons/greenArrow.png' : '/icons/redArrow.png'}" style="max-width: 25px;"></td>
				    <td>${new Date(transaction.createdOnLocalTime).toLocaleString('en-US', {
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