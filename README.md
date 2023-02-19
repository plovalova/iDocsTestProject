# iDocsTestProject

Реализован основной функционал, который описан в задании
<br>
Запуск приложения через docker (linux контейнеры):
<br>
```bash
./iDocsTestProject/iDocsTestProject docker-compose build
./iDocsTestProject/iDocsTestProject docker-compose run
```
Данные по док-ту хранятся в MSSQL, сам файл в MongoDB (используется gridfs для разделения файла на чанки)
<br>
Для сброса пароля небходимо получить токен из api/Identity/ForgotPassword и подать его в api/Identity/ResetPassword
<br>
Добавлен endpoint /api/Users/GetRecievers (получение списка пользователей) - для удобного создания документа
<br>
Добавлен Swagger localhost:8080/swagger
<br>
Добавлена отправка логов в ElasticSearch и Kibana для их просмотра - localhost:5601
