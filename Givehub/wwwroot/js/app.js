
﻿function sendMail() {
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


//----------------Item Page Start ---------------------//



// "Add More" button will add a identical input form for donation entry
document.querySelector(".add-more-button").addEventListener("click", function (e) {

    e.preventDefault(); // Prevent the page from reloading

    const wrapper = document.getElementById("donation-wrapper");
    const firstForm = document.querySelector(".donation-entry");
    const newForm = firstForm.cloneNode(true);

    newForm.querySelector("input").value = "";

    // Insert the new form before the button section**
    const buttonSection = document.querySelector(".button-section");
    const dateSection = document.querySelector(".delivery-date");

    wrapper.insertBefore(newForm, dateSection);
});

// "Remove" button will remove the respective donation entry form
document.addEventListener("click", function (e) {
    if (e.target.closest(".remove-donation-button")) {
        const row = e.target.closest(".donation-entry");
        row.remove();
    }
})

//-----------------Item Page End---------------------//