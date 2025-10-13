function downloadFromBase64String(fileName, base64String) {
    var a = document.createElement('a');
    a.href = "data:application/octet-stream;base64," + base64String;
    a.download = fileName;
    a.click();
    a.remove();
}
//# sourceMappingURL=helper.js.map