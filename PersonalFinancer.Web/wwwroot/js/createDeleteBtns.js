function eventSetter(elToListen, newElemDiv, selectField, inputField, url, userId, deleteBtn) {
    if (elToListen.id.includes('create')) {
        elToListen.addEventListener('click', async () => {
            if (newElemDiv.style.display == 'none') {
                newElemDiv.style.display = 'block';
            } else {
                await add(newElemDiv, inputField, url, selectField, deleteBtn);
            }
        });
    } else if (elToListen.id.includes('delete')) {
        elToListen.addEventListener('click', async () => {
            if (confirm('Are you sure you want to delete this?')) {
                await remove(selectField, url, userId, elToListen);
            }
        });
    } else if (elToListen.id.includes('Field')) {
        elToListen.addEventListener('change', () => {
            deleteBtnController(selectField, deleteBtn, userId);
        });
    }
}

async function add(newElemDiv, inputField, url, selectField, deleteBtn) {
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
        newElemDiv.children[2].textContent = error.errors.Name[0];
    }
}

async function remove(selectField, url, userId, deleteBtn) {
    let elemId = selectField.value;
    let ownerId = selectField.selectedOptions[0].attributes['userid'].value;

    if (ownerId != userId) {
        return alert("You can't delete this!")
    }

    let response = await fetch(url + elemId, { method: 'DELETE' });

    if (response.status == 204) {
        selectField.removeChild(document.getElementById(elemId));
        deleteBtnController(selectField, deleteBtn);
    } else {
        alert('Somethink happend!')
    }
}

function deleteBtnController(dropdownField, deleteBtn) {
    if (dropdownField.selectedOptions[0] != undefined) {
        deleteBtn.style.display = 'block';
    } else {
        deleteBtn.style.display = 'none';
    }
}