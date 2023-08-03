let container = document.querySelector('tbody');

function render(model) {
    let innerHtml = '';

    if (model.transactions.length == 0) {
        innerHtml = `
            <tr>
                <td colspan="5">
                    <div class="d-flex justify-content-center">
                        <p>You have no transactions matching the specified criteria.</p>
                    </div>
                </td>
            </tr>
		`;
    }

    for (let transaction of model.transactions) {
        let tr = `
                <tr role="button" data-bs-toggle="modal" data-bs-target="#staticBackdrop" transactionId="${transaction.id}">
					<td><img src="${transaction.transactionType == 'Income' ? '/icons/greenArrow.png' : '/icons/redArrow.png'}" class="smallIcon" alt="Arrow" /></td>
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

    container.innerHTML = innerHtml;
}