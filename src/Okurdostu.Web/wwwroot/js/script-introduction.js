$("#modalLoginButton").click(function () {
    $("#signup").modal("hide"), $("#login").modal("show");
}),
    $("#modalSignupButton").click(function () {
        $("#login").modal("hide"), $("#signup").modal("show");
    }),
    (function (t) {
        "use strict";
        t(window).on("load", function () {
            var o,
                n = 500;
            (o = t(".spinner-wrapper")),
                setTimeout(function () {
                    o.fadeOut(n);
                }, 500);
        }),
            t(window).on("scroll load", function () {
                t(".navbar").offset().top > 20 ? t(".fixed-top").addClass("top-nav-collapse") : t(".fixed-top").removeClass("top-nav-collapse");
            }),
            t(function () {
                t(document).on("click", "a.page-scroll", function (o) {
                    var n = t(this);
                    t("html, body")
                        .stop()
                        .animate({ scrollTop: t(n.attr("href")).offset().top }, 600, "easeInOutExpo"),
                        o.preventDefault();
                });
            }),
            t(".navbar-nav li a").on("click", function (o) {
                t(this).parent().hasClass("dropdown") || t(".navbar-collapse").collapse("hide");
            });
        var o = 0;
        t(window).scroll(function () {
            if (t("#counter").length) {
                var n = t("#counter").offset().top - window.innerHeight;
                0 == o &&
                    t(window).scrollTop() > n &&
                    (t(".counter-value").each(function () {
                        var o = t(this),
                            n = o.attr("data-count");
                        t({ countNum: o.text() }).animate(
                            { countNum: n },
                            {
                                duration: 2e3,
                                easing: "swing",
                                step: function () {
                                    o.text(Math.floor(this.countNum));
                                },
                                complete: function () {
                                    o.text(this.countNum);
                                },
                            }
                        );
                    }),
                        (o = 1));
            }
        }),
            t("input, textarea").keyup(function () {
                "" != t(this).val() ? t(this).addClass("notEmpty") : t(this).removeClass("notEmpty");
            }),
            t(".button, a, button").mouseup(function () {
                t(this).blur();
            });
    })(jQuery);
