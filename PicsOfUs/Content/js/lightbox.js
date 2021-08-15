let s;
const Lightbox = {
    settings: {
        isLightboxOpen: false,
        isMembersDetailsOpen: false,
        navbarDesktop: $('#navbar-desktop'),
        navbarMobile: $('#navbar-mobile'),
        lightbox: $('#lightbox'),
        closerX: $('#lightbox-closer-x'),
        memberDetailsSection: $('#member-details'),
        lovePic: $('#love-pic'),
        leftArrow: $('#left-arrow'),
        rightArrow: $('#right-arrow'),
        resultsBody: $('#results-body'),
        mobileNavHeight: null,
        lightboxPic: $('#lightbox-pic'),
        lightboxCaption: $('#caption'),
        captureDateArea: $('#capture-date'),
        subjectsArea: $('#pic-subjects'),
        loader: $('#loader'),
        resultPicMaxId: 0
    },
    initialize: function () {
        s = this.settings;
        this.calculateResultPicMaxId();
        this.bindUIEvents();
    },
    bindUIEvents: function () {

        s.resultsBody.on('click', '.trigger', function () {
            const resultId = parseInt($(this).attr('data-result-id'));
            Lightbox.openLightbox(resultId);
        });
        $(document).on('keyup', function (event) {
            if (event.key === 'Escape') {
                Lightbox.closeMemberDetailsOrLightbox();
            }
        });
        s.closerX.on('click', '', function () {
            Lightbox.closeMemberDetailsOrLightbox();
        });
        s.leftArrow.on('click', '', function () {
            Lightbox.moveToPic(parseInt(s.lightbox.attr('data-result-id')) - 1);
        });
        s.rightArrow.on('click', '', function () {
            Lightbox.moveToPic(parseInt(s.lightbox.attr('data-result-id')) + 1);
        });
        s.lovePic.on('click', '', function () {
            Lightbox.toggleLovePic(s.lovePic.attr('data-is-loved') === 'true');
        });
        $('#pic-subjects').on('click', '.mini-profile', function () {
            Lightbox.openMemberDetails(parseInt($(this).attr('data-member-id')));
        });
        s.memberDetailsSection.on('click', '.mini-profile', function () {
            Lightbox.openMemberDetails(parseInt($(this).attr('data-member-id')));
        });
        $('.member-details-closer').on('click', '', function () {
            Lightbox.closeMemberDetails();
        });
    },
    closeMemberDetailsOrLightbox: function () {
        if (s.isMembersDetailsOpen) {
            Lightbox.closeMemberDetails();
        }
        else if (s.isLightboxOpen) {
            Lightbox.closeLightbox();
        }
    },
    openLightbox: function (resultId) {
        Lightbox.resetLightbox();
        Lightbox.moveToPic(resultId);
        s.lightbox.removeClass('hidden');
        s.navbarDesktop.addClass('hidden');
        s.navbarMobile.addClass('hidden');
        $('body').addClass('restrict-scroll');
        s.isLightboxOpen = true;
    },
    closeLightbox: function () {
        s.lightbox.addClass('hidden');
        s.navbarDesktop.removeClass('hidden');
        s.navbarMobile.removeClass('hidden');
        Lightbox.closeMemberDetails();
        $('body').removeClass('restrict-scroll');
        s.isLightboxOpen = false;
    },
    resetLightbox: function () {
        s.lightboxPic.attr('src', '');
        s.lightboxPic.attr('alt', '');
        s.lightboxCaption.text('');
        s.captureDateArea.text('');
        Lightbox.setLovedPic(false);
        s.subjectsArea.html('');
    },
    moveToPic: function (resultId) {

        s.lightbox.scrollTop(0);

        Lightbox.showArrowsBasedOnResultId(resultId);

        const picId = s.resultsBody
            .find(`[data-result-id=${resultId}]`)
            .attr('data-pic-id');
        const def = $.Deferred();
        def
            .then(function () {
                return $.get(`/api/pics/${picId}`).done(function (pic) {

                    s.lightbox.attr('data-result-id', resultId);
                    s.lightbox.attr('data-pic-id', picId);

                    s.lightboxPic.attr('src', pic.url);
                    s.lightboxPic.attr('alt', pic.caption);

                    s.lightboxCaption.text(pic.caption);

                    if (pic.captureDate) {
                        const formattedDate = new Date(pic.captureDate);
                        s.captureDateArea.text(formattedDate.toLocaleDateString());
                        s.captureDateArea.removeClass('hidden');
                    } else {
                        s.captureDateArea.addClass('hidden');
                    }

                    Lightbox.setLovedPic(pic.isLoved);
                });
            })
            .then(function (pic) {
                return $.get('/Static/PicProfile.html').done(function (emptyProfile) {
                    s.subjectsArea.html('');
                    console.log('html loaded', emptyProfile);

                    if (pic.subjects) {
                        $.each(pic.subjects, function (index, subject) {

                            const picProfile = Lightbox.insertMemberIntoProfile(subject, emptyProfile, pic.captureDate);
                            picProfile.appendTo(s.subjectsArea);
                        });
                    }
                });
            })
            .then(function () { s.loader.addClass('hidden'); })
            .fail(function (error) {
                console.log('there was an error!, log to global logger', error);
                if (confirm('Something went wrong... reload the pic?')) {
                    Lightbox.moveToPic(resultId);
                } else {
                    Lightbox.closeLightbox();
                }
            });
        s.loader.removeClass('hidden');
        def.resolve();
    },
    fillInTheMembersDetails: function (member) {
        console.log('loading member-details', member);

        s.memberDetailsSection.find('#member-name').text(member.name);

        const months =
            ['January', 'February', 'March', 'April', 'May', 'June', 'July',
                'August', 'September', 'October', 'November', 'December'];
        const birthDate = new Date(member.birthDate);
        const birthMonth = months[birthDate.getMonth()];
        const birthday = birthMonth + ' ' + birthDate.getDate();
        s.memberDetailsSection.find('#member-birthday').text(birthday);
    },
    fillInRelativeInfo: function (member, emptyProfile) {
        console.log('member', member);
        console.log('emptyProfile', emptyProfile);
        const parentsArea = $('#member-parents');
        Lightbox.insertProfilesIntoArea(member.parents, emptyProfile, parentsArea);

        const siblingsArea = $('#member-siblings');
        Lightbox.insertProfilesIntoArea(member.siblings, emptyProfile, siblingsArea);

        const childrenArea = $('#member-children');
        Lightbox.insertProfilesIntoArea(member.children, emptyProfile, childrenArea);
    },
    openMemberDetails: function (memberId) {
        Lightbox.resetMemberDetails();

        console.log(`open the details section for member with id: ${memberId}`);

        const def = $.Deferred();
        def
            .then(function () {
                return $.get(`/api/members/${memberId}`).done(Lightbox.fillInTheMembersDetails);
            })
            .then(function (member) {
                return $.get('/Static/PicProfile.html').done(function (emptyProfile) {
                    Lightbox.fillInRelativeInfo(member, emptyProfile);
                });
            })
            .then(function () {
                s.loader.addClass('hidden');
                s.isMembersDetailsOpen = true;
                s.memberDetailsSection.removeClass('hidden');
                s.lightbox.addClass('restrict-scroll');
                s.lightbox.scrollTop(0);
            })
            .fail(function (error) {
                console.log('member call failed, display message and log to global logger', error);
                if (confirm('Something went wrong... reload?')) {
                    Lightbox.openMemberDetails(memberId);
                } else {
                    Lightbox.closeMemberDetails();
                    s.loader.addClass('hidden');
                }
            });
        s.loader.removeClass('hidden');
        def.resolve();
    },
    closeMemberDetails: function () {
        s.memberDetailsSection.addClass('hidden');
        s.lightbox.removeClass('restrict-scroll');
        s.isMembersDetailsOpen = false;
    },
    resetMemberDetails: function () {
        s.memberDetailsSection.find('#member-parents').empty();
        s.memberDetailsSection.find('#member-siblings').empty();
        s.memberDetailsSection.find('#member-children').empty();
    },
    insertMemberIntoProfile: function (member, emptyProfile, picCaptureDate) {

        const memberProfile = $(emptyProfile).clone();
        memberProfile.filter('.mini-profile').attr('data-member-id', member.id);
        memberProfile.find('.name').text(member.name);

        if (picCaptureDate) {

            const ageInMilliseconds = Math.abs(Date.parse(picCaptureDate) - Date.parse(member.birthDate));
            const oneYearInMilliseconds = 1000 * 3600 * 24 * 365;
            const ageInYears = Math.floor(ageInMilliseconds / oneYearInMilliseconds);

            const ageArea = memberProfile.find('.age');
            ageArea.append(ageInYears);
            ageArea.removeClass('hidden');
        }
        return memberProfile;
    },
    insertProfilesIntoArea: function (members, emptyProfile, area) {
        $.each(members, function (index, member) {
            const newProfile = Lightbox.insertMemberIntoProfile(member, emptyProfile);
            newProfile.appendTo(area);
        });
    },
    toggleLovePic: function (isLoved) {

        isLoved = !isLoved;

        const def = $.Deferred();
        def
            .then(function () {
                return $.ajax({
                    url: `/api/pics/${s.lightbox.attr('data-pic-id')}`,
                    type: 'PATCH',
                    contentType: 'application/json',
                    data: JSON.stringify({ IsLoved: isLoved })
                });
            })
            .then(function () {
                Lightbox.setLovedPic(isLoved);
            })
            .fail(function (error) {
                console.log('An error occurred', error);
            });
        def.resolve();
    },
    setLovedPic: function (isLoved) {
        s.lovePic.attr('data-is-loved', isLoved.toString());

        const lovedPicIcon = s.lovePic.children('.fa-heart');
        const lovedPicText = s.lovePic.children('span');

        if (isLoved) {
            lovedPicText.text('Loved');
            lovedPicIcon.addClass('fas');
            lovedPicIcon.removeClass('far');
        } else {
            lovedPicText.text('Love');
            lovedPicIcon.addClass('far');
            lovedPicIcon.removeClass('fas');
        }
    },
    calculateResultPicMaxId: function () {
        const resultPics = $('#results-body .result-pic');

        resultPics.each(function () {
            const value = parseInt($(this).attr('data-result-id'));
            s.resultPicMaxId = value > s.resultPicMaxId ? value : s.resultPicMaxId;
        });
    },
    showArrowsBasedOnResultId: function (resultId) {
        console.log('resultId', resultId);
        console.log('resultPicMaxId', s.resultPicMaxId);
        console.log('all results', $('#results-body .result-pic'));

        s.leftArrow.removeClass('hidden');
        s.rightArrow.removeClass('hidden');
        if (resultId === 0)
            s.leftArrow.addClass('hidden');
        if (resultId === s.resultPicMaxId)
            s.rightArrow.addClass('hidden');
    }
};

$(function () { Lightbox.initialize(); });