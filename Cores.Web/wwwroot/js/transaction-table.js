let accountOptions;
let rowTemplate;

function initTable(accountOpts) {
    accountOptions = accountOpts;
    console.log(accountOptions);
    /*updateTotals();
    // Store template only if we have existing rows
    if ($('#detailsTableBody tr').length > 0) {
        rowTemplate = $('#detailsTableBody tr:first').clone();
    }*/
}

function addDetailRow() {
    let rowCount = $('#detailsTableBody tr').length;
    let newRow;

    // If we have a template, use it, otherwise create new row
    if (rowTemplate) {
        newRow = rowTemplate.clone();
        // Clear values
        newRow.find('input').val('');
        newRow.find('select').val('').trigger('change');
        // Update row number
        newRow.find('.row-number').text(rowCount + 1);
        // Update input names and ids
        updateRowIndexes(newRow, rowCount);
    } else {
        newRow = createNewRow(rowCount);
    }

    // Add to table
    $('#detailsTableBody').append(newRow);

    // Initialize Select2 for new row
    newRow.find('.select2').select2({
        width: '100%',
        placeholder: '-- Select Account --'
    });

    updateTotals();
}

function createNewRow(index) {
    return $(`
        <tr>
            <td class="row-number">${index + 1}</td>
            <td>
                <select name="Transaction.Details[${index}].AccountId" 
                        class="form-control select2"
                        required>
                    <option value="" disabled selected>-- Select Account --</option>
                    ${accountOptions.map(opt => `<option value="${opt.value}">${opt.text}</option>`).join('')}
                </select>
            </td>
            <td>
                <input name="Transaction.Details[${index}].Description" 
                       class="form-control"
                       placeholder="Line description">
            </td>
            <td>
                <div class="input-group">
                    <span class="input-group-text">$</span>
                    <input name="Transaction.Details[${index}].DebitAmount" 
                           class="form-control debit-amount text-end" 
                           type="number" 
                           min="0"
                           onchange="handleAmountChange(this, 'debit')"
                           onfocusout="formatNumber(this)"
                           placeholder="0.00">
                </div>
            </td>
            <td>
                <div class="input-group">
                    <span class="input-group-text">$</span>
                    <input name="Transaction.Details[${index}].CreditAmount" 
                           class="form-control credit-amount text-end" 
                           type="number" 
                           min="0"
                           onchange="handleAmountChange(this, 'credit')"
                           onfocusout="formatNumber(this)"
                           placeholder="0.00">
                </div>
            </td>
            <td class="text-center">
                <div class="btn-group btn-group-sm">
                    <button type="button" class="btn btn-outline-danger" onclick="removeDetailRow(this)">
                        <i class="fas fa-trash"></i>
                    </button>
                    <button type="button" class="btn btn-outline-secondary" onclick="clearRow(this)">
                        <i class="fas fa-eraser"></i>
                    </button>
                </div>
            </td>
        </tr>
    `);
}

function updateRowIndexes(row, index) {
    row.find('select, input').each(function() {
        let name = $(this).attr('name');
        if (name) {
            let newName = name.replace(/\[\d+\]/, `[${index}]`);
            $(this).attr('name', newName);
        }
    });
}

function removeDetailRow(btn) {
    let row = $(btn).closest('tr');
    let table = row.closest('table');

    if (table.find('tbody tr').length > 1) {
        row.remove();
        renumberRows();
        updateTotals();
    } else {
        clearRow(btn);
    }
}

function clearRow(btn) {
    let row = $(btn).closest('tr');
    row.find('input').val('');
    row.find('select').val('').trigger('change');
    updateTotals();
}

function renumberRows() {
    $('#detailsTableBody tr').each(function(index) {
        $(this).find('.row-number').text(index + 1);
        updateRowIndexes($(this), index);
    });
}

function handleAmountChange(input, type) {
    let row = $(input).closest('tr');
    let otherInput = type === 'debit'
        ? row.find('.credit-amount')
        : row.find('.debit-amount');

    // Ensure only one amount type per row
    if (input.value && input.value > 0) {
        otherInput.val('').prop('readonly', true);
    } else {
        otherInput.prop('readonly', false);
    }

    updateTotals();
}

function formatNumber(input) {
    if (input.value) {
        input.value = parseFloat(input.value).toFixed(2);
    }
}

function updateTotals() {
    let totalDebit = 0;
    let totalCredit = 0;

    $('.debit-amount').each(function() {
        totalDebit += parseFloat(this.value || 0);
    });

    $('.credit-amount').each(function() {
        totalCredit += parseFloat(this.value || 0);
    });

    $('#totalDebit').val(totalDebit.toFixed(2));
    $('#totalCredit').val(totalCredit.toFixed(2));
    $('#hiddenTotalDebit').val(totalDebit.toFixed(2));
    $('#hiddenTotalCredit').val(totalCredit.toFixed(2));

    let difference = Math.abs(totalDebit - totalCredit);
    if (difference > 0) {
        $('#differenceLine').removeClass('d-none');
        $('#totalDifference').val(difference.toFixed(2));
        $('#submitButton').prop('disabled', true);
    } else {
        $('#differenceLine').addClass('d-none');
        $('#submitButton').prop('disabled', false);
    }
}


function validateForm() {
    let isValid = true;
    let totalDebit = parseFloat($('#totalDebit').val());
    let totalCredit = parseFloat($('#totalCredit').val());

    // Check if totals match
    if (Math.abs(totalDebit - totalCredit) > 0.01) {
        alert('Total debits must equal total credits.');
        return false;
    }

    // Check if at least one detail line exists
    if ($('#detailsTableBody tr').length === 0) {
        alert('Please add at least one transaction detail.');
        return false;
    }

    // Validate each row
    $('#detailsTableBody tr').each(function () {
        let row = $(this);
        let account = row.find('select').val();
        let debit = parseFloat(row.find('.debit-amount').val() || 0);
        let credit = parseFloat(row.find('.credit-amount').val() || 0);

        if (!account || (debit === 0 && credit === 0)) {
            row.addClass('invalid-row');
            isValid = false;
        } else {
            row.removeClass('invalid-row');
        }
    });

    if (!isValid) {
        alert('Please complete all required fields and ensure each line has either a debit or credit amount.');
    }

    return isValid;
}

function postTransaction(id) {
    if (!confirm('Are you sure you want to post this transaction? This action cannot be undone.')) {
        return;
    }

    $.post(`/Transactions/Post/${id}`, function (response) {
        if (response.success) {
            window.location.href = '/Transactions';
        } else {
            alert(response.message || 'Failed to post transaction.');
        }
    });
}

function copyLastRow() {
    let lastRow = $('#detailsTableBody tr:last');
    let rowCount = $('#detailsTableBody tr').length;

    // Get values before cloning
    let selectedAccount = lastRow.find('select').val();
    let description = lastRow.find('input[name*="Description"]').val();
    let debitAmount = lastRow.find('.debit-amount').val();
    let creditAmount = lastRow.find('.credit-amount').val();

    // Create fresh row from template
    let newRow = createNewRow(rowCount);

    // Set the values
    newRow.find('select').val(selectedAccount);
    newRow.find('input[name*="Description"]').val(description);
    newRow.find('.debit-amount').val(debitAmount);
    newRow.find('.credit-amount').val(creditAmount);

    // Append the new row
    $('#detailsTableBody').append(newRow);

    // Initialize Select2
    newRow.find('select').select2({
        width: '100%',
        placeholder: '-- Select Account --'
    });

    // Update totals after copying
    updateTotals();

    // Handle readonly states for debit/credit fields
    if (debitAmount && debitAmount > 0) {
        newRow.find('.credit-amount').prop('readonly', true);
    } else if (creditAmount && creditAmount > 0) {
        newRow.find('.debit-amount').prop('readonly', true);
    }
}

$('form').on('submit', function(e) {
    try {
        // Validate the form first
        if (!validateForm()) {
            e.preventDefault();
            return false;
        }

        // Get the transaction header data
        let transactionData = {
            details: getSerializedTransactionDetails()
        };

        // Set the serialized value
        $('#serializedTransaction').val(JSON.stringify(transactionData));
        return true;
    } catch (error) {
        e.preventDefault();
        console.error("Error serializing transaction:", error);
        alert("An error occurred while preparing the transaction data. Please check all fields and try again.");
        return false;
    }
});

// Modify the validation function to ensure proper data
function validateForm() {
    let isValid = true;
    let totalDebit = parseFloat($('#totalDebit').val() || 0);
    let totalCredit = parseFloat($('#totalCredit').val() || 0);

    // Check if totals match
    if (Math.abs(totalDebit - totalCredit) > 0.01) {
        alert('Total debits must equal total credits.');
        return false;
    }

    // Check if at least one detail line exists with valid data
    let validLines = getSerializedTransactionDetails();
    if (validLines.length === 0) {
        alert('Please add at least one valid transaction detail with account and amount.');
        return false;
    }

    // Validate each row
    $('#detailsTableBody tr').each(function() {
        let row = $(this);
        let account = row.find('select').val();
        let debit = parseFloat(row.find('.debit-amount').val() || 0);
        let credit = parseFloat(row.find('.credit-amount').val() || 0);

        if (!account || (debit === 0 && credit === 0)) {
            row.addClass('invalid-row');
            isValid = false;
        } else {
            row.removeClass('invalid-row');
        }
    });

    if (!isValid) {
        alert('Please complete all required fields and ensure each line has either a debit or credit amount.');
    }

    return isValid;
}

function getSerializedTransactionDetails() {
    let details = [];

    $('#detailsTableBody tr').each(function() {
        let row = $(this);
        let accountId = row.find('select[name*="AccountId"]').val();
        let debitAmount = parseFloat(row.find('.debit-amount').val() || 0);
        let creditAmount = parseFloat(row.find('.credit-amount').val() || 0);
        let description = row.find('input[name*="Description"]').val();

        // Only include rows that have an account selected and either a debit or credit amount
        if (accountId && (debitAmount > 0 || creditAmount > 0)) {
            details.push({
                transactionId: $('#transactionId').val() || 0,
                accountId: parseInt(accountId),
                description: description || '',
                debitAmount: debitAmount,
                creditAmount: creditAmount
            });
        }
    });

    return details;
}