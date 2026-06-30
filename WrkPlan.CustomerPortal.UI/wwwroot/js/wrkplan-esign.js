(function () {
    function signatureTextToDataUrl(text) {
        var value = (text || "").trim();
        if (!value) {
            return "";
        }

        var canvas = document.createElement("canvas");
        canvas.width = 640;
        canvas.height = 180;

        var ctx = canvas.getContext("2d");
        ctx.fillStyle = "#ffffff";
        ctx.fillRect(0, 0, canvas.width, canvas.height);

        ctx.fillStyle = "#111111";
        ctx.font = "700 52px 'Segoe Script', cursive";
        ctx.textBaseline = "middle";
        ctx.fillText(value, 24, canvas.height / 2);

        return canvas.toDataURL("image/png");
    }

    window.wrkplanESign = {
        signatureTextToDataUrl: signatureTextToDataUrl
    };
})();
