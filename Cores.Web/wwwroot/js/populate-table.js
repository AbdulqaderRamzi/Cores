function populateTable(data, property) {
    let table = $("#dataTable");
    table.empty(); // Clear the entire table

    // Define headers based on property
    const headers = {
        'Subordinates': ['Name', 'Email', 'Position', 'Department'],
        'LeaveRequests': ['Leave Type', 'Start Date', 'End Date', 'Status'],
        'Attendances': ['Date', 'Time In', 'Time Out'],
        'Salaries': ['Date', 'Base', 'Bonuses', 'Deductions'],
        'EmployeeTrainings': ['Training Name', 'Start Date', 'Trainer'],
        'EmployeeBenefits': ['Benefit Name', 'Start Date', 'End Date'],
        'Documents': ['File Type', 'Upload Date', 'Expiry Date']
    };

    if (!headers.hasOwnProperty(property)) {
        console.error("Unknown property:", property);
        return;
    }

    // Add header
    let thead = $('<thead>').appendTo(table);
    let headerRow = $('<tr>').appendTo(thead);
    headers[property].forEach(header => headerRow.append($('<th>').text(header)));
    headerRow.append($('<th>').text('Actions')); // Add Actions column

    // Add footer
    let tfoot = $('<tfoot>').appendTo(table);
    let footerRow = $('<tr>').appendTo(tfoot);
    headers[property].forEach(footer => footerRow.append($('<th>').text(footer)));
    footerRow.append($('<th>').text('Actions')); // Add Actions column

    // Add body
    let tbody = $('<tbody>').appendTo(table);
    if (Array.isArray(data)) {
        data.forEach(function(item) {
            let row = $('<tr>');
            switch(property) {
                case 'Subordinates':
                    row.append($('<td>').text(`${item.FirstName} ${item.LastName}`));
                    row.append($('<td>').text(item.Email));
                    row.append($('<td>').text(item.Position.Title));
                    row.append($('<td>').text(item.Position.Department.Name));
                    break;
                case 'LeaveRequests':
                    row.append($('<td>').text(item.LeaveType.Name));
                    row.append($('<td>').text(formatDateOnly(item.StartDate)));
                    row.append($('<td>').text(formatDateOnly(item.EndDate)));
                    row.append($('<td>').text(item.LeaveStatus));
                    break;
                case 'Attendances':
                    row.append($('<td>').text(formatDateOnly(item.Date)));
                    row.append($('<td>').text(item.TimeIn ? formatTimeOnly(item.TimeIn) : ''));
                    row.append($('<td>').text(item.TimeOut ? formatTimeOnly(item.TimeOut) : ''));
                    break;
                case 'Salaries':
                    row.append($('<td>').text(formatDateTime(item.EffectiveDate)));
                    row.append($('<td>').text(formatCurrency(item.BaseSalary)));
                    row.append($('<td>').text(formatCurrency(item.Bonuses)));
                    row.append($('<td>').text(formatCurrency(item.Deductions)));
                    break;
                case 'EmployeeTrainings':
                    row.append($('<td>').text(item.Training.Name));
                    row.append($('<td>').text(formatDateTime(item.Training.TrainingDate)));
                    row.append($('<td>').text(`${item.Training.Trainer.FirstName} ${item.Training.Trainer.LastName}`));
                    break;
                case 'EmployeeBenefits':
                    row.append($('<td>').text(item.Benefit.Name));
                    row.append($('<td>').text(formatDateTime(item.StartDate)));
                    row.append($('<td>').text(formatDateTime(item.EndDate)));
                    break;
                case 'Documents':
                    row.append($('<td>').text(item.ArchiveTypeId ? item.ArchiveType.Name : ''));
                    row.append($('<td>').text(item.UploadDate ? formatDateTime(item.UploadDate) : ''));
                    row.append($('<td>').text(item.ExpiryDate ? formatDateTime(item.ExpiryDate) : ''));
                    break;
            }
            row.append($('<td>').html('<button class="btn btn-primary btn-sm">View</button>'));
            tbody.append(row);
        });
    } else {
        console.error("Data is not an array:", data);
    }
}

function formatDateTime(dateString) {
    if (!dateString) return 'N/A';
    let trainingDate = new Date(dateString);
    let options = { year: 'numeric', month: '2-digit', day: '2-digit' };
    return trainingDate.toLocaleDateString(undefined, options);
}

function formatDateOnly(dateString) {
    if (!dateString) return 'N/A';
    const [year, month, day] = dateString.split('-');
    return `${month.padStart(2, '0')}/${day.padStart(2, '0')}/${year}`;
}

function formatTimeOnly(timeString) {
    if (!timeString) return 'N/A';
    const [hours, minutes] = timeString.split(':');
    return `${hours.padStart(2, '0')}:${minutes.padStart(2, '0')}`;
}

function formatCurrency(amount) {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(amount);
}