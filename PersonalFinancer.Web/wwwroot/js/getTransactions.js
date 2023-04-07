let paginations = document.getElementsByClassName('pagination');

for (let pagination of paginations) {
    pagination.addEventListener('click', (e) => {
        if (e.target.tagName == 'A') {
            getTransactions(e.target.getAttribute('page'));
        }
    })
}

async function getTransactions(page) {
    let response = await fetch(params.url, {
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
            ownerId: params.ownerId
        })
    });

    if (response.status == 200) {
        let model = await response.json();
        let innerHtml = '';

        for (let transaction of model.transactions) {
            let tr = `
                <tr role="button" onclick=location.href='${model.transactionDetailsUrl + transaction.id}'>
					<td><img src="${transaction.transactionType == 'Income' ? '/icons/greenArrow.png' : '/icons/redArrow.png'}" style="max-width: 38px;"></td>
				    <td>${new Date(transaction.createdOn).toLocaleString()}</td>
				    <td>${transaction.categoryName}</td>
				    <td>${transaction.amount.toFixed(2)} ${transaction.accountCurrencyName}</td>
				    <td>${transaction.refference}</td>
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