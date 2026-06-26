window.wrkplanNavGuard = (function () {
    let enabled = false;

    function onPopState() {
        if (!enabled) return;
        history.pushState({ wrkplanGuard: true }, "", location.href);
    }

    return {
        enable: function () {
            if (enabled) return;
            enabled = true;
            history.pushState({ wrkplanGuard: true }, "", location.href);
            window.addEventListener("popstate", onPopState);
        },
        disable: function () {
            if (!enabled) return;
            enabled = false;
            window.removeEventListener("popstate", onPopState);
        }
    };
})();
