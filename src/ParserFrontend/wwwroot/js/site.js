// Write your Javascript code.
$(function () {


})

// Upload Page
//$(".upload-form").submit(function (ev) { return uploadFormSubmit(); });

function uploadFormSubmit() {
    var waitImage = "<img src='/images/wait.gif'>";

    // disable all the controls and hide the submit button
    $(".upload-form input").prop('disabled', true);
    $(".upload-form input[type='submit']").hide();
    $(".upload-form .wait-image").show();  

    return true;
}
