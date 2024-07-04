$(document).ready(function() {
    $('.select2').select2();

    $("#customerSelect").on("change", loadCustomerInfo);
    loadCustomerInfo(); // Load customer info if a customer is already selected 
    
    $("#status").on("change", toggleSolutionSection); // When Closed is chosen (e.g. Create) 
    toggleSolutionSection(); // When Closed is already exits (e.g. Update)
    
    
});

function toggleSolutionSection() {
    const status = $("#status").val();
    if (status === "Closed") {
        $("#solutionSection").show();
    } else {
        $("#solutionSection").hide();
    }
}

function loadCustomerInfo() {
    const customerId = $("#customerSelect").val();
    if (customerId) {
        showCustomerInfo(customerId);
    }
}

function showCustomerInfo(customerId) {
    if(customerId) {
        //window.alert(customerId);
        $('#customerPhone, #customerEmail').val('Loading...'); // Show loading indicator
    }
    $.ajax({
        url: `/CRM/Problem/GetContactById/${customerId}`,
        type: 'GET',
        data: { id: customerId },
        success: function(customer) {
            if (customer) {
                $('#customerPhone').val(customer.phoneNumber);
                $('#customerEmail').val(customer.email);
            }
        },
        error: function(xhr, status, error) {
            console.error("Error fetching customer details:", error);
            alert('Failed to fetch customer details. Please try again or contact support.');
        }
    });
}

