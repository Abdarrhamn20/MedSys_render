#!/bin/sh
set -e

if [ "$RUN_DATA_IMPORT" = "1" ]; then
  echo "==> [data-import] Applying clinic data to database from DATABASE_URL..."
  psql "$DATABASE_URL" -v ON_ERROR_STOP=1 -f /app/import.sql
  echo "==> [data-import] Import finished successfully."
fi

exec dotnet Backend.dll