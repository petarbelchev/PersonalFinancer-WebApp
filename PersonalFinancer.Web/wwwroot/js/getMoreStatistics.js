let statisticsUl = document.getElementById('statistics');

async function getMoreStatistics(url) {
    statisticsUl.innerHTML = `
	    <div class="d-flex justify-content-center m-2">
		    <div class="spinner-border" role="status"></div>
	    </div>
    `;

    let response = await fetch(url);

    if (response.status == 200) {
        let data = await response.json();
        renderStatistics(data);
    } else {
        statisticsUl.innerHTML = '';
    }
}

function renderStatistics(data) {
    let innerHtml = '';

    for (let currency of data) {
        innerHtml += `
            <li class="list-group-item">
				<img src="icons/icons8-banknotes-64.png" style="max-width: fit-content;" alt="Icon of banknotes" />
				<span>
					<b>${currency.name}</b>: The total income made by our users is <b>${currency.incomes.toFixed(2)}</b>, 
                    and the total expense is <b>${currency.expenses.toFixed(2)}</b>!
				</span>
			</li>
        `;
    }

    statisticsUl.innerHTML = innerHtml;
}