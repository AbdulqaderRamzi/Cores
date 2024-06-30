function initializeProductTable(existingProducts, products) {
 
    let table = $('#productTable').DataTable({
        columns: [
            {
                data: 'name',
                render: function (data, type, row) {
                    let options = products.map(product => {
                        console.log(product.name);
                        return `<option value="${product.name}" data-unit-price="${product.unitPrice}" ${product.name === row.name ? 'selected' : ''}>${product.name}</option>`;
                    }).join('');
                    return `
                        <select class="form-control product-name" required>
                            <option value="">Select Product</option>
                            ${options}
                        </select>
                        <div class="invalid-feedback">This field is required</div>
                    `;
                }
            },
            {
                data: 'quantity',
                render: function (data, type, row) {
                    return '<input type="number" class="form-control product-quantity text-right" value="' + (data || 1) + '" min="1" required>' +
                        '<div class="invalid-feedback">Quantity must be at least 1</div>';
                }
            },
            {
                data: 'unitPrice',
                render: function (data, type, row) {
                    return '<input type="number" class="form-control product-unit-price text-right" value="' + (data || 0).toFixed(2) + '" min="0.01" step="0.01" required readonly>' +
                        '<div class="invalid-feedback">Unit price must be greater than 0</div>';
                }
            },
            {
                data: 'totalPrice',
                render: function (data, type, row) {
                    return '<input type="number" class="form-control product-total-price text-right" value="' + (data || 0).toFixed(2) + '" readonly>';
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `
                    <div class="btn-group" role="group">
                        <button type="button" class="btn btn-outline-primary btn-sm edit-product">
                            <i class="fas fa-edit"></i> Edit
                        </button>
                        <button type="button" class="btn btn-outline-danger btn-sm delete-product">
                            <i class="fas fa-trash"></i> Delete
                        </button>
                    </div>`;
                }
            }
        ],
        pageLength: 5,
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "All"]],
        footerCallback: function (row, data, start, end, display) {
            let api = this.api();
            let total = api
                .column(3)
                .data()
                .reduce((acc, val) => acc + parseFloat(val), 0);

            $('#grandTotal').text('Grand Total: $' + total.toFixed(2));
        },
        createdRow: function(row, data, dataIndex) {
            $(row).attr('data-id', data.id || '');
        }
    });

    // Add new product
    $('#addProductBtn').click(function () {
        try {
            saveAllUnsavedRows();
        } catch (error) {
            return;
        }

        let emptyRow = table.rows().data().toArray().find(row => !row.name);
        if (emptyRow) {
            alert("Please fill in all fields of the current product before adding a new one.");
            return;
        }

        let newRow = {
            Id: '',
            name: '',
            quantity: 1,
            unitPrice: 0,
            totalPrice: 0
        };

        let addedRow = table.row.add(newRow).draw(false);
        let $addedRow = $(addedRow.node());
        makeRowEditable($addedRow);
        setSaveButton($addedRow);
        $addedRow.find('.product-name').focus();
        updateGrandTotal();
    });

    $('#productTable').on('change', '.product-name', function () {
        let $row = $(this).closest('tr');
        let selectedOption = $(this).find('option:selected');
        let productId = selectedOption.val();
        let unitPrice = selectedOption.data('unit-price');
        $row.attr('data-id', productId);
        $row.find('.product-unit-price').val(unitPrice.toFixed(2));
        updateTotalPrice($row);
        updateRowData($row);
    });

    function setSaveButton($row) {
        $row.find('.edit-product')
            .html('<i class="fas fa-save"></i> Save')
            .removeClass('edit-product btn-outline-primary')
            .addClass('save-product btn-outline-success');
    }

    // Delete product
    $('#productTable').on('click', '.delete-product', function () {
        if (confirm('Are you sure you want to delete this product?')) {
            table.row($(this).closest('tr')).remove().draw(false);
            updateGrandTotal();
        }
    });

    // Edit/Save product
    $('#productTable').on('click', '.edit-product, .save-product', function () {
        let $row = $(this).closest('tr');
        let $button = $(this);

        if ($button.hasClass('edit-product')) {
            makeRowEditable($row);
            setSaveButton($row);
        } else {
            if (validateRow($row)) {
                makeRowReadonly($row);
                $button.html('<i class="fas fa-edit"></i> Edit')
                    .removeClass('save-product btn-outline-success')
                    .addClass('edit-product btn-outline-primary');
                updateRowData($row);
            }
        }
    });

    $('#productTable').on('input', '.product-quantity', function () {
        let $row = $(this).closest('tr');
        updateTotalPrice($row);
        validateInput($(this));
        updateRowData($row);
    });

    function makeRowEditable($row) {
        $row.find('.product-name').prop('disabled', false);
        $row.find('.product-quantity').prop('readonly', false);
    }

    function makeRowReadonly($row) {
        $row.find('.product-name').prop('disabled', true);
        $row.find('input').prop('readonly', true);
    }

    function updateRowData($row) {
        let rowData = {
            id: $row.attr('data-id'),
            name: $row.find('.product-name option:selected').text(),
            quantity: parseInt($row.find('.product-quantity').val()) || 0,
            unitPrice: parseFloat($row.find('.product-unit-price').val()) || 0,
            totalPrice: parseFloat($row.find('.product-total-price').val()) || 0
        };
        table.row($row).data(rowData).draw(false);
        updateGrandTotal();
    }

    function updateTotalPrice($row) {
        let quantity = parseFloat($row.find('.product-quantity').val()) || 0;
        let unitPrice = parseFloat($row.find('.product-unit-price').val()) || 0;
        let totalPrice = (quantity * unitPrice).toFixed(2);
        $row.find('.product-total-price').val(totalPrice);
    }

    function validateInput($input) {
        let value = $input.val().trim();
        let isValid = value !== '' &&
            ($input.attr('type') !== 'number' || parseFloat(value) >= parseFloat($input.attr('min')));

        $input.toggleClass('is-invalid', !isValid);
        $input.toggleClass('is-valid', isValid);
    }

    function validateRow($row) {
        let isValid = true;

        let $productSelect = $row.find('.product-name');
        if ($productSelect.val() === "") {
            $productSelect.addClass('is-invalid');
            isValid = false;
        } else {
            $productSelect.removeClass('is-invalid');
        }

        $row.find('input').not('.product-total-price').not('.product-unit-price').each(function() {
            validateInput($(this));
            if ($(this).hasClass('is-invalid')) {
                isValid = false;
            }
        });
        return isValid;
    }

    function updateGrandTotal() {
        let total = table.column(3).data().reduce((acc, val) => acc + parseFloat(val), 0);
        $('#grandTotal').text('Grand Total: $' + total.toFixed(2));
    }

    function saveAllUnsavedRows() {
        table.rows().every(function() {
            let $row = $(this.node());
            let $saveButton = $row.find('.save-product');
            if ($saveButton.length > 0) {
                if (validateRow($row)) {
                    makeRowReadonly($row);
                    $saveButton.html('<i class="fas fa-edit"></i> Edit')
                        .removeClass('save-product btn-outline-success')
                        .addClass('edit-product btn-outline-primary');
                    updateRowData($row);
                } else {
                    alert("Please fill in all required fields correctly before adding a new product.");
                    $row.find('.product-name').focus();
                    throw new Error("Validation failed");
                }
            }
        });
    }

    $('form').on('submit', function(e) {
        try {
            saveAllUnsavedRows();
            let productsData = table.rows().data().toArray().filter(row => row.name.trim() !== '');

            // Remove the id from each product before serializing
            let productsWithoutId = productsData.map(product => {
                let { id, ...productWithoutId } = product;
                return productWithoutId;
            });
    
            $('#serializedProducts').val(JSON.stringify(productsWithoutId));
        } catch (error) {
            e.preventDefault();
            console.error("Error serializing products:", error);
            alert("Please fill in all required fields correctly before submitting.");
        }
    });

    if (existingProducts && existingProducts.length > 0) {
        table.rows.add(existingProducts).draw();
        updateGrandTotal();
    }
}
