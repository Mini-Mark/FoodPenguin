if (window.location.pathname != "/") {
    if (sessionStorage.getItem("user_id") == null) {
        location.href = "/";
        alert("Please Login..");
    }
}

$(".profile .name").html(
    sessionStorage.getItem("firstname") +
    " " +
    sessionStorage.getItem("lastname")
);

//EditProfile
$(".profile .dropdown-content .dropdown-item-1").on("click", function () {
    $("#popup-show").show();
    $.ajax({
        url: "/Home/profile",
        dataType: "text",
        success: function (data) {
            $("#popup-show").html(data);
            $("#modal-close-btn").on("click", function () {
                $("#popup-show").hide();
            });
        },
    });
});

//Logout
$(".profile .dropdown-content .dropdown-item-2").on("click", function () {
    sessionStorage.clear();
    location.href = "/";
});

//Main - Menu
$(
    ".main-container .main-left .menu .item-1, .main-container .footer .menu .item-1"
).on("click", function () {
    location.href = "/Home/Main";
});

//History - Menu
$(
    ".main-container .main-left .menu .item-2, .main-container .footer .menu .item-2"
).on("click", function () {
    location.href = "/Home/History";
});

//Progess - Menu
$(
    ".main-container .main-left .menu .item-3, .main-container .footer .menu .item-3"
).on("click", function () {
    $.ajax({
        url: `/List/getListProgressFromUser`,
        type: "GET",
        data: {
            user_id: sessionStorage.getItem("user_id"),
        },
        success: function (data) {
            console.log(data);

            type = (data.value != undefined ? data.value.type : "");

            if (type == "order") {
                location.href = `/home/Progress_OrderList`
            } else if (type == "receive") {
                location.href = `/Home/Receive_Progress_Step_1?id=${data.value.recvp_id}`;
            } else {
                location.href = `/home/Progress_OrderList?NoOrder=true`
            }

        },
        error: function (error) {
            console.log(`Error ${error}`);
        },
    });
});

//Image to Base 64
function readImage(inputElement) {
    var deferred = $.Deferred();

    var files = inputElement.get(0).files;
    if (files && files[0]) {
        var fr = new FileReader();
        fr.onload = function (e) {
            deferred.resolve(e.target.result);
        };
        fr.readAsDataURL(files[0]);
    } else {
        deferred.resolve(undefined);
    }

    return deferred.promise();
}

//Profile on Header
if (sessionStorage.getItem("profile_img") != "") {
    $(".profile .image img").attr("src", sessionStorage.getItem("profile_img"));
}

//Open Detail
function openOrderDetailPopup(id) {
    $("#popup-show").html("");
    $.ajax({
        url: `/Home/orderDetail`,
        dataType: "text",
        success: function (data) {
            $("#popup-show").html(
                `<input type="hidden" id="hiddenData" value="${id}">` + data
            );
            $("#popup-show").show();
            $("#modal-close-btn").on("click", function () {
                $("#popup-show").hide();
            });
        },
    });
}

function openReceiveDetailPopup(id) {
    $("#popup-show").html("");
    $("#popup-show").show();
    $.ajax({
        url: `/Home/receiveDetail`,
        dataType: "text",
        success: function (data) {
            $("#popup-show").html(
                `<input type="hidden" id="hiddenData" value="${id}">` + data
            );
            $("#popup-show").show();
            $("#modal-close-btn").on("click", function () {
                $("#popup-show").hide();
            });
        },
    });
}


//Show Image
function showImage(url) {
    $("#showImage-Zone").html(`

        <div class="showImage" onclick="hideImage()"><img src="${url}" /></div>
        
    `);
}

function hideImage() {
    $("#showImage-Zone").html(``);
}

//Left Menu Animation Added
$(".main-container .main-left:first").append(`<div class="area">
            <ul class="circles">
                <li></li>
                <li></li>
                <li></li>
                <li></li>
                <li></li>
                <li></li>
                <li></li>
                <li></li>
                <li></li>
                <li></li>
            </ul>
        </div>`);