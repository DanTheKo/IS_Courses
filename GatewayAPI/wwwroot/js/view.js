let editor = new EasyMDE({
    toolbar: false,
    status: false,
    spellChecker: false,
    shortcuts: {}
});

document.addEventListener("DOMContentLoaded", function () {

    const element = document.getElementById("easymde");
    editor.element = element;

    editor.codemirror.setOption("readOnly", true);
    editor.togglePreview();
    checkAndSync();

});

function setContent(content) {
    if (content) {
        editor.value(content);
    }
}

function syncViewHeights() {
    const codeMirrorSizer = document.querySelector('.CodeMirror');
    const editorPreview = document.querySelector('.editor-preview-full');
    if (editorPreview && editorPreview.offsetHeight > 0 && codeMirrorSizer) {
        const previewHeight = editorPreview.offsetHeight-5 + 'px';
        codeMirrorSizer.style.height = previewHeight;
        codeMirrorSizer.style.minHeight = previewHeight;
        return true;
    }
    return false;
}

function checkAndSync(attempts = 50) {
    if (attempts <= 0) {
        console.warn('Элемент .editor-preview-full не найден или высота = 0');
        return;
    }
    if (!syncViewHeights()) {
        setTimeout(() => checkAndSync(attempts - 1), 100);
    }
}

