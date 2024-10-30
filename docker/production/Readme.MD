# You should be at the correct directory where docker compose is executed afterward

docker run -it --rm   -v "$(pwd)/certbot/conf:/etc/letsencrypt"   -v "$(pwd)/certbot/www:/var/www/certbot"   -p 80:80   certbot/certbot certonly   --standalone   --preferred-challenges http   --email dev@obert.cz   --agree-tos   --no-eff-email   -d pantheon.obert.cz
