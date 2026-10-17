# LocatCam - Serveur (API ASP.NET Core)

API REST pour l'application LocatCam — recherche de logement étudiant au Cameroun.

## 📁 Structure

```
LocatCam_serveur/
├── LocatCam.slnx                 → Solution
├── LocatCam.Api/                 → Projet API (Controllers, Middleware, Program.cs)
│   ├── Controllers/
│   ├── Middleware/
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── LocatCam.Api.http         → Collection d'endpoints pour tests
├── LocatCam.Core/                → Entités, DTOs, interfaces, enums, services métier
│   ├── Entities/
│   ├── Dtos/
│   ├── Interfaces/
│   ├── Services/
│   ├── Enums/
│   └── Validation/
├── LocatCam.Infrastructure/      → EF Core, DbContext, repositories, Cloudinary, JWT, Seed
│   ├── Data/
│   ├── Repositories/
│   ├── Services/
│   └── Seed/
├── LocatCam.Tests/               → Tests unitaires
├── Dockerfile
├── docker-compose.yml
└── README.md
```

## 🚀 Lancement

### Avec Docker

```bash
docker-compose up -d
```

L'API est accessible sur `http://localhost:5206`.

### Sans Docker

```bash
dotnet run --project LocatCam.Api/LocatCam.Api.csproj
```

## 🗄️ Base de données

- Migrations EF Core : `LocatCam.Infrastructure/Data/Migrations/`
- Seed automatique en environnement Development (`DataSeeder.cs`)

## 🔑 Endpoints principaux

| Méthode | Endpoint | Description | Auth |
|---------|----------|-------------|------|
| POST | `/api/auth/register` | Inscription | Non |
| POST | `/api/auth/login` | Connexion (JWT + Refresh) | Non |
| POST | `/api/auth/refresh` | Renouveler token | Non |
| GET | `/api/auth/me` | Profil connecté | JWT |
| GET | `/api/annonces` | Liste paginée + filtres | Non |
| GET | `/api/annonces/{id}` | Détail annonce | Non |
| POST | `/api/annonces` | Créer annonce | JWT (Bailleur) |
| PUT | `/api/annonces/{id}` | Modifier annonce | JWT (Propriétaire) |
| DELETE | `/api/annonces/{id}` | Supprimer annonce | JWT (Propriétaire) |
| GET | `/api/annonces/map` | Bounding box carte | Non |
| GET | `/api/annonces/mes-annonces` | Mes annonces | JWT (Bailleur) |
| POST | `/api/photos/signature` | Signature Cloudinary | Non |
| GET | `/api/favoris` | Mes favoris | JWT |
| POST | `/api/favoris/{id}` | Ajouter favori | JWT |
| DELETE | `/api/favoris/{id}` | Retirer favori | JWT |
| POST | `/api/messages` | Envoyer message | JWT |
| GET | `/api/messages/conversations` | Conversations | JWT |
| GET | `/api/messages/conversation/{id}` | Détail conversation | JWT |
| PUT | `/api/messages/{id}/lu` | Marquer lu | JWT |
| POST | `/api/signalements` | Signaler annonce | JWT |
| PUT | `/api/admin/annonces/{id}/verifier` | Vérifier annonce | JWT (Admin) |
| PUT | `/api/admin/users/{id}/bannir` | Bannir utilisateur | JWT (Admin) |

## 🧪 Tests

```bash
dotnet test LocatCam.Tests/LocatCam.Tests.csproj
```

## 📦 Technologies

- ASP.NET Core 10
- Entity Framework Core + PostgreSQL (Npgsql)
- JWT Bearer Authentication
- FluentValidation
- AutoMapper
- BCrypt.Net-Next
- Serilog (console + fichier)
- CloudinaryDotNet
- Swagger / OpenAPI (.NET 10 minimal API)

## 🚀 Déploiement sur VPS (Contabo)

### Prérequis
- Serveur VPS avec Ubuntu/Debian
- Accès root SSH
- Nom de domaine ou IP publique pointant vers le VPS

### Déploiement automatique (recommandé)

```bash
# Se connecter en root
ssh root@<IP_VPS>

# Télécharger le script de déploiement
wget https://raw.githubusercontent.com/TSEFACKDEV/LocatCam_server/main/deploy-vps.sh
chmod +x deploy-vps.sh

# Exécuter le déploiement
bash deploy-vps.sh
```

Le script fait :
1. Mise à jour du système
2. Création de l'utilisateur `locatcam`
3. Installation de Docker + Docker Compose
4. Configuration du pare-feu (UFW : ports 22, 5206, 5432)
5. Clonage du dépôt GitHub
6. Création du fichier `.env`
7. Lancement des conteneurs (PostgreSQL + API)
8. Test automatique des routes

### Test manuel des routes API

Après déploiement, exécuter depuis votre machine :

```bash
bash test-api.sh 169.58.69.36 5206
```

Ou tester individuellement avec curl :

```bash
# Swagger
curl http://169.58.69.36:5206/openapi/v1.json

# Register
curl -X POST http://169.58.69.36:5206/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"nom":"Test","email":"test@locatcam.cm","telephone":"+237 600 000 000","motDePasse":"Test@123","role":"Etudiant"}'

# Login
curl -X POST http://169.58.69.36:5206/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@locatcam.cm","motDePasse":"Test@123"}'

# Liste annonces
curl http://169.58.69.36:5206/api/annonces?page=1&pageSize=10
```

### Variables d'environnement (`.env`)

Copier `.env.example` vers `.env` et adapter les valeurs :

```bash
cp .env.example .env
```

Les clés Cloudinary sont déjà configurées :
- CloudName : `dgpt66w4z`
- ApiKey : `715983593278264`
- ApiSecret : `o1x6LrZvHXOmlg2hpcNwouZ1B1Y`

### Accès SSH

- Utilisateur : `locatcam`
- Répertoire : `/home/locatcam/locatcam`
- Commandes Docker : `sudo -u locatcam docker-compose ...`

### URLs

| Service | URL |
|---------|-----|
| API | `http://169.58.69.36:5206` |
| Swagger | `http://169.58.69.36:5206/openapi/v1.json` |
| PostgreSQL | `169.58.69.36:5432` |

