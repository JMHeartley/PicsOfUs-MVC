$(function () {
  let navbarDesktop = $('#navbar-desktop');
  let navbarMobile = $('#navbar-mobile');

  let lightboxContainer = $('#lightbox-container');
  let lightboxCloserButton = $('#lightbox-closer-button');
  let memberDetailsSection = $('#member-details');
  let lovePic = $('#love-pic');
  let leftArrow = $('#left-arrow');
  let rightArrow = $('#right-arrow');
  let mobileNavHeight =
    navbarMobile.height() +
    parseInt(navbarMobile.css('padding-top')) +
    parseInt(navbarMobile.css('padding-bottom'));

  //#region Move to site.js

  // dynamically set the height of the mobile nav buttons, which will overlay on the mobile navbar
  let mobileNavButtons = $('.mobile-nav-button');
  // let mobileNavHeight =
  //   navbarMobile.height() +
  //   parseInt(navbarMobile.css('padding-top')) +
  //   parseInt(navbarMobile.css('padding-bottom'));
  console.log('mobileNavHeight', mobileNavHeight);
  mobileNavButtons.height(mobileNavHeight);

  //#endregion

  //#region Event Listener Setup

  $('#results-body').on('click', '.trigger', function () {
    let resultId = parseInt($(this).attr('data-result-id'));
    openLightbox(resultId);
  });
  $('.lightbox-closer').on('click', '', function () {
    closeLightbox();
  });
  leftArrow.on('click', '', function () {
    moveToPic(parseInt(lightboxContainer.attr('data-result-id')) - 1);
  });
  rightArrow.on('click', '', function () {
    moveToPic(parseInt(lightboxContainer.attr('data-result-id')) + 1);
  });
  lovePic.on('click', '', function () {
    toggleLovePic(lovePic.attr('data-is-loved') === 'true');
  });
  $('#pic-subjects').on('click', '.fake-profile', function () {
    openMemberDetails(parseInt($(this).attr('data-member-id')));
  });
  $('#back').on('click', '', function () {
    closeMemberDetails();
  });

  lightboxContainer.on('scroll', '', function () {
    if (lightboxContainer.scrollTop() > mobileNavHeight / 2) {
      lightboxCloserButton.removeClass('hidden');
    } else {
      lightboxCloserButton.addClass('hidden');
    }
  });

  //#endregion

  //#region Lightbox Functions

  function openLightbox(resultId) {
    lightboxContainer.removeClass('hidden');
    navbarDesktop.addClass('hidden');
    navbarMobile.addClass('hidden');
    moveToPic(resultId);
    lightboxContainer.css('padding-bottom', mobileNavHeight);
    $('body').addClass('restrict-scroll');
  }

  function closeLightbox() {
    lightboxContainer.addClass('hidden');
    lightboxCloserButton.addClass('hidden');
    navbarDesktop.removeClass('hidden');
    navbarMobile.removeClass('hidden');
    closeMemberDetails();
    lightboxContainer.css('padding-bottom', 0);
    $('body').removeClass('restrict-scroll');
  }

  function moveToPic(resultId) {
    
    let resultPicMaxId = parseInt(
      $('#results-body .photo-result:last-child').attr('data-result-id')
    );
    leftArrow.removeClass('hidden');
    rightArrow.removeClass('hidden');

    if (resultId === resultPicMaxId) rightArrow.addClass('hidden');
    else if (resultId === 0) leftArrow.addClass('hidden');

    let photoId = $('#results-body')
      .find('[data-result-id=' + resultId + ']')
      .attr('data-photo-id');
    let cachedPhotoMembers;
    let cachedCaptureDate;
    let def = $.Deferred();
    def
      .then(function () {
        return $.get('/api/photos/' + photoId).done(function (photo) {
          console.log('photo loaded to lightbox', photo);
          
          lightboxContainer.attr('data-result-id', resultId);

          let pic = $('#lightbox-pic');
          pic.attr('src', photo.url);
          pic.attr('alt', photo.caption);

          $('#caption').text(photo.caption);

          let captureArea = $('#capture-date');
          let formattedDate;

          if (photo.captureDate !== null) {
            formattedDate = new Date(photo.captureDate);
            captureArea.text(formattedDate.toLocaleDateString());
            captureArea.removeClass('hidden');
          } else {
            captureArea.addClass('hidden');
            console.log(formattedDate);
          }

          console.log('photo.captureDate', formattedDate);

          cachedPhotoMembers = photo.members;
          cachedCaptureDate = formattedDate;

          // set initial loved pic val
          // toggleLovePic(!photo.isLoved);
        });
      })
      .then(function () {
        return $.get('/Static/PicProfile.html').done(function (picProfile) {
          console.log('loading photo.members', cachedPhotoMembers);
          let subjectsArea = $('#pic-subjects');
          subjectsArea.html('');

          if (cachedPhotoMembers !== undefined) {
            $.each(cachedPhotoMembers, function (index, element) {
              let newProfile = $(picProfile).clone();

              newProfile
                .filter('.fake-profile')
                .attr('data-member-id', element.id);

              newProfile.find('.name').text(element.name);

              let ageArea = newProfile.find('.age');

              if (cachedCaptureDate === undefined) {
                ageArea.addClass('hidden');
              } else {
                let birthDate = new Date(element.birthDate);
                let yearsInMilliseconds = 1000 * 3600 * 24 * 365;
                let ageInYears = Math.floor(
                  (cachedCaptureDate.getTime() - birthDate.getTime()) /
                  yearsInMilliseconds
                );
                ageArea.append(ageInYears);
              }

              newProfile.appendTo(subjectsArea);
            });
          }
        });
      })
      .fail(function (error) {
        console.log('there was an error!, log to global logger', error);
      })
      .always(function () {
        lightboxContainer.scrollTop(0);
      });
    def.resolve();
  }

  function toggleLovePic(isLoved) {
    isLoved = !isLoved;

    console.log('make ajax call to toggle love pic', isLoved);

    $('#love-pic').attr('data-is-loved', isLoved.toString());

    let lovedPicIcon = lovePic.children('.fa-heart');
    let lovedPicText = lovePic.children('span');

    if (isLoved) {
      lovedPicText.text('Loved Pic');
      lovedPicIcon.addClass('fas');
      lovedPicIcon.removeClass('far');
    } else {
      lovedPicText.text('Love Pic');
      lovedPicIcon.addClass('far');
      lovedPicIcon.removeClass('fas');
    }
    //}
  }

  function openMemberDetails(memberId) {

    console.log('open the details section for member with id: ' + memberId);
    memberDetailsSection.removeClass('hidden');

    $.ajax({
      url: '/api/members/' + memberId,
      type: 'GET',
      contentType: 'application/json',
    })
      .done(function (data) {
        console.log("loading member-details", data);

        memberDetailsSection.find('#member-name').text(data.name);

        let birthDate = new Date(data.birthDate);
        let birthday = birthDate.getMonth() + 1 + ' ' + birthDate.getDate();
        memberDetailsSection.find('#member-birthday').text(birthday);
      })
      .fail(function (error) {
        console.log('member call failed, display message and log to global logger', error);
      })
      .always(function () {
        lightboxContainer.scrollTop(0);
        lightboxContainer.addClass("restrict-scroll");
      });
  }

  function closeMemberDetails() {
    memberDetailsSection.addClass('hidden');
    lightboxContainer.removeClass("restrict-scroll");
  }

  //#endregion
});