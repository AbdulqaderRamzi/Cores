function initializeWorkScheduleTable() {
    let table = $('#scheduleTable').DataTable({
        pageLength: 7,
        lengthMenu: [[7, 14, 21, -1], [7, 14, 21, "All"]],
        columns: [
            {
                data: 'dayOfWeek',
                render: function(data, type, row) {
                    let options = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
                    let select = '<div class="field-container"><select class="form-control day-of-week">';
                    options.forEach((day, index) => {
                        select += `<option value="${index}" ${data == index ? 'selected' : ''}>${day}</option>`;
                    });
                    select += '</select></div>';
                    return select;
                }
            },
            {
                data: 'startTime',
                render: function(data, type, row) {
                    return '<div class="field-container"><input type="text" class="form-control start-time" value="' + (data || '') + '"><div class="warning-text"></div></div>';
                }
            },
            {
                data: 'endTime',
                render: function(data, type, row) {
                    return '<div class="field-container"><input type="text" class="form-control end-time" value="' + (data || '') + '"><div class="warning-text"></div></div>';
                }
            },
            {
                data: null,
                render: function(data, type, row) {
                    return '<button type="button" class="btn btn-danger btn-sm delete-schedule"><i class="fas fa-trash"></i> Delete</button>';
                }
            }
        ],
        createdRow: function(row, data, dataIndex) {
            $(row).attr('data-id', data.id || '');
            initializeFlatpickr(row);
        }
    });

    $('#addScheduleRow').click(function() {
        if (validateLastRow(table)) {
            let newRow = table.row.add({
                dayOfWeek: 0,
                startTime: '',
                endTime: ''
            }).draw(false);
            initializeFlatpickr(newRow.node());
        }
    });

    $('#scheduleTable').on('click', '.delete-schedule', function() {
        table.row($(this).parents('tr')).remove().draw();
    });

    // Add change event listeners to validate fields as they're filled
    $('#scheduleTable').on('change', '.day-of-week, .start-time, .end-time', function() {
        validateField($(this));
    });

    return table;
}

function initializeFlatpickr(row) {
    $(row).find('.start-time, .end-time').each(function() {
        flatpickr(this, {
            enableTime: true,
            noCalendar: true,
            dateFormat: "H:i",
            time_24hr: true,
            minuteIncrement: 15
        });
    });
}

function validateLastRow(table) {
    let lastRow = table.row(':last').node();
    if (!lastRow) return true; // If there are no rows, allow adding a new one

    let $lastRow = $(lastRow);
    let isValid = true;

    $lastRow.find('.day-of-week, .start-time, .end-time').each(function() {
        if (!validateField($(this))) {
            isValid = false;
        }
    });

    return isValid;
}

function validateField($field) {
    let value = $field.val();

    if (!value) {
        $field.addClass('is-invalid'); // Add the red border
        return false;
    } else {
        $field.removeClass('is-invalid'); // Remove the red border
        return true;
    }
}

function collectScheduleData() {
    let table = $('#scheduleTable').DataTable();
    let schedules = [];
    let isValid = true;

    table.rows().every(function() {
        let $row = $(this.node());
        let dayOfWeek = $row.find('.day-of-week').val();
        let startTime = $row.find('.start-time').val();
        let endTime = $row.find('.end-time').val();

        if (!startTime || !endTime) {
            isValid = false;
            return false;  // break the loop
        }

        let schedule = {
            dayOfWeek: dayOfWeek,
            startTime: startTime,
            endTime: endTime
        };

        schedules.push(schedule);
    });

    if (!isValid) {
        return null;
    }

    console.log('Final schedules array:', schedules);
    return JSON.stringify(schedules);
}

