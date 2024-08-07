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
        saveAllUnsavedRows();
        

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
        

        // Make only the new row editable
        table.rows().every(function() {
            makeRowReadonly($(this.node()));
        });
        makeRowEditable($addedRow);

        // Calculate the page of the newly added row
        let info = table.page.info();
        let rowIndex = table.rows().nodes().indexOf(addedRow.node());
        let pageOfNewRow = Math.floor(rowIndex / info.length);

        // Move to the page of the new row
        table.page(pageOfNewRow).draw('page');
        
        // Focus on the first input of the new row
        $addedRow.find('.product-name').focus();

        makeRowEditable($addedRow);

        $addedRow.find('.edit-product')
            .html('<i class="fas fa-save"></i> Save')
            .removeClass('edit-product btn-outline-primary')
            .addClass('save-product btn-outline-success');
            
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
            $button.removeClass('btn-outline-primary edit-product')
                .addClass('btn-outline-success save-product')
                .html('<i class="fas fa-save"></i> Save');
        } else if ($button.hasClass('save-product')) {
            if (saveRow($row)) {
                makeRowReadonly($row);
                // The button state is already changed in saveRow function
                updateGrandTotal();
            }
        }
    });

    $('#productTable').on('input', '.product-quantity', function () {
        let $row = $(this).closest('tr');
        updateTotalPrice($row);
        validateInput($(this));
        updateGrandTotal();
    });

    $('#productTable').on('change', '.product-quantity', function () {
        let $row = $(this).closest('tr');
        updateRowData($row);
    });


    function makeRowEditable($row) {
        $row.find('.product-name').prop('disabled', false);
        $row.find('.product-quantity').prop('readonly', false);
    }

    function makeRowReadonly($row) {
        $row.find('.product-name').prop('disabled', true);
        $row.find('.product-quantity').prop('readonly', true);
    }

    function updateRowData($row) {
        let name = '';
        if ($row.find('.product-name option:selected').text() !== "Select Product") {
            name = $row.find('.product-name option:selected').text();
        }
        let rowData = {
            id: $row.attr('data-id'),
            name: name,
            quantity: parseInt($row.find('.product-quantity').val()) || 0,
            unitPrice: parseFloat($row.find('.product-unit-price').val()) || 0,
            totalPrice: parseFloat($row.find('.product-total-price').val()) || 0
        };
        // Store the current button state
        let $button = $row.find('.edit-product, .save-product');
        let buttonHtml = $button.html();
        let buttonClasses = $button.attr('class');
        
        table.row($row).data(rowData).draw(false);

        // Restore the button state
        $row.find('.edit-product, .save-product')
            .html(buttonHtml)
            .attr('class', buttonClasses);
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

        let $quantity = $row.find('.product-quantity');
        if ($quantity.val() === "" || parseInt($quantity.val()) < 1) {
            $quantity.addClass('is-invalid');
            isValid = false;
        } else {
            $quantity.removeClass('is-invalid');
        }

        return isValid;
    }

  /*  function updateGrandTotal() {
        let total = table.column(3).data().reduce((acc, val) => acc + parseFloat(val), 0);
        let formattedTotal = total.toFixed(2);
        $('#grandTotal').text('Grand Total: $' + formattedTotal);
        // Update the Purchase Amount field
        $('#purchaseAmount').val(formattedTotal);
    }*/

    function updateGrandTotal() {
        let subtotal = table.column(3).data().reduce((acc, val) => acc + parseFloat(val), 0);
        let formattedSubtotal = subtotal.toFixed(2);
        $('#grandTotal').text('Subtotal: $' + formattedSubtotal);

        let taxAmount = calculateTax(subtotal);
        let total = subtotal + taxAmount;

        $('#purchaseAmount').val(total.toFixed(2)); 
    }
    
    $('#Purchase_TaxId').on('change', function() {
        updateGrandTotal();
    });

    function calculateTax(subtotal) {
        let taxRateText = $('#Purchase_TaxId option:selected').text();
        // Extract the number in parentheses
        let match = taxRateText.match(/\((\d+(?:\.\d+)?)\)/);
        let taxRate = 0;
        if (match) {
            taxRate = parseFloat(match[1]) / 100;
        }
        return subtotal * taxRate;
    }
    
    function saveAllUnsavedRows() {
        table.rows().every(function() {
            let $row = $(this.node());
            let $saveButton = $row.find('.save-product');
            if ($saveButton.length > 0) {
                saveRow($row);
            }
        });
    }

    function saveRow($row) {
        if (validateRow($row)) {
            makeRowReadonly($row);
            updateRowData($row);
            $row.find('.save-product')
                .removeClass('btn-outline-success save-product')
                .addClass('btn-outline-primary edit-product')
                .html('<i class="fas fa-edit"></i> Edit');
            return true;
        } else {
            alert('Please fill in all required fields correctly.');
            return false;
        }
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
    updateGrandTotal();
}


