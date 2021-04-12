$(function () {
    // pre-load needed static HTML components via ajax
    let lightbox = $("#lightbox");
    let lightboxContainer = $("#lightbox-container");
    let lightboxCloserButton = $("#lightbox-closer-button");
    let memberDetailsSection = $('#member-details');
    let lovePic = $("#love-pic");
    // dynamically set the height of the mobile nav buttons, which will overlay on the mobile navbar
    // let mobileNavButtons = $(".mobile-nav-button");
    // let mobileNavbar = $("#mobile-navbar");
    // mobileNavButtons.css('height', mobileNavbar.css('height'));
    //#region Event Listener Setup
    // convert to listener on results body that filters by result-pic class
    //  therefore 1 listener
    $(".trigger").on('click', '', function () {
        let resultId = parseInt($(this).attr('data-result-id'));
        OpenLightbox(resultId);
    });
    $(".lightbox-closer").on('click', '', function () {
        CloseLightbox();
    });
    $('#left-arrow').on('click', '', function () {
        MoveToPic(parseInt(lightboxContainer.attr("data-result-id")) - 1);
    });
    $('#right-arrow').on('click', '', function () {
        MoveToPic(parseInt(lightboxContainer.attr("data-result-id")) + 1);
    });
    lovePic.on('click', '', function () {
        ToggleLovePic(lovePic.attr('data-is-loved') === 'true');
    });
    $("#pic-subjects").on('click', '.fake-profile', function () {
        OpenMemberDetails(parseInt($(this).attr('data-member-id')));
    });
    $("#back").on('click', '', function () {
        CloseMemberDetails();
    });
    lightboxContainer.on('scroll', '', function () {
        if (lightboxContainer.scrollTop() > 10) {
            lightboxCloserButton.removeClass('hidden');
        }
        else {
            lightboxCloserButton.addClass('hidden');
        }
    });
    //#endregion
    //#region Lightbox Functions
    function OpenLightbox(resultId) {
        // insert lightbox into DOM with $('body').append()
        lightboxContainer.scrollTop(0);
        lightboxContainer.removeClass("hidden");
        MoveToPic(resultId);
        $('body').addClass('restrict-scroll');
    }
    function CloseLightbox() {
        // 1. replace this line with code that removes lightbox from the DOM
        lightboxContainer.addClass("hidden");
        lightboxCloserButton.addClass("hidden");
        CloseMemberDetails();
        $('body').removeClass('restrict-scroll');
    }
    function MoveToPic(resultId) {
        alert('populate lightbox with data from picture with data-result-id: ' + resultId);
        let photoId = $('#results-body').find('[data-result-id=' + resultId + ']').attr('data-photo-id');
        alert('photoId: ' + photoId);
        let def = $.Deferred();
        let cachedPhotoMembers;
        let cachedCaptureDate;
        // make ajax call for photo data using URL query string
        def
            .then(function () {
                // get photoId using jQuery to select element using data-resutlId attribute
                return $.get('/api/photos/' + photoId).done(function (photo) {
                console.log('call to retrieve photo with id: ' + photo.id);
                // populate form
                console.log(photo);
                //      update lightbox-container-result-id attr
                lightboxContainer.attr("data-result-id", resultId);
                // lightbox-pic
                let pic = $('#lightbox-pic');
                pic.attr('src', photo.url);
                pic.attr('alt', photo.caption);
                // lightbox-details
                //     caption
                $("#caption").text(photo.caption);
                //     capture date
                let formattedDate = new Date(photo.captureDate);
                $("#capture-date").text(formattedDate.toLocaleDateString());
                //     love-pic (isLoved)
                //      set intital loved pic val
                //      ToggleLovePic(!actualValue);
                // caching for next call
                cachedPhotoMembers = photo.members;
                cachedCaptureDate = formattedDate;
            });
        })
            .then(function () {
            //  get profile html
            return $.get('/Static/PicProfile.html').done(function (picProfile) {
                console.log('second call made');
                let subjectsArea = $('#pic-subjects');
                subjectsArea.html("");
                $.each(cachedPhotoMembers, function (index, element) {
                    let newProfile = $(picProfile).clone();
                    $(newProfile).filter('.fake-profile').attr('data-member-id', element.id);
                    $(newProfile).find('.name').text(element.name);
                    let birthDate = new Date(element.birthDate);
                    let yearsInMilliseconds = (1000 * 3600 * 24 * 365);
                    let ageInYears = Math.floor((cachedCaptureDate.getTime() - birthDate.getTime())
                        / yearsInMilliseconds);
                    console.log('age: ' + ageInYears);
                    //$(newProfile).find('.age').text(ageInYears);
                    $(newProfile).appendTo(subjectsArea);
                });
            });
        })
            .fail(function (error) {
            alert('there was an error!');
            console.error(error);
            // if error display message and log to global logger
        })
            .always(function () {
            console.log('always');
        });
        def.resolve();
    }
    function ToggleLovePic(isLoved) {
        isLoved = !isLoved;
        // 1. make ajax call
        alert('make ajax call to toggle love pic! isLoved: ' + isLoved);
        //    a. if sucessful change icon and text
        //{
        $("#love-pic").attr('data-is-loved', isLoved.toString());
        let text;
        if (isLoved) {
            text = 'Loved Pic';
            lovePic.children('.fa-heart').addClass('fas');
            lovePic.children('.fa-heart').removeClass('far');
        }
        else {
            text = 'Love Pic';
            lovePic.children('.fa-heart').addClass('far');
            lovePic.children('.fa-heart').removeClass('fas');
        }
        lovePic.children('span').text(text);
        //}
        //    b. if error display message and log to global logger
    }
    function OpenMemberDetails(memberId) {
        alert('open the details section for member with id: ' + memberId);
        // 2. show member section 
        memberDetailsSection.removeClass("hidden");
        // 3. make ajax call for member data
        $.ajax({
            url: '/api/members/' + memberId,
            type: 'GET',
            contentType: 'application/json',
        })
            .done(function (data) {
                alert('member retrieved!');
            })
            .fail(function (error) {
                alert('there was an error');
                switch (error.status) {
                    case 404:
                        console.log(error.responseText);
                        break;
                    case 500:
                        console.log(error.responseText);
                        break;
                    default:
                        console.log(error);
                        break;
                }
            })
            .always(function () {
                alert('api call is finished!');
            });
        //   a. if successful populate member details section with data
        lightboxContainer.scrollTop(0);
        //   b. if error display message and log to global logger
    }
    function CloseMemberDetails() {
        memberDetailsSection.addClass("hidden");
    }
    //#endregion
});
