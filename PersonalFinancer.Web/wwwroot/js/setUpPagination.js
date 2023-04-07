function setUpPagination(page, pagination) {
    page = Number(page);

    for (let element of document.getElementsByClassName('elementsStats')) {
        element.textContent = `${pagination.firstElement} to ${pagination.lastElement} from ${pagination.totalElements} ${pagination.elementsName}`;
    }

    let innerHtml = `
            <li class="page-item ${page == 1 ? 'disabled' : ''}">
                <a class="page-link" role="button" style="color: black;" page="${page - 1}">Previous</a>
		    </li >
		    <li class="page-item ${page == 1 ? 'active' : ''}">
			    <a class="page-link" role="button" style="color: black; ${page == 1 ? 'background-color: gray; border-color: gray;' : ''}" 
                page="1">1</a>
		    </li>
    `;

    if (page - 3 > 1) {
        innerHtml += '<li class="page-item"><span class="page-link" style="color: black">...</span></li>';
    }

    for (let i = page - 2; i <= page + 2; i++) {
        if (i > 1 && i < pagination.pages) {
            innerHtml += `
                    <li class="page-item ${page == i ? 'active' : ''}">
                        <a class="page-link" role="button" style="color: black; ${page == i ? 'background-color: gray; border-color: gray;' : ''}" 
                        page="${i}">${i}</a>
                    </li>
            `;
        }
    }

    if (page + 3 < pagination.pages) {
        innerHtml += '<li class="page-item"><span class="page-link" style="color: black">...</span></li>';
    }

    if (pagination.pages > 1) {
        innerHtml += `
                <li class="page-item ${page == pagination.pages ? 'active' : ''}">
                <a class="page-link" role="button" style="color: black; ${page == pagination.pages ? 'background-color: gray; border-color: gray;' : ''}"
                 page="${pagination.pages}">${pagination.pages}</a>
        `;
    }

    innerHtml += `
            <li class="page-item ${page == pagination.pages ? 'disabled' : ''}">
			    <a class="page-link" role="button" style="color: black;" page="${page + 1}">Next</a>
		    </li>
    `;

    for (let pagination of paginations) {
        pagination.innerHTML = innerHtml;
    }
}