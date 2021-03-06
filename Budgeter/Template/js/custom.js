$(document).ready(function() {
    $(function() {
        $(".preloader").fadeOut();
        $('#side-menu').metisMenu();
    });

    // Theme settings

    //Open-Close-right sidebar
    $(".right-side-toggle").click(function() {
        $(".right-sidebar").slideDown(50);
        $(".right-sidebar").toggleClass("shw-rside");

        // Fix header

        $(".fxhdr").click(function() {
            $("body").toggleClass("fix-header");
        });

        // Fix sidebar

        $(".fxsdr").click(function() {
            $("body").toggleClass("fix-sidebar");
        });

        // Service panel js

        if ($("body").hasClass("fix-header")) {
            $('.fxhdr').attr('checked', true);
        } else {
            $('.fxhdr').attr('checked', false);
        }

        if ($("body").hasClass("fix-sidebar")) {
            $('.fxsdr').attr('checked', true);
        } else {
            $('.fxsdr').attr('checked', false);
        }

    });

    //Loads the correct sidebar on window load,
    //collapses the sidebar on window resize.
    // Sets the min-height of #page-wrapper to window size
    $(function() {
        $(window).bind("load resize", function() {
            topOffset = 60;
            width = (this.window.innerWidth > 0) ? this.window.innerWidth : this.screen.width;
            if (width < 768) {
                $('div.navbar-collapse').addClass('collapse');
                topOffset = 100; // 2-row-menu
            } else {
                $('div.navbar-collapse').removeClass('collapse');
            }

            height = ((this.window.innerHeight > 0) ? this.window.innerHeight : this.screen.height) - 1;
            height = height - topOffset;
            if (height < 1) height = 1;
            if (height > topOffset) {
                $("#page-wrapper").css("min-height", (height) + "px");
            }
        });

        var url = window.location;
        var element = $('ul.nav a').filter(function() {
            return this.href == url || url.href.indexOf(this.href) == 0;
        }).addClass('active').parent().parent().addClass('in').parent();
        if (element.is('li')) {
            element.addClass('active');
        }
    });

    // This is for resize window
    $(function() {
        $(window).bind("load resize", function() {
            width = (this.window.innerWidth > 0) ? this.window.innerWidth : this.screen.width;
            if (width < 1170) {
                $('body').addClass('content-wrapper');
                $(".open-close i").removeClass('icon-arrow-left-circle');
                $(".sidebar").css("overflow", "inherit").parent().css("overflow", "visible");
                
            } else {
                $('body').removeClass('content-wrapper');
                $(".open-close i").addClass('icon-arrow-left-circle');
                
            }
        });
    });


    // This is for click on open close button
    // Sidebar open close
        $(".open-close").on('click', function() {
            if ($("body").hasClass("content-wrapper")) {
                $("body").trigger("resize");

                $(".sidebar-nav, .slimScrollDiv").css("overflow", "hidden").parent().css("overflow", "visible");
                $("body").removeClass("content-wrapper");
                $(".open-close i").addClass("icon-arrow-left-circle");
                $(".logo span").show();

            } else {
                $("body").trigger("resize");
                $(".sidebar-nav, .slimScrollDiv").css("overflow", "inherit").parent().css("overflow", "visible");

                $("body").addClass("content-wrapper");
                $(".open-close i").removeClass("icon-arrow-left-circle");
                $(".logo span").hide();
            }
        });        
        

        // Collapse Panels

        (function($, window, document) {
            var panelSelector = '[data-perform="panel-collapse"]';

            $(panelSelector).each(function() {
                var $this = $(this),
                    parent = $this.closest('.panel'),
                    wrapper = parent.find('.panel-wrapper'),
                    collapseOpts = {
                        toggle: false
                    };

                if (!wrapper.length) {
                    wrapper =
                        parent.children('.panel-heading').nextAll()
                        .wrapAll('<div/>')
                        .parent()
                        .addClass('panel-wrapper');
                    collapseOpts = {};
                }
                wrapper
                    .collapse(collapseOpts)
                    .on('hide.bs.collapse', function() {
                        $this.children('i').removeClass('ti-minus').addClass('ti-plus');
                    })
                    .on('show.bs.collapse', function() {
                        $this.children('i').removeClass('ti-plus').addClass('ti-minus');
                    });
            });
            $(document).on('click', panelSelector, function(e) {
                e.preventDefault();
                var parent = $(this).closest('.panel');
                var wrapper = parent.find('.panel-wrapper');
                wrapper.collapse('toggle');
            });
        }(jQuery, window, document));

        // Remove Panels

        (function($, window, document) {
            var panelSelector = '[data-perform="panel-dismiss"]';
            $(document).on('click', panelSelector, function(e) {
                e.preventDefault();
                var parent = $(this).closest('.panel');
                removeElement();

                function removeElement() {
                    var col = parent.parent();
                    parent.remove();
                    col.filter(function() {
                        var el = $(this);
                        return (el.is('[class*="col-"]') && el.children('*').length === 0);
                    }).remove();
                }
            });
        }(jQuery, window, document));


        //tooltip
        $(function() {
            $('[data-toggle="tooltip"]').tooltip()
        })


        //Popover
        $(function() {
            $('[data-toggle="popover"]').popover()
        })


        // Task
        $(".list-task li label").click(function() {
            $(this).toggleClass("task-done");
        });
        
        $(".settings_box a").click(function() {
            $("ul.theme_color").toggleClass("theme_block");
        });

        
    });

    //Colepsible toggle

    $(".collapseble").click(function() {
        $(".collapseblebox").fadeToggle(350);
    });

    // Sidebar
    $('.slimscrollright').slimScroll({
        height: '100%',
        position: 'right',
        size: "5px",
        color: '#dcdcdc',

    });
    $('.chat-list').slimScroll({
        height: '100%',
        position: 'right',
        size: "0px",
        color: '#dcdcdc',

    });

    // Resize all elements
    $("body").trigger("resize");

    // visited ul li
    $('.visited li a').click(function(e) {

        $('.visited li').removeClass('active');

        var $parent = $(this).parent();
        if (!$parent.hasClass('active')) {
            $parent.addClass('active');
        }
        e.preventDefault();
    });
    
    // Login and recover password
    $('#to-recover').click(function () {
        $("#login-form-wrapper").slideUp();
        $("#recoverform").fadeIn(function () {
            loginBoxResize();
        });
    });
    // Hide recover password, show login.
    $('#back-to-login').click(function () {
        $("#recoverform").fadeOut();
        $("#login-form-wrapper").slideDown(function () {
            loginBoxResize();
        });

    });
    //Hide Login, show register.
    $('#go-to-register').click(function () {
        $("#login-form-wrapper").slideUp();
        $("#registerform").fadeIn(function () {
            loginBoxResize();
        });

    });
    //Hide Register, show login
    $('#to-login-from-register').click(function () {
        $("#registerform").fadeOut();
        $("#login-form-wrapper").slideDown(function () {
            loginBoxResize();
        });

    });
    loginBoxResize();

    function loginBoxResize() {
        var marginTop = -(($('.login-box').height()) / 2);
        var marginleft = -(($('.login-box').width()) / 2);
        $('.login-box').css('margin-left', marginleft);
        $('.login-box').css('margin-top', marginTop);
    };

    //$('.login-box').css(function () {
    //    var boxWidth = this.width();

    //});


/*Edit Household, Add Email Input Fields*/
    $(document).ready(function () {
        var counter = 0;
        $("#addDom").click(function () {
            var domElement = $('  <div class="form-group email-input-fields"> <div class="col-md-2"> <label style="padding-top: 9px;" for="email">Email: </label></div> <div class="col-md-8"><input type="email" name="emailInvites[' + counter + ']" class="form-control" id="email"></div></div>');
            $(this).after(domElement);
            counter++;
        });
    });

    //Create/Edit Transaction Forms
    //shows/hides category form-group when expense/income is selected
    $("#IsExpense").change(function () {
        var value = $(this).val();
        if (value == "false") {
            $("#category-form-group").hide();
            $("#CategoryId").val("6");
        }
        else {
            $("#CategoryId").val("");
            $("#category-form-group").show();
        }
    });

        //Transaction Options
    //$('.transaction-options').popover({ html: true, container: '#transactionsPopoverContainer', trigger: "click" });
        //Transaction Options
        $(function () {
            $(".date-picker").datepicker({ dateformat: "mm/dd/yy" });
        });
        //document.getElementById("displayTransactionsPanelWrapper").addEventListener("scroll", myFunction);

        if (!$.datepicker.initialized) {
            $(document).mousedown($.datepicker._checkExternalClick)
                // !!!!!!!!!!
                // The next code line has to be added again so that the date picker
                // shows up when the popup is opened more than once without reloading
                // the "base" page.
                // !!!!!!!!!!
                .find(document.body).append($.datepicker.dpDiv);
            $.datepicker.initialized = true;
        };

/*MyHousehold Members Partial*/
    $(document).ready(function () {
        var counter = 0;

        $("#addUserBtn").click(function () {
            var domElement = $('  <div class="row addUsersElement" style="padding-top: 5px;"> <div class="col-md-2"> <label style="padding-top: 9px;" for="email">Email: </label></div> <div class="col-md-8"><input type="email" name="emailInvites" class="form-control" id="email"></div></div>');
            $('#addUserEmailInputRow').append(domElement);
            counter++;
            $('.addUsersElement').show();
        });
        $('#btn-ClearInviteDom').click(function () {
            counter = 0;
            $('#addUserEmailInputRow').empty();
            $("#addUserSubmitRow").empty();
            $('.addUsersElement').hide();
        });
        $('#btn-SendInvites').click(function () {
            counter = 0;
            //$('#MyHousehold-Members-Wrapper').append(' <div class="col-xs-12 col-md-11 center-block"><div class="row m-t-30"><h3 style="text-align: center;"><i class="fa fa-spin fa-spinner" style="font-size: 25px;"></i> Fetching Household Data... </h3></div></div>');
            //$('#addUserEmailInputRow').empty();
            //$("#addUserSubmitRow").empty();
            //$('.addUsersElement').hide();
            //$("#addUserSubmitRow").empty();
            //$('.addUsersElement').hide();
        })
       
        $(function () {
            $(".date-picker").datepicker({ dateformat: "mm/dd/yy"});
        });

        /*Invitations*/
        /*My Household Section*/
        $(".leave-household-btn").click(function () {
            $(".popover-content").each(function () {
                var text = $(this).html();
                $(this).html(text.replace('LeaveLinkHere!', '<a href="@Url.Action("Edit", "Households", new { id = Model.Id })"class="fcbtn btn btn-info btn-outline btn-1d">Edit</a>'));
            })
        })

 

        $('.leave-household-btn').popover({ html: true, container: '#MyHousehold-Options-Wrapper', trigger: "focus" });
        $('.popover-info-anchor').popover({ html: true, container: '#myHousehold-Accounts-Panel .popover-container', trigger: "hover" });
        //$('.popover-info-ManageAccount').popover({ html: true, container: '.popover-info-container', trigger: "click" });
        //$('.account-options').popover({ html: true, container: '#myHousehold-Accounts-tablebody', trigger: "click" });
        //$('.popover-info-anchor').hover(function () {

        //});
        $('#myHousehold-Accounts-Panel .popover').css("width", "276px");
        //$('.account-options').popover("toggle");


    });
    $(document).ready(function () {
        /*Stand DataTable*/
        var table = $('.myTablez').DataTable({
            "order": []//Disable Initial Sort
        });

        //var dropdownelements = [];
        //document.getElementById("dropdown-gears").addEventListener("click", GetDropdownID(this.id));
        //function GetDropdownID(){
        //    var dropdownId = this.id;
        //    if($.inArray(dropdownId, dropdownelements)<0){
        //        dropdownelements.push(dropdownId);
        //    }
        //    else{
                
        //    }
            
        //}


        //$("table-responsive").addEventListener("scroll", $('btnOpen').click());

        //$("dropdown-gears").addEventListener("click", dropdownFunc());
        //function dropdownFunc() {
        //    $("dropdown-gears").addClass("btnOpen");
        //}
    });