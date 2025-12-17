
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

//----------------Dropdown Start ---------------------//
$(document).ready(function () {
    $('.user-icon').click(function (event) {
        event.stopPropagation();
        $(this).siblings('.dropdown-menu').toggle();
    });

    $(document).click(function () {
        $('.dropdown-menu').hide();
    });

    //// ----------------- Auto logout functionality -----------------
    //var logoutTimer;
    //var logoutTime = 1 * 60 * 1000;

    //function resetLogoutTimer() {
    //    clearTimeout(logoutTimer);
    //    logoutTimer = setTimeout(autoLogout, logoutTime);
    //}

    //function autoLogout() {
    //    alert("You have been inactive for 1 minutes. Logging out...");
    //    window.location.href = '/Account/Logout'; // Replace with your actual logout URL
    //}

    //// Listen for user activity
    //$(document).on('mousemove keydown click scroll', resetLogoutTimer);

    //// Start the timer when page loads
    //resetLogoutTimer();
});

//----------------Dropdown Start ---------------------//

//----------------Item Page Start ---------------------//

// "Add More" button will add a identical input form for donation entry
document.querySelector(".add-more-button").addEventListener("click", function (e) {

    e.preventDefault(); // Prevent the page from reloading

    const wrapper = document.getElementById("donation-wrapper");
    const firstForm = document.querySelector(".donation-entry");
    const newForm = firstForm.cloneNode(true);

    //Clear the previous input
    newForm.querySelector(".item-qty-input").value = "";
        
    //get the total number of donation entry form that existing
    const index = wrapper.querySelectorAll(".donation-entry").length;

    const select = newForm.querySelector(".item-name-input");
    const qty = newForm.querySelector(".item-qty-input");

    select.name = `Items[${index}].ItemName`;
    qty.name = `Items[${index}].Quantity`;

    // Insert the new form before the button section**
    const dateSection = document.querySelector(".delivery-date");

    wrapper.insertBefore(newForm, dateSection);
});

// "Remove" button will remove the respective donation entry form
document.addEventListener("click", function (e) {
    if (e.target.closest(".remove-donation-button")) {
        e.preventDefault();
        const row = e.target.closest(".donation-entry");
        row.remove();

        const rows = document.querySelectorAll(".donation-entry");
        rows.forEach((r, i) => {
            r.querySelector(".item-name-input").name = `Items[${i}].ItemName`;
            r.querySelector(".item-qty-input").name = `Items[${i}].Quantity`;

        });
    }
});

//-----------------Item Page End---------------------//

document.addEventListener('DOMContentLoaded', () => {
    const amountInput = document.getElementById('amountInput');
    const checkoutBtn = document.getElementById('checkoutBtn');

    function updateBtnState() {
        const amount = parseFloat(amountInput.value) || 0;
        const enabled = amount > 0;

        checkoutBtn.disabled = !enabled;
    }

    amountInput.addEventListener('input', updateBtnState);
    updateBtnState();
})
