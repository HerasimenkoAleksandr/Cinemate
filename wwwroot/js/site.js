﻿
    document.addEventListener('DOMContentLoaded', () => {
        // Обработчик для формы аутентификации
        const authForm = document.getElementById('authForm');
        if (authForm)
        {
            authForm.addEventListener('submit', handleFormSubmit);
        }

        const commentForm = document.getElementById('commentForm');
        if (commentForm) {
            commentForm.addEventListener('submit', handleCommentFormSubmit);
        }

        const deleteCommentForm = document.getElementById('deleteComment');
        if (deleteCommentForm) {
            deleteCommentForm.addEventListener('submit', handleDeleteCommentFormSubmit);
        }
        // Обработчик для формы добавления в избранное
        const favoriteButton = document.getElementById('favoriteButton');
        if (favoriteButton) {
            favoriteButton.addEventListener('click', handleFavoriteFormSubmit);
        }

        const deleteMovieFromFavorites = document.getElementById('favoriteButtonDelete');
        if (deleteMovieFromFavorites) {
            deleteMovieFromFavorites.addEventListener('click', handleDeleteMovieFromFavoritesSubmit);
        }

        const deleteAuthButton = document.getElementById('deleteAuthUser');
        if (deleteAuthButton) {
            deleteAuthButton.addEventListener('click', signOutUser);
        }
        const banMovieForm = document.getElementById('banMovieForm');
        console.log('Form loaded:', banMovieForm); // Логування форми
        if (banMovieForm) {
            banMovieForm.addEventListener('submit', (event) => {
                event.preventDefault();
                const formData = new FormData(banMovieForm);     
                const action = event.submitter.value
                console.log('Action:', action); 
                if (action === 'ban') {
                    handleBan(event);
                } else if (action === 'unban') {
                    handleUnban(event);
                }
            });
        }
    });

    function handleUnban(event) {
    event.preventDefault(); // Предотвращаем стандартное действие формы

    // Получаем значение из поля idMovieBan
    const movieId = document.getElementById('idMovieBan').value.trim();
    if (!movieId) {
        console.error("Фільм не знайдено");
        return;
    }

    const url = `/api/movies/${movieId}/unban`;

    fetch(url, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json'

        }
    })
        .then(response => {
            return response.json();
            if (response.ok) {
                return response.json(); // Если успешно, возвращаем JSON ответ
            } else {
                throw new Error('Сталася помилка');
            }
        })
        .then(data => {
            console.log(data.message); 
            document.getElementById('resultBanMessage').textContent = "Фільм розблоковано!";
            document.getElementById('idMovieBan').value = ''; // Очищаем значение поля
        })
        .catch(error => {
            console.error('Ошибка:', error); // Обработка ошибки
            document.getElementById('resultBanMessage').textContent = "Сталася помилка";
        });
}

    function handleBan(event) {
    event.preventDefault(); // Предотвращаем стандартное действие формы

    // Получаем значение из поля idComment
    const movieId = document.getElementById('idMovieBan').value.trim();
    if (!movieId) {
        console.error("Фільм не знайдено");
        return;
    }

    const url = `/api/movies/${movieId}/ban`;

    fetch(url, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json'

        }
    })
        .then(response => {
            return response.json();
            if (response.ok) {
                return response.json(); // Если успешно, возвращаем JSON ответ
            } else {
                throw new Error('Сталася помилка');
            }
        })
        .then(data => {
            console.log(data.message); // Выводим сообщение об успешном удалении
            document.getElementById('resultBanMessage').textContent = "Фільм заблоковано!";
            document.getElementById('idMovieBan').value = ''; // Очищаем значение поля
        })
        .catch(error => {
            console.error('Ошибка:', error); // Обработка ошибки
            document.getElementById('resultBanMessage').textContent = "Сталася помилка";
        });
}
    function signOutUser() {

    const url = `/api/auth`;
    fetch(url, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json"
            // Добавьте другие заголовки, если необходимо, например, для аутентификации
        }
    })
        .then(response => {
            if (response.ok) {
                // Успешный выход, обработайте его
                return response.json();
            } else {
                throw new Error("Ошибка при выходе");
            }
        })
        .then(data => {
            // Здесь можно обработать успешный ответ, например:
            console.log(data.status); // "OK"
            // Перенаправление на главную страницу или обновление интерфейса
            window.location.href = '/'; // Перенаправление на главную страницу
        })
        .catch(error => {
            console.error("Произошла ошибка:", error);
            // Обработка ошибок (например, показать уведомление пользователю)
        });
}
    function handleDeleteMovieFromFavoritesSubmit(event) {
    event.preventDefault(); // Предотвращаем стандартное действие формы

    const movieId = "5DF3E4B9-F8D1-4E0A-82DD-012444EFF7E5";
  
    // Вызываем функцию удаления комментария
    deleteMovieFromFavorites(movieId);
}
    function deleteMovieFromFavorites(movieId) {
    const url = `/api/favoritemovies/${movieId}`;

    fetch(url, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            if (response.ok) {
                return response.json(); // Если успешно, возвращаем JSON ответ
            } else {
                throw new Error('Помилка при видаленні');
            }
        })
        .then(data => {
            console.log(data.message); // Выводим сообщение об успешном удалении
            document.getElementById('resultDeleteMovieFromFavoritesMessage').textContent = "Видалено фільм з обраних!";

            // Дополнительно можно удалить элемент комментария из DOM
            // document.getElementById(`comment-${commentId}`).remove(); // Пример удаления
        })
        .catch(error => {
            console.error('Ошибка:', error); // Обработка ошибки
            document.getElementById('resultDeleteMovieFromFavoritesMessage').textContent = "Помилка при видаленні";
        });
}
    function handleFavoriteFormSubmit(event) {
    event.preventDefault(); // Предотвращаем стандартное поведение формы

    // Получаем ID фильма из поля ввода
    const movieId = "5DF3E4B9-F8D1-4E0A-82DD-012444EFF7E5";

    // Создаем объект для отправки данных
    const formData = new FormData();
    formData.append('movieId', movieId);

    // Отправляем POST-запрос на сервер
        fetch(`/api/FavoriteMovies`, {
        method: 'POST',
        body: formData
    })
      .then(response => {
          if (!response.ok) {
              return response.text().then(text => {
                  throw new Error(`Network response was not ok: ${text}`);
              });
          }
          return response.json();
    })
    .then(data => {
        document.getElementById('favoriteResponse').textContent = JSON.stringify(data, null, 2);
    })
    .catch(error => {
        console.error('Ошибка:', error);
        document.getElementById('favoriteResponse').textContent = 'Ошибка добавления фильма в избранное.';
    });
    }
    function handleDeleteCommentFormSubmit(event) {
            event.preventDefault(); // Предотвращаем стандартное действие формы

            // Получаем значение из поля idComment
            const commentId = document.getElementById('idComment').value;
            if (!commentId) {
                console.error("Комментарий не найден");
                return;
            }

            // Вызываем функцию удаления комментария
            deleteComment(commentId);
        }
    function deleteComment(commentId) {
            const url = `/api/comment/${commentId}`;

            fetch(url, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => {
                    if (response.ok) {
                        return response.json(); // Если успешно, возвращаем JSON ответ
                    } else {
                        throw new Error('Помилка при видаленні');
                    }
                })
                .then(data => {
                    console.log(data.message); // Выводим сообщение об успешном удалении
                    document.getElementById('resultDeleteCommentMessage').textContent = "Коментар видалено!";

                    // Дополнительно можно удалить элемент комментария из DOM
                    // document.getElementById(`comment-${commentId}`).remove(); // Пример удаления
                })
                .catch(error => {
                    console.error('Ошибка:', error); // Обработка ошибки
                    document.getElementById('resultDeleteCommentMessage').textContent = "Помилка при видаленні";
                });
        }
    function handleCommentFormSubmit(event) {
            event.preventDefault(); // Предотвращаем стандартное поведение формы

            const commentId = document.getElementById("commentId").value; // Получаем текст комментария
            const movieId = "5DF3E4B9-F8D1-4E0A-82DD-012444EFF7E5"; // ID фильма (должен быть установлен)

            // Получаем данные формы
            const formData = new FormData();
            formData.append("IdMovie", movieId); // Добавляем ID фильма
            formData.append("Comment", commentId); // Добавляем текст комментария
            // Отправляем POST-запрос на сервер
            fetch('/api/comment',
                {
                    method: 'POST',
                    body: formData
                })
                .then(response => {
                    if (!response.ok) {
                        return response.text().then(text => {
                            throw new Error(`Network response was not ok: ${text}`);
                        });
                    }
                    return response.json();
                })
                .then(data => {
              
                    document.getElementById("commentId").value = '';
                    document.getElementById('commentresponse').textContent = JSON.stringify(data, null, 2);
                   
                })
                .catch(error => {
                    console.error('Error:', error);
                    document.getElementById('responseMessage').innerText = 'An error occurred.';
                });
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
                    document.getElementById('likeCount').textContent = JSON.stringify(data, null, 2);
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
    
    updateButtonStates(); // Инициализация состояния кнопок при загрузке страницы
        
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
                        if (error.status === 404) {
                            // Если статус 404, выводим сообщение от API
                            $(outputId).html('<p>' + error.responseText + '</p>');
                        } else {
                            // В остальных случаях выводим общее сообщение об ошибке
                            $(outputId).html('<p>An error occurred</p>');
                        }
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
            loadData('#loadComment', ' /api/comment/5DF3E4B9-F8D1-4E0A-82DD-012444EFF7E5', '#commentsAll');
            loadData('#loadFavoriteMovies', ' /api/FavoriteMovies', '#resultFavoriteMovies');
            loadData('#loadMoviesFromStatusBlokce', ' api/movies/banned', '#moviesFromStatusBlokce');
        });
    

