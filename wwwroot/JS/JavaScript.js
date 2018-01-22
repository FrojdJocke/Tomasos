
$(".back-to-top").css({ "display": "none" });

jQuery(document).ready(function() {

    var offset = 50;

    var duration = 300;

    jQuery(window).scroll(function() {

        if (jQuery(this).scrollTop() > offset) {

            jQuery(".back-to-top").fadeIn(duration);

        } else {

            jQuery(".back-to-top").fadeOut(duration);

        }

    });


    jQuery(".back-to-top").click(function(event) {

        event.preventDefault();

        jQuery("html, body").animate({ scrollTop: 0 }, duration);

        return false;

    });

});

$(document).ready(function() {
    $("#edit-role-btn").hide();
})

function doStuff() {


    $("#edit-role-btn").show();


}