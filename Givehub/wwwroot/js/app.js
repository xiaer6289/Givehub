function sendMail() {
    let parms = {
        name: document.getElementById("name").value,
        email: document.getElementById("email").value,
        subject: document.getElementById("subject").value,
        message: document.getElementById("phoneNo").value,
    }
}


window.generatePDF = function (elementId, title) {
    const pdfElement = document.getElementById(elementId);
    if (!pdfElement) {
        alert("PDF content not found: " + elementId);
        return;
    }

    const fileName = title + ".pdf";

    html2pdf()
        .set({
            margin: [10, 10, 30, 10], // increase spacce
            filename: fileName,
            html2canvas: {
                scale: 2, // higher resolution & fewer clipping issues
                useCORS: true
            },
            jsPDF: {
                unit: "mm",
                format: "a4",
                orientation: "portrait"
            },
            pagebreak: {
                mode: ['avoid-all', 'css', 'legacy'] // prevent cutting elements
            }
        })
        .from(pdfElement).save();
};

//const fileName = "Order_" + new Date().toISOString().slice(0, 10) + ".pdf";
