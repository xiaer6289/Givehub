
//----------------Item Page End ---------------------//



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
