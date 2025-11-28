function sendMail() {
    let parms = {
        name: document.getElementById("name").value,
        email: document.getElementById("email").value,
        subject: document.getElementById("subject").value,
        message: document.getElementById("phoneNo").value,
    }
}


window.generatePDF = function () {
    const pdf = document.getElementById("PDF");

    //dynamically set the filename with date
    const fileName = "Order_" + new Date().toISOString().slice(0, 10) + ".pdf";

    html2pdf().from(pdf).save(fileName);
};