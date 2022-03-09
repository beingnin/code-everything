//Jquery extensions
jQuery.fn.selectText = function () {

    try {
        var range = document.createRange();
        range.selectNodeContents(this[0]);
        var sel = window.getSelection();
        sel.removeAllRanges();
        sel.addRange(range);
        this[0].focus();
    } catch (error) {
        console.log(error);
    }
    return this;
};

jQuery.fn.spinner = function (action = 'start') {

    if (action === 'start') {
        let html = `<div class="spinner-grow spinner-grow-sm" role="status">
                    <span class="sr-only">Loading...</span>
                    </div>`;

        let spinner = $(html).data('old-text', this.html());
        this.html(spinner);
        return;

    }

    if (action === 'stop') {
        let spinner = this.find('div.spinner-grow');
        let old = spinner.data('old-text');
        spinner.remove();
        this.html(old);
        return;
    }



}


$(document).ready(async function () {
    const app =
    {
        platforms: {
            list: [
                { name: 'M-Portal', desc: 'Citizen\'s portal in react native', code: 'MP', icon: 'fa-mobile-android', colour: 'bg-warning--light' },
                { name: 'M-Egate', desc: 'Egate app in react native', code: 'ME', icon: 'fa-mobile-android', colour: 'bg-warning--light' },
                { name: 'B-Egate', desc: 'API for egate', code: 'BE', icon: 'fa-database', colour: 'bg-danger--light' },
                { name: 'B-HRMS', desc: 'API for hrms', code: 'BH', icon: 'fa-database', colour: 'bg-danger--light' },
                { name: 'B-Menu', desc: 'Shared API for menu structure', code: 'BM', icon: 'fa-database', colour: 'bg-danger--light' },
                { name: 'W-Egate', desc: 'Web app for egate', code: 'WE', icon: 'fa-network-wired', colour: 'bg-success--light' },
                { name: 'W-HRMS', desc: 'Web app for hrms', code: 'WH', icon: 'fa-network-wired', colour: 'bg-success--light' },
                { name: 'W-Portal', desc: 'Web app for citizen\'s portal', code: 'WP', icon: 'fa-browser', colour: 'bg-warning--light' }
            ],
            getName: function (code) { return app.platforms.list.filter(function (p) { return p.code === code })[0].name },
            getCode: function (name) { return app.platforms.list.filter(function (p) { return p.name === name })[0].code },
            getIcon: function (code) { return app.platforms.list.filter(function (p) { return p.code === code })[0].icon },
            getIconColor: function (code) { return app.platforms.list.filter(function (p) { return p.code === code })[0].colour }
        },
        vars: {
            intobs: null
        },
        profile: {},
        templates: {
            phrasesTranslated: $('#tmpl-phrase-item-translated').html(),
            platformDdlItem: $('#tmpl-platform-ddl-item').html()
        },
        loader: function () {
            let shimmer = $($('#tmpl-shimmer').html());
            $('#js-phrase-list').append(shimmer);
            return function () { shimmer.remove() }
        },
        provider:
        {
            base: '/',
            post: function (url, data) {
                return $.ajax({
                    method: 'POST',
                    url: app.provider.base + url,
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    data: JSON.stringify(data),
                    headers: {
                        authorization: 'bearer ' + app.profile.token || ''
                    },
                    error: function (xhr) {
                        if (xhr.status === 401) {
                            setTimeout(() => {
                                $('#loginModalToggle').modal('show');
                            }, 500);
                        }

                    }
                })
            },
            get: function (url) {
                return $.ajax({
                    url: app.provider.base + url,
                    dataType: 'json',
                });
            }
        },
        error: function (error, title = null) {

            let container = $('#js-header-container');
            container.find('h5').hide();
            container.find('.alert').text(error).fadeIn('slow');

            setTimeout(() => {
                container.find('.alert').fadeOut('slow', function () { container.find('h5').fadeIn() });

            }, 3000);
        },
        timeAgo: function (time) {
            switch (typeof time) {
                case 'number':
                    break;
                case 'string':
                    time = +new Date(time);
                    break;
                case 'object':
                    if (time.constructor === Date) time = time.getTime();
                    break;
                default:
                    time = +new Date();
            }
            var time_formats = [
                [60, 'seconds', 1], // 60
                [120, '1 minute ago', '1 minute from now'], // 60*2
                [3600, 'minutes', 60], // 60*60, 60
                [7200, '1 hour ago', '1 hour from now'], // 60*60*2
                [86400, 'hours', 3600], // 60*60*24, 60*60
                [172800, 'Yesterday', 'Tomorrow'], // 60*60*24*2
                [604800, 'days', 86400], // 60*60*24*7, 60*60*24
                [1209600, 'Last week', 'Next week'], // 60*60*24*7*4*2
                [2419200, 'weeks', 604800], // 60*60*24*7*4, 60*60*24*7
                [4838400, 'Last month', 'Next month'], // 60*60*24*7*4*2
                [29030400, 'months', 2419200], // 60*60*24*7*4*12, 60*60*24*7*4
                [58060800, 'Last year', 'Next year'], // 60*60*24*7*4*12*2
                [2903040000, 'years', 29030400], // 60*60*24*7*4*12*100, 60*60*24*7*4*12
                [5806080000, 'Last century', 'Next century'], // 60*60*24*7*4*12*100*2
                [58060800000, 'centuries', 2903040000] // 60*60*24*7*4*12*100*20, 60*60*24*7*4*12*100
            ];
            var seconds = (+new Date() - time) / 1000,
                token = 'ago',
                list_choice = 1;

            if (seconds == 0) {
                return 'Just now'
            }
            if (seconds < 0) {
                seconds = Math.abs(seconds);
                token = 'from now';
                list_choice = 2;
            }
            var i = 0,
                format;
            while (format = time_formats[i++])
                if (seconds < format[0]) {
                    if (typeof format[2] == 'string')
                        return format[list_choice];
                    else
                        return Math.floor(seconds / format[2]) + ' ' + format[1] + ' ' + token;
                }
            return time;
        },
        intersection: function (callback, targetSelector, rootSelector, offset = 0) {
            function select() {
                var targets = document.querySelectorAll(targetSelector);
                if (targets.length === 0) {
                    //no entries found
                }
                else if (targets.length > offset) {
                    return targets[targets.length - 1 - offset];
                }
                else {
                    return targets[targets.length - 1];
                }
            }
            var target = select();
            let page = 1;
            let processing = false;
            let options =
            {
                root: rootSelector ? document.querySelector(rootSelector) : null,
                rootMargin: '0px',
                threshold: 1.0
            };

            let observer = new IntersectionObserver(function (entries, observer) {
                var t = entries[0];

                if (t.isIntersecting && !processing) {
                    processing = true;
                    callback(t.target, page);
                }
            }, options);
            observer.observe(target);

            return {
                next: function () {
                    observer.unobserve(target);
                    target = select();
                    observer.observe(target);
                    page++;
                    processing = false;
                },
                continue: function () {
                    processing = false;
                }
            }
        }

    }



    //private fns
    const startTour = function () {

        if (localStorage.getItem('tours.status.main') === 'completed') {
            console.log("you have completed this main tour");
            return;
        }
        const tourEnd = function () {
            localStorage.setItem('tours.status.main', 'completed');
            removeAllHighlights();
            $('.js-phrase-list-item:first-child')[0].scrollIntoView({
                behavior: "smooth",
                block: "center",
                inline: "center"
            });
        }
        const removeAllHighlights = function () {
            $('.js-phrase-list-item').css('box-shadow', '');
            $('*').removeClass('blurout');
        }
        const highlightPhrase = function (row) {
            $('.js-phrase-list-item').addClass('blurout');
            row.removeClass('blurout');
            row.css('border-color', 'black !important').css('box-shadow', '0 0 20px black');
        }
        const highlightPart = function (part) {
            part.closest('.js-phrase-list-item').find('.app-lang-card__item').addClass('blurout');
            part.closest('.app-lang-card__item').removeClass('blurout');
        }
        let examplePhrase = $('.js-phrase-list-item')[3];
        let editBtn = $(examplePhrase).find('.js-phrase-edit');
        if (editBtn.attr('data-mode') === 'edit') {
            editBtn.click();
            examplePhrase = $('.js-phrase-list-item')[3]
        }
        var tour = {
            id: 'main',
            showPrevButton: true,
            onEnd: tourEnd,
            onClose: tourEnd,
            steps: [
                {
                    title: 'Welcome to i18n <i class="fad fa-globe-asia"></i>',
                    content: `i18n is a project intended to ease the effort brought up to internationalize all 
                    the applications used by sharjah police department and it\'s subsidiaries<br/><br/>
                    
                    <b>Please do not interact with the application till the tour is over</b>`,
                    target: $('nav .navbar-brand')[0],
                    placement: 'bottom',
                    onShow: function () {
                        removeAllHighlights();
                    }
                },
                {
                    title: 'This whole row in here, we call it as a phrase',
                    content: 'The generic characteristic of a phrase is that it must have a <b>key</b>, <b>english</b> part and an <b>arabic</b> part. ',
                    target: examplePhrase,
                    placement: 'bottom',
                    xOffset: 'center',
                    arrowOffset: 'center',
                    onShow: function () {
                        removeAllHighlights();
                        highlightPhrase($(examplePhrase))
                    }
                },
                {
                    title: `Key of the phrase`,
                    content: `<code>${$(examplePhrase).find('.js-key').text()}</code> is the key of this particular phrase.<br/><br/>
                             You never can edit the key of a phrase once after it is created. <br/><br/>
                             <b>Note:</b> A key can exist in a platform only once. 
                             In other words, no platform can have more than one phrase with the same value as key`,
                    target: $(examplePhrase).find('.js-key')[0],
                    placement: 'bottom',
                    xOffset: 'right',
                    arrowOffset: 'left',
                    onShow: function () {
                        removeAllHighlights();
                        highlightPhrase($(examplePhrase), false)
                        highlightPart($(examplePhrase).find('.js-key'))
                    }
                },
                {
                    title: `English part of the phrase`,
                    content: `<code>${$(examplePhrase).find('.js-en').text()}</code> is the english part of this particular phrase.<br/><br/>
                             Only users with <b>developer</b> role has the privilege to modify this part of the phrase.<br/><br/>
                             <b>Note: </b> If the english part of a phrase is modified, that phrase will be considered as <b>non-translated</b>.
                             `,
                    target: $(examplePhrase).find('.js-en')[0],
                    placement: 'bottom',
                    xOffset: 'center',
                    arrowOffset: 'left',
                    onShow: function () {
                        removeAllHighlights();
                        highlightPhrase($(examplePhrase))
                        highlightPart($(examplePhrase).find('.js-en'))
                    }
                },
                {
                    title: `Arabic part of the phrase`,
                    content: `<code>${$(examplePhrase).find('.js-ar').text()}</code> is the arabic part of this particular phrase.<br/><br/>
                             <b>Note : </b> If an user with <b>developer</b> role changes the arabic part of a phrase, 
                             that phrase will be considered as <b>non-translated</b>. 
                             In the meantime, if an user with translator role has changed the arabic part, it will be considered as a <b>translated</b> phrase`,
                    target: $(examplePhrase).find('.js-ar')[0],
                    placement: 'bottom',
                    xOffset: 'center',
                    arrowOffset: 'right',
                    onShow: function () {
                        removeAllHighlights();
                        highlightPhrase($(examplePhrase))
                        highlightPart($(examplePhrase).find('.js-ar'))
                    }
                },
                {
                    title: `Usages`,
                    content: `The platforms checked in this drop down list are those applications where this particular phrase is used`,
                    target: $(examplePhrase).find('.js-plus-more').closest('.app-lang-card__item')[0],
                    placement: 'top',
                    xOffset: -100,
                    arrowOffset: 'center',
                    onShow: function () {
                        removeAllHighlights();
                        highlightPhrase($(examplePhrase))
                        highlightPart($(examplePhrase).find('.js-plus-more'))
                        setTimeout(() => {
                            $(examplePhrase).find('.js-plus-more').dropdown('show');
                        }, 100);
                    }
                },
                {
                    title: `Add a new phrase`,
                    content: `If you are an user with <b>developer</b> role, you can add a new phrase by clicking this button and filling 
                              the subsequent form which opens.<br/><br/> 
                              <b>Note: </b> Even if this button is shown regardless of the user's role, only an user with <b>developer</b> 
                              role can finally create a phrase in the system`,
                    target: $('[href="#addModalToggle"]')[0],
                    placement: 'bottom',
                    onShow: function () {
                        removeAllHighlights();
                    }
                },
                {
                    title: `Export to JSON`,
                    content: `You can download all the phrases of a specific platform as a json file from here. <br/><br/>
                             You can further use this json file in the respective application for internationalization purposes.<br/><br/>
                             
                             <b>Note: </b> The MIME type of the exported file will be <i>application/json; charset=UTF-8</i>`,
                    target: $('#js-download-list').closest('.dropdown').find('button')[0],
                    placement: 'bottom',
                    arrowOffset: 'center',
                    xOffset: -100,
                    onShow: function () {
                        removeAllHighlights();
                    }
                },
                {
                    title: `Toggle quick edit mode`,
                    content: `If you are an user with translator role and wish to translate a lot of phrases quickly and easily, 
                              you might feel to have a wider screen so that more phrases can be appeared on the view port. <br/><br/>
                              
                              <b>Tip: </b>Use quick edit mode along with keyboard shortcuts to translate more phrases speedly<br/><br/>

                              <b>Keyboard Shortcuts</b>

                              <ul type="disc">
                              <li><kbd>Enter</kbd>                  :   Go to the next editable position</li>
                              <li><kbd>ctrl</kbd> + <kbd>s</kbd>    :   Save the current phrase</li>
                              <li><kbd>ctrl</kbd> + <kbd>z</kbd>    :   Cancel edit</li>
                              </ul>`,
                    target: $('#js-toggle-fullscreen')[0],
                    placement: 'bottom',
                    arrowOffset: 250,
                    xOffset: -250,
                    onShow: function () {
                        removeAllHighlights();
                    }
                },

                {
                    width: 400,
                    title: 'Edit or translate a phrase',
                    content: `Double clicking on a row containing a phrase will open it up for editing.<br/><br/>
                              If you are an user with <b>translator</b> role, all the phrases which are not yet translated are by default opened for editing. 
                              You just need to navigate through all those phrases and make necessary amendments.<br/><br/>

                              <b>Keyboard Shortcuts</b>

                              <ul type="disc">
                              <li><kbd>Enter</kbd>                  :   Go to the next editable position</li>
                              <li><kbd>ctrl</kbd> + <kbd>s</kbd>    :   Save the current phrase</li>
                              <li><kbd>ctrl</kbd> + <kbd>z</kbd>    :   Cancel edit</li>
                              </ul>`,
                    target: examplePhrase,
                    placement: 'bottom',
                    xOffset: 'center',
                    arrowOffset: 'center',
                    onShow: function () {
                        removeAllHighlights();
                        highlightPhrase($(examplePhrase))
                    }
                },
                {
                    width: 1000,
                    title: 'History',
                    content: `All the changes made by all of the users in a phrase are recorded and can be seen 
                              in the reverse chronological order of the changes by clicking <i class="far fa-clock"></i> button. 
                              Also the values of the phrase which have been changed with respect to it's previous version will be highlighted 
                              in green like the screenshot below<br/><br/>
                              <img style="width:950px"  src="/img/history_sample.png" />`,
                    target: $(examplePhrase).find('.js-view-history')[0],
                    placement: 'bottom',
                    xOffset: -1000,
                    arrowOffset: 990,
                    yOffset: -10,
                    onShow: function () {
                        removeAllHighlights();
                        highlightPhrase($(examplePhrase))
                    }
                },
                {
                    width: 400,
                    title: 'Load more phrases',
                    content: `For performance reasons, only those 50 phrases which got created in the most recent time are initially loaded in the grid. 
                              If there are more phrases yet to come,they will be deferred to be added to the list once you reach down the bottom of the list<br></br>
                              <b>Note: </b>To be loaded or not, it's depended on the total phrases in the system and the filter you applied`,
                    target: examplePhrase,
                    placement: 'bottom',
                    xOffset: 'center',
                    arrowOffset: 'center',
                    showCTAButton: true,
                    ctaLabel: 'Scroll down and check',
                    onShow: function () {
                        removeAllHighlights();
                    },
                    onCTA: function () {
                        $('.js-phrase-list-item:last-child')[0].scrollIntoView({
                            behavior: "smooth",
                            block: "center",
                            inline: "center"
                        });
                    }
                },
                {
                    width: 400,
                    title: 'Filters',
                    content: `You can do advanced searchs using the filters provided in this pane. 
                    If used wisely, you would be able to get several useful record sets such as but not limited to: 
                    <ul>
                    <li>Phrases having pending translation</li>
                    <li>Phrases using the same key value</li>
                    <li>Phrases of a specific platform</li>
                    <li>Phrases having it's english or arabic part with some text you have in your mind</li>
                    <li>Or any combination of the above and upto your notion</li>
                    </ul>
                    Also there are in-built queries for exceptional cases for finding
                    <ul>
                    <li>All suspicious phrases which do not have a single letter from the arabic alphabets/number system as its's arabic part</li>    
                    <li>All the deleted phrases so far</li>    
                    </ul>`,
                    target: $('#js-filter-container')[0],
                    placement: 'left',
                    xOffset: -220,
                    arrowOffset: 'center',
                    onShow: function () {
                        removeAllHighlights();
                        setTimeout(() => {
                            $('#js-filter-container button[data-bs-toggle="dropdown"]').dropdown('show');
                        }, 100);
                    },
                },
            ]
        };

        hopscotch.startTour(tour);
    }

    const setProfile = function (data) {
        let profile = $('#js-profile-container');
        profile.find('.email').text(data.email);
        profile.find('.role').text(data.role);
        $('#js-signin-btn').hide();
        profile.show();
        app.profile = data;
    }
    const unSetProfile = function () {
        $('#js-profile-container').hide();
        $('#js-signin-btn').show();
        app.profile = {};
    }
    const renderPhraseTranslated = function (phrase, isEditable = false) {
        let item = $(app.templates.phrasesTranslated);
        item.data('_phrase', phrase);
        item.find('.js-key').text(phrase.key);
        item.find('.js-ar').text(phrase.ar);
        item.find('.js-en').text(phrase.en);
        let platforms = [];
        $.each(app.platforms.list, function (i) {
            let p = $(app.templates.platformDdlItem);
            p.find('.js-platform').text(this.name).attr('title',this.desc);
            let chk = p.find('input[type=checkbox]');
            chk.val(this.code);
            if (phrase.usage.indexOf(this.code) !== -1) {
                chk.attr('checked', 'checked')
            }
            let uid = phrase._id + '_' + i;
            chk.attr('id', uid);
            p.find('label').attr('for', uid);
            platforms.push(p);
        });
        item.find('.js-platform-list').append(platforms);
        item.find('.js-platform-container')
            .prepend(phrase.usage.slice(0, 1).map(function (x) { return `<button type="button" class="btn btn-sm btn-secondary">${x.toUpperCase()}</button>` }));
        if (phrase.usage.length > 1) {
            item.find('.js-plus-more').text('+' + (phrase.usage.length - 1).toString())
        }
        if (isEditable) {
            item = makePhraseEditable(item);
        }
        item.find('.js-phrase-edit').on("click", function () {
            if ($(this).attr('data-mode') === 'edit') {
                $(this).closest('.js-phrase-list-item').replaceWith(renderPhraseTranslated(item.data('_phrase'), false));
            }
            else {
                item = makePhraseEditable(item);
            }
        });
        return item;
    }
    const makePhraseEditable = function (jRow) {
        jRow.removeClass('app-lang-card--success').addClass('js-editing-phrase');
        jRow.find('.js-action-icon').removeClass().addClass('js-action-icon fa fa-pen');
        let en = jRow.find('.js-en');
        en.attr('contenteditable', app.profile.role === 'developer' ? 'true' : 'false');
        let ar = jRow.find('.js-ar');
        ar.attr('contenteditable', 'true');
        app.profile.role === 'developer' && en.focus().selectText();
        app.profile.role === 'translator' && ar.focus().selectText();
        app.profile.role === 'developer' && jRow.find('.js-platform-list input[type=checkbox]').removeAttr('disabled');
        jRow.find('.js-update-phrase').show();
        jRow.find('.js-view-history').hide();
        jRow.find('.js-phrase-edit').attr('data-mode', 'edit').find('span').text('Close editing');
        addKeyBoardSupport(jRow, true);
        return jRow;
    }

    //add keyboard support

    const addKeyBoardSupport = function (jRow, enable) {
        if (!jRow) {
            jRow = $('.js-phrase-list-item.js-editing-phrase');
        }
        if (!enable) {
            jRow.find('div[contenteditable=true]').off('keydown');
            // $('.js-quick-edit-avoidable').show();
            return;
        }
        // $('.js-quick-edit-avoidable').hide();

        jRow.find('div[contenteditable=true]').off('keydown').keydown(function (e) {

            let key = e.which || e.keyCode;
            let target = $(e.target);
            let focusable = null;
            if (key == 13) {    //jump to next content

                if (target.hasClass('js-en')) {
                    focusable = target.next('div[contenteditable=true]');
                }
                if (target.hasClass('js-ar')) {
                    focusable = target.closest('.js-phrase-list-item').nextAll('.js-phrase-list-item.js-editing-phrase')
                        .find('div[contenteditable=true]');
                }


            }
            if (e.ctrlKey && key == 83) { //ctrl + s
                e.preventDefault();
                target.closest('.js-phrase-list-item').find('.js-update-phrase').click();
                focusable = target.closest('.js-phrase-list-item').nextAll('.js-phrase-list-item.js-editing-phrase')
                    .find('div[contenteditable=true]');
            }

            if (e.ctrlKey && key == 90) { //ctrl + z
                e.preventDefault();
                focusable = target.closest('.js-phrase-list-item').nextAll('.js-phrase-list-item.js-editing-phrase')
                    .find('div[contenteditable=true]');
                target.closest('.js-phrase-list-item').find('.js-phrase-edit').click();
            }
            if (focusable) {
                focusable.eq(0).selectText();
                e.stopPropagation();
                e.preventDefault();
                // $('#js-phrase-list').animate({
                //     scrollTop: $(focusable.eq(0)).offset().top - $(window).height() / 2
                // });
                focusable[0].scrollIntoView({
                    behavior: "smooth",
                    block: "center",
                    inline: "center"
                });
            }
        })
    }

    // render filter values and also return filter object for API
    const renderFilter = (extra = '') => {
        let translatedFlag = $('#js-filter-container input[name=js-filter-translated-radio]:checked').val();
        switch (translatedFlag) {
            case 'translated':
                translatedFlag = true;
                break;
            case 'non-translated':
                translatedFlag = false;
                break;
            default:
                translatedFlag = null;
                break;
        }

        let filter = {
            en: $('#js-filter-en').val().trim(),
            ar: $('#js-filter-ar').val().trim(),
            key: $('#js-filter-key').val().trim(),
            translated: translatedFlag,
            usage: $('#js-filter-usage-container input[type=checkbox]:checked').toArray().map(function (u) { return $(u).val() })
        }

        $('#js-filter-container').find('.js-filter-tag').remove();
        for (let i in filter) {
            if (typeof filter[i] === 'string' && filter[i]) {
                $('#js-filter-container').prepend(`<span class="js-filter-tag label m-0 me-3">${i} &nbsp; <i class="fal fa-times"></i></span>`);
            }
            if (Array.isArray(filter[i]) && filter[i].length > 0) {
                $('#js-filter-container').prepend(filter[i].map(function (u) {
                    return $(`<span class="js-filter-tag label m-0 me-3">${app.platforms.getName(u)} &nbsp; <i class="fal fa-times"></i></span>`);
                }))
            }
            if (typeof filter[i] === 'boolean' && i === 'translated') {
                $('#js-filter-container').prepend(`<span class="js-filter-tag label m-0 me-3">${filter[i] ? 'Translated only' : 'Pending translation'} &nbsp; <i class="fal fa-times"></i></span>`);
            }
        }

        if (extra)
            $('#js-filter-container').prepend(`<span class="js-filter-tag label m-0 me-3">${extra} &nbsp; <i class="fal fa-times"></i></span>`);


        return filter;
    }

    const populatePhrases = async function (filter = {}, page = 1, size = 50) {
        try {
            let phrases = await app.provider.post(`internationalization/search?page=${page}&size=${size}`, filter);
            let list = [];
            $.each(phrases, function () {
                list.push(renderPhraseTranslated(this, !this.translated && app.profile.role == 'translator'));
            });
            if (list.length > 0) {
                $('#js-phrase-list').append(list);
                console.log(`rendered page ${page}`);
            }
            return phrases.length > 0;

        } catch (error) {
            app.error(error.statusText)
        }
    }

    const populateHistoryRow = function (history) {

        return `<tr>
            <td>${history.en}</td>
            <td>${history.ar}</td>
            <td>${history.usage.map(function (u) { return `<kbd title = "${app.platforms.getName(u)}" >${u}</kbd>` })}</td>
            <td class="js-no-change">${history.modifiedBy}</td>
            <td class="js-no-change" title="${history.modifiedAt}">${app.timeAgo(history.modifiedAt)}</td>
            <td class="js-no-change">${history.action}</td>
        </tr>`
    }
    const showChanges = function () {
        let trs = $('#historyModalToggle table tbody tr');
        for (let i = trs.length - 1; i >= 0; i--) {

            if (i > 0) {
                let c = $(trs[i]), n = $(trs[i - 1]);
                let tdsc = c.find('td').not('.js-no-change');
                let tdsn = n.find('td').not('.js-no-change');

                for (let j = 0; j < tdsc.length; j++) {
                    if ($(tdsc[j]).html() !== $(tdsn[j]).html()) {
                        $(tdsn[j]).css('background-color', '#d8edde');
                    }
                }
            }
        }

    }
    const loadSuspicious = async function () {
        $('#js-filter-container form')[0].reset();
        renderFilter('Suspicious');
        $("#js-filter-container .dropdown").trigger('click.bs.dropdown');
        $('#js-phrase-list').empty();
        try {
            let stopper = app.loader();
            let phrases = await app.provider.get(`internationalization/suspect`);
            let list = [];
            $.each(phrases, function () {
                list.push(renderPhraseTranslated(this, !this.translated && app.profile.role == 'translator'));
            });
            stopper();
            $('#js-phrase-list').append(list);

        } catch (error) {
            app.error(error.statusText)
        }
    }


    const loadDeleted = async function () {
        $('#js-filter-container form')[0].reset();
        renderFilter('Deleted');
        $("#js-filter-container .dropdown").trigger('click.bs.dropdown');
        $('#js-phrase-list').empty();
        try {
            let stopper = app.loader();
            let phrases = await app.provider.get(`internationalization/getdeleted`);
            let list = [];
            $.each(phrases, function () {
                list.push(renderPhraseTranslated(this, false));
            });
            stopper();
            $('#js-phrase-list').append(list);

        } catch (error) {
            app.error(error.statusText)
        }
    }

    const initialLoad = async function () {


        $("#js-filter-container .dropdown").trigger('click.bs.dropdown');
        $('#js-phrase-list').empty();
        let stopper = app.loader();
        await populatePhrases(renderFilter());
        stopper();
        startTour();
        //starting intersection observer
        app.vars.intobs = app.intersection(async function (item, page) {
            let stop = app.loader();
            let hasAnyData = await populatePhrases(renderFilter(), page + 1);
            stop();
            if (hasAnyData)
                app.vars.intobs.next();
            else
                app.vars.intobs.continue();

        }, '.js-phrase-list-item', '#js-phrase-list')
    }
    // populate download list

    $.each(app.platforms.list, function () {
        let item = $($('#tmpl-download-item').html());
        item.find('.js-name').text(this.name).attr('title',this.desc);
        item.attr('href', '/internationalization/export/' + this.code);
        $('#js-download-list').append(item);

    });

    //check if already signed in
    let profile = localStorage.getItem('_t');
    if (profile) {
        setProfile(JSON.parse(profile))
    }
    else {
        unSetProfile();
    }

    initialLoad();

    //populate platforms in add modal
    $.each(app.platforms.list, function (i) {
        let item = $($('#tmpl-add-usage-item').html());
        item.find('.js-add-usage-checkbox').attr('id', i + this.code).attr('data-code', this.code);
        item.find('.js-label').attr('for', i + this.code).text(this.name).attr('title',this.desc);
        $('#addModalToggle').find('#js-add-phrase-usage-container').append(item);

        //populate platforms in filter modal
        let iteminFilter = $($('#tmpl-filter-usage-item').html());
        iteminFilter.find('input[type=checkbox]').val(this.code).attr('id', 'filter-usage-' + i);
        iteminFilter.find('label').attr('for', 'filter-usage-' + i).text(this.name).attr('title',this.desc);

        $('#js-filter-usage-container').append(iteminFilter);
    })






    //all events

    $('#js-login').click(async function () {
        try {
            $(this).spinner();
            let pwd = $('#password').val();
            let uname = $('#username').val();
            let result = await app.provider.post('authentication/gettoken', { email: uname, password: pwd })

            if (!result || !result.token)
                throw new Error('Username or password incorrect. Try again later');
            localStorage.setItem('_t', JSON.stringify(result));

            $('#loginModalToggle').modal('hide');

            setProfile(result);

        }
        catch (err) {
            console.error(err);
            app.error(err.statusText.toString());
        }
        finally {
            $(this).spinner('stop');
        }
    });

    $('#js-signout').click(function () {
        try {

            localStorage.removeItem('_t', null);
            unSetProfile();
        }
        catch (err) {
            console.error(err);
            app.error(err.toString());
        }
    });

    $('#js-toggle-fullscreen').click(function () {

        let elem = $('#js-list-wrapper')[0];

        if (!document.fullscreenElement) {

            elem.requestFullscreen().catch(err => {
                alert(`Error attempting to enable full-screen mode: ${err.message} (${err.name})`);
            }).then(function () {
            });
        } else {
            document.exitFullscreen();
        }
    });
    $('#js-phrase-list').on('click', '.js-view-history', async function (e) {

        let phrase = $(e.target).closest('.js-phrase-list-item').data('_phrase');
        let result = await app.provider.get('internationalization/getHistory/' + phrase._id);
        if (result.length > 0) {

            let rows = result.map(function (h) { return populateHistoryRow(h) });
            $('#historyModalToggleLabel').text('History : ' + result[0].key);
            $('#historyModalToggle table tbody').html(rows);
            $('#historyModalToggle').modal('show');
            showChanges();
        }
        else {
            app.error("Couldn't find any history for key: " + phrase.key);
        }

    });

    $('#js-phrase-list').on('dblclick', '.js-phrase-list-item', function () {
        let btn = $(this).find('.js-phrase-edit');
        if (btn.attr('data-mode') === 'edit')
            return;
        btn.click();
    });

    $('#js-phrase-list').on('focus', 'div[contenteditable=true]', function () {
        $('#js-phrase-list .js-phrase-list-item').css('border-color', '');
        $(this).closest('.js-phrase-list-item').css('border-color', 'black');
    })

    $('#js-phrase-list').on('click', '.js-update-phrase', async function () {

        let element = $(this).closest('.js-phrase-list-item');
        let original = element.data('_phrase');
        let data = {
            _id: original._id,
            en: element.find('.js-en').text(),
            ar: element.find('.js-ar').text(),
            usage: element.find('.js-platform-list input[type=checkbox]:checked').toArray().map(function (u) { return $(u).val() })
        }
        try {
            $(this).spinner();
            let response = await app.provider.post('internationalization/update', data);
            element.replaceWith(renderPhraseTranslated(response, false));
        } catch (error) {
            element.addClass('app-lang-card--error')
            app.error(error.statusText);
            $(this).removeClass('btn-info').addClass('btn-danger').text('Retry')
        }
        finally {
            $(this).spinner('stop');
        }

    });

    $('#js-new-phrase-review').click(async function (e) {

        if (!$('#js-key-name')[0].checkValidity()) {
            app.error('Set a key before saving any phrase');
            return;
        }

        if (!$('#js-text-english')[0].checkValidity()) {
            app.error('Add english before saving any phrase');
            return;
        }

        if (!$('#js-text-arabic')[0].checkValidity()) {
            app.error('Add at arabic before saving any phrase');
            return;
        }



        let phrase = {
            ar: $('#js-text-arabic').val(),
            en: $('#js-text-english').val(),
            key: $('#js-key-name').val(),
            usage: $('.js-add-usage-checkbox:checked').toArray().map(function (x) {
                return $(x).attr('data-code')
            })
        }

        if (phrase.usage.length == 0) {
            app.error('Add at lease one usage before saving any phrase');
            return;
        }

        $(this).data('_phrase', phrase);

        $('#js-review-key').text(phrase.key);
        $('#js-review-text-english').text(phrase.en);
        $('#js-review-text-arabic').text(phrase.ar);
        //pollulate review usage list
        $('#confirmationModalToggle').find('#js-review-usage-container').empty();
        let html = $('#tmpl-review-usage-item').html();
        $.each(phrase.usage, function (i, u) {
            let item = $(html);
            let code = u;
            let iconName = app.platforms.getIcon(code);
            let iconColour = app.platforms.getIconColor(code);
            let usageId = 'js-review-usage-id' + i;
            let name = app.platforms.getName(code);
            item.find('.app-usage__icon').addClass(iconColour);
            item.find('.fal').addClass(iconName);
            item.find('.js-usage-name').attr('id', usageId).attr('data-code', code).text(name);
            $('#confirmationModalToggle').find('#js-review-usage-container').append(item);
        })
        try {
            $(this).spinner();
            let similarList = await app.provider.get('internationalization/findby/similarity/' + phrase.en);
            if (similarList.length != 0) {
                $('#js-review-phrase-similar-keys').removeClass('d-none');
                let html = $('#tmpl-similar-phrase-item').html();
                $('#js-similar-phrase-list-container').empty();
                $.each(similarList, function (i, x) {
                    let item = $(html)
                    item.find('.js-similar-phrase-key').text(x.key);
                    item.find('.js-similar-phrase-english-text').text(x.en);
                    item.find('.js-similar-phrase-arabic-text').text(x.ar);
                    item.find('.js-similar-phrase-used-in')
                        .text(x.usage.map(function (u) { return app.platforms.getName(u) }).join(', '));
                    $('#js-similar-phrase-list-container').append(item);

                })
            }
        } catch (error) {
            console.error(error.statusText)
        }
        finally {
            $(this).spinner('stop');
        }


        $("#addModalToggle").modal('hide');
        $('#confirmationModalToggle').modal('show');

    });

    $('#js-text-arabic').focus(function () {

        let en = $('#js-text-english').val();
        if (!$(this).val() && en) {
            $(this).val(en + '-ar');
        }
    });
    $('#js-new-phrase-save').click(async function () {

        try {
            $(this).spinner();
            let newPhrase = $('#js-new-phrase-review').data('_phrase');
            let result = await app.provider.post('internationalization/add', newPhrase);

            $("#addModalToggle").modal('hide');
            $('#confirmationModalToggle').modal('hide');
            $('#js-new-phrase-review-clear').click();

            let element = renderPhraseTranslated(result, false);
            $('#js-phrase-list').prepend(element);

        } catch (error) {
            app.error(error.statusText);
        }
        finally {
            $(this).spinner('stop');
        }
    });


    $('#js-phrase-list').on('click', '.js-phrase-delete', async function () {

        let element = $(this).closest('.js-phrase-list-item');
        let original = element.data('_phrase');
        try {
            let key = prompt("Are you sure you want to delete the phrase? \n \n Retyping the key value of the phrase confirms deletion");
            if (key == null) return;
            if (key !== original.key) {
                throw new Error('Wrong confirmation key');
            }
            let response = await app.provider.post('internationalization/delete/' + original._id);
            element.remove();
        } catch (error) {
            app.error(error.statusText || error);
        }
    });

    $('#js-filter-suspect').click(function () {
        loadSuspicious();
    });

    $('#js-filter-deleted').click(function () {
        loadDeleted();
    });

    $('#js-filter-clear').click(function () {
        $('#js-filter-container form')[0].reset();
        initialLoad();
    });

    $('#js-filter-apply').click(function () {
        initialLoad();
    });

    $('#js-toggle-quick-edit').click(function () {
        if ($(this).attr('data-enabled') === 'true') {
            $(this).attr('data-enabled', 'false');
            $(this).css('color', '')
            addKeyBoardSupport(null, false);
        }
        else {
            $(this).attr('data-enabled', 'true');
            $(this).css('color', 'red')
            addKeyBoardSupport(null, true);
        }

    });

    $('#username, #password').keydown(function (e) {
        let key = e.keyCode || e.which;
        key == 13 && $('#js-login').click();
    });


    $('#js-start-tour').click(function () {

        localStorage.removeItem('tours.status.main');
        startTour();
    });
});
