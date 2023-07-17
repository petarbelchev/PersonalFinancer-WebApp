async function getMoreStatistics(url) {
    let response = await fetch(url);

    if (response.status == 200) {
        let data = await response.json();
        renderStatistics(data);
    } else {
        let error = await response.json();
        alert(`${error.status} ${error.title}`);
    }
}

function renderStatistics(data) {
    let ul = document.createElement('ul');
    ul.className = 'list-group list-group-flush card offset-lg-2 col-lg-8';
    let ulInnerHtml = '';

    for (let currency of data) {
        ulInnerHtml += `
            <li class="list-group-item">
				<img src="icons/icons8-banknotes-64.png" style="max-width: fit-content;">
				<span>
					<b>${currency.name}</b>: The total income made by our users is <b>${currency.incomes.toFixed(2)}</b>, 
                    and the total expense is <b>${currency.expenses.toFixed(2)}</b>!
				</span>
			</li>
        `;
    }

    ul.innerHTML = ulInnerHtml;
    document.getElementById('statistics').replaceChildren(ul);
}