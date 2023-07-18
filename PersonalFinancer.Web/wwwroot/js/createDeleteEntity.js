const divs = document.getElementsByClassName('newElemDiv')
let mainDiv;
let selectTag;
let ownerId;
let url;
let newElemInputDiv;
let spinner;

for (var div of divs) {
    div.addEventListener('click', async (e) => await eventHandler(e));
}

async function eventHandler(e) {
    if (e.target.tagName != 'A') {
        return;
    }

    mainDiv = e.target.parentElement;
    selectTag = mainDiv.querySelector('select');
    ownerId = mainDiv.attributes[1].value;
    url = mainDiv.attributes[2].value;
    newElemInputDiv = mainDiv.querySelector('div');
    spinner = mainDiv.querySelector('.spinner-border');

    if (e.target.classList.contains('createBtn')) {
        if (newElemInputDiv.style.display == 'none') {
            newElemInputDiv.style.display = 'block';
        } else {
            await createEntity();
        }
    } else if (e.target.classList.contains('deleteBtn')) {
        if (confirm('Are you sure you want to delete this?')) {
            await deleteEntity();
        }
    }
}

async function createEntity() {
    let inputField = newElemInputDiv.querySelector('input');
    let errorMsgField = newElemInputDiv.querySelector('p');

    for (let elem of selectTag.children) {
        if (elem.innerText.toLowerCase() == inputField.value.toLowerCase().trim()) {
            errorMsgField.textContent = 'You already have this. Try another one!';
            return;
        }
    }

    newElemInputDiv.style.display = 'none';
    spinner.style.display = 'block';

    let response = await fetch(url, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify({ name: inputField.value.trim(), ownerId })
    });

    if (response.status == 201) {
        let data = await response.json();
        renderNewEntity(data, inputField, errorMsgField);
        deleteBtnController();
    } else if (response.status == 400) {
        let error = await response.json();
        newElemInputDiv.querySelector('p').textContent = error;
    }
}

function renderNewEntity(data, inputField, errorMsgField) {
    let optionTag = document.createElement('option');
    optionTag.value = data.id;
    optionTag.setAttribute("id", data.id);
    optionTag.textContent = data.name;

    selectTag.appendChild(optionTag);
    selectTag.value = data.id;

    spinner.style.display = 'none';
    errorMsgField.textContent = '';
    inputField.value = '';
}

async function deleteEntity() {
    spinner.style.display = 'block';
    let elemId = selectTag.value;

    let response = await fetch(url + elemId, {
        method: 'DELETE',
        headers: {
            'RequestVerificationToken': document.querySelector('[name="__RequestVerificationToken"]').value
        }
    });

    if (response.status == 204) {
        selectTag.removeChild(document.getElementById(elemId));
        spinner.style.display = 'none';
        deleteBtnController();
    } else {
        let error = await response.json();
        alert(`${error.status} ${error.title}`);
    }
}

function deleteBtnController() {
    deleteBtn = mainDiv.querySelector('.deleteBtn');

    if (selectTag.selectedOptions[0] != undefined) {
        deleteBtn.style.display = 'block';
    } else {
        deleteBtn.style.display = 'none';
    }
}
