let pagination = document.getElementById('pagination');

pagination.addEventListener('click', async (e) => {
    if (e.target.tagName == 'A') {
        let page = e.target.getAttribute('page');
        loadPage(page);
    }
});

let currentPage = 1;

async function loadPage(page) {
    let response = await getData(page);

    if (response.status == 200) {
        let model = await response.json();
        render(model);
        setUpPagination(model.pagination);
        currentPage = page;
    } else {
        alert('Oops... Something goes wrong!');
        container.innerHTML = '';
    }
}

async function getData(page) {
    container.innerHTML = `
        <tr>
            <td colspan="5">
	            <div class="d-flex justify-content-center">
		            <div class="spinner-border" role="status"></div>
	            </div>
            </td>
        </tr>
	`;

    let url = params.url + page;
    let options = { method: "GET" };

    if (params.body != undefined) {
        url = params.url;

        options = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify({
                id: params.body.id,
                page: page,
                ownerId: params.body.ownerId,
                fromLocalTime: params.body.fromLocalTime,
                toLocalTime: params.body.toLocalTime,
                accountId: params.body.accountId == '' ? null : params.body.accountId,
                accountTypeId: params.body.accountTypeId == '' ? null : params.body.accountTypeId,
                categoryId: params.body.categoryId == '' ? null : params.body.categoryId,
                currencyId: params.body.currencyId == '' ? null : params.body.currencyId,
                search: params.body.search
            })
        }
    }

    return await fetch(url, options);
}

let elementsStats = pagination.querySelector('#elementsStats');
let paginationPages = pagination.querySelector('#pages');

function setUpPagination(pagination) {
    page = pagination.page;
    elementsStats.textContent = `${pagination.firstElement} to ${pagination.lastElement} from ${pagination.totalElements} ${pagination.elementsName}`;

    let innerHtml = `
        <li class="page-item ${page == 1 ? 'disabled' : ''}">
            <a class="page-link" role="button" style="color: black;" page="${page - 1}">Previous</a>
		</li >
		<li class="page-item ${page == 1 ? 'disabled' : ''}">
			<a class="page-link" role="button" style="color: black; ${page == 1 ? 'background-color: gray; border-color: gray;' : ''}" 
            page="1">1</a>
		</li>
    `;

    if (page - 2 > 1) {
        innerHtml += '<li class="page-item disabled"><span class="page-link" style="color: black">...</span></li>';
    }

    for (let i = page - 1; i <= page + 1; i++) {
        if (i > 1 && i < pagination.pages) {
            innerHtml += `
                <li class="page-item ${page == i ? 'disabled' : ''}">
                    <a class="page-link" role="button" style="color: black; ${page == i ? 'background-color: gray; border-color: gray;' : ''}" 
                    page="${i}">${i}</a>
                </li>
            `;
        }
    }

    if (page + 2 < pagination.pages) {
        innerHtml += '<li class="page-item disabled"><span class="page-link" style="color: black">...</span></li>';
    }

    if (pagination.pages > 1) {
        innerHtml += `
            <li class="page-item ${page == pagination.pages ? 'disabled' : ''}">
            <a class="page-link" role="button" style="color: black; ${page == pagination.pages ? 'background-color: gray; border-color: gray;' : ''}"
                page="${pagination.pages}">${pagination.pages}</a>
        `;
    }

    innerHtml += `
        <li class="page-item ${page == pagination.pages ? 'disabled' : ''}">
			<a class="page-link" role="button" style="color: black;" page="${page + 1}">Next</a>
		</li>
    `;

    paginationPages.innerHTML = innerHtml;
}

window.addEventListener('DOMContentLoaded', async () => await loadPage(1));