let canvas = document.querySelector('#doughnutChart');
let chart;
let isChartDrawed = false;

function loadChart(currencyIndex) {
	let categories = [];
	let amounts = [];

	if (currencies.length == 0) {
		return;
    }

	for (let category of currencies[currencyIndex].ExpensesByCategories) {
		categories.push(category.CategoryName);
		amounts.push(category.ExpensesAmount.toFixed(2));
	}

	if (isChartDrawed) {
		chart.destroy();
	}

	chart = new Chart(canvas, {
		type: 'doughnut',
		data: {
			labels: categories,
			datasets: [{
				label: 'Amount',
				data: amounts,
				hoverOffset: 15
			}]
		}
	});

	isChartDrawed = true;
};

loadChart(0);