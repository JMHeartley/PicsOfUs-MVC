let s;
const Lightbox = {
    settings: {
        isLightboxOpen: false,
        isMembersDetailsOpen: false,
        navbarDesktop: $('#navbar-desktop'),
        navbarMobile: $('#navbar-mobile'),
        lightbox: $('#lightbox'),
        closerButton: $('#lightbox-closer-button'),
        closer: $('.lightbox-closer'),
        memberDetailsSection: $('#member-details'),
        lovePic: $('#love-pic'),
        leftArrow: $('#left-arrow'),
        rightArrow: $('#right-arrow'),
        resultsBody: $('#results-body'),
        mobileNavButtons: $('.mobile-nav-button'),
        mobileNavBuffers: $('.add-mobile-nav-buffer'),
        mobileNavHeight: null
    },
    init: function () {
        s = this.settings;

        this.bindUIEvents();
        s.mobileNavHeight = s.navbarMobile.height() +
            parseInt(s.navbarMobile.css('padding-top')) +
            parseInt(s.navbarMobile.css('padding-bottom'));
        s.mobileNavButtons.height(s.mobileNavHeight);
        s.mobileNavBuffers.css('padding-bottom', s.mobileNavHeight);
    },
    bindUIEvents: function () {
        s.resultsBody.on('click', '.trigger', function () {
            const resultId = parseInt($(this).attr('data-result-id'));
            Lightbox.openLightbox(resultId);
        });

        s.closer.on('click', '', function () {
            Lightbox.closeLightbox();
        });
        $(document).on('keyup', function (event) {
            if (event.key === 'Escape') {
                if (s.isMembersDetailsOpen) {
                    Lightbox.closeMemberDetails();
                }
                else if (s.isLightboxOpen) {
                    Lightbox.closeLightbox();
                }
            }
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
            Lightbox.resetMemberDetails();
            Lightbox.openMemberDetails(parseInt($(this).attr('data-member-id')));
        });
        $('.member-details-closer').on('click', '', function () {
            Lightbox.closeMemberDetails();
        });

        s.lightbox.on('scroll', '', function () {
            if (s.lightbox.scrollTop() > s.mobileNavHeight / 2) {
                s.closerButton.removeClass('hidden');
            } else {
                s.closerButton.addClass('hidden');
            }
        });
    },
    openLightbox: function (resultId) {
        s.lightbox.removeClass('hidden');
        s.navbarDesktop.addClass('hidden');
        s.navbarMobile.addClass('hidden');
        Lightbox.moveToPic(resultId);
        //lightbox.css('padding-bottom', s.mobileNavHeight);
        $('body').addClass('restrict-scroll');
        s.isLightboxOpen = true;
    },
    closeLightbox: function () {
        s.lightbox.addClass('hidden');
        s.closerButton.addClass('hidden');
        s.navbarDesktop.removeClass('hidden');
        s.navbarMobile.removeClass('hidden');
        Lightbox.closeMemberDetails();
        //lightbox.css('padding-bottom', 0);
        $('body').removeClass('restrict-scroll');
        s.isLightboxOpen = false;
    },
    moveToPic: function (resultId) {

        s.lightbox.scrollTop(0);

        const resultPicMaxId = parseInt(
            $('#results-body .pic-result:last-child').attr('data-result-id')
        );

        s.leftArrow.removeClass('hidden');
        s.rightArrow.removeClass('hidden');
        if (resultId === resultPicMaxId)
            s.rightArrow.addClass('hidden');
        if (resultId === 0)
            s.leftArrow.addClass('hidden');

        const picId = $('#results-body')
            .find(`[data-result-id=${resultId}]`)
            .attr('data-pic-id');
        const def = $.Deferred();
        def
            .then(function () {
                return $.get(`/api/pics/${picId}`).done(function (pic) {
                    console.log('pic loaded to lightbox', pic);

                    s.lightbox.attr('data-result-id', resultId);
                    s.lightbox.attr('data-pic-id', picId);

                    const lightboxPic = $('#lightbox-pic');
                    lightboxPic.attr('src', pic.url);
                    lightboxPic.attr('alt', pic.caption);

                    $('#caption').text(pic.caption);

                    const captureArea = $('#capture-date');

                    if (pic.captureDate !== null) {
                        const formattedDate = new Date(pic.captureDate);
                        captureArea.text(formattedDate.toLocaleDateString());
                        captureArea.removeClass('hidden');
                    } else {
                        captureArea.addClass('hidden');
                    }

                    Lightbox.setLovedPic(pic.isLoved);
                });
            })
            .then(function (pic) {
                return $.get('/Static/PicProfile.html').done(function (emptyProfile) {

                    console.log('html loaded', emptyProfile);

                    const subjectsArea = $('#pic-subjects');
                    subjectsArea.html('');

                    if (pic.subjects) {
                        $.each(pic.subjects, function (index, subject) {

                            const picProfile = Lightbox.insertIntoProfile(subject, emptyProfile, pic.captureDate);
                            picProfile.appendTo(subjectsArea);
                        });
                    }
                });
            })
            .fail(function (error) {
                console.log('there was an error!, log to global logger', error);
            });
        def.resolve();
    },
    openMemberDetails: function (memberId) {

        console.log(`open the details section for member with id: ${memberId}`);

        const def = $.Deferred();
        def
            .then(function () {
                return $.get(`/api/members/${memberId}`).done(function (data) {
                    console.log('loading member-details', data);

                    s.memberDetailsSection.find('#member-name').text(data.name);

                    const months =
                        ['January', 'February', 'March', 'April', 'May', 'June', 'July',
                            'August', 'September', 'October', 'November', 'December'];
                    const birthDate = new Date(data.birthDate);
                    const birthMonth = months[birthDate.getMonth()];
                    const birthday = birthMonth + ' ' + birthDate.getDate();
                    s.memberDetailsSection.find('#member-birthday').text(birthday);
                });
            })
            .then(function (member) {
                return $.get('/Static/PicProfile.html').done(function (emptyProfile) {

                    const parentsArea = $('#member-parents');

                    $.each(member.parents, function (index, parent) {
                        console.log('insert parent', parent.name);

                        const newProfile = Lightbox.insertIntoProfile(parent, emptyProfile);
                        newProfile.appendTo(parentsArea);
                    });

                    const siblingsArea = $('#member-siblings');

                    $.each(member.siblings, function (index, sibling) {
                        console.log('insert sibling', sibling.name);

                        const newProfile = Lightbox.insertIntoProfile(sibling, emptyProfile);
                        newProfile.appendTo(siblingsArea);
                    });

                    const childrenArea = $('#member-children');

                    $.each(member.children, function (index, child) {
                        console.log('insert child', child.name);

                        const newProfile = Lightbox.insertIntoProfile(child, emptyProfile);
                        newProfile.appendTo(childrenArea);
                    });
                });
            })
            .then(s.isMembersDetailsOpen = true)
            .fail(function (error) {
                console.log('member call failed, display message and log to global logger', error);
            })
            .always(function () {
                s.lightbox.scrollTop(0);
                s.lightbox.addClass('restrict-scroll');
                s.memberDetailsSection.removeClass('hidden');
                //memberDetailsSection.css('padding-bottom', s.mobileNavHeight);
                console.log("open?", s.isMembersDetailsOpen);
            });
        def.resolve();
    },
    closeMemberDetails: function () {
        s.memberDetailsSection.addClass('hidden');
        s.lightbox.removeClass('restrict-scroll');
        //memberDetailsSection.css('padding-bottom', 0);
        s.isMembersDetailsOpen = false;
        console.log('open?', s.isMembersDetailsOpen);
    },
    resetMemberDetails: function () {
        s.memberDetailsSection.find('#member-parents').empty();
        s.memberDetailsSection.find('#member-siblings').empty();
        s.memberDetailsSection.find('#member-children').empty();
    },
    insertIntoProfile: function (member, emptyProfile, picCaptureDate) {
        console.log(`insertIntoProfile: member: ${member}\ncaptureDate: ${picCaptureDate}\npicProfile: ${emptyProfile}`);
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
    toggleLovePic: function (isLoved) {

        isLoved = !isLoved;

        let def = $.Deferred();
        def
            .then(function () {
                return $.ajax({
                    url: "/api/pics/" + s.lightbox.attr("data-pic-id"),
                    type: "PATCH",
                    contentType: "application/json",
                    data: JSON.stringify({
                        IsLoved: isLoved
                    })
                });
            })
            .then(function () {
                Lightbox.setLovedPic(isLoved)
            })
            .fail(function (error) {
                console.log("An error occurred", error);
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
    }
};

$(function () { Lightbox.init(); });