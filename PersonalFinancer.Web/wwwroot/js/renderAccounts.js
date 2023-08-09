let container = document.getElementById('accounts');

function render(model) {
	let innerHtml = model.accountsCards.length > 0
		? ''
		: `
			<div class="d-flex justify-content-center">
				<p>The users doesn't have any created accounts.</p>
			</div>
		`;

    for (let account of model.accountsCards) {
        let div = `
			<div class="col-sm-6 col-lg-4 col-xl-3">
				<div class="card align-items-center shadow mb-4">
					<div class="d-flex align-items-center">
						<div class="p-2">
							<img src="/icons/icons8-wallet-64.png" class="largeIcon" alt="Wallet" />
						</div>
						<div class="card-body p-2 flex-grow-1">
							<h5 class="card-title display-6 fs-5"><b>${account.name}</b></h5>
							<p class="card-text display-6 fs-6">${account.balance.toFixed(2)} ${account.currencyName}</p>
						</div>
					</div>
					<div class="d-flex">
						<a href = '/Accounts/Details?id=${account.id}&returnUrl=/Admin/Accounts' class="btn btn-secondary m-2">Details</a>
						<a href = '/Admin/Users/Details/${account.ownerId}' class="btn btn-secondary m-2">Owner</a>
					</div>
				</div>
			</div>
        `;

        innerHtml += div;
    }

	container.innerHTML = innerHtml;
}