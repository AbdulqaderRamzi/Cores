/*
$(document).ready(function () {
    // Fetch the default country list from intlTelInput and filter out Israel (il)
    const allCountries = window.intlTelInputGlobals.getCountryData();
    const filteredCountries = allCountries.filter(country => country.iso2 !== "il");

    // Function to initialize intlTelInput
    function initializePhoneInput(phoneInput) {
        return window.intlTelInput(phoneInput, {
            initialCountry: "auto",
            onlyCountries: filteredCountries.map(country => country.iso2),
            preferredCountries: ["us", "gb", "au", "in", "ca"],
            separateDialCode: true,
            autoPlaceholder: "aggressive",
            utilsScript: "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.19/js/utils.js",
            formatOnDisplay: true
        });
    }

    // Loop through each element with the class "phone" and initialize intlTelInput
    $(".phone").each(function () {
        const phoneInput = this; // 'this' refers to the current .phone input
        const iti = initializePhoneInput(phoneInput);

        // Set the initial value if it exists
        const initialValue = $(phoneInput).siblings('.full-phone-number').val();
        if (initialValue) {
            iti.setNumber(initialValue);
        }

        // Remove invalid class on focus
        $(phoneInput).on("focus", function () {
            $(this).removeClass("is-invalid");
        });

        // Handle validation on blur
        $(phoneInput).on("blur", function () {
            if (!iti.isValidNumber()) {
                $(this).addClass("is-invalid");
            } else {
                $(this).removeClass("is-invalid");
            }
        });
    });

    // Handle form submission
    $('form').on('submit', function (event) {
        let allValid = true;

        // Validate phone inputs
        $(".phone").each(function () {
            const phoneInput = this;
            const iti = window.intlTelInputGlobals.getInstance(phoneInput);

            if (!iti.isValidNumber()) {
                allValid = false;
                $(phoneInput).addClass("is-invalid");
            } else {
                $(phoneInput).removeClass("is-invalid");

                // Get the national number without leading zero
                let nationalNumber = iti.getNumber(intlTelInputUtils.numberFormat.NATIONAL);
                nationalNumber = nationalNumber.startsWith('0') ? nationalNumber.substring(1) : nationalNumber;
                $(phoneInput).val(nationalNumber);

                // Set the full international number in the hidden input
                const fullPhoneNumber = iti.getNumber(intlTelInputUtils.numberFormat.E164);
                $(phoneInput).siblings('.full-phone-number').val(fullPhoneNumber);
            }
        });

        // Prevent form submission if validation fails
        if (!allValid) {
            event.preventDefault();
            // You might want to add a validation message here
        }
    });
});
*/






$(document).ready(function () {

    // Fetch the default country list from intlTelInput and filter out Israel (il)
    const allCountries = window.intlTelInputGlobals.getCountryData();
    const filteredCountries = allCountries.filter(country => country.iso2 !== "il");


    // Loop through each element with the class "phone"
    $(".phone").each(function () {
        // Initialize intlTelInput for each phone input
        const phoneInput = this; // 'this' refers to the current .phone input
        const iti = window.intlTelInput(phoneInput, {
            initialCountry: "auto",
            onlyCountries: filteredCountries.map(country => country.iso2),
            preferredCountries: ["us", "gb", "au", "in", "ca"],
            separateDialCode: true,
            autoPlaceholder: "aggressive",
            utilsScript: "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.19/js/utils.js",
            formatOnDisplay: true,
        });

        // Handle form submission
        $('form').on('submit', function (event) {
            if (!iti.isValidNumber()) {
                event.preventDefault();
                $(phoneInput).addClass("is-invalid");
                // You might want to add a validation message here
                return false;
            }
            // Set the full number including country code before submit
            $(phoneInput).val(iti.getNumber());

        });

        // Remove invalid class on focus
        $(phoneInput).on("focus", function () {
            $(this).removeClass("is-invalid");
        });

        // Handle validation on blur
        $(phoneInput).on("blur", function () {
            if (!iti.isValidNumber()) {
                $(this).addClass("is-invalid");
            } else {
                $(this).removeClass("is-invalid");
            }
        });
    });
});

