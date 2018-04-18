// Write your Javascript code.
$(function () {
})

// Upload Page
$(".upload-form").submit(uploadFormSubmit);

function uploadFormSubmit(ev) {
    var waitImage = "<img src='/images/wait.gif'>";

    // disable all the controls and hide the submit button
    //$(".upload-form .form-group input").prop('disabled', true);
    $(".upload-form input[type='submit']").hide();
    $(".upload-form .wait-image").show();

    return true;
}

$(".show-form .button-reprocess").click(showReprocessDocument);

function showReprocessDocument(ev) {
    
}