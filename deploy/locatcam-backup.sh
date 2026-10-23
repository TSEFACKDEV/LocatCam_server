#!/usr/bin/env bash
# Sauvegarde quotidienne de la base LocatCam (format pg_dump custom, compressé).
set -euo pipefail

BACKUP_DIR="${BACKUP_DIR:-/var/backups/locatcam}"
RETENTION_DAYS="${RETENTION_DAYS:-14}"
CONTAINER="${CONTAINER:-locatcam-db}"

mkdir -p "$BACKUP_DIR"
chmod 700 "$BACKUP_DIR"

stamp="$(date -u +%Y%m%dT%H%M%SZ)"
target="$BACKUP_DIR/locatcam_${stamp}.dump"

docker exec "$CONTAINER" pg_dump -U postgres -d locatcam -Fc > "$target"
chmod 600 "$target"

# Vérifie que le dump est lisible avant de considérer la sauvegarde valide.
docker exec -i "$CONTAINER" pg_restore --list < "$target" > /dev/null

find "$BACKUP_DIR" -type f -name 'locatcam_*.dump' -mtime "+${RETENTION_DAYS}" -delete

echo "$(date -u +%FT%TZ) sauvegarde OK: $target"
