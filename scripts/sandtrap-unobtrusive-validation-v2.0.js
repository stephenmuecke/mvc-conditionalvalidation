﻿sandtrapValidation = {
    getDependentElement: function (validationElement, dependentProperty) {
        var dependentElement = $('#' + dependentProperty);
        if (dependentElement.length === 1) {
            return dependentElement;
        }
        var name = validationElement.name;
        var index = name.lastIndexOf(".") + 1;
        var id = (name.substr(0, index) + dependentProperty).replace(/[\.\[\]]/g, "_");
        dependentElement = $('#' + id);
        if (dependentElement.length === 1) {
            return dependentElement;
        }
        // Try using the name attribute
        name = (name.substr(0, index) + dependentProperty);
        dependentElement = $('[name="' + name + '"]');
        if (dependentElement.length > 0) {
            return dependentElement.first();
        }
        return null;
    }
}

$.validator.addMethod("requiredif", function (value, element, params) {
    if ($(element).val() != '') {
        // The element has a value so its OK
        return true;
    }
    if (!params.dependentelement) {
        return true;
    }
    var dependentElement = $(params.dependentelement);
    if (dependentElement.is(':checkbox')) {
        var dependentValue = dependentElement.is(':checked') ? 'True' : 'False';
        return dependentValue != params.targetvalue;
    } else if (dependentElement.is(':radio')) {
        // If its a radio button, we cannot rely on the id attribute
        // So use the name attribute to get the value of the checked radio button
        var dependentName = dependentElement[0].name;
        dependentValue = $('input[name="' + dependentName + '"]:checked').val();
        return dependentValue != params.targetvalue;
    }
    return dependentElement.val() !== params.targetvalue;
});

$.validator.unobtrusive.adapters.add("requiredif", ["dependentproperty", "targetvalue"], function (options) {
    var element = options.element;
    var dependentproperty = options.params.dependentproperty;
    var dependentElement = sandtrapValidation.getDependentElement(element, dependentproperty);
    options.rules['requiredif'] = {
        dependentelement: dependentElement,
        targetvalue: options.params.targetvalue
    };
    options.messages['requiredif'] = options.message;
});

$.validator.addMethod("requiredifcontains", function (value, element, params) {
    if ($(element).val() != '') {
        // The element has a value so its OK
        return true;
    }
    if (!params.dependentelement) {
        return true;
    }
    var dependentElement = $(params.dependentelement);
    var targetValues = params.targetvalues;
    var dependentValue;
    if (dependentElement.is(':radio')) {
        // If its a radio button, we cannot rely on the id attribute
        // So use the name attribute to get the value of the checked radio button
        var dependentName = dependentElement[0].name;
        dependentValue = $('input[name="' + dependentName + '"]:checked').val().toLowerCase();
    } else {
        dependentValue = dependentElement.val().toLowerCase();
    }
    return $.inArray(dependentValue, targetValues) === -1;
});

$.validator.unobtrusive.adapters.add("requiredifcontains", ["dependentproperty", "targetvalues"], function (options) {
    var element = options.element;
    var dependentproperty = options.params.dependentproperty;
    var dependentElement = sandtrapValidation.getDependentElement(element, dependentproperty);
    options.rules['requiredifcontains'] = {
        dependentelement: dependentElement,
        targetvalues: options.params.targetvalues.split(',')
    };
    options.messages['requiredifcontains'] = options.message;
});

// FileSizeAttribute

$.validator.unobtrusive.adapters.add('filesize', ['maxsize'], function (options) {
    options.rules['filesize'] = { maxsize: options.params.maxsize };
    options.messages['filesize'] = options.message;
});

// FileTypeAttribute

$.validator.unobtrusive.adapters.add('filetype', ['validtypes'], function (options) {
    options.rules['filetype'] = { validtypes: options.params.validtypes.split(',') };
    options.messages['filetype'] = options.message;
});

$.validator.addMethod("filetype", function (value, element, param) {
    for (var i = 0; i < element.files.length; i++) {
        var extension = getFileExtension(element.files[0].name);
        if ($.inArray(extension, param.validtypes) === -1) {
            return false;
        }
    }
    return true;
});

function getFileExtension(fileName) {
    if (/[.]/.exec(fileName)) {
        return /[^.]+$/.exec(fileName)[0].toLowerCase();
    }
    return null;
}

$.validator.addMethod('filesize', function (value, element, param) {
    var maxSize = parseInt(param.maxsize);
    for (var i = 0; i < element.files.length; i++) {
        var filesize = parseInt(element.files[i].size);
        if (filesize > maxSize) {
            return false;
        }
    }
    return true;
});
