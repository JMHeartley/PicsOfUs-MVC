const Autocollapse = {
    settings: {
        SmBreakpoint: 576,
        MdBreakpoint: 768,
        LgBreakpoint: 992,
        XlBreakpoint: 1200,
        Sm: $('[data-autocollapse="sm"]'),
        Md: $('[data-autocollapse="md"]'),
        Lg: $('[data-autocollapse="lg"]'),
        Xl: $('[data-autocollapse="xl"]')
    },
    initialize: function () {
        this.bindUIEvents();

        Autocollapse.collapse();
    },
    bindUIEvents: function () {
        $(window).on('resize', Autocollapse.collapse);
    },
    collapse: function () {
        if ($(window).width() < Autocollapse.settings.SmBreakpoint) {
            // extra small screens
            Autocollapse.collapseSm();
            Autocollapse.collapseMd();
            Autocollapse.collapseLg();
        }
        else if ($(window).width() >= Autocollapse.settings.SmBreakpoint && $(window).width() < Autocollapse.settings.MdBreakpoint) {
            // small screens
            Autocollapse.showSm();
            Autocollapse.collapseMd();
            Autocollapse.collapseLg();
            Autocollapse.collapseXl();
        }
        else if ($(window).width() >= Autocollapse.settings.MdBreakpoint && $(window).width() < Autocollapse.settings.LgBreakpoint) {
            // medium screens
            Autocollapse.showSm();
            Autocollapse.showMd()
            Autocollapse.collapseLg();
            Autocollapse.collapseXl();

        }
        else if ($(window).width() >= Autocollapse.settings.LgBreakpoint && $(window).width() < Autocollapse.settings.XlBreakpoint) {
            // large screens
            Autocollapse.showSm();
            Autocollapse.showMd()
            Autocollapse.showLg()
            Autocollapse.collapseXl();
        }
        else if ($(window).width() >= Autocollapse.settings.XlBreakpoint) {
            // large screens
            Autocollapse.showSm();
            Autocollapse.showMd()
            Autocollapse.showLg()
            Autocollapse.showXl()
        }
    },
    collapseSm: function () {
        if (Autocollapse.settings.Sm) {
            Autocollapse.settings.Sm.collapse('hide');
        }
    },
    showSm: function () {
        if (Autocollapse.settings.Sm) {
            Autocollapse.settings.Sm.collapse('show');
        }
    },
    collapseMd: function () {
        if (Autocollapse.settings.Md) {
            Autocollapse.settings.Md.collapse('hide');
        }
    },
    showMd: function () {
        if (Autocollapse.settings.Md) {
            Autocollapse.settings.Md.collapse('show');
        }
    },
    collapseLg: function () {
        if (Autocollapse.settings.Lg) {
            Autocollapse.settings.Lg.collapse('hide');
        }
    },
    showLg: function () {
        if (Autocollapse.settings.Lg) {
            Autocollapse.settings.Lg.collapse('show');
        }
    },
    collapseXl: function () {
        if (Autocollapse.settings.Xl) {
            Autocollapse.settings.Xl.collapse('hide');
        }
    },
    showXl: function () {
        if (Autocollapse.settings.Xl) {
            Autocollapse.settings.Xl.collapse('show');
        }
    }
};

$(function () { Autocollapse.initialize(); });