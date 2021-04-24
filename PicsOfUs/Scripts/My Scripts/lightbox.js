$(function () {
  const navbarDesktop = $('#navbar-desktop');
  const navbarMobile = $('#navbar-mobile');

  const lightboxContainer = $('#lightbox-container');
  const lightboxCloserButton = $('#lightbox-closer-button');
  const memberDetailsSection = $('#member-details');
  const lovePic = $('#love-pic');
  const leftArrow = $('#left-arrow');
  const rightArrow = $('#right-arrow');
  const mobileNavHeight = navbarMobile.height() +
    parseInt(navbarMobile.css('padding-top')) +
    parseInt(navbarMobile.css('padding-bottom'));

  //#region Move to site.js

  // dynamically set the height of the mobile nav buttons, which will overlay on the mobile navbar
  const mobileNavButtons = $('.mobile-nav-button');
  const mobileNavBuffers = $('.add-mobile-nav-buffer');
  // let mobileNavHeight =
  //   navbarMobile.height() +
  //   parseInt(navbarMobile.css('padding-top')) +
  //   parseInt(navbarMobile.css('padding-bottom'));
    mobileNavButtons.height(mobileNavHeight);
    mobileNavBuffers.css('padding-bottom', mobileNavHeight);

    //#endregion

  //#region Event Listener Setup

  $('#results-body').on('click', '.trigger', function () {
    const resultId = parseInt($(this).attr('data-result-id'));
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
  memberDetailsSection.on('click', '.fake-profile', function () {
    resetMemberDetails();
    openMemberDetails(parseInt($(this).attr('data-member-id')));
  });
  $('.member-details-closer').on('click', '', function () {
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

  //#region Functions

  function openLightbox(resultId) {
    lightboxContainer.removeClass('hidden');
    navbarDesktop.addClass('hidden');
    navbarMobile.addClass('hidden');
    moveToPic(resultId);
    //lightboxContainer.css('padding-bottom', mobileNavHeight);
    $('body').addClass('restrict-scroll');
  }

  function closeLightbox() {
    lightboxContainer.addClass('hidden');
    lightboxCloserButton.addClass('hidden');
    navbarDesktop.removeClass('hidden');
    navbarMobile.removeClass('hidden');
    closeMemberDetails();
    //lightboxContainer.css('padding-bottom', 0);
    $('body').removeClass('restrict-scroll');
  }

  function moveToPic(resultId) {

    lightboxContainer.scrollTop(0);

    const resultPicMaxId = parseInt(
      $('#results-body .photo-result:last-child').attr('data-result-id')
    );
    leftArrow.removeClass('hidden');
    rightArrow.removeClass('hidden');

    if (resultId === resultPicMaxId) rightArrow.addClass('hidden');
    else if (resultId === 0) leftArrow.addClass('hidden');

    const photoId = $('#results-body')
      .find(`[data-result-id=${resultId}]`)
      .attr('data-photo-id');
    const def = $.Deferred();
    def
      .then(function () {
        return $.get(`/api/photos/${photoId}`).done(function (photo) {
          console.log('photo loaded to lightbox', photo);

          lightboxContainer.attr('data-result-id', resultId);

          const pic = $('#lightbox-pic');
          pic.attr('src', photo.url);
          pic.attr('alt', photo.caption);

          $('#caption').text(photo.caption);

          const captureArea = $('#capture-date');

          if (photo.captureDate !== null) {
            const formattedDate = new Date(photo.captureDate);
            captureArea.text(formattedDate.toLocaleDateString());
            captureArea.removeClass('hidden');
          } else {
            captureArea.addClass('hidden');
          }

          // set initial loved pic val
          // toggleLovePic(!photo.isLoved);
        });
      })
      .then(function (photo) {
        return $.get('/Static/PicProfile.html').done(function (emptyProfile) {

          console.log('html loaded', emptyProfile);

          const subjectsArea = $('#pic-subjects');
          subjectsArea.html('');

          if (photo.members) {
            $.each(photo.members, function (index, member) {

              const picProfile = insertIntoProfile(member, emptyProfile, photo.captureDate);
              picProfile.appendTo(subjectsArea);
            });
          }
        });
      })
      .fail(function (error) {
        console.log('there was an error!, log to global logger', error);
      });
    def.resolve();
  }

  function toggleLovePic(isLoved) {
    isLoved = !isLoved;

    console.log('make ajax call to toggle love pic', isLoved);

    $('#love-pic').attr('data-is-loved', isLoved.toString());

    const lovedPicIcon = lovePic.children('.fa-heart');
    const lovedPicText = lovePic.children('span');

    if (isLoved) {
      lovedPicText.text('Pic Loved');
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

    console.log(`open the details section for member with id: ${memberId}`);

    const def = $.Deferred();
    def
      .then(function () {
        return $.get(`/api/members/${memberId}`).done(function (data) {
          console.log('loading member-details', data);

          memberDetailsSection.find('#member-name').text(data.name);

            const months =
                ['January', 'February', 'March', 'April', 'May', 'June', 'July',
                    'August', 'September', 'October', 'November', 'December'];
            const birthDate = new Date(data.birthDate);
            const birthMonth = months[birthDate.getMonth()];
          const birthday = birthMonth + ' ' + birthDate.getDate();
          memberDetailsSection.find('#member-birthday').text(birthday);
        });
      })
      .then(function (member) {
        return $.get('/Static/PicProfile.html').done(function (emptyProfile) {

          const parentsArea = $('#member-parents');


          $.each(member.parents, function (index, parent) {
            console.log('insert parent', parent.name);

            const newProfile = insertIntoProfile(parent, emptyProfile);
            newProfile.appendTo(parentsArea);
          });

          const siblingsArea = $('#member-siblings');

          $.each(member.siblings, function (index, sibling) {
            console.log('insert sibling', sibling.name);

            const newProfile = insertIntoProfile(sibling, emptyProfile);
            newProfile.appendTo(siblingsArea);
          });

          const childrenArea = $('#member-children');

          $.each(member.children, function (index, child) {
            console.log('insert child', child.name);

            const newProfile = insertIntoProfile(child, emptyProfile);
            newProfile.appendTo(childrenArea);
          });
        });
      })
      .fail(function (error) {
        console.log('member call failed, display message and log to global logger', error);
      })
      .always(function () {
        lightboxContainer.scrollTop(0);
        lightboxContainer.addClass('restrict-scroll');
          memberDetailsSection.removeClass('hidden');
          //memberDetailsSection.css('padding-bottom', mobileNavHeight);
      });
    def.resolve();
  }

  function closeMemberDetails() {
    memberDetailsSection.addClass('hidden');
      lightboxContainer.removeClass('restrict-scroll');
      //memberDetailsSection.css('padding-bottom', 0);
  }

  function resetMemberDetails() {
    memberDetailsSection.find('#member-parents').empty();
    memberDetailsSection.find('#member-siblings').empty();
    memberDetailsSection.find('#member-children').empty();
  }

  function insertIntoProfile(member, emptyProfile, captureDate) {
    console.log(`insertIntoProfile: member: ${member}\ncaptureDate: ${captureDate}\npicProfile: ${emptyProfile}`);
    const cloneProfile = $(emptyProfile).clone();

    cloneProfile
      .filter('.fake-profile')
      .attr('data-member-id', member.id);

    cloneProfile.find('.name').text(member.name);

    const ageArea = cloneProfile.find('.age');
    if (captureDate) {

      const captureDateClass = Date.parse(captureDate);
      const birthDateClass = Date.parse(member.birthDate);
      const timeDifferenceInMilliseconds = Math.abs(captureDateClass - birthDateClass);

      const yearsInMilliseconds = 1000 * 3600 * 24 * 365;
      const ageInYears = Math.floor(timeDifferenceInMilliseconds / yearsInMilliseconds);
      ageArea.append(ageInYears);

      ageArea.removeClass('hidden');
    }

    return cloneProfile;
  }
  //#endregion
});