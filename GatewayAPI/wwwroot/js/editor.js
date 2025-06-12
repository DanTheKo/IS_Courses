

// Обработка кнопки добавления модуля
document.getElementById('addModuleBtn').addEventListener('click', function () {
    var modal = new bootstrap.Modal(document.getElementById('moduleModal'));
    modal.show();
});

document.getElementById('updateCourseBtn').addEventListener('click', function () {
    var modal = new bootstrap.Modal(document.getElementById('courseModal'));
    modal.show();
});

document.addEventListener('DOMContentLoaded', function () {

    courseItemActionsMenu();
});

function courseItemActionsMenu() {

    const actionsMenu = document.createElement('div');
    actionsMenu.className = 'module-actions-menu';

    const hideButton = document.createElement('button');
    hideButton.textContent = 'Скрыть';
    hideButton.addEventListener('click', function () {
        const id = actionsMenu.currentModule.id;
        console.log('Скрыть модуль с ID:', id);
        hideCourseItem(id);
        hideActionsMenu();
    });

    const deleteButton = document.createElement('button');
    deleteButton.textContent = 'Удалить';
    deleteButton.addEventListener('click', function () {
        const id = actionsMenu.currentModule.id;
        console.log('Удалить модуль с ID:', id);
        if (confirm('Вы уверены, что хотите удалить этот модуль?')) {
            deleteCourseItem(id);
            actionsMenu.currentModule.parentElement.parentElement.remove();
        }
        hideActionsMenu();
    });



    actionsMenu.appendChild(hideButton);
    actionsMenu.appendChild(deleteButton);
    document.body.appendChild(actionsMenu);

    const moduleItems = document.querySelectorAll('.module-menu-btn');
    let currentActiveItem = null;

    moduleItems.forEach(item => {
        item.addEventListener('click', function (e) {
            if (e.button === 0 && !e.ctrlKey && !e.metaKey) {
                e.preventDefault();
                showActionsMenu(item);
            }
        });
    });
    function showActionsMenu(item) {
        if (currentActiveItem) {
            currentActiveItem.classList.remove('active');
        }

        const rect = item.getBoundingClientRect();
        actionsMenu.style.display = 'flex';
        actionsMenu.style.top = `${rect.top}px`;
        actionsMenu.style.left = `${rect.right + 5}px`;

        item.classList.add('active');
        currentActiveItem = item;

        actionsMenu.currentModule = item;
    }
    function hideActionsMenu() {
        actionsMenu.style.display = 'none';
        if (currentActiveItem) {
            currentActiveItem.classList.remove('active');
            currentActiveItem = null;
        }
    }
    document.addEventListener('click', function (e) {
        if (!actionsMenu.contains(e.target) && !e.target.classList.contains('module-menu-btn')) {
            hideActionsMenu();
        }
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            hideActionsMenu();
        }
    });
}

function hideCourseItem(id) {
    console.log('Скрываем модуль ID:', id);
}

function deleteCourseItem(id) {
    console.log('Удаляем модуль ID:', id);

    const data = {
        Id: id,
    };
    fetch(`?handler=DeleteCourseItem`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify(data)
    });
}

