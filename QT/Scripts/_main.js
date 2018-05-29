deliveryType = "";

$(document).ready(function () {
    $("#datepickerfrom").datepicker({
        dateFormat: "yy-mm-dd"
    });
    $("#datepickerto").datepicker({
        dateFormat: "yy-mm-dd"
    });
    //$.datepicker.formatDate("yy-mm-dd", new Date(2007, 1 - 1, 26));
    $("#fromCreateOrder").on("keyup keypress",
        function(e) {
            var keyCode = e.keyCode || e.which;
            if (($(event.target)[0] !== $("textarea")[0]) && keyCode === 13) {
                e.preventDefault();
                return false;
            }
            return false;
        });

    //$('#showTime').on('hover',
    //        function (e) {
    //            $('#timeShow').css('display', 'block');
    //        });


    $("#bookingSearchField").on("keyup keypress", function (e) {
        var keyCode = e.keyCode || e.which;
        if (keyCode === 13) {
            getBooking();
            return true;
        }
        return true;
    });

    $("#Pickup").change(function () {
        $("#NbrItemsPickup").toggle();
        $("#NbrItemsPickupLable").toggle();
    });

    $("#Canceled").change(function () {
        $("#NbrItemsLable").toggle();
        $("#NbrItems").toggle();
        $("#showInfo").toggle();
    });

    $("#Leverera").change(function () {
        $("#NbrItems").toggle();
        $("#NbrItemsLable").toggle();
    });

    $("#Exchange").change(function () {
        $("#ExchangeNbrItems").toggle();
        $("#NbrItemsExchangeLable").toggle();
    });

    $("#changeWayOfDelivery").change(function () {

        var ch = !this.checked;

        if (!ch)
            $("input#WayOfDelivery").remove();
        else {
            var index = window.deliveryType === "Home" ? 0 : 1;
            $("select#WayOfDelivery").prop("selectedIndex", index);
            $("#fromCreateOrder").append('<input id="WayOfDelivery" name="WayOfDelivery" type="hidden" value="' +
                window.deliveryType +
                '">');
        }

        $("select#WayOfDelivery").prop("disabled", ch);

        getPrice();
        //$("#Zone").html("<Sele")
    });

    //$('#fromCreateOrder').on('change', '#Zone', getPrice());
    $("#Zone").on("change", getPrice);
    $("#NbrItems").on("change", getPrice);
    $("#Pickup").on("change", getPrice);
    $("#NbrItemsPickup").on("change", getPrice);

    $("#Leverera").on("change", getPrice);
    $("#Exchange").on("change", getPrice);
    $("#ExchangeNbrItems").on("change", getPrice);

    $("#fromCreateOrder").bind("#WayOfDelivery").on("change", getPrice);

    initMap();
});

function ShowTimeList() {
    $("#timeShow").css("display", "block");
}
function SetDeliveryType() {
    window.deliveryType = $("#WayOfDelivery").val();
}

//function fillBookingList(value) {
//    $.ajax({
//        type: 'post',
//        url: 'bookings/getbookingsbyorder',
//        data: { orderid: value },
//        success: function(result) {
//            var country = JSON.parse(result);
//            $("#country").select2({
//                data: country
//            });
//        }
//    });
//}

function getPrice() {

    //if ($('#Type').val() === "Monting")
        //return;

    var dt = $("#fromCreateOrder").serialize();

    $.ajax({
        type: "POST",
        url: "/Bokings/GetPriceZone",
        data: dt,
        success: function(result) {
            $("#DeliveryCostSummary").val(result);
        },
        error: function() {
            //alert(xhr.responseText);  
        }
    });
}
function getPriceMonting() {
    
    $.ajax({
        type: "POST",
        url: "/Bokings/GetPriceMonting",
        success: function (result) {
            $("#DeliveryCostSummary").val(result);
        },
        error: function () {
            //alert(xhr.responseText);  
        }
    });
}

//function getPriceRetur() {
//    var dt = $('#fromCreateOrder').serialize();
//    $.ajax({
//        type: 'POST',
//        url: '/Bokings/GetPriceZoneReturn',
//        data: dt,
//        success: function (result) {
//            $('#DeliveryCostSummary').val(result);
//        },
//        error: function (xhr) {
//            //alert(xhr.responseText);  
//        }
//    });
//}

function getBooking() {
    var txt = $("#bookingSearchField").val();

    $.ajax({
        type: "POST",
        url: "/Bokings/GetBookingByIdOrOrderId",
        data: { orderNbr: txt },
        //contentType: "application/json; charset=utf-8",
        //dataType: "json",
        success: function (result) {
            var booking = JSON.parse(result);
            $("#OrderNbr").val(booking.OrderNbr);
            $("#Customer_Name").val(booking.Customer.Name);
            $("#Customer_Email").val(booking.Customer.Email);
            $("#Customer_Phone1").val(booking.Customer.Phone1);
            $("#Customer_Address1").val(booking.Customer.Address1);
            $("#Customer_Name").val(booking.Customer.Name);
            $("#Customer_ZipCode").val(booking.Customer.ZipCode);
            $("#Customer_Floor").val(booking.Customer.Floor);
            $("#Customer_PortCode").val(booking.Customer.PortCode);
            $("#OrderAmount").val(booking.OrderAmount);
            $("#NbrItems").val(booking.NbrItems);
            $("#Distance").val(booking.Distance);
            //////////$('#WayOfDelivery').val(booking.WayOfDelivery);
            $("#Zone").val(booking.Zone);
            $("#Remarks").val(booking.Remarks);

            $("#PayBySupplier").prop("checked", booking.PayBySupplier);

            getPrice();
        },
        error: function () {
            //alert(xhr.responseText);  
        }
    });
}

function addProduct() {
    var id = $("#productslist").val();

    $.ajax({
        type: "POST",
        url: "/Bokings/AddProductById",
        data: { productId: id },
        success: function (result) {
            $("#productsListContainer").html(result);
            getPrice();
            //getPriceMonting();
        },
        error: function () {
            //alert(xhr.responseText);  
        }
    });
}
function removeProduct(id) {
    
    $.ajax({
        type: "POST",
        url: "/Bokings/RemoveProductById",
        data: { productId: id },
        success: function (result) {
            $("#productsListContainer").html(result);
            getPrice();
            //getPriceMonting();
        },
        error: function () {
            //alert(xhr.responseText);  
        }
    });
}

function printOrder() {
    var originalContents = document.body.innerHTML;
    document.body.innerHTML = $("#orderPrint").innerHTML;

    window.print();
    document.body.innerHTML = originalContents;
}

function calculateDistanceMultipleRoutes() {

    var destination = $("#Customer_Address1").val() + "," + $("#Customer_ZipCode").val() + " " + $("#Customer_City").val() + ", sweden";
    calculateDistanceMultipleRoutes(destination);
}


function calculateDistanceMultipleRoutes(destination) {

    var origin = "xxxlutx malmö, sweden";

    //var service = new google.maps.DirectionsService;
    var service = new window.google.maps.DirectionsService;
    service.route({
        origin: origin,
        destination: destination,
        waypoints: [],
        travelMode: "DRIVING",
        avoidTolls: true
    }, callback2);
}

function callback2(response, status) {
    if (status === "OK") {
        var distance = 0;
        var responseLegs = response.routes;
        for (var i = 0; i < responseLegs.length; i++) {
            for (var j = 0; j < responseLegs[i].legs.length; j++) {
                var a = responseLegs[i].legs[j];
                var b = a.distance.value;
                if (b > distance)
                    distance = Math.floor((b / 1000) + 1);
            }
        }


        //var originLink = "Cederströmsgatan+2,21239+malmö,sweden";
        //var destination = $("#Customer_Address1").val() + "," + $("#Customer_ZipCode").val() + " " + $("#Customer_City").val() + ", sweden";
        //var destinationLink = destination.replace(/\s/g, "+");
        //var mapLink = "https://www.google.com/maps/embed/v1/directions?key=AIzaSyAbHqUFwRAyNX7xezF1qgSEzVvcQ042mLs&origin=" + originLink + "&destination=" + destinationLink + "&mode=driving";

        //$('#txtDistance').val(longest + ' km');
        $("#Distance").val(distance);

        var index = distance >= 70 ? 2 : (distance >= 37 ? 1 : 0);
        selectZone(index);
        getPrice();
        $("#Customer_Floor").focus();

        //$("#map").html('<iframe width="600" height="450" frameborder="0" style="border:0" src="' + mapLink + '" allowfullscreen></iframe>');
    }
}
function selectZone(index) {

    var lght = $("#Zone ").children("option[value='Zon3']").length;
    if (index === 2 && lght === 0) {
        alert("Zon3 kan väljas endast tisdag och torsdag.");
        index--;
    } else {
        $("#Zone").prop("selectedIndex", index);
    }
}
function initMap() {
    var input = document.getElementById("pac-input");

    var autocomplete = new window.google.maps.places.Autocomplete(input);

    //var infowindow = new window.google.maps.InfoWindow();

    autocomplete.addListener("place_changed", function () {
        var place = autocomplete.getPlace();
        if (!place.geometry) {
            return;
        }
        var street = (place.address_components[1] && place.address_components[1].short_name || "") + " " + (place.address_components[0] && place.address_components[0].short_name || "");
        var zip = (place.address_components[6] && place.address_components[6].short_name || "");
        var city = (place.address_components[3] && place.address_components[3].short_name || "");

        $("#Customer_Address1").val(street);
        $("#Customer_ZipCode").val(zip);
        $("#Customer_City").val(city);

        var address = "";
        if (place.address_components) {
            address = [street, ", ", zip, city, ", sweden"].join(" ");
        }
        calculateDistanceMultipleRoutes(address);
    });
}