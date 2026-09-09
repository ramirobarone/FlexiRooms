# Docker image de PostgreSQL para FlexiRooms

Este Dockerfile crea una imagen de PostgreSQL con:

- Base de datos: `flexirooms`
- Usuario: `postgres`
- Password fija: `FlexiRooms2026!` (configuración solicitada)
- Volumen de datos: `/var/lib/postgresql/data`

## Build

```bash
docker build -t <tu-usuario-dockerhub>/flexirooms-postgres:latest -f Database/Dockerfile .
```

## Run

```bash
docker run -d \
  --name flexirooms-postgres \
  -p 5432:5432 \
  -v flexirooms-postgres-data:/var/lib/postgresql/data \
  <tu-usuario-dockerhub>/flexirooms-postgres:latest
```

## Connection string para tu aplicación

```text
Host=localhost;Port=5432;Database=flexirooms;Username=postgres;Password=FlexiRooms2026!;
```

## Nota de seguridad

Para producción se recomienda usar variables de entorno en runtime y no dejar credenciales fijas en la imagen.
