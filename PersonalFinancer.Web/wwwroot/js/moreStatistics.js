async function moreStatistics(url) {

    let response = await fetch(url);

    if (response.status == 200) {

        let data = await response.json();
        let ul = document.createElement('ul');
        ul.className = 'list-group list-group-flush card offset-lg-2 col-lg-8';
        let ulInnerHtml = '';

        for (let i in data) {
            ulInnerHtml += `
                <li class="list-group-item">
					<img src="icons/icons8-banknotes-64.png" style="max-width: fit-content;">
					<span>
						<b>${data[i].name}</b>: <b>${data[i].incomes.toFixed(2)}</b> is a total <b>incomes </b> and 
                        <b>${data[i].expenses.toFixed(2)}</b> is a total <b>expenses</b>  maded by our users!
					</span>
				</li>
            `;
        }

        ul.innerHTML = ulInnerHtml;
        document.getElementById('statistics').replaceChildren(ul);

    } else {

        let error = await response.json();
        alert(`${error.status} ${error.title}`);
    }
}