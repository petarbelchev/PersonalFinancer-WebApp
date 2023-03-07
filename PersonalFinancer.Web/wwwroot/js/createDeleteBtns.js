function eventSetter(elToListen, newElemDiv, selectField, inputField, url, userId, deleteBtn) {
    if (elToListen.id.includes('create')) {
        elToListen.addEventListener('click', async () => {
            await create(newElemDiv, inputField, url, selectField, deleteBtn);
        });
    } else if (elToListen.id.includes('delete')) {
        elToListen.addEventListener('click', async () => {
            await del(selectField, url, userId, elToListen);
        });
    } else if (elToListen.id.includes('Field')) {
        elToListen.addEventListener('change', () => {
            deleteBtnController(selectField, deleteBtn, userId);
        });
    }
}

async function create(newElemDiv, inputField, url, selectField, deleteBtn) {

    if (newElemDiv.style.display == 'none') {
        newElemDiv.style.display = 'block';
    } else {
        for (let elem of selectField.children) {
            if (elem.innerText.toLowerCase() == inputField.value.toLowerCase().trim()) {
                newElemDiv.children[2].textContent = 'You already have this. Try another one!';
                return;
            }
        }

        let response = await fetch(url, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ name: inputField.value.trim() })
        });

        if (response.status == 201) {
            let data = await response.json();

            let optionTag = document.createElement('option');
            optionTag.value = data.id;
            optionTag.setAttribute("userId", data.userId);
            optionTag.setAttribute("id", data.id);
            optionTag.textContent = data.name;

            selectField.appendChild(optionTag);
            selectField.value = data.id;

            newElemDiv.style.display = 'none';
            inputField.value = '';
            deleteBtn.style.display = 'block';
        } else if (response.status == 400) {
            let error = await response.json();
            newElemDiv.children[2].textContent = error;
        }
    }
}

async function del(selectField, url, userId, deleteBtn) {
    let id = selectField.value;
    let ownerId = selectField.selectedOptions[0].attributes['userid'].value;

    if (ownerId == 'notOwnedCategory' || ownerId != userId) {
        return alert("You can not delete this Account Type!")
    }

    let response = await fetch(url + id, { method: 'DELETE' });

    if (response.status == 204) {
        selectField.removeChild(document.getElementById(id));
        deleteBtnController(selectField, deleteBtn, userId);
    } else {
        alert('Somethink happend!')
    }
}

function deleteBtnController(dropdownField, deleteBtn, userId) {
    let ownerId = dropdownField.selectedOptions[0].attributes['userid'].value;

    if (ownerId != 'notOwnedCategory' && ownerId == userId) {
        deleteBtn.style.display = 'block';
    } else {
        deleteBtn.style.display = 'none';
    }
}