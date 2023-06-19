const divs = document.getElementsByClassName('newElemDiv')

for (var div of divs) {
    div.addEventListener('click', async (e) => {
        if (e.target.tagName != 'A') {
            return;
        }

        const mainDiv = e.target.parentElement;
        const selectField = mainDiv.querySelector('select');
        const ownerId = mainDiv.attributes[1].value
        const url = mainDiv.attributes[2].value

        if (e.target.classList.contains('createBtn')) {
            let newElemDiv = mainDiv.querySelector('div');

            if (newElemDiv.style.display == 'none') {
                newElemDiv.style.display = 'block';
            } else {
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

                    let optionTag = document.createElement('option');
                    optionTag.value = data.id;
                    optionTag.setAttribute("id", data.id);
                    optionTag.textContent = data.name;

                    selectField.appendChild(optionTag);
                    selectField.value = data.id;

                    newElemDiv.style.display = 'none';
                    errorMsgField.textContent = '';
                    inputField.value = '';
                    deleteBtnController(selectField, mainDiv.querySelector('.deleteBtn'));
                } else if (response.status == 400) {
                    let error = await response.json();
                    newElemDiv.querySelector('p').textContent = error;
                }
            }
        } else if (e.target.classList.contains('deleteBtn')) {
            if (confirm('Are you sure you want to delete this?')) {
                let elemId = selectField.value;

                let response = await fetch(url + elemId, {
                    method: 'DELETE',
                    headers: {
                        'RequestVerificationToken': document.querySelector('[name="__RequestVerificationToken"]').value
                    }
                });

                if (response.status == 204) {
                    selectField.removeChild(document.getElementById(elemId));
                    deleteBtnController(selectField, e.target);
                } else {
                    let error = await response.json();
                    alert(`${error.status} ${error.title}`);
                }
            }
        }
    })
}

function deleteBtnController(dropdownField, deleteBtn) {
    if (dropdownField.selectedOptions[0] != undefined) {
        deleteBtn.style.display = 'block';
    } else {
        deleteBtn.style.display = 'none';
    }
}