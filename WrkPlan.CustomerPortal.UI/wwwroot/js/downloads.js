window.wrkplan = window.wrkplan || {};

window.wrkplan.createObjectUrlFromBytes = function (contentType, base64Data) {
    try {
        const binary = atob(base64Data || '');
        const len = binary.length;
        const bytes = new Uint8Array(len);

        for (let i = 0; i < len; i++) {
            bytes[i] = binary.charCodeAt(i);
        }

        const blob = new Blob([bytes], { type: contentType || 'application/octet-stream' });
        const url = URL.createObjectURL(blob);
        console.log('[wrkplan] blob URL created', { contentType, size: len, url });
        return url;
    } catch (err) {
        console.error('[wrkplan] createObjectUrlFromBytes failed', err);
        return '';
    }
};

window.wrkplan.revokeObjectUrl = function (url) {
    if (url && typeof url === 'string' && url.startsWith('blob:')) {
        URL.revokeObjectURL(url);
    }
};

window.wrkplan.downloadFileFromBytes = function (fileName, contentType, base64Data) {
    const objectUrl = window.wrkplan.createObjectUrlFromBytes(contentType, base64Data);
    const link = document.createElement('a');
    link.href = objectUrl;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    setTimeout(() => window.wrkplan.revokeObjectUrl(objectUrl), 0);
};
