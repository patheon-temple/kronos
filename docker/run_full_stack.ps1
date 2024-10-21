docker compose -f docker-compose-infrastructure.yml -f docker-compose-api.yml -p pantheon down
docker compose -f docker-compose-infrastructure.yml -f docker-compose-api.yml -p pantheon up -d