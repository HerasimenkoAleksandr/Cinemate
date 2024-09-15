
    document.addEventListener('DOMContentLoaded', () => {
        // Обработчик для формы аутентификации
        const authForm = document.getElementById('authForm');
        if (authForm)
        {
            authForm.addEventListener('submit', handleFormSubmit);
        }
        function handleFormSubmit(event)
        {
        event.preventDefault(); // Предотвращаем стандартное поведение формы

         // Получаем данные формы
         const formData = new FormData(event.target);
         // Отправляем POST-запрос на сервер
            fetch('/api/auth',
                {
                method: 'POST',
                body: formData
                 })
                .then(response =>
                {
                      if (!response.ok)
                      {
                          return response.text().then(text =>
                          {
                             throw new Error(`Network response was not ok: ${text}`);
                          });
                      }
                  return response.json();
                 })
                    .then(data => {
                        if (data.status === 'OK')
                        {
                         window.location.href = '/'; // Перенаправление на главную страницу
                        }
                        else
                        {
                         document.getElementById('responseMessage').innerText = 'Login failed: ' + data.status;
                        }})
                .catch(error =>
                {
                    console.error('Error:', error);
                    document.getElementById('responseMessage').innerText = 'An error occurred.';
                });
        }

    // Обработчики кнопок "Like" и "Dislike"
    const likeButton = document.getElementById('likeButton');
    const dislikeButton = document.getElementById('dislikeButton');

        if (likeButton && dislikeButton)
        {
            likeButton.addEventListener('click', () =>
            {
                 handleLikeOrDislike(likeButton, true, false);
            });

            dislikeButton.addEventListener('click', () =>
            {
                 handleLikeOrDislike(dislikeButton, false, true);
            });
        }

        function handleLikeOrDislike(button, isLiked, isDisliked) {
            const movieId = button.dataset.movieId;
            const requestBody = {
                MovieId: movieId,
                IsLiked: isLiked,
                IsDisliked: isDisliked
            };

            console.log('Request body:', JSON.stringify(requestBody));
            fetch('/api/likes/toggle-like', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestBody)
            })
                .then(response => response.json())
                .then(data => {
                    console.log('Success:', data);
                    updateButtonStates(); // Обновите состояния кнопок
                })
                .catch(error => {
                    console.error('Error:', error);
                });
        }


    function updateButtonStates() {
            //const movieId = likeButton ? likeButton.dataset.movieId : null;
        const movieId = likeButton.dataset.movieId;
    if (movieId) {
        fetch(`/api/likes/get-status?movieId=${movieId}`, {
            method: 'GET'
        })
            .then(response => response.json())
            .then(data => {
                document.getElementById('likeresult').textContent = JSON.stringify(data, null, 2);
                console.log('Success:', data);
            })
            .catch(error => {
                console.error('Error:', error);
            });
            }
        }

    // Инициализация состояния кнопок при загрузке страницы
    updateButtonStates();

    // jQuery обработчики для AJAX запросов
    $(document).ready(function () {
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

            // Привязка обработчиков к кнопкам
            loadData('#loadCategories', '/api/Categories', '#categories');
    loadData('#loadSubCategories', '/api/Categories/61D20472-5017-48B9-B976-07E36A396A95', '#subcategories');
    loadData('#loadMoviesFromCategories', '/api/movies?categoryId=61D20472-5017-48B9-B976-07E36A396A95', '#moviesFromCategories');
    loadData('#loadMoviesFromSubCategories', '/api/movies?subcategoryId=0966A6CA-BE90-4AFF-BE04-CF4EFF71236E', '#moviesFromSubCategories');
        loadData('#loadMovie', '/api/movies/5DF3E4B9-F8D1-4E0A-82DD-012444EFF7E5', '#movie');
        });
    });

