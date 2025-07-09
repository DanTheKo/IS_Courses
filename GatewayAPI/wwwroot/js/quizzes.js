// Текущий редактируемый тест
let currentQuiz = null;
// Список вопросов текущего теста
let questions = [];

// Инициализация модальных окон
const quizModal = new bootstrap.Modal(document.getElementById('quizModal'));
const questionModal = new bootstrap.Modal(document.getElementById('questionModal'));

// Обработчики событий
document.getElementById('create-quiz-btn').addEventListener('click', () => showQuizModal());
document.getElementById('edit-quiz-btn')?.addEventListener('click', () => showQuizModal(true));
document.getElementById('add-question-btn')?.addEventListener('click', () => showQuestionModal());
document.getElementById('save-quiz').addEventListener('click', saveQuiz);
document.getElementById('create-quiz').addEventListener('click', createQuiz);
document.getElementById('save-question').addEventListener('click', saveQuestion);

// Обновление предпросмотра при изменении полей
document.getElementById('question-type').addEventListener('change', updateQuestionForm);
document.getElementById('question-text').addEventListener('input', updateQuestionPreview);
document.getElementById('question-options').addEventListener('input', updateQuestionPreview);
document.getElementById('correct-answer').addEventListener('input', updateQuestionPreview);

// Показать модальное окно теста
function showQuizModal(isEdit = false) {
    const modalTitle = document.getElementById('quizModalLabel');

    if (isEdit && currentQuiz) {
        // Редактирование существующего теста
        modalTitle.textContent = 'Редактировать тест';
        document.getElementById('quiz-id').value = currentQuiz.id;
        document.getElementById('quiz-title').value = currentQuiz.title;
        document.getElementById('quiz-type').value = currentQuiz.type;
        document.getElementById('quiz-description').value = currentQuiz.description;
    } else {
        // Создание нового теста
        modalTitle.textContent = 'Создать новый тест';
        document.getElementById('quiz-id').value = '';
        document.getElementById('quiz-title').value = '';
        document.getElementById('quiz-type').value = '';
        document.getElementById('quiz-description').value = '';
    }

    quizModal.show();
}


// Сохранить тест
function saveQuiz() {
    const quizId = document.getElementById('quiz-id').value;
    const courseItemId = document.getElementById('course-item-id').value;
    const title = document.getElementById('quiz-title').value.trim();
    const type = document.getElementById('quiz-type').value.trim();
    const description = document.getElementById('quiz-description').value.trim();

    if (!title || !type) {
        alert('Заполните обязательные поля (Название и Тип теста)');
        return;
    }

    currentQuiz = {
        id: quizId || "",
        courseItemId: courseItemId,
        title: title,
        type: type,
        description: description,
    };

    // Обновляем отображение информации о тесте
    updateQuizDisplay();

    // Показываем основной интерфейс
    document.getElementById('quiz-container').style.display = 'block';

    quizModal.hide();
}


function setQuizData(quiz, newQuestions) {
    if (quiz) {
/*        console.log(quiz);
        console.log(newQuestions);*/
        currentQuiz = quiz;
/*        currentQuiz.questions = newQuestions;*/
    }
    if (newQuestions) {
        
        questions = newQuestions;
    }

    updateQuizDisplay();
    loadQuestions();
    document.getElementById('quiz-container').style.display = 'block';
}

let data = {
    Quiz: capitalizeObjectKeys(currentQuiz),
    Questions: questions.map(q => capitalizeObjectKeys(q))
};

function capitalizeObjectKeys(obj) {
    if (!obj) return obj;
    return Object.keys(obj).reduce((newObj, key) => {
        newObj[key.charAt(0).toUpperCase() + key.slice(1)] = obj[key];
        return newObj;
    }, {});
}
async function createQuiz() {
    data = {
        Quiz: capitalizeObjectKeys(currentQuiz),
        Questions: questions.map(q => capitalizeObjectKeys(q))
    };
    console.log(JSON.stringify(data));
    const response = await fetch('?handler=CreateQuiz', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify(data)
    });
    if (!response.ok) {
        const errors = await response.json();
        console.error("Ошибка:", errors);
        return;
    }

    /*    const result = await response.json();
        window.location.href = result.redirect;*/
}

async function updateQuiz() {
    let data = {
        Quiz: currentQuiz,
        Questions: questions
    };
    const response = await fetch(`?handler=UpdateQuiz`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify(data)
    });
    if (!response.ok) {
        const errors = await response.json();
        console.error("Ошибка:", errors);
        return;
    }

    /*    const result = await response.json();
        window.location.href = result.redirect;*/
}

// Обновить отображение информации о тесте
function updateQuizDisplay() {
    if (!currentQuiz) return;

    document.getElementById('quiz-title-display').textContent = currentQuiz.title;
    document.getElementById('quiz-type-display').textContent = `Тип: ${getQuizTypeName(currentQuiz.type)}`;
    document.getElementById('quiz-id-display').textContent = `ID: ${currentQuiz.id}`;
    document.getElementById('quiz-description-display').textContent = currentQuiz.description || 'Описание отсутствует';

    loadQuestions();
}

// Получить читаемое название типа теста
function getQuizTypeName(type) {
    const types = {
        'Exam': 'Экзамен',
        'Test': 'Тест',
        'Quiz': 'Викторина',
        'Practice': 'Практика'
    };
    return types[type] || type;
}

// Показать модальное окно вопроса
function showQuestionModal(questionId = null) {
    if (!currentQuiz) {
        alert('Сначала создайте или откройте тест');
        return;
    }

    const modalTitle = document.getElementById('questionModalLabel');

    if (questionId) {
        // Редактирование существующего вопроса
        const question = questions.find(q => q.id === questionId);
        if (!question) return;

        modalTitle.textContent = 'Редактировать вопрос';
        document.getElementById('question-id').value = question.id;
        document.getElementById('question-type').value = question.questionType;
        document.getElementById('question-text').value = question.questionText;
        document.getElementById('question-options').value = question.options;
        document.getElementById('correct-answer').value = question.correctAnswer;
        document.getElementById('max-score').value = question.maxScore;
        document.getElementById('question-order').value = question.order;
    } else {
        // Добавление нового вопроса
        modalTitle.textContent = 'Добавить вопрос';
        document.getElementById('question-id').value = '';
        document.getElementById('question-type').value = '';
        document.getElementById('question-text').value = '';
        document.getElementById('question-options').value = '';
        document.getElementById('correct-answer').value = '';
        document.getElementById('max-score').value = '1';

        // Установить порядковый номер
        const nextOrder = questions.length > 0 ?
            Math.max(...questions.map(q => q.order)) + 1 : 1;
        document.getElementById('question-order').value = nextOrder;
    }

    document.getElementById('question-quiz-id').value = currentQuiz.id;
    updateQuestionForm();
    updateQuestionPreview();
    questionModal.show();
}

// Обновить форму вопроса в зависимости от типа
function updateQuestionForm() {
    const questionType = document.getElementById('question-type').value;
    const optionsContainer = document.getElementById('options-container');

    if (questionType === 'SingleChoice' || questionType === 'MultipleChoice') {
        optionsContainer.style.display = 'block';
    } else {
        optionsContainer.style.display = 'none';
    }
}

// Обновить предпросмотр вопроса
function updateQuestionPreview() {
    const questionText = document.getElementById('question-text').value.trim();
    const questionType = document.getElementById('question-type').value;
    const optionsText = document.getElementById('question-options').value.trim();
    const correctAnswer = document.getElementById('correct-answer').value.trim();

    const questionPreview = document.getElementById('question-preview');
    const answerPreview = document.getElementById('answer-preview');
    const optionsPreview = document.getElementById('options-preview');
    const textAnswerPreview = document.getElementById('text-answer-preview');

    // Обновление текста вопроса
    questionPreview.innerHTML = questionText ? `<p><strong>${questionText}</strong></p>` : '<p>Текст вопроса появится здесь...</p>';

    // Очистка предпросмотра
    optionsPreview.innerHTML = '';
    textAnswerPreview.style.display = 'none';

    // Обновление вариантов ответа в зависимости от типа вопроса
    if (questionType === 'SingleChoice' || questionType === 'MultipleChoice') {
        const options = optionsText.split('\n').filter(opt => opt.trim() !== '');

        if (options.length > 0) {
            options.forEach((option, index) => {
                const inputType = questionType === 'SingleChoice' ? 'radio' : 'checkbox';
                const optionId = `preview-opt-${index}`;

                const optionDiv = document.createElement('div');
                optionDiv.className = 'form-check';
                optionDiv.innerHTML = `
                            <input class="form-check-input" type="${inputType}" name="preview-options" id="${optionId}">
                            <label class="form-check-label" for="${optionId}">${option}</label>
                        `;
                optionsPreview.appendChild(optionDiv);
            });
        } else {
            optionsPreview.innerHTML = '<p class="text-muted">Варианты ответов появятся здесь...</p>';
        }

        // Подсветка правильного ответа
        if (correctAnswer) {
            setTimeout(() => {
                const options = optionsPreview.querySelectorAll('.form-check-label');
                options.forEach(opt => {
                    if (opt.textContent.trim() === correctAnswer.trim()) {
                        opt.classList.add('text-success');
                        opt.innerHTML = `${opt.textContent} <small>(правильный ответ)</small>`;
                    }
                });
            }, 100);
        }
    } else if (questionType === 'TextAnswer') {
        textAnswerPreview.style.display = 'block';
    }
}

// Сохранить вопрос
function saveQuestion() {
    const questionId = document.getElementById('question-id').value;
    const quizId = document.getElementById('question-quiz-id').value;
    const questionType = document.getElementById('question-type').value;
    const questionText = document.getElementById('question-text').value.trim();
    const options = document.getElementById('question-options').value.trim();
    const correctAnswer = document.getElementById('correct-answer').value.trim();
    const maxScore = parseInt(document.getElementById('max-score').value);
    const order = parseInt(document.getElementById('question-order').value);

    if (!questionType || !questionText || !correctAnswer || isNaN(maxScore) || isNaN(order)) {
        alert('Заполните все обязательные поля');
        return;
    }

    const question = {
        id: questionId || "",
        quizId: quizId,
        questionType: questionType,
        questionText: questionText,
        options: options,
        correctAnswer: correctAnswer,
        maxScore: maxScore,
        order: order
    };

    // Если это редактирование, найти и обновить вопрос
    if (questionId) {
        const index = questions.findIndex(q => q.id === questionId);
        if (index !== -1) {
            questions[index] = question;
        }
    } else {
        // Иначе добавить новый вопрос
        questions.push(question);
    }

    // Обновляем текущий тест
/*    if (currentQuiz) {
        currentQuiz.questions = questions;
    }*/

    questionModal.hide();
    loadQuestions();
}

// Загрузить список вопросов
function loadQuestions() {
    const container = document.getElementById('questions-container');
    container.innerHTML = '';

    if (!currentQuiz || !questions || questions.length === 0) {
        container.innerHTML = '<div class="no-questions">Нет добавленных вопросов</div>';
        return;
    }

    // Сортировка по порядковому номеру
    const sortedQuestions = [...questions].sort((a, b) => a.order - b.order);

    sortedQuestions.forEach(question => {
        const questionEl = document.createElement('div');
        questionEl.className = 'card question-card';

        // Создаем элементы для предпросмотра
        const questionPreview = document.createElement('div');
        const answerPreview = document.createElement('div');
        answerPreview.className = 'answer-section mt-3';

        // Заполняем текст вопроса
        questionPreview.innerHTML = question.questionText ?
            `<p class="card-text"><strong>${question.questionText}</strong></p>` :
            '<p class="card-text">Текст вопроса</p>';

        // Обрабатываем варианты ответов в зависимости от типа вопроса
        if (question.questionType === 'SingleChoice' || question.questionType === 'MultipleChoice') {
            const optionsPreview = document.createElement('div');
            optionsPreview.className = 'options-list';

            if (question.options) {
                const options = question.options.split('\n').filter(opt => opt.trim() !== '');

                if (options.length > 0) {
                    options.forEach((option, index) => {
                        const inputType = question.questionType === 'SingleChoice' ? 'radio' : 'checkbox';
                        const optionId = `question-${question.id}-opt-${index}`;

                        const optionDiv = document.createElement('div');
                        optionDiv.className = 'form-check';
                        optionDiv.innerHTML = `
                            <input class="form-check-input" type="${inputType}" name="question-${question.id}-options" id="${optionId}">
                            <label class="form-check-label" for="${optionId}">${option}</label>
                        `;

                        // Подсветка правильного ответа
                        if (option.trim() === question.correctAnswer.trim()) {
                            const label = optionDiv.querySelector('.form-check-label');
                            label.classList.add('text-success');
                            label.innerHTML = `${option} <small>(правильный ответ)</small>`;
                        }

                        optionsPreview.appendChild(optionDiv);
                    });
                } else {
                    optionsPreview.innerHTML = '<p class="text-muted">Нет вариантов ответа</p>';
                }
            } else {
                optionsPreview.innerHTML = '<p class="text-muted">Нет вариантов ответа</p>';
            }

            answerPreview.appendChild(optionsPreview);
        }
        else if (question.questionType === 'TextAnswer') {
            const textAnswerDiv = document.createElement('div');
            textAnswerDiv.innerHTML = `
                <label for="answer-${question.id}" class="form-label">Ваш ответ:</label>
                <textarea id="answer-${question.id}" class="form-control" rows="3"></textarea>
                <div class="mt-2">
                    <strong>Правильный ответ:</strong> ${question.correctAnswer}
                </div>
            `;
            answerPreview.appendChild(textAnswerDiv);
        }

        // Собираем карточку вопроса
        questionEl.innerHTML = `
            <div class="card-body">
                <h5 class="card-title">Вопрос ${question.order}</h5>
                <div class="d-flex gap-3 text-muted mb-2">
                    <span>Тип: ${getQuestionTypeName(question.questionType)}</span>
                    <span>Баллы: ${question.maxScore}</span>
                </div>
            </div>
        `;

        // Добавляем элементы в карточку
        questionEl.querySelector('.card-body').appendChild(questionPreview);
        questionEl.querySelector('.card-body').appendChild(answerPreview);

        // Добавляем кнопки действий
        const actionsDiv = document.createElement('div');
        actionsDiv.className = 'question-actions mt-3';
        actionsDiv.innerHTML = `
            <button onclick="showQuestionModal('${question.id}')" class="btn btn-sm btn-outline-primary me-2">
                Редактировать
            </button>
            <button onclick="deleteQuestion('${question.id}')" class="btn btn-sm btn-outline-danger">
                Удалить
            </button>
        `;

        questionEl.querySelector('.card-body').appendChild(actionsDiv);
        container.appendChild(questionEl);
    });
}

// Получить читаемое название типа вопроса
function getQuestionTypeName(type) {
    const types = {
        'SingleChoice': 'Один вариант',
        'MultipleChoice': 'Несколько вариантов',
        'TextAnswer': 'Текстовый ответ'
    };
    return types[type] || type;
}
function createNewId() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        const r = Math.random() * 16 | 0;
        const v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}
// Удалить вопрос
window.deleteQuestion = function (questionId) {
    if (confirm('Вы уверены, что хотите удалить этот вопрос?')) {
        questions = questions.filter(q => q.id !== questionId);
/*        if (currentQuiz) {
            currentQuiz.questions = questions;
        }*/
        loadQuestions();
    }
};

