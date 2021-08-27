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
        lightboxPic: $('#lightbox-pic'),
        lightboxCaption: $('#caption'),
        captureDateArea: $('#capture-date'),
        subjectsArea: $('#pic-subjects'),
        loader: $('#loader'),
        resultPicMaxId: 0,
        editButton: $('#lightbox-edit'),
        deleteButton: $('#lightbox-delete')
    },
    initialize: function () {
        this.calculateResultPicMaxId();
        this.bindUIEvents();
    },
    bindUIEvents: function () {
        Lightbox.settings.resultsBody.on('click', '.trigger', function () {
            const resultId = parseInt($(this).attr('data-result-id'));
            Lightbox.open(resultId);
        });
        $(document).on('keyup', function (event) {
            if (event.key === 'Escape') {
                Lightbox.closeActiveWindow();
            }
        });
        Lightbox.settings.closerX.on('click', '', function () {
            Lightbox.closeActiveWindow();
        });
        Lightbox.settings.leftArrow.on('click', '', function () {
            Lightbox.moveToPic(parseInt(Lightbox.settings.lightbox.attr('data-result-id')) - 1);
        });
        Lightbox.settings.rightArrow.on('click', '', function () {
            Lightbox.moveToPic(parseInt(Lightbox.settings.lightbox.attr('data-result-id')) + 1);
        });
        Lightbox.settings.lovePic.on('click', '', function () {
            Lightbox.toggleLovePic(Lightbox.settings.lovePic.attr('data-is-loved') === 'true');
        });
        Lightbox.settings.subjectsArea.on('click', '.mini-profile', function () {
            Lightbox.openMemberDetails(parseInt($(this).attr('data-member-id')));
        });
        Lightbox.settings.memberDetailsSection.on('click', '.mini-profile', function () {
            Lightbox.openMemberDetails(parseInt($(this).attr('data-member-id')));
        });
        $('.member-details-closer').on('click', '', function () {
            Lightbox.closeMemberDetails();
        });
        Lightbox.settings.editButton.on('click', '', function () {
            Lightbox.edit(Lightbox.settings.lightbox.attr('data-pic-id'));
        });
        Lightbox.settings.deleteButton.on('click', '', function () {
            Lightbox.delete(Lightbox.settings.lightbox.attr('data-pic-id'));
        });
    },
    closeActiveWindow: function () {
        if (Lightbox.settings.isMembersDetailsOpen) {
            Lightbox.closeMemberDetails();
        }
        else if (Lightbox.settings.isLightboxOpen) {
            Lightbox.close();
        }
    },
    open: function (resultId) {
        Lightbox.reset();
        Lightbox.moveToPic(resultId);
        Lightbox.settings.lightbox.removeClass('hidden');
        Lightbox.settings.navbarDesktop.addClass('hidden');
        Lightbox.settings.navbarMobile.addClass('hidden');
        $('body').addClass('restrict-scroll');
        Lightbox.settings.isLightboxOpen = true;
    },
    close: function () {
        Lightbox.settings.lightbox.addClass('hidden');
        Lightbox.settings.navbarDesktop.removeClass('hidden');
        Lightbox.settings.navbarMobile.removeClass('hidden');
        Lightbox.closeMemberDetails();
        $('body').removeClass('restrict-scroll');
        Lightbox.settings.isLightboxOpen = false;
    },
    reset: function () {
        Lightbox.settings.lightboxPic.attr('src', '');
        Lightbox.settings.lightboxPic.attr('alt', '');
        Lightbox.settings.lightboxCaption.text('');
        Lightbox.settings.captureDateArea.text('');
        Lightbox.setLovedPic(false);
        Lightbox.settings.subjectsArea.html('');
    },
    moveToPic: function (resultId) {

        Lightbox.settings.lightbox.scrollTop(0);

        Lightbox.showArrowsBasedOnResultId(resultId);

        const picId = Lightbox.settings.resultsBody
            .find(`[data-result-id=${resultId}]`)
            .attr('data-pic-id');
        const def = $.Deferred();
        def
            .then(function () {
                return $.get(`/api/pics/${picId}`).done(function (pic) {
                    Lightbox.settings.lightbox.attr('data-result-id', resultId);
                    Lightbox.settings.lightbox.attr('data-pic-id', picId);

                    Lightbox.settings.lightboxPic.attr('src', pic.url);
                    Lightbox.settings.lightboxPic.attr('alt', pic.caption);

                    Lightbox.settings.lightboxCaption.text(pic.caption);

                    if (pic.captureDate) {
                        const formattedDate = new Date(pic.captureDate);
                        Lightbox.settings.captureDateArea.text(formattedDate.toLocaleDateString());
                        Lightbox.settings.captureDateArea.removeClass('hidden');
                    } else {
                        Lightbox.settings.captureDateArea.addClass('hidden');
                    }

                    Lightbox.setLovedPic(pic.isLoved);
                });
            })
            .then(function (pic) {
                return $.get('/Static/PicProfile.html').done(function (emptyProfile) {
                    Lightbox.settings.subjectsArea.html('');
                    console.log('html loaded', emptyProfile);

                    if (pic.subjects) {
                        $.each(pic.subjects, function (index, subject) {

                            const picProfile = Lightbox.insertMemberIntoProfile(subject, emptyProfile, pic.captureDate);
                            picProfile.appendTo(Lightbox.settings.subjectsArea);
                        });
                    }
                });
            })
            .then(function () { Lightbox.settings.loader.addClass('hidden'); })
            .fail(function (error) {
                console.log('there was an error!, log to global logger', error);
                if (confirm('Something went wrong... reload the pic?')) {
                    Lightbox.moveToPic(resultId);
                } else {
                    Lightbox.close();
                }
            });
        Lightbox.settings.loader.removeClass('hidden');
        def.resolve();
    },
    fillInTheMembersDetails: function (member) {
        console.log('loading member-details', member);

        Lightbox.settings.memberDetailsSection.find('#member-name').text(member.name);

        const months =
            ['January', 'February', 'March', 'April', 'May', 'June', 'July',
                'August', 'September', 'October', 'November', 'December'];
        const birthDate = new Date(member.birthDate);
        const birthMonth = months[birthDate.getMonth()];
        const birthday = birthMonth + ' ' + birthDate.getDate();
        Lightbox.settings.memberDetailsSection.find('#member-birthday').text(birthday);
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
                Lightbox.settings.loader.addClass('hidden');
                Lightbox.settings.isMembersDetailsOpen = true;
                Lightbox.settings.memberDetailsSection.removeClass('hidden');
                Lightbox.settings.lightbox.addClass('restrict-scroll');
                Lightbox.settings.lightbox.scrollTop(0);
            })
            .fail(function (error) {
                console.log('Member call failed', error);
                if (confirm('Something went wrong :( Do you want to try again?')) {
                    Lightbox.openMemberDetails(memberId);
                } else {
                    Lightbox.closeMemberDetails();
                    Lightbox.settings.loader.addClass('hidden');
                }
            });
        Lightbox.settings.loader.removeClass('hidden');
        def.resolve();
    },
    closeMemberDetails: function () {
        Lightbox.settings.memberDetailsSection.addClass('hidden');
        Lightbox.settings.lightbox.removeClass('restrict-scroll');
        Lightbox.settings.isMembersDetailsOpen = false;
    },
    resetMemberDetails: function () {
        Lightbox.settings.memberDetailsSection.find('#member-parents').empty();
        Lightbox.settings.memberDetailsSection.find('#member-siblings').empty();
        Lightbox.settings.memberDetailsSection.find('#member-children').empty();
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
                    url: `/api/pics/${Lightbox.settings.lightbox.attr('data-pic-id')}`,
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
        Lightbox.settings.lovePic.attr('data-is-loved', isLoved.toString());

        const lovedPicIcon = Lightbox.settings.lovePic.children('.fa-heart');
        const lovedPicText = Lightbox.settings.lovePic.children('span');

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
            Lightbox.settings.resultPicMaxId = value > Lightbox.settings.resultPicMaxId ? value : Lightbox.settings.resultPicMaxId;
        });
    },
    showArrowsBasedOnResultId: function (resultId) {
        console.log('resultId', resultId);
        console.log('resultPicMaxId', Lightbox.settings.resultPicMaxId);
        console.log('all results', $('#results-body .result-pic'));

        Lightbox.settings.leftArrow.removeClass('hidden');
        Lightbox.settings.rightArrow.removeClass('hidden');
        if (resultId === 0)
            Lightbox.settings.leftArrow.addClass('hidden');
        if (resultId === Lightbox.settings.resultPicMaxId)
            Lightbox.settings.rightArrow.addClass('hidden');
    },
    edit: function (picId) {
        window.location.href = `/Browse/Edit/${picId}`;
    },
    delete: function (picId) {
        if (confirm("Are you sure you want to delete this pic?")) {
            let def = $.Deferred();
            def
                .then(function () {
                    return $.ajax({
                        url: `/api/pics/${picId}`,
                        type: 'DELETE'
                    }).done(function () {
                        const searchFormSubmitButton = $('#search-form button[type="submit"]')[0];
                        searchFormSubmitButton.click();
                    });
                })
                .fail(function (error) {
                    console.log('Pic deletion failed!', error);
                    alert(`The pic wasn't deleted!`);
                });
            def.resolve();
        }
    }
};

$( Lightbox.initialize() );