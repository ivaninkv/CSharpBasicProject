#!/bin/bash

# Массив с путями к директориям для проверки и создания
DIRECTORIES=("/logs/FlightsMetaSubscriber.App")

# Перебрать массив директорий
for DIRECTORY in "${DIRECTORIES[@]}"; do
  # Проверить наличие папки
  if [ ! -d "$DIRECTORY" ]; then
    # Создать папку
    mkdir -p "$DIRECTORY"
    echo "Directory $DIRECTORY created."
  else
    echo "Directory $DIRECTORY already exists."
  fi
done
