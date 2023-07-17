let container = document.querySelector('tbody');

function render(model) {
    let innerHtml = '';

    for (let transaction of model.transactions) {
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

    container.innerHTML = innerHtml;
}