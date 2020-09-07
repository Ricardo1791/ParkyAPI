﻿var dataTable;

$(document).ready(function () {
    loadDataTable();

})

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/trail/GetAllTrail",
            "type": "GET",
            "dataType": "json"
        },
        "columns": [
            { "data": "nationalPark.name", "width": "20%" },
            { "data": "name", "width": "20%" },
            { "data": "distance", "width": "15%" },
            { "data": "elevation", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center"><a href="/trail/upsert/${data}" class="btn btn-success text-white" style="cursor:pointer;"><i class='far fa-edit'></i></a>
                            &nbsp;
                            <a onclick=Delete("/trail/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer;"><i class='far fa-trash-alt'></i></a>
                            </div>
                    `;
                    //return `<div class="text-center">
                    //            <a href="/nationalParks/Upsert/${data}" class='btn btn-success text-white'
                    //                style='cursor:pointer;'> <i class='far fa-edit'></i></a>
                    //                &nbsp;
                    //            <a onclick=Delete("/nationalParks/Delete/${data}") class='btn btn-danger text-white'
                    //                style='cursor:pointer;'> <i class='far fa-trash-alt'></i></a>
                    //            </div>
                    //        `;

                }, "width": "30%"
            }
        ]
    });
}

function Delete(url) {
    swal({
        title: "Are you sure you want to delete?",
        text: "You will not be able to restore the data!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}