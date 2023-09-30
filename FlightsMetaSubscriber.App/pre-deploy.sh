#!/bin/bash

USERNAME="postgres"

# Проверить наличие пользователя
if id -u $USERNAME >/dev/null 2>&1; then
  echo "User $USERNAME already exists."
else
  # Создать пользователя без возможности входа в систему
  sudo useradd --shell /usr/sbin/nologin $USERNAME
  echo "User $USERNAME created without login."
fi

# Массив с путями к директориям для проверки и создания
DIRECTORIES=("/logs/FlightsMetaSubscriber.App" "/psql-data/FlightsMetaSubscriber.App/pgdata")

# Перебрать массив директорий
for DIRECTORY in "${DIRECTORIES[@]}"; do
  # Проверить наличие папки
  if [ ! -d "$DIRECTORY" ]; then
    # Создать папку
    mkdir -p "$DIRECTORY"
    echo "Directory $DIRECTORY created."
    if [ "$DIRECTORY" = "/psql-data/FlightsMetaSubscriber.App/pgdata" ]; then
      chown "$USERNAME:$USERNAME" "$DIRECTORY"
      chmod 777 "$DIRECTORY"
      echo "Changed permissions and owner for $DIRECTORY"
    fi
  else
    echo "Directory $DIRECTORY already exists."
  fi
done
