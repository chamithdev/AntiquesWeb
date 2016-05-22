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
			auto: true
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
	//$('.fader-wrapper').show();
   }; 
})(jQuery);


$('#popoverBtn').click(function(e){

	$('.pop-box').show();
	//$('.fader-wrapper').show();

	e.preventDefault();
		slider2.reloadSlider({
			captions: true,
			pager:false,
			infiniteLoop:true,
			adaptiveHeight:true,
			responsive:true,
			auto: true,
			adaptiveHeight: true
	});

});

$('#popClose').click(function(){
	$('.pop-box').hide();
	$('.fader-wrapper').hide();
});

$(document).keyup(function(e) {
	if (e.keyCode == 27) { // escape key maps to keycode `27`
		$('.pop-box').hide();
		$('.fader-wrapper').hide();
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

    // slider 			
 $(".gallery:first a[rel^='prettyPhoto']").prettyPhoto({
     animation_speed: 'normal',
     slideshow: 3000,
     autoplay_slideshow: true,
     social_tools: false

 });



}); // end document ready


function ToggleCategoryList(id,textid)
{
    var found = false;
    $('#'+id+' li.show').each(function () {
        $(this).removeClass("show");
        $(this).addClass("hide");
        found = true;
    });
    
    if(!found)
    {
        $('#' + id + ' li.hide').each(function () {
            $(this).removeClass("hide");
            $(this).addClass("show");
            found = true;
        });
        $('#' + textid).html("See less");
    }else {
        $('#' + textid).html("See more");
    }
}

function getSearchData()
{
    var searchObject = new Object();
    var selectedStyles = [];
    var selectedMaterials = [];
    var selectedStylesNames = "";
    var selectedMaterialsNames = "";
    $('#ulStyleList input:checkbox').each(function () {
        if (this.checked) {
            selectedStyles.push($(this).val());
            selectedStylesNames += "," + ($(this).data("text"));
        }
            
        
    });

    $('#ulMaterialList input:checkbox').each(function () {
        if (this.checked) {
            selectedMaterials.push($(this).val());
            selectedMaterialsNames += "," + ($(this).data("text"));
        }
            

    });
    selectedStylesNames = selectedStylesNames.substring(1);
    selectedMaterialsNames = selectedMaterialsNames.substring(1);
    if (selectedStyles.length > 3)
        selectedStylesNames = "Multiple";

    if (selectedMaterials.length > 3)
        selectedMaterialsNames = "Multiple";

    var priceMin = $('#txtMinPrice').val();
    var priceMax = $('#txtMaxPrice').val();

    var widthMin = $('#hdWidthMin').val();
    var widthMax = $('#hdWidthMax').val();

    var heightMin = $('#hdHeightMin').val();
    var heightMax = $('#hdHeightMax').val();

    var color = $('#ddlColor').val();

    var designed = $('#ddlDesignBy').val();

    var circaDateFrom = $('#txtCircaDateFrom').val();
    var circaDateTo = $('#txtCircaDateTo').val();
    var catName = $('#hdCatName').val();
    searchObject.ss = selectedStyles;
    searchObject.sms = selectedMaterials;
    searchObject.c = color;
    searchObject.d = designed;
    searchObject.pm = priceMin;
    searchObject.pmx = priceMax;
    searchObject.hm = heightMin;
    searchObject.hmx = heightMax;
    searchObject.wm = widthMin;
    searchObject.wmx = widthMax;
    searchObject.cdf = circaDateFrom;
    searchObject.cdt = circaDateTo;
    searchObject.q = $('#txtq').val();
    searchObject.cid = $('#hdCatId').val();
    searchObject.pg = 0;

    var filterList = "";
    if (catName !== "")
        filterList += "Category = " + catName + '<br/>';

    if (circaDateFrom !== '' && circaDateTo !== '')
        filterList += 'Circa Date > ' + circaDateFrom + "< " + circaDateTo + '<br/>';

    if (parseFloat(priceMin) > 0 && parseFloat(priceMax) > parseFloat(priceMin))
        filterList += "Price > " + priceMin + "< " + priceMax + '<br/>';

    if (color !== "")
        filterList += "Color = " + color + '<br/>';
    if (designed !== "")
        filterList += "Designed By = " + designed + '<br/>';

    if (parseFloat(heightMin) > 0 && parseFloat(heightMax) > parseFloat(heightMin))
        filterList += "Height > " + heightMin + "< " + heightMax + '<br/>';

    if (parseFloat(widthMin) > 0 && parseFloat(widthMax) > parseFloat(widthMin))
        filterList += "Width > " + widthMin + "< " + widthMax + '<br/>';

    if (selectedStylesNames !== "")
        filterList += "Styles = " + selectedStylesNames + '<br/>';

    if (selectedMaterialsNames !== "")
        filterList += "Materials = " + selectedMaterialsNames + '<br/>';

    if (filterList === ''){
        $('#filters').html('No Filters');
        $('#clearLnk').hide();
    }        
    else
    {
        $('#filters').html(filterList);
        $('#clearLnk').show();
    }
        

    return searchObject;
}

function SearchProducts(id,name)
{
    var searchData = getSearchData();
    searchData.cid = $('#hdCatId').val(id);
    $('#hdCatName').val(name);
    pagerClick(0);

}