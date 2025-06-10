function syncViewHeights() {
    const codeMirrorSizer = document.querySelector('.CodeMirror');
    const editorPreview = document.querySelector('.editor-preview-full');
    if (editorPreview && editorPreview.offsetHeight > 0 && codeMirrorSizer) {
        const previewHeight = editorPreview.offsetHeight + 'px';
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

checkAndSync();