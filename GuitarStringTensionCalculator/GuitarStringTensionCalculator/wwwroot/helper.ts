function downloadFromBase64String(fileName: string, base64String: string): void {
    const a = document.createElement('a');
    a.href = "data:application/octet-stream;base64," + base64String;
    a.download = fileName;
    a.click();
    a.remove();
}

