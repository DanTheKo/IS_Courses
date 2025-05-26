
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
                label.textContent = 'Код видео (YouTube/Vimeo)';
                dataField.placeholder = '<iframe...>';
                break;
        }

        modal.show();
    });
});