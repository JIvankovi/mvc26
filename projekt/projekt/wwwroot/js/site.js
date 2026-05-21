// Custom application behaviors for Lab 4
// Search, autocomplete dropdown, date/time control and client validation.

window.lab4 = {
    debounce: function (fn, delay) {
        var timer = null;
        return function () {
            var args = arguments;
            clearTimeout(timer);
            timer = setTimeout(function () {
                fn.apply(null, args);
            }, delay);
        };
    },

    initSearch: function (inputSelector, containerSelector, url) {
        var $input = $(inputSelector);
        var $container = $(containerSelector);
        if (!$input.length || !$container.length) {
            return;
        }

        var renderResults = function () {
            var term = $input.val();
            $container.addClass('lab4-loading');

            $.get(url, { term: term })
                .done(function (html) {
                    $container.html(html);
                    lab4.animateUpdate($container);
                })
                .fail(function () {
                    $container.html('<div class="alert alert-danger">Server search failed. Please try again.</div>');
                })
                .always(function () {
                    $container.removeClass('lab4-loading');
                });
        };

        var executeSearch = lab4.debounce(renderResults, 260);
        $input.on('input', executeSearch);
        $(inputSelector + '-clear').on('click', function () {
            $input.val('');
            executeSearch();
            $input.focus();
        });
    },

    initAutocompleteDropdowns: function () {
        $('.autocomplete-dropdown').each(function () {
            var $wrapper = $(this);
            var $hidden = $wrapper.find('.autocomplete-hidden');
            var $textHidden = $wrapper.find('.autocomplete-selected-text');
            var $input = $wrapper.find('.autocomplete-input');
            var $results = $wrapper.find('.autocomplete-results');
            var $message = $wrapper.find('.field-validation-valid');
            var url = $wrapper.data('autocomplete-url');
            var required = $wrapper.data('required') === true || $wrapper.data('required') === 'true';

            if (!$input.length || !$hidden.length || !url) {
                return;
            }

            function setSelection(id, text) {
                $hidden.val(id);
                $textHidden.val(text);
                $input.val(text);
                $message.text('');
            }

            function showValidationMessage(text) {
                $message.text(text);
            }

            function clearSelection() {
                $hidden.val('');
                $textHidden.val('');
                $input.val('');
                $results.empty().hide();
                if (required) {
                    showValidationMessage('Please select a valid item from the list.');
                }
            }

            function renderItems(items) {
                $results.empty();
                if (!items || items.length === 0) {
                    $results.html('<div class="autocomplete-empty p-2 text-muted">No results found.</div>').show();
                    return;
                }

                items.forEach(function (item) {
                    var caption = item.name || 'Unknown';
                    var detailText = item.manufacturer || item.email || item.location || item.buildingCode || '';
                    var html = '<button type="button" class="autocomplete-item list-group-item list-group-item-action py-2" data-id="' + item.id + '" data-text="' + caption + '">';
                    html += '<strong>' + caption + '</strong>';
                    if (detailText) {
                        html += '<div class="text-muted small">' + detailText + '</div>';
                    }
                    html += '</button>';
                    $results.append(html);
                });
                $results.show();
            }

            var fetchResults = lab4.debounce(function () {
                var query = $input.val().trim();
                if (query.length < 1) {
                    $results.empty().hide();
                    if (required && !$hidden.val()) {
                        showValidationMessage('Please select a valid item from the list.');
                    }
                    return;
                }

                $.get(url, { term: query })
                    .done(function (data) {
                        renderItems(data);
                    })
                    .fail(function () {
                        $results.html('<div class="autocomplete-empty p-2 text-danger">Unable to load suggestions.</div>').show();
                    });
            }, 180);

            $input.on('input', function () {
                fetchResults();
                $hidden.val('');
                $textHidden.val('');
            });

            $wrapper.on('click', '.autocomplete-item', function () {
                var $item = $(this);
                setSelection($item.data('id'), $item.data('text'));
                $results.empty().hide();
            });

            $wrapper.find('.autocomplete-clear').on('click', function () {
                clearSelection();
            });

            $input.on('blur', function () {
                setTimeout(function () {
                    if (!$wrapper.find(':focus').length) {
                        $results.hide();
                        if (required && !$hidden.val()) {
                            showValidationMessage('Please select a valid item from the list.');
                        }
                    }
                }, 120);
            });

            if ($textHidden.val() && !$input.val()) {
                $input.val($textHidden.val());
            }
        });
    },

    initDateTimeControls: function () {
        $('.datetime-control').each(function () {
            var $wrapper = $(this);
            var $hidden = $wrapper.find('.datetime-hidden');
            var $date = $wrapper.find('.datetime-date');
            var $time = $wrapper.find('.datetime-time');
            var $validation = $wrapper.find('.datetime-validation');
            var required = $wrapper.data('required') === true || $wrapper.data('required') === 'true';
            var locale = navigator.language || 'en-US';
            var isHr = locale.toLowerCase().startsWith('hr');

            var placeholder = isHr ? 'dd.mm.yyyy' : 'mm/dd/yyyy';
            $date.attr('placeholder', placeholder);

            if ($hidden.val()) {
                var parsed = new Date($hidden.val());
                if (!isNaN(parsed.getTime())) {
                    $date.val(lab4.formatLocaleDate(parsed, locale));
                    $time.val(lab4.twoDigits(parsed.getHours()) + ':' + lab4.twoDigits(parsed.getMinutes()));
                }
            }

            function validate() {
                var dateText = $date.val().trim();
                var timeText = $time.val().trim();

                if (!dateText && !timeText) {
                    if (required) {
                        $hidden.val('');
                        $validation.text('Please provide a valid date and time.');
                    } else {
                        $hidden.val('');
                        $validation.text('');
                    }
                    return;
                }

                var parsed = lab4.parseDateTime(dateText, timeText, isHr);
                if (!parsed || isNaN(parsed.getTime())) {
                    $hidden.val('');
                    $validation.text('Invalid date or time format.');
                    return;
                }

                $hidden.val(parsed.toISOString());
                $validation.text('');
            }

            $date.on('blur', validate);
            $time.on('blur', validate);
        });
    },

    formatLocaleDate: function (date, locale) {
        return new Intl.DateTimeFormat(locale, { day: '2-digit', month: '2-digit', year: 'numeric' }).format(date);
    },

    twoDigits: function (value) {
        return value < 10 ? '0' + value : value;
    },

    parseDateTime: function (dateText, timeText, isHrLocale) {
        if (!dateText) {
            return null;
        }

        var dateParts = dateText.split(/[\.\-/\s]+/).filter(Boolean);
        if (dateParts.length !== 3) {
            return null;
        }

        var day = 0;
        var month = 0;
        var year = 0;

        if (isHrLocale) {
            day = parseInt(dateParts[0], 10);
            month = parseInt(dateParts[1], 10);
            year = parseInt(dateParts[2], 10);
        } else {
            month = parseInt(dateParts[0], 10);
            day = parseInt(dateParts[1], 10);
            year = parseInt(dateParts[2], 10);
        }

        if (!year || !month || !day) {
            return null;
        }

        if (!timeText) {
            timeText = '00:00';
        }

        var timeParts = timeText.split(':').filter(Boolean);
        if (timeParts.length !== 2) {
            return null;
        }

        var hours = parseInt(timeParts[0], 10);
        var minutes = parseInt(timeParts[1], 10);
        if (isNaN(hours) || isNaN(minutes) || hours < 0 || hours > 23 || minutes < 0 || minutes > 59) {
            return null;
        }

        return new Date(year, month - 1, day, hours, minutes);
    },

    attachBlurValidation: function () {
        $('form').on('blur', 'input, select, textarea', function () {
            if (typeof $(this).valid === 'function') {
                $(this).valid();
            }
        });
    },

    animateUpdate: function ($container) {
        $container.addClass('lab4-updated');
        setTimeout(function () {
            $container.removeClass('lab4-updated');
        }, 600);
    }
};

$(function () {
    lab4.initAutocompleteDropdowns();
    lab4.initDateTimeControls();
    lab4.attachBlurValidation();
});
