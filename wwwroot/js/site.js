$(document).ready(function () {
    // Function to handle AJAX requests and update the corresponding element
    function loadData(buttonId, url, outputId) {
        $(buttonId).click(function () {
            $.ajax({
                url: url,
                method: 'GET',
                success: function (data) {
                    $(outputId).html('<p>' + JSON.stringify(data, null, 2) + '</p>');
                },
                error: function (error) {
                    $(outputId).html('<p>An error occurred</p>');
                }
            });
        });
    }

    // Attach event handlers to buttons
    loadData('#loadCategories', '/api/Categories', '#categories');
    loadData('#loadSubCategories', '/api/Categories/61D20472-5017-48B9-B976-07E36A396A95', '#subcategories');
    loadData('#loadMoviesFromCategories', '/api/movies?categoryId=61D20472-5017-48B9-B976-07E36A396A95', '#moviesFromCategories');
    loadData('#loadMoviesFromSubCategories', '/api/movies?subcategoryId=0966A6CA-BE90-4AFF-BE04-CF4EFF71236E', '#moviesFromSubCategories');
    loadData('#loadMovie', '/api/movies/5DF3E4B9-F8D1-4E0A-82DD-012444EFF7E5', '#movie');
});

