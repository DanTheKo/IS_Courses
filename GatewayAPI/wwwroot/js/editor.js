

// Обработка кнопки добавления модуля
document.getElementById('addModuleBtn').addEventListener('click', function () {
    var modal = new bootstrap.Modal(document.getElementById('moduleModal'));
    modal.show();
});


document.addEventListener('DOMContentLoaded', function () {

    courseItemActionsMenu();
});

function courseItemActionsMenu() {
    const style = document.createElement('style');
    style.textContent = `
        .module-actions-menu {
            position: fixed;
            background-color: white;
            border: 1px solid #ddd;
            border-radius: 4px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.2);
            z-index: 1000;
            display: none;
            flex-direction: column;
            min-width: 120px;
        }
        .module-actions-menu button {
            padding: 8px 12px;
            text-align: left;
            background: none;
            border: none;
            cursor: pointer;
            font-size: small;
        }
        .module-actions-menu button:hover {
            background-color: #f5f5f5;
        }
        .module-menu-btn {
            position: relative;
            cursor: pointer;
        }
        .module-menu-btn.active {
            background-color: #f0f0f0;
        }
    `;
    document.head.appendChild(style);

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

