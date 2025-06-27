
const editor = new EasyMDE({
    // Оставляем стандартный тулбар и добавляем свою кнопку
    toolbar: [
        "bold", "italic", "heading", "|",
        "quote", "unordered-list", "ordered-list", "|",
        "link", "image", "|",
        "preview", "side-by-side", "fullscreen", "|",
        {
            name: "interactive-quiz",
            action: function (editor) {
                // Создаем модальное окно
                const modal = document.createElement('div');
                modal.style.cssText = `
          position: fixed;
          top: 50%;
          left: 50%;
          transform: translate(-50%, -50%);
          background: white;
          padding: 20px;
          border: 1px solid #ddd;
          z-index: 1000;
          width: 500px;
          max-width: 90%;
          box-shadow: 0 0 20px rgba(0,0,0,0.2);
        `;

                modal.innerHTML = `
          <h3>Создать интерактивный тест</h3>
          <div style="margin-bottom: 15px;">
            <label>Вопрос:</label>
            <input type="text" id="quiz-question" style="width: 100%; padding: 8px;">
          </div>
          <div id="quiz-options" style="margin-bottom: 15px;">
            <div class="option" style="display: flex; margin-bottom: 5px;">
              <input type="radio" name="correct-option" value="0" checked>
              <input type="text" class="option-text" style="flex-grow: 1; margin-left: 5px;" placeholder="Правильный ответ">
            </div>
            <div class="option" style="display: flex; margin-bottom: 5px;">
              <input type="radio" name="correct-option" value="1">
              <input type="text" class="option-text" style="flex-grow: 1; margin-left: 5px;" placeholder="Вариант ответа">
            </div>
          </div>
          <button id="add-option-btn" style="margin-right: 10px;">+ Добавить вариант</button>
          <button id="insert-quiz-btn" style="background: #4CAF50; color: white;">Вставить тест</button>
          <button id="close-modal-btn" style="float: right;">Отмена</button>
        `;

                document.body.appendChild(modal);

                // Обработчики событий
                document.getElementById('add-option-btn').addEventListener('click', function () {
                    const optionsContainer = document.getElementById('quiz-options');
                    const optionCount = optionsContainer.children.length;
                    const newOption = document.createElement('div');
                    newOption.className = 'option';
                    newOption.style.cssText = 'display: flex; margin-bottom: 5px;';
                    newOption.innerHTML = `
            <input type="radio" name="correct-option" value="${optionCount}">
            <input type="text" class="option-text" style="flex-grow: 1; margin-left: 5px;" placeholder="Вариант ответа">
          `;
                    optionsContainer.appendChild(newOption);
                });

                document.getElementById('insert-quiz-btn').addEventListener('click', function () {
                    const question = document.getElementById('quiz-question').value;
                    const options = Array.from(document.querySelectorAll('.option-text')).map(el => el.value);
                    const correctIndex = document.querySelector('input[name="correct-option"]:checked').value;

                    // Генерируем уникальное имя для группы radio-кнопок
                    const quizName = 'quiz-' + Math.random().toString(36).substr(2, 9);

                    let quizHTML = `<div class="quiz">\n<h3>${question}</h3>\n<ul>\n`;

                    options.forEach((option, index) => {
                        if (option.trim()) {
                            const correctAttr = index == correctIndex ? ' data-correct' : '';
                            quizHTML += `<li><input type="radio" name="${quizName}"${correctAttr}> ${option}</li>\n`;
                        }
                    });

                    quizHTML += `</ul>\n<button onclick="checkQuiz(this)">Проверить</button>\n</div>\n`;
                    quizHTML += `<script>\nfunction checkQuiz(btn) {\n`;
                    quizHTML += `  const selected = btn.parentNode.querySelector('input:checked');\n`;
                    quizHTML += `  if (selected?.hasAttribute('data-correct')) {\n`;
                    quizHTML += `    alert("Правильно!");\n  } else {\n`;
                    quizHTML += `    alert("Неправильно!");\n  }\n}\n</script>`;

                    editor.codemirror.replaceSelection(quizHTML);
                    modal.remove();
                });

                document.getElementById('close-modal-btn').addEventListener('click', function () {
                    modal.remove();
                });
            },
            className: "fa fa-question-circle",
            title: "Добавить интерактивный тест",
        }
    ],
    sanitizer: false, // Разрешаем HTML для интерактивных элементов
    spellChecker: false, // Отключаем проверку орфографии (по желанию)
});


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

    const element = document.getElementById("easymde");
    editor.element = element;
    editor.sanitizer = false;
    configureToolbar();
    configureSaveAllButton();
});

function showMessage(message) {
            const msgElement = document.createElement('div');
            msgElement.textContent = message;
            msgElement.style.position = 'fixed';
            msgElement.style.bottom = '20px';
            msgElement.style.right = '20px';
            msgElement.style.padding = '15px 25px';
            msgElement.style.backgroundColor = '#fff';
            msgElement.style.color = 'black';
            msgElement.style.borderRadius = '5px';
            msgElement.style.boxShadow = '0 2px 10px rgba(0,0,0,0.2)';
            msgElement.style.zIndex = '1000';
            msgElement.style.animation = 'fadein 0.5s, fadeout 0.5s 2.5s forwards';

            document.body.appendChild(msgElement);

            setTimeout(() => {
                msgElement.remove();
            }, 3000);
}


function configureSaveAllButton(){
    document.getElementById('saveAllBtn').addEventListener('click', async function () {
        const courseItemId = document.getElementById('courseItemId').value;
        const courseItemTitle = document.getElementById('courseItemTitle').value;
        const courseItemType = document.getElementById('courseItemType').value;

        const contentId = document.getElementById('mainContentId').value;
        const contentType = "Base";
        const contentData = editor.value();



        const content = {
            Id: contentId,
            Type: contentType,
            Data: contentData,
            Order: 0,
            CourseItemId: courseItemId
        };
        const courseItem = {
            Id: courseItemId,
            Title: courseItemTitle,
            Type: courseItemType
        }

        const data = {
            Content: content,
            CourseItem: courseItem
        };

        const response = await fetch('?handler=SaveData', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify(data)
        });
        console.log('Raw response:', response);
        if (response.ok) {
            showMessage('Данные успешно сохранены!');
        } else {
            showMessage('Ошибка! Данные не сохранены!');
        }


    });
}


function setContent(content) {
    if (content) {
        editor.value(content);
    }
}
function configureToolbar() {
    const toolbar = document.querySelector('.editor-toolbar');
    const newParent = document.querySelector('.sidebar.right-sidebar');
    const referenceElement = document.getElementById('saveAllBtn');
    newParent.insertBefore(toolbar, referenceElement);
}

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

async function deleteCourseItem(id) {
    console.log('Удаляем модуль ID:', id);
    const response = await fetch(`?handler=DeleteCourseItem`, {
        method: 'POST',
        headers: {
            'itemId': id,
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
    });
    if (!response.ok) {
        const errors = await response.json();
        console.error("Ошибка:", errors);
        return;
    }

    const result = await response.json();
    window.location.href = result.redirect;
}

