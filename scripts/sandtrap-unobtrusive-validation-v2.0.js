sandtrapValidation = {
    getDependantElement: function (validationElement, dependentProperty) {
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
    var dependentElement = sandtrapValidation.getDependantElement(element, dependentproperty);
    options.rules['requiredifcontains'] = {
        dependentelement: dependentElement,
        targetvalues: options.params.targetvalues.split(',')
    };
    options.messages['requiredifcontains'] = options.message;
});
