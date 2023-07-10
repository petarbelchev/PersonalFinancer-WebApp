const myModal = document.getElementById('staticBackdrop');
const modalBody = myModal.querySelector('.modal-body');
const modalFooter = myModal.querySelector('.modal-footer');
const buttons = myModal.getElementsByTagName('a');
const closeBtn = buttons[0];
const editBtn = buttons[1];
const deleteBtn = buttons[2];
const initialBalanceMessage = modalFooter.querySelector('p');

myModal.addEventListener('show.bs.modal', async (e) => {
    let transactionId = e.relatedTarget.attributes.transactionId.textContent;
    await getTransactionData(transactionId);
});

myModal.addEventListener('hidden.bs.modal', () => {
    modalBody.innerHTML = `
	    <div class="d-flex justify-content-center">
		    <div class="spinner-border" role="status">
			    <span class="visually-hidden">Loading...</span>
		    </div>
	    </div>
	`;

    editBtn.style.display = 'none';
    initialBalanceMessage.style.display = 'none';
});

deleteBtn.addEventListener('click', async (e) => await deleteTransaction(e));

async function getTransactionData(transactionId) {
    let response = await fetch(params.transactionEndpoint + transactionId);

    if (response.status == 200) {
        renderTransaction(await response.json());
    } else {
        alert('Oops! Something was wrong!')
        closeBtn.click();
    }
}

function renderTransaction(data) {
    let modalBodyHTML = `
		<ul class="list-group list-group-flush">
			<li class="list-group-item d-flex justify-content-between">
				<span class="p-2">Amount: </span>
				<span class="p-2">${data.amount.toFixed(2)} ${data.accountCurrencyName}</span>
			</li>
			<li class="list-group-item d-flex justify-content-between">
				<div class="p-2"><span>Created On: </span></div>
				<div class="p-2">
                    <span>
                        ${new Date(data.createdOnLocalTime).toLocaleString('en-US', {
        year: "numeric",
        month: "long",
        day: "numeric",
        hour: "numeric",
        minute: "numeric"
    })}
                    </span>
				</div>
			</li>
			<li class="list-group-item d-flex justify-content-between">
				<span class="p-2">Category: </span>
				<span class="p-2">${data.categoryName}</span>
			</li>
			<li class="list-group-item d-flex justify-content-between">
				<span class="p-2">Account: </span>
				<span class="p-2">${data.accountName}</span>
			</li>
			<li class="list-group-item d-flex justify-content-between">
				<span class="p-2">Type: </span>
				<span class="p-2">${data.transactionType}</span>
			</li>
			<li class="list-group-item d-flex justify-content-between">
				<span class="p-2">Reference: </span>
				<span class="p-2">${data.reference}</span>
			</li>
		</ul>
    `;

    modalBody.innerHTML = modalBodyHTML;

    if (data.isInitialBalance) {
        initialBalanceMessage.style.display = 'block';
    }
    else {
        editBtn.href = params.editTransactionPath + data.id;
        editBtn.style.display = 'block';
    }

    deleteBtn.setAttribute('transactionId', data.id);
}

async function deleteTransaction(e) {
    if (confirm('Are you sure you want to delete this transaction?')) {
        let transactionId = e.target.attributes.transactionId.textContent;

        let response = await fetch(params.transactionEndpoint + transactionId, {
            method: 'DELETE',
            headers: {
                'RequestVerificationToken': document.querySelector('[name="__RequestVerificationToken"]').value
            }
        });

        if (response.status == 200) {
            alert(await response.text());
            closeBtn.click();
            getTransactions(currentPage);
        } else {
            alert('Oops! Something was wrong!')
            closeBtn.click();
        }
    }
}