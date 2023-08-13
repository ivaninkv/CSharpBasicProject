# Проектная работа по курсу C# Basic

[![Open in Dev Containers](https://img.shields.io/static/v1?label=Dev%20Containers&message=Open&color=blue&logo=visualstudiocode)](https://vscode.dev/redirect?url=vscode://ms-vscode-remote.remote-containers/cloneInVolume?url=https://github.com/ivaninkv/CSharpBasicProject)

Для запуска проекта в Dev Container нужно создать `.env` файл в директории [FlightsMetaSubscriber.App](FlightsMetaSubscriber.App) с переменными окружения. Пример файла [здесь](FlightsMetaSubscriber.App/.env_example).

Для того, чтобы создать структуру БД при первом запуске, выполните в консоли следующую команду:
```bash
psql -h postgres -U postgres -d fms_db -f FlightsMetaSubscriber.App/psql-init/DDL.sql
```

Бот позволяет подписаться на перелеты из нескольких исходных городов, на диапазон дат и в несколько целевых городов. Из всех возможных вариантов, выбирается самый дешевый и раз в сутки пользователю отправляется цена и ссылка на билет.

Узнать больше информации и попробовать бот в работе можно [здесь](https://telegra.ph/Bot-po-poisku-aviabiletov-03-16).
