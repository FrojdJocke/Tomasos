//// When the user scrolls down 20px from the top of the document, show the button
//window.onscroll = function () { scrollFunction() };

//function scrollFunction() {
//    if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
//        document.getElementById("myBtn").style.display = "block";
//    } else {
//        document.getElementById("myBtn").style.display = "none";
//    }
//}

//// When the user clicks on the button, scroll to the top of the document
//function topFunction() {
//    document.body.scrollTop = 0;
//    document.documentElement.scrollTop = 0;
//}
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