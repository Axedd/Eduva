// wwwroot/js/edit-student.js
document.getElementById('toggleButtonFirstName').addEventListener('click', function () {
    toggleReadonly('firstName', this);
});

document.getElementById('toggleButtonLastName').addEventListener('click', function () {
    toggleReadonly('lastName', this);
});

document.getElementById('toggleButtonClassName').addEventListener('click', function () {
    toggleDisabled('className', this);
});


function toggleDisabled(inputId, button) {
    const input = document.getElementById(inputId);
    input.disabled = !input.disabled;
}

function toggleReadonly(inputId, button) {
    const input = document.getElementById(inputId);
    if (input.readOnly) {
        input.readOnly = false;
        button.textContent = 'Make Read-Only';
    } else {
        input.readOnly = true;
        button.textContent = 'Make Editable';
    }
}