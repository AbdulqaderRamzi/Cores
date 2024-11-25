/*
    function displayFileInfo(file) {
        let fileName = file.name;
        let fileExtension = fileName.split('.').pop().toLowerCase();
    
        $('#fileName').text(fileName);
    
        // Set appropriate icon based on file type
        if (fileExtension === 'pdf') {
            $('#fileIcon').removeClass().addClass('fas fa-file-pdf fa-5x mb-3 text-danger');
        } else if (['jpg', 'jpeg', 'png', 'gif'].includes(fileExtension)) {
            $('#fileIcon').removeClass().addClass('fas fa-file-image fa-5x mb-3 text-primary');
        } else {
            $('#fileIcon').removeClass().addClass('fas fa-file fa-5x mb-3 text-secondary');
        }
    
        $('#currentDocument').show();
    }
    
    function removeFile() {
        $('#file').val('');
        $('.custom-file-label').html('Choose file');
        $('#currentDocument').hide();
        $('#downloadDocument').hide();
    }
    
    function initializeFileUpload() {
        $('.custom-file-input').on('change', function(e) {
            let fileInput = $(this);
            let fileName = fileInput.val().split('\\').pop();
    
            if (fileName) {
                fileInput.next('.custom-file-label').html("Choose new file");
                if (this.files && this.files[0]) {
                    displayFileInfo(this.files[0]);
                    // Hide download button for new file upload
                    $('#downloadDocument').hide();
                }
            } else {
                // Do not reset the label or hide the document info if no new file is selected
                if (!$('#currentDocument').is(':visible')) {
                    fileInput.next('.custom-file-label').html('Choose file');
                    $('#currentDocument').hide();
                }
            }
        });
    
        $('#myDropzone').on('dragover', function(e) {
            e.preventDefault();
            $(this).addClass('dropzone-active');
        }).on('dragleave', function() {
            $(this).removeClass('dropzone-active');
        }).on('drop', function(e) {
            e.preventDefault();
            $(this).removeClass('dropzone-active');
            let file = e.originalEvent.dataTransfer.files[0];
            $('#file').prop('files', e.originalEvent.dataTransfer.files);
            $('.custom-file-label').html("Choose new file");
            displayFileInfo(file);
            // Hide download button for new file upload
            $('#downloadDocument').hide();
        });
    
        // Add click event for remove button
        $('#removeDocument').on('click', removeFile);
    }
    
    function displayExistingDocument(resumePath) {
        if (resumePath !== '') {
            let fileName = resumePath.split('/').pop();
            $('#fileName').text(fileName);
            $('.custom-file-label').html('Choose new file');
            let fileExtension = fileName.split('.').pop().toLowerCase();
            if (fileExtension === 'pdf') {
                $('#fileIcon').removeClass().addClass('fas fa-file-pdf fa-5x mb-3 text-danger');
            } else if (['jpg', 'jpeg', 'png', 'gif'].includes(fileExtension)) {
                $('#fileIcon').removeClass().addClass('fas fa-file-image fa-5x mb-3 text-primary');
            } else {
                $('#fileIcon').removeClass().addClass('fas fa-file fa-5x mb-3 text-secondary');
            }
            $('#currentDocument').show();
            $('#downloadDocument').show();
        } else {
            $('#currentDocument').hide();
            $('#downloadDocument').hide();
        }
    }

*/



function displayFileInfo(file) {
    let fileName = file.name;
    let fileExtension = fileName.split('.').pop().toLowerCase();

    $('#fileName').text(fileName);

    // Set appropriate icon based on file type
    if (fileExtension === 'pdf') {
        $('#fileIcon').removeClass().addClass('fas fa-file-pdf fa-5x mb-3 text-danger');
    } else if (['jpg', 'jpeg', 'png', 'gif'].includes(fileExtension)) {
        $('#fileIcon').removeClass().addClass('fas fa-file-image fa-5x mb-3 text-primary');
    } else {
        $('#fileIcon').removeClass().addClass('fas fa-file fa-5x mb-3 text-secondary');
    }

    $('#currentDocument').show();
}

$('.custom-file-input').on('change', function(e) {
    let fileInput = $(this);
    let fileName = fileInput.val().split('\\').pop();

    if (fileName) {
        fileInput.next('.custom-file-label').html("Choose new file");
        if (this.files && this.files[0]) {
            displayFileInfo(this.files[0]);
            // Hide download button for new file upload
            $('#downloadDocument').hide();
        }
    } else {
        // Do not reset the label or hide the document info if no new file is selected
        if (!$('#currentDocument').is(':visible')) {
            fileInput.next('.custom-file-label').html('Choose file');
            $('#currentDocument').hide();
        }
    }
});

$('#myDropzone').on('dragover', function(e) {
    e.preventDefault();
    $(this).addClass('dropzone-active');
}).on('dragleave', function() {
    $(this).removeClass('dropzone-active');
}).on('drop', function(e) {
    e.preventDefault();
    $(this).removeClass('dropzone-active');
    let file = e.originalEvent.dataTransfer.files[0];
    $('#file').prop('files', e.originalEvent.dataTransfer.files);
    $('.custom-file-label').html("Choose new file");
    displayFileInfo(file);
    // Hide download button for new file upload
    $('#downloadDocument').hide();
});

function removeFile() {
    $('#file').val('');
    $('.custom-file-label').html('Choose file');
    $('#currentDocument').hide();
    $('#downloadDocument').hide();
}

// Add click event for remove button
$('#removeDocument').on('click', removeFile);


function displayExistingDocument(file) {
    if (file !== '') {
        let fileName = file.split('/').pop();
        $('#fileName').text(fileName);
        $('.custom-file-label').html('Choose new file');
        let fileExtension = fileName.split('.').pop().toLowerCase();
        if (fileExtension === 'pdf') {
            $('#fileIcon').removeClass().addClass('fas fa-file-pdf fa-5x mb-3 text-danger');
        } else if (['jpg', 'jpeg', 'png', 'gif'].includes(fileExtension)) {
            $('#fileIcon').removeClass().addClass('fas fa-file-image fa-5x mb-3 text-primary');
        } else {
            $('#fileIcon').removeClass().addClass('fas fa-file fa-5x mb-3 text-secondary');
        }
        $('#currentDocument').show();
        $('#downloadDocument').show();
    } else {
        $('#currentDocument').hide();
        $('#downloadDocument').hide();
    }
}
