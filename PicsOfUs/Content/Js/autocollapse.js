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

        Autocollapse.autocollapse();
    },
    bindUIEvents: function () {
        $(window).on('resize', Autocollapse.autocollapse);
    },
    autocollapse: function () {
        if ($(window).width() < Autocollapse.settings.SmBreakpoint) {
            // extra small screens
            Autocollapse.hide(Autocollapse.settings.Sm);
            Autocollapse.hide(Autocollapse.settings.Md);
            Autocollapse.hide(Autocollapse.settings.Lg);
            Autocollapse.hide(Autocollapse.settings.Xl);
        }
        else if ($(window).width() >= Autocollapse.settings.SmBreakpoint && $(window).width() < Autocollapse.settings.MdBreakpoint) {
            // small screens
            Autocollapse.show(Autocollapse.settings.Sm);
            Autocollapse.hide(Autocollapse.settings.Md);
            Autocollapse.hide(Autocollapse.settings.Lg);
            Autocollapse.hide(Autocollapse.settings.Xl);
        }
        else if ($(window).width() >= Autocollapse.settings.MdBreakpoint && $(window).width() < Autocollapse.settings.LgBreakpoint) {
            // medium screens
            Autocollapse.show(Autocollapse.settings.Sm);
            Autocollapse.show(Autocollapse.settings.Md);
            Autocollapse.hide(Autocollapse.settings.Lg);
            Autocollapse.hide(Autocollapse.settings.Xl);
        }
        else if ($(window).width() >= Autocollapse.settings.LgBreakpoint && $(window).width() < Autocollapse.settings.XlBreakpoint) {
            // large screens
            Autocollapse.show(Autocollapse.settings.Sm);
            Autocollapse.show(Autocollapse.settings.Md);
            Autocollapse.show(Autocollapse.settings.Lg);
            Autocollapse.hide(Autocollapse.settings.Xl);

        }
        else if ($(window).width() >= Autocollapse.settings.XlBreakpoint) {
            // large screens
            Autocollapse.show(Autocollapse.settings.Sm);
            Autocollapse.show(Autocollapse.settings.Md);
            Autocollapse.show(Autocollapse.settings.Lg);
            Autocollapse.show(Autocollapse.settings.Xl);
        }
    },
    hide: function (setting) {
        if (setting) {
            setting.collapse('hide');
        }
    },
    show: function (setting) {
        if (setting) {
            setting.collapse('show');
        }
    }
};

$( Autocollapse.initialize() );