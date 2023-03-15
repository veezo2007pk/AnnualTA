$(document).ready(function () {
    $('.updateStatus').click(function () {
        var url = $(this).data('url');
        var id = $(this).attr('id');
        $.get(url, function (data) {
            if (data.success == "0") {
                $('#' + id).removeClass('label-success');
                $('#' + id).addClass('label-danger');
                $('#' + id).text('InActive');
            }
            else if (data.success == "1") {
                $('#' + id).removeClass('label-danger');
                $('#' + id).addClass('label-success');
                $('#' + id).text('Active');
            }
            else if (data.success == "3") {
                $('#message').html(data.message);
            }
        });
        return false;
    });

    $('.btn-bootstrap-dialog').click(function () {
        var url = $(this).data('url');
        $.get(url, function (data) {
            $('#bootstrapDialog').html(data);
            $('#bootstrapDialog').modal('show');
        });
        return false;
    });
});

function UpdateToDoStatus(ID) {
    var url = "/todo/UpdateStatus/" + ID;
    $.get(url,
        function (data) {
            //successMessage("To do Status Changed");
            $('#div_UnFinishtodo').load("/Admin/ToDo/UnFinishToDoList");
            $('#div_Finishtodo').load("/Admin/ToDo/FinishToDoList");
        });
}