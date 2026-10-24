-- ============================================================================
-- LOCATCAM — Mise à jour des annonces de démonstration
-- - Remplace toutes les images mortes par de vraies photos de logements (Pexels)
-- - Ajoute 8 annonces autour de la position actuelle de test :
--   Douala, environ 4.0483 / 9.7043 (Bonabéri, Makepe, Logbessou, ...)
-- Script idempotent : les annonces curatées ne sont pas dupliquées.
-- ============================================================================

BEGIN;

-- ----------------------------------------------------------------------------
-- 1. Remplacer les URLs des photos existantes par des images valides
-- ----------------------------------------------------------------------------
WITH urls AS (
    SELECT ARRAY[
        'https://images.pexels.com/photos/1643383/pexels-photo-1643383.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/271624/pexels-photo-271624.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/1571460/pexels-photo-1571460.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/1571463/pexels-photo-1571463.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/1457842/pexels-photo-1457842.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/106399/pexels-photo-106399.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/323780/pexels-photo-323780.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/259588/pexels-photo-259588.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/2102587/pexels-photo-2102587.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/276724/pexels-photo-276724.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/271816/pexels-photo-271816.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/262048/pexels-photo-262048.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/1329711/pexels-photo-1329711.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/1918291/pexels-photo-1918291.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/2029667/pexels-photo-2029667.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/2082087/pexels-photo-2082087.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/1648776/pexels-photo-1648776.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/1743229/pexels-photo-1743229.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/2062426/pexels-photo-2062426.jpeg?auto=compress&cs=tinysrgb&w=1200',
        'https://images.pexels.com/photos/3935350/pexels-photo-3935350.jpeg?auto=compress&cs=tinysrgb&w=1200'
    ]::text[] AS liste
),
numerotees AS (
    SELECT "Id", (row_number() OVER (ORDER BY "DateCreation", "Id"))::int AS rn
    FROM "AnnoncePhotos"
)
UPDATE "AnnoncePhotos" p
SET "UrlCloudinary" = u.liste[((n.rn - 1) % array_length(u.liste, 1)) + 1]
FROM numerotees n, urls u
WHERE p."Id" = n."Id";

-- ----------------------------------------------------------------------------
-- 2. Ajouter les annonces proches de la position actuelle (Douala)
-- ----------------------------------------------------------------------------
DO $$
DECLARE
    v_bailleur UUID;
    v_annonce  UUID;
    r          RECORD;
BEGIN
    SELECT "Id" INTO v_bailleur
    FROM "Users"
    WHERE "Role" = 1
    ORDER BY "Email"
    LIMIT 1;

    IF v_bailleur IS NULL THEN
        RAISE EXCEPTION 'Aucun compte Bailleur trouvé : impossible d''insérer les annonces.';
    END IF;

    FOR r IN
        SELECT * FROM (VALUES
            ('Chambre meublée à Bonabéri — Proche marché',
             'Chambre propre et meublée à Bonabéri, proche du marché et des transports. Eau et électricité incluses. Idéal pour étudiant.',
             45000::numeric, 0, 'Bonabéri', 'Douala', 'Bonabéri, Douala', 4.0490::double precision, 9.7060::double precision, true, 1,
             ARRAY[
                'https://images.pexels.com/photos/271624/pexels-photo-271624.jpeg?auto=compress&cs=tinysrgb&w=1200',
                'https://images.pexels.com/photos/262048/pexels-photo-262048.jpeg?auto=compress&cs=tinysrgb&w=1200',
                'https://images.pexels.com/photos/2082087/pexels-photo-2082087.jpeg?auto=compress&cs=tinysrgb&w=1200'
             ]::text[]),

            ('Studio moderne à Bonabéri — Tout équipé',
             'Studio moderne avec kitchenette, salle de bain et coin nuit. Quartier calme, sécurisé, proche des commerces.',
             75000::numeric, 1, 'Bonabéri', 'Douala', 'Bonabéri, Douala', 4.0460, 9.7020, true, 1,
             ARRAY[
                'https://images.pexels.com/photos/1643383/pexels-photo-1643383.jpeg?auto=compress&cs=tinysrgb&w=1200',
                'https://images.pexels.com/photos/1457842/pexels-photo-1457842.jpeg?auto=compress&cs=tinysrgb&w=1200',
                'https://images.pexels.com/photos/6585758/pexels-photo-6585758.jpeg?auto=compress&cs=tinysrgb&w=1200'
             ]::text[]),

            ('Appartement T2 meublé à Makepe',
             'Bel appartement T2 meublé avec salon, cuisine équipée et balcon. Résidence sécurisée à Makepe.',
             150000::numeric, 2, 'Makepe', 'Douala', 'Makepe, Douala', 4.0520, 9.7130, true, 2,
             ARRAY[
                'https://images.pexels.com/photos/1571460/pexels-photo-1571460.jpeg?auto=compress&cs=tinysrgb&w=1200',
                'https://images.pexels.com/photos/276724/pexels-photo-276724.jpeg?auto=compress&cs=tinysrgb&w=1200',
                'https://images.pexels.com/photos/3935350/pexels-photo-3935350.jpeg?auto=compress&cs=tinysrgb&w=1200'
             ]::text[]),

            ('Studio cosy à Logbessou',
             'Studio cosy et lumineux à Logbessou, proche des écoles et de la route principale. Disponible immédiatement.',
             90000::numeric, 1, 'Logbessou', 'Douala', 'Logbessou, Douala', 4.0630, 9.7150, false, 1,
             ARRAY[
                'https://images.pexels.com/photos/2102587/pexels-photo-2102587.jpeg?auto=compress&cs=tinysrgb&w=1200',
                'https://images.pexels.com/photos/1643383/pexels-photo-1643383.jpeg?auto=compress&cs=tinysrgb&w=1200',
                'https://images.pexels.com/photos/1918291/pexels-photo-1918291.jpeg?auto=compress&cs=tinysrgb&w=1200'
             ]::text[]),

            ('Appartement T3 à Bonamoussadi',
             'Appartement T3 spacieux avec 2 chambres, salon, cuisine américaine et balcon. Quartier résidentiel prisé.',
             220000::numeric, 2, 'Bonamoussadi', 'Douala', 'Bonamoussadi, Douala', 4.0720, 9.7300, false, 3,
             ARRAY[
                'https://images.pexels.com/photos/1571463/pexels-photo-1571463.jpeg?auto=compress&cs=tinysrgb&w=1200',
                'https://images.pexels.com/photos/2029667/pexels-photo-2029667.jpeg?auto=compress&cs=tinysrgb&w=1200',
                'https://images.pexels.com/photos/276724/pexels-photo-276724.jpeg?auto=compress&cs=tinysrgb&w=1200'
             ]::text[]),

            ('Maison 3 chambres à Bonabéri',
             'Maison familiale avec 3 chambres, salon, cuisine, 2 salles de bain et cour. Quartier calme et sécurisé.',
             300000::numeric, 3, 'Bonabéri', 'Douala', 'Bonabéri, Douala', 4.0356, 9.7333, true, 3,
             ARRAY[
                'https://images.pexels.com/photos/106399/pexels-photo-106399.jpeg?auto=compress&cs=tinysrgb&w=1200',
                'https://images.pexels.com/photos/259588/pexels-photo-259588.jpeg?auto=compress&cs=tinysrgb&w=1200',
                'https://images.pexels.com/photos/3935350/pexels-photo-3935350.jpeg?auto=compress&cs=tinysrgb&w=1200'
             ]::text[]),

            ('Chambre étudiante à Deido',
             'Chambre simple et sécurisée à Deido, proche des universités et des transports en commun. Loyer accessible.',
             35000::numeric, 0, 'Deido', 'Douala', 'Deido, Douala', 4.0437, 9.7683, false, 1,
             ARRAY[
                'https://images.pexels.com/photos/1743229/pexels-photo-1743229.jpeg?auto=compress&cs=tinysrgb&w=1200',
                'https://images.pexels.com/photos/1329711/pexels-photo-1329711.jpeg?auto=compress&cs=tinysrgb&w=1200',
                'https://images.pexels.com/photos/6585758/pexels-photo-6585758.jpeg?auto=compress&cs=tinysrgb&w=1200'
             ]::text[]),

            ('Appartement meublé à Bonapriso',
             'Appartement meublé haut standing à Bonapriso : salon, cuisine équipée, chambre climatisée. Proche restaurants et commerces.',
             250000::numeric, 2, 'Bonapriso', 'Douala', 'Bonapriso, Douala', 4.0511, 9.7679, true, 2,
             ARRAY[
                'https://images.pexels.com/photos/1648776/pexels-photo-1648776.jpeg?auto=compress&cs=tinysrgb&w=1200',
                'https://images.pexels.com/photos/1918291/pexels-photo-1918291.jpeg?auto=compress&cs=tinysrgb&w=1200',
                'https://images.pexels.com/photos/2062426/pexels-photo-2062426.jpeg?auto=compress&cs=tinysrgb&w=1200'
             ]::text[])
        ) AS v(titre, description, prix, type, quartier, ville, adresse, latitude, longitude, meuble, pieces, images)
    LOOP
        IF NOT EXISTS (SELECT 1 FROM "Annonces" WHERE "Titre" = r.titre) THEN
            v_annonce := gen_random_uuid();

            INSERT INTO "Annonces" (
                "Id", "Titre", "Description", "Prix", "Type", "Quartier", "Ville", "Adresse",
                "Latitude", "Longitude", "EstMeuble", "NombrePieces", "Disponible",
                "UserId", "EstVerifie", "Statut", "DateCreation"
            ) VALUES (
                v_annonce, r.titre, r.description, r.prix, r.type, r.quartier, r.ville, r.adresse,
                r.latitude, r.longitude, r.meuble, r.pieces, true,
                v_bailleur, true, 1, NOW()
            );

            INSERT INTO "AnnoncePhotos" (
                "Id", "AnnonceId", "UrlCloudinary", "PublicId", "Ordre", "EstPrincipale", "DateCreation"
            )
            SELECT
                gen_random_uuid(),
                v_annonce,
                img.url,
                'seed/' || regexp_replace(lower(r.titre), '[^a-z0-9]+', '-', 'g') || '-' || img.ord,
                (img.ord - 1)::int,
                img.ord = 1,
                NOW()
            FROM unnest(r.images) WITH ORDINALITY AS img(url, ord);
        END IF;
    END LOOP;
END $$;

COMMIT;

-- Contrôle : annonces proches de la position actuelle (rayon ~5 km)
SELECT "Titre", "Quartier", "Prix", "Latitude", "Longitude"
FROM "Annonces"
WHERE "Ville" = 'Douala'
  AND "Latitude" BETWEEN 4.00 AND 4.10
  AND "Longitude" BETWEEN 9.66 AND 9.75
ORDER BY "Prix";
