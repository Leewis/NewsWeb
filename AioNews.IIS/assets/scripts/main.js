$.fn.handleTopMenu = function() {
    $(".all-link").click(function(){
        let $allMenu = $(".wrap-all-menu");
        if ($allMenu.hasClass('show-all-menu')){
           $allMenu.removeClass('show-all-menu');
        } else {
           $allMenu.addClass('show-all-menu');
        }
      
    });
    $(".header-menu .close-menu").click(function(){
        $(".wrap-all-menu").removeClass('show-all-menu');
    });
    $('.btn-view-cate').click(function(){
        let self=$(this);
        let liElement = $(this).parent().find('li');
        console.log(liElement);
        if(liElement.length> 0 ){           
            Array.prototype.forEach.call(liElement, child => {
                if($(child).hasClass('hidden')){
                    $(child).removeClass('hidden');
                    self.hide();
                }
              });
        }
    });
}

$.fn.stickyMenu = function() {
    var header = document.getElementById("myHeader");
    var sticky = header.offsetTop;
    if (window.pageYOffset > sticky) {
        header.classList.add("sticky-header");
    } else {
        header.classList.remove("sticky-header");
    } 

    if(window.innerWidth <= 991) {
        $('.top-header').addClass('sticky-header');
    }
}

$.fn.viewMore = function () {
    $('.btn-view-more').click(function(){
        let self=$(this);
        let liElement = $(this).parent().find('li');
        if(liElement.length> 0 ){           
            Array.prototype.forEach.call(liElement, child => {
                if($(child).hasClass('d-none')){
                    $(child).removeClass('d-none');
                    self.hide();
                }
              });
        }
    });
}
$.fn.viewDate = function () {
    $(".calendar").flatpickr({
        dateFormat: "d/m/Y"
    });
}

$.fn.mobileMenu = function() {
    let sideMenu = $("#mySidenav");
    let overlay =  $('.close-side-overlay');

   $('.view-mb-menu').click(function(){
    sideMenu.addClass('act-mb-open');
    overlay.addClass('overlay-opened');
   });
   overlay.click(function () {
    if ($(this).hasClass('overlay-opened')) {
        $(this).removeClass('overlay-opened');
        sideMenu.removeClass('act-mb-open');
    };
    });
    $('.btn-mb-close').click(function () {
        sideMenu.removeClass('act-mb-open');
        overlay.removeClass('overlay-opened');
    });
}

window.onscroll = function () {  
    $.fn.stickyMenu();
};

$(document).ready(function () {
       //handle tat-ca button on menu  
       $.fn.handleTopMenu();
       //display menu when reload page
       $.fn.stickyMenu();
       //silder bts4
       $('.carousel').carousel();
       //view more action
       $.fn.viewMore();
       //view date
       $.fn.viewDate();
       $.fn.mobileMenu();
});