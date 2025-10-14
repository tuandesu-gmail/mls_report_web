app.filter('YesNo', function () {
    return function (value) {
        if (value === "C") {
            return value;
        }

        return "";
    };
});

app.filter('Padding', function () {
    return function (value, index) {
        for (var i = 1; i < index; i++) {
            value = " " + value;
        }

        return value;
    };
});

