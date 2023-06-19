const divs = document.getElementsByClassName('newElemDiv')
let mainDiv;
let selectField;
let ownerId;
let url;

for (var div of divs) {
    div.addEventListener('click', await eventHandler(e));
}

async function eventHandler(e) {
    if (e.target.tagName != 'A') {
        return;
    }

    mainDiv = e.target.parentElement;
    selectField = mainDiv.querySelector('select');
    ownerId = mainDiv.attributes[1].value;
    url = mainDiv.attributes[2].value;

    if (e.target.classList.contains('createBtn')) {
        let newElemDiv = mainDiv.querySelector('div');

        if (newElemDiv.style.display == 'none') {
            newElemDiv.style.display = 'block';
        } else {
            await createEntity(newElemDiv);
        }

    } else if (e.target.classList.contains('deleteBtn')) {
        if (confirm('Are you sure you want to delete this?')) {
            await deleteEntity();
        }
    }
}

async function createEntity(newElemDiv) {
    let inputField = newElemDiv.querySelector('input');
    let errorMsgField = newElemDiv.querySelector('p');

    for (let elem of selectField.children) {
        if (elem.innerText.toLowerCase() == inputField.value.toLowerCase().trim()) {
            errorMsgField.textContent = 'You already have this. Try another one!';
            return;
        }
    }

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
        newElemDiv.querySelector('p').textContent = error;
    }
}

async function deleteEntity() {
    let elemId = selectField.value;

    let response = await fetch(url + elemId, {
        method: 'DELETE',
        headers: {
            'RequestVerificationToken': document.querySelector('[name="__RequestVerificationToken"]').value
        }
    });

    if (response.status == 204) {
        selectField.removeChild(document.getElementById(elemId));
        deleteBtnController();
    } else {
        let error = await response.json();
        alert(`${error.status} ${error.title}`);
    }
}

function renderNewEntity(data, inputField, errorMsgField) {
    let optionTag = document.createElement('option');
    optionTag.value = data.id;
    optionTag.setAttribute("id", data.id);
    optionTag.textContent = data.name;

    selectField.appendChild(optionTag);
    selectField.value = data.id;

    newElemDiv.style.display = 'none';
    errorMsgField.textContent = '';
    inputField.value = '';
}

function deleteBtnController() {
    deleteBtn = mainDiv.querySelector('.deleteBtn');

    if (selectField.selectedOptions[0] != undefined) {
        deleteBtn.style.display = 'block';
    } else {
        deleteBtn.style.display = 'none';
    }
}