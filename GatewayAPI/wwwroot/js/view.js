

// Обработка кнопки добавления модуля
document.getElementById('addModuleBtn').addEventListener('click', function () {
    var modal = new bootstrap.Modal(document.getElementById('moduleModal'));
    modal.show();
});

// Обработка кнопок добавления контента
document.querySelectorAll('.add-content').forEach(btn => {
    btn.addEventListener('click', function () {
        const type = this.dataset.type;
        const modal = new bootstrap.Modal(document.getElementById('contentModal'));
        const label = document.getElementById('contentLabel');
        const dataField = document.getElementById('contentData');

        document.getElementById('contentType').value = type;

        switch (type) {
            case 'Text':
                label.textContent = 'Текст';
                dataField.placeholder = 'Введите текст...';
                break;
            case 'Image':
                label.textContent = 'URL изображения';
                dataField.placeholder = 'https://example.com/image.jpg';
                break;
            case 'Video':
                label.textContent = 'Код видео';
                dataField.placeholder = '<iframe...>';
                break;
            case 'Code':
                label.textContent = '';
                dataField.placeholder = '';
                break;
        }

        modal.show();
    });
});

function formatTextWithParagraphs(text) {
    // Обработка пустых строк между абзацами
    return text
        .split(/\n\s*\n/) // Разделение на абзацы по пустым строкам
        .map(paragraph => {
            if (!paragraph.trim()) return '';
            // Заменяем одиночные переносы внутри абзаца на <br>
            return `<p>${paragraph.replace(/\n/g, '<br>')}</p>`;
        })
        .join('');
}
function escapeHtml(unsafe) {
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

// Получаем данные из C# и применяем форматирование
document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll('.text-content').forEach(content => {
        const contentData = content.innerHTML;
        const formattedText = formatTextWithParagraphs(contentData);
        content.innerHTML = formattedText;
    })
/*    document.querySelectorAll('.language-code').forEach(content => {
        const contentData = content.innerHTML;
        const formattedText = escapeHtml(contentData);
        content.innerHTML = formattedText;
    })*/

});

const newContents = [];
courseItemId = document.querySelector('.content-container').id;

document.addEventListener('DOMContentLoaded', function () {
    // Обработчик клика на кнопку "Добавить"
    document.querySelectorAll('#create-content').forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            // Получаем данные из формы
            const contentType = document.getElementById('contentType').value;
            const contentData = document.getElementById('contentData').value;

            // Создаем новый элемент контента
            const contentItem = document.createElement('div');
            contentItem.className = 'content-item';
            contentItem.id = 'content-' + Date.now();
            

            newContents.push({
                Type: contentType,
                Data: contentData,
                Order: 0,
                CourseItemId: courseItemId
            })
            // В зависимости от типа контента создаем разный HTML
            if (contentType === 'Text') {

                contentItem.innerHTML = `
                <div class="text-content">${formatTextWithParagraphs(contentData)}</div>
            `;
            } else if (contentType === 'Image') {
                contentItem.innerHTML = `
                <img data-src="${contentData}" class="img-fluid">
            `;
            } else if (contentType === 'Video') {
                contentItem.innerHTML = `
                <iframe width="560" height="315" 
                    src="https://www.youtube.com/embed/" 
                    frameborder="0" allowfullscreen></iframe>
            `;
            } else if (contentType === 'Code') {
                const language = "code";
                contentItem.innerHTML = `
                    <div class="code-content">
                        <pre><code class="language-${language}">${escapeHtml(contentData)}</code></pre>
                        <div class="code-language-badge">${language}</div>
                    </div>`;
            }else {
                contentItem.innerHTML = `
                <div class="text-content">${contentData}</div>
            `;
            }

            const deleteBtn = document.createElement('button');
            deleteBtn.className = 'btn btn-sm btn-outline-danger ';
            deleteBtn.textContent = 'Удалить';
            deleteBtn.onclick = function () {
                contentItem.remove();
            };

            contentItem.appendChild(deleteBtn);

            // Добавляем новый элемент в контейнер
            document.querySelector('.content-container').appendChild(contentItem);

            // Очищаем форму
            document.getElementById('contentData').value = '';

            if (contentType === 'Code') {
                hljs.highlightElement(contentItem.querySelector('code'));
            }
        })
    })

});



function deleteItem(id) {
    fetch(`/DeleteItem?id=${id}`, { method: 'POST' })
        .then(response => {
            if (response.ok) {
                document.getElementById(`item-${id}`).remove();
            }
        });
}

/*document.getElementById('old').addEventListener('click', async function () {

    const response = await fetch('?handler=CreateContents', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify(newContents)
    });

    console.log(response.status);

});*/
