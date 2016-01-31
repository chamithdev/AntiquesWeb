// JavaScript Document

$(document).ready(function() {

    $('.dropdown-pannel').click(function(){
		if ($( ".dropdown-content" ).is(":visible") === false ) {
			$( ".dropdown-content" ).slideDown(200);
		} else {
			 $( ".dropdown-content" ).slideUp(200);
		}
	});
		
	// Page back to top --------------------	
    $('body').append('<div id="toTop" class="btn btn-info"><i class="fa fa-arrow-up"><img src="/Themes/IB/Content/images/arrowhead-up.png" /></i>Top</div>');
    	$(window).scroll(function () {
			if ($(this).scrollTop() != 0) {
				$('#toTop').fadeIn();
			} else {
				$('#toTop').fadeOut();
			}
		}); 
    $('#toTop').click(function(){
        $("html, body").animate({ scrollTop: 0 }, 600);
        return false;
    });

	
// end page back to top --------------------------

$(function(){
	 try {
      
		var slider = $('.bxslider').bxSlider({
			captions: true,
			pager:false,
			infiniteLoop:true,
			adaptiveHeight:true,
			responsive:true,
			auto: true,
		});

		$('.bx-next, .bx-prev').click(function(e){
			var i = $(this).index();
			slider.goToSlide(i);
			slider.stopAuto();
			restart=setTimeout(function(){
				slider.startAuto();
			},500);
			return false;
		});
		 } catch(e) {
        // handle an exception here if lettering doesn't exist or throws an exception
    }
});	// END Slider
 try {
       var slider2 = $('#slider2').bxSlider();
    } catch(e) {
        // handle an exception here if lettering doesn't exist or throws an exception
    }

// Page onload popup function
(function( $ ){
   $.fn.onloadPopup = function() {
    $('.pop-box').show();
	$('.fader').show();
   }; 
})(jQuery);


$('#popoverBtn').click(function(e){

	$('.pop-box').show();
	$('.fader').show();

	e.preventDefault();
		slider2.reloadSlider({
			captions: true,
			pager:false,
			infiniteLoop:true,
			adaptiveHeight:true,
			responsive:true,
			auto: true,
	});

});

$('#popClose').click(function(){
	$('.pop-box').hide();
	$('.fader').hide();
});

$(document).keyup(function(e) {
	if (e.keyCode == 27) { // escape key maps to keycode `27`
		$('.pop-box').hide();
		$('.fader').hide();
    }
});

// Text box default value
  $('.js-default-val-text').each( function () {
	 
    $(this).val($(this).attr('defaultVal'));
    $(this).css({color:'grey'});
      });

  $('.js-default-val-text').focus(function(){
    if ( $(this).val() == $(this).attr('defaultVal') ){
      $(this).val('');
      $(this).css({color:'black'});
    }
    });
  $('.js-default-val-text').blur(function(){
    if ( $(this).val() == '' ){
      $(this).val($(this).attr('defaultVal'));
      $(this).css({color:'grey'});
    }
    });
// END: Text box default value

// if the object not found 
 try {
// Sortable
  $(function() {
    $("#sortable").sortable();
  //  $( "#sortable" ).disableSelection();
  });
  

// Dropzone custom Message
	Dropzone.autoDiscover = false;
	//$(".custom-msg-main").dropzone({
	//	dictDefaultMessage: "Click to Upload <br />or <br />Edit Main Boutique Shop Photo",
	//	addRemoveLinks: true,
	//});
	//Dropzone.autoDiscover = false;
	//$(".custom-msg").dropzone({
	//	dictDefaultMessage: "Click To Edit Product <br /> Drag to Arrange Order of Page",
	//	addRemoveLinks: true,
	//});
	//Dropzone.autoDiscover = false;
	//$(".custom-msg-thumb").dropzone({
	//	dictDefaultMessage: "PIC 1<br />Load Picture<br />Click & Drag For Picture Order Or Delete option",
	//	addRemoveLinks: true,
	//});
	//Dropzone.autoDiscover = false;
	//$(".custom-msg-stock").dropzone({
	//	dictDefaultMessage: "Why not upload a picture or similar item of what your looking for<br /> Click here",
	//	addRemoveLinks: true,
	//});
	 } catch(e) {
        // handle an exception here if lettering doesn't exist or throws an exception
    }
}); // end document ready