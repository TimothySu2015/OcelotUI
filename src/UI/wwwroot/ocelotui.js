window.downloadFile = function(filename, contentType, content) {
    var blob = new Blob([content], { type: contentType });
    var url = URL.createObjectURL(blob);
    var a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    setTimeout(function() {
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    }, 100);
};

window.startPanelResize = function(e) {
    e.preventDefault();
    var handle = e.currentTarget;
    var panel = document.getElementById('json-preview-panel');
    if (!panel) return;
    var startX = e.clientX;
    var startWidth = panel.offsetWidth;
    handle.classList.add('dragging');
    document.body.style.cursor = 'col-resize';
    document.body.style.userSelect = 'none';

    function onMove(ev) {
        var delta = startX - ev.clientX;
        var newWidth = Math.max(280, Math.min(startWidth + delta, window.innerWidth * 0.6));
        panel.style.width = newWidth + 'px';
    }

    function onUp() {
        handle.classList.remove('dragging');
        document.body.style.cursor = '';
        document.body.style.userSelect = '';
        document.removeEventListener('mousemove', onMove);
        document.removeEventListener('mouseup', onUp);
    }

    document.addEventListener('mousemove', onMove);
    document.addEventListener('mouseup', onUp);
};

// Sync edit toolbar right edge with JSON preview panel
(function() {
    var _resizeObs = new ResizeObserver(syncToolbar);
    var _lastPanel = null;

    function syncToolbar() {
        var panel = document.getElementById('json-preview-panel');
        var toolbars = document.querySelectorAll('.edit-toolbar-bottom');
        var handle = panel ? panel.previousElementSibling : null;
        var anchorLeft = handle ? handle.getBoundingClientRect().left : (panel ? panel.getBoundingClientRect().left : 0);
        var right = panel ? (document.documentElement.clientWidth - anchorLeft) + 'px' : '0px';
        toolbars.forEach(function(tb) { tb.style.right = right; });

        if (panel !== _lastPanel) {
            if (_lastPanel) _resizeObs.unobserve(_lastPanel);
            if (panel) _resizeObs.observe(panel);
            _lastPanel = panel;
        }
    }

    new MutationObserver(syncToolbar)
        .observe(document.body, { childList: true, subtree: true });
})();
