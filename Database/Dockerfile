FROM postgres:16

# Configuración solicitada
ENV POSTGRES_DB=flexirooms
ENV POSTGRES_USER=postgres
ENV POSTGRES_PASSWORD=FlexiRooms2026!

# Puerto por defecto de PostgreSQL
EXPOSE 5432

# Persistencia de datos
VOLUME ["/var/lib/postgresql/data"]

# Healthcheck para verificar disponibilidad de la DB
HEALTHCHECK --interval=10s --timeout=5s --retries=5 CMD pg_isready -U postgres -d flexirooms || exit 1
