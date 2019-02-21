$(function () {
    var code = $('.register-code-panel input');
    var alert_div = $('.register-code-panel .alert');

    function displayMessage(success, message) {
        alert_div.text(message);
        if (success)
            alert_div.removeClass('alert-danger').addClass('alert-success');
        else
            alert_div.removeClass('alert-success').addClass('alert-danger');

        alert_div.removeClass('hidden');
    }

    $('.register-code-panel button').click(function (e) {
        alert_div.addClass('hidden');

        if (code.val().length == 0) {
            displayMessage(false, 'Enter a code.');
            return;
        }

        $.post('/RegisterCode/Register', { code: code.val() }, function (data) {
            displayMessage(true, "The code was successfully aded. \n\r Please reload the page.");
            code.val('');
        }).fail(function (xlr, status, error) {
            displayMessage(false, "Could not register code.");
        });
    });
});