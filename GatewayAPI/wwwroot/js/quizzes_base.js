// Текущий редактируемый тест
let currentQuiz = null;
// Список вопросов текущего теста
let questions = [];

// Инициализация модальных окон
const quizModal = new bootstrap.Modal(document.getElementById('quizModal'));
const questionModal = new bootstrap.Modal(document.getElementById('questionModal'));

// Общие константы и функции
const QUIZ_MODE = {
    VIEW: 'view',
    EDIT: 'edit'
};

let currentMode = QUIZ_MODE.EDIT; // По умолчанию режим редактирования

// Общие функции для обоих режимов
function initQuiz(initialMode = QUIZ_MODE.EDIT) {
    currentMode = initialMode;
    setupEventHandlers();
}

function updateQuestionForm() {
    const questionType = document.getElementById('question-type').value;
    const optionsContainer = document.getElementById('options-container');

    if (questionType === 'SingleChoice' || questionType === 'MultipleChoice') {
        optionsContainer.style.display = 'block';
    } else {
        optionsContainer.style.display = 'none';
    }
}

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
        currentQuiz = quiz;
    }
    else {
        return;
    }
    if (newQuestions) {

        questions = newQuestions;
    }

    updateQuizDisplay();
    document.getElementById('quiz-container').style.display = 'block';
}

/*let data = {
    Quiz: capitalizeObjectKeys(currentQuiz),
    Questions: questions.map(q => capitalizeObjectKeys(q))
};
*/
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

}


async function submitQuiz() {
    if (!currentQuiz) {
        alert('Тест не загружен');
        return;
    }

    let data = {
        QuizResponse: { QuizId: currentQuiz.id, IdentityId: "" },
        QuestionAnswers: collectAnswers(),
        Questions: questions.map(q => capitalizeObjectKeys(q))
    };
    console.log(JSON.stringify(data));
    const response = await fetch('?handler=SubmitQuizResponse', {
        method: 'POST',
        headers: {
            'questions': questions,
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
    else {
        const result = await response.json();
        showResults(result);

    }
}




// Обновить отображение информации о тесте
function updateQuizDisplay() {
    if (!currentQuiz) {
        document.getElementById('quiz-container').style.display = 'none';

        return;
    }


    document.getElementById('quiz-title-display').textContent = currentQuiz.title;
    document.getElementById('quiz-type-display').textContent = `Вид тестирования: ${getQuizTypeName(currentQuiz.type)}`;
    /*    document.getElementById('quiz-id-display').textContent = `ID: ${currentQuiz.id}`;*/
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


function setupEventHandlers() {
    if (currentMode === QUIZ_MODE.EDIT) {
        // Только для редактора
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

    }
    else {
        document.getElementById('submit-quiz').addEventListener('click', () => submitQuiz());
    }

    // Общие обработчики
}

// Общая функция загрузки вопросов
function loadQuestions() {
    const container = document.getElementById('questions-container');
    container.innerHTML = '';

    if (!currentQuiz || !questions || questions.length === 0) {
        container.innerHTML = '<div class="no-questions">Нет вопросов</div>';
        return;
    }

    const sortedQuestions = [...questions].sort((a, b) => a.order - b.order);

    sortedQuestions.forEach(question => {
        const questionEl = createQuestionElement(question);
        container.appendChild(questionEl);
    });
}

// Создание элемента вопроса (разные для режимов)
function createQuestionElement(question) {

    if (currentMode === QUIZ_MODE.EDIT) {
        return createEditorQuestionElement(question);
    } else {
        return createViewerQuestionElement(question);
    }
}

// Функции только для редактора
function createEditorQuestionElement(question) {
       
    return renderAnswerSection(question);
}

// Функции только для просмотра
function createViewerQuestionElement(question) {
    const questionEl = document.createElement('div');
    questionEl.className = 'card question-card';
    questionEl.id = question.id;

    questionEl.innerHTML = `
    <div class="card-body">
      <h5 class="card-title">Вопрос ${question.order}</h5>
      <div class="d-flex gap-3 text-muted mb-2">
        <span>Баллы: ${question.maxScore}</span>
      </div>
      <p class="card-text">${question.questionText}</p>
      ${renderAnswerSection(question, true)}
    </div>
  `;

    return questionEl;
}

function renderAnswerSection(question, isViewMode = false) {
    if (isViewMode) {
        // Версия для просмотра с интерактивными элементами
        if (question.questionType === 'SingleChoice' || question.questionType === 'MultipleChoice') {
            return `
        <div class="options-list">
          ${question.options.split('\n').map((opt, i) => `
            <div class="form-check">
              <input class="form-check-input" type="${question.questionType === 'SingleChoice' ? 'radio' : 'checkbox'}" 
                     name="question-${question.id}" id="option-${question.id}-${i}">
              <label class="form-check-label" for="option-${question.id}-${i}">${opt}</label>
            </div>
          `).join('')}
        </div>
      `;
        } else {
            return `
        <div class="form-group">
          <textarea class="form-control" rows="3" placeholder="Введите ваш ответ..."></textarea>
        </div>
      `;
        }
    } else {
        // Версия для редактора
        // ... (как в предыдущей реализации)
        const questionEl = document.createElement('div');
        questionEl.className = 'card question-card';

        // Создаем элементы для предпросмотра
        const questionPreview = document.createElement('div');
        const answerPreview = document.createElement('div');
        answerPreview.className = 'answer-section mt-3';

        // Заполняем текст вопроса
        questionPreview.innerHTML = question.questionText ?
            `<p class="card-text">${question.questionText}</p>` :
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
                <div class="mt-2 text-muted">
                    Правильный ответ: ${question.correctAnswer}
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
            <button onclick="showQuestionModal('${question.id}')" class="btn btn-sm btn-outline-secondary">
            <i class="fas fa-edit"></i>
            </button>
            <button onclick="deleteQuestion('${question.id}')" class="btn btn-sm btn-outline-secondary">
                <i class="fas fa-trash"></i>
            </button>
        `;

        questionEl.querySelector('.card-body').appendChild(actionsDiv);
        return questionEl;
    }
}



// Функции для работы с модальными окнами
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
    questionPreview.innerHTML = questionText ? `<p>${questionText}</p>` : '<p>Текст вопроса появится здесь...</p>';

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
        id: questionId || createNewId(),
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


function getQuestionTypeName(type) {
    const types = {
        'SingleChoice': 'Один вариант',
        'MultipleChoice': 'Несколько вариантов',
        'TextAnswer': 'Текстовый ответ'
    };
    return types[type] || type;
}

let numId = 0
function createNewId() {
    numId = numId + 1;
    return numId.toString();
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        const r = Math.random() * 16 | 0;
        const v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}
// Удалить вопрос
window.deleteQuestion = function (questionId) {
    if (confirm('Вы уверены, что хотите удалить этот вопрос?')) {

        deleteQuestionFetch(questionId);
        questions = questions.filter(q => q.id !== questionId);
        loadQuestions();
    }
};

window.deleteQuiz = function () {
    if (confirm('Вы уверены, что хотите удалить тестирование?')) {

        deleteQuizFetch(currentQuiz.id);
        currentQuiz = null;
        questions = [];
        updateQuizDisplay();
    }
};



async function deleteQuestionFetch(id) {
    const response = await fetch(`?handler=DeleteQuestion`, {
        method: 'POST',
        headers: {
            'questionId': id,
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
    });
    if (!response.ok) {
        const errors = await response.json();
        console.error("Ошибка:", errors);
        return;
    }
}

async function deleteQuizFetch(id) {
    const response = await fetch(`?handler=DeleteQuiz`, {
        method: 'POST',
        headers: {
            'quizId': id,
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
    });
    if (!response.ok) {
        const errors = await response.json();
        console.error("Ошибка:", errors);
        return;
    }
}




function collectAnswers() {
    const answers = [];

    // Перебираем все вопросы
    document.querySelectorAll('.question-card').forEach(questionEl => {
        const questionId = questionEl.id;
        const question = questions.find(q => q.id === questionId);

        if (!question) return;

        let answerValue;

        if (question.questionType === 'SingleChoice' || question.questionType === 'MultipleChoice') {
            const selectedOptions = Array.from(
                questionEl.querySelectorAll('input[type="radio"]:checked, input[type="checkbox"]:checked')
            ).map(input => {
                // Находим соответствующий label для input
                const label = questionEl.querySelector(`label[for="${input.id}"]`);
                return label ? label.textContent.trim() : '';
            }).filter(text => text !== '');

            answerValue = question.questionType === 'SingleChoice'
                ? selectedOptions[0] || 'NoAnswer'
                : selectedOptions.join(' ');
        } else {
            // Для текстовых ответов
            answerValue = questionEl.querySelector('textarea').value.trim();
            if (!answerValue) {
                answerValue = 'NoAnswer'
            }
        }

        answers.push({
            QuestionId: questionId,
            QuizResponseId: "",
            AnswerText: answerValue,
            SelectedOptions: answerValue
        });
    });

    return answers;
}

function showResults(result) {
    const resultsContainer = document.createElement('div');
    resultsContainer.className = 'quiz-results';

    console.log(result);

    // Рассчитываем процент правильных ответов
    const percentage = result.maxPossibleScore > 0
        ? Math.round((result.totalScore / result.maxPossibleScore) * 100)
        : 0;

    // Создаем HTML для отображения результатов
    resultsContainer.innerHTML = `
        <div class="quiz-results-header">
            <h3>Результаты теста</h3>
            <div class="progress mb-4">
                <div class="progress-bar ${percentage >= 80 ? 'bg-success' : percentage >= 50 ? 'bg-warning' : 'bg-danger'}" 
                     role="progressbar" 
                     style="width: ${percentage}%" 
                     aria-valuenow="${percentage}" 
                     aria-valuemin="0" 
                     aria-valuemax="100">
                    ${percentage}%
                </div>
            </div>
            <p class="score-summary">
                Вы набрали <strong>${result.totalScore}</strong> из 
                <strong>${result.maxPossibleScore}</strong> возможных баллов
            </p>
        </div>
        
        <div class="results-details">
            ${result.results.map((item, index) => `
                <div class="question-result ${item.isCorrect ? 'correct' : 'incorrect'}">
                    <div class="question-header">
                        <span class="question-number">Вопрос ${index + 1}:</span>
                        <span class="question-score">${item.score}/${item.maxScore} баллов</span>
                    </div>
                    <p class="question-text">${item.questionText}</p>
                    <div class="answer-comparison">
                        <div class="user-answer">
                            <span class="answer-label">Ваш ответ:</span>
                            <span>${item.userAnswer || 'Нет ответа'}</span>
                        </div>
                        ${!item.isCorrect ? `
                            <div class="correct-answer">
                                <span class="answer-label">Правильный ответ:</span>
                                <span>${item.correctAnswer}</span>
                            </div>
                        ` : ''}
                    </div>
                </div>
            `).join('')}
        </div>
        
        <div class="quiz-actions mt-4">
            <button onclick="location.reload()" class="btn btn-secondary">
                <i class="fas fa-redo"></i> Пройти еще раз
            </button>
    `;

    // Очищаем контейнер и добавляем результаты
    const quizContainer = document.getElementById('quiz-container');
    quizContainer.innerHTML = '';
    quizContainer.appendChild(resultsContainer);

    // Добавляем плавное появление
    resultsContainer.style.opacity = '0';
    setTimeout(() => {
        resultsContainer.style.transition = 'opacity 0.5s ease';
        resultsContainer.style.opacity = '1';
    }, 100);
}