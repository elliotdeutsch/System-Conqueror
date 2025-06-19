# Fog of War - System Conqueror

## Description

Le système de Fog of War a été implémenté pour masquer les planètes qui ne sont pas directement connectées aux planètes conquises par le joueur.

## Fonctionnement

### Mode Activé (par défaut)

-   Seules les planètes conquises par le joueur et leurs voisines directes sont visibles
-   Les planètes hors champ de vision sont complètement masquées (pas de sprite, pas de texte)
-   Les lignes de connexion vers les planètes masquées sont également cachées

### Mode Désactivé

-   Toutes les planètes sont visibles
-   Toutes les informations (nom, nombre d'unités) sont affichées

## Contrôles

### Pendant le jeu

-   **Touche F** : Bascule entre les modes activé/désactivé
-   L'état actuel est affiché dans la console Unity

### Configuration initiale

-   Dans l'écran de configuration, cochez/décochez "Show Far Stars" pour définir l'état initial
-   Par défaut, le fog of war est **activé** (showFarStars = false)

## Modifications apportées

### Star.cs

-   `SetVisibility()` : Masque complètement les étoiles non visibles (sprite désactivé)
-   `Conquer()` : Met à jour le fog of war après une conquête

### GalaxyManager.cs

-   `showFarStars` : Valeur par défaut changée à `false`
-   `UpdateFogOfWar()` : Améliorée avec des logs de debug

### PlayerController.cs

-   `HandleFogOfWarToggle()` : Gestion de la touche F
-   `UpdateFogOfWarDisplay()` : Affichage de l'état

### LineVisibility.cs

-   `UpdateVisibility()` : Masque complètement les lignes vers les étoiles invisibles

### FogOfWarUI.cs

-   Nouveau script pour afficher l'état du fog of war dans l'UI

## Utilisation

1. **Démarrage** : Le fog of war est activé par défaut
2. **Pendant le jeu** : Appuyez sur F pour basculer
3. **Conquête** : Le fog of war se met à jour automatiquement quand vous conquérez une nouvelle planète

## Debug

-   Les logs dans la console Unity indiquent l'état du fog of war
-   Le nombre d'étoiles visibles est affiché en temps réel
