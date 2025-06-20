# Effet de Glow/Neon pour les Unités - System Conqueror

## Description

Système d'effet de glow/neon pour les unités en transit, avec une couleur correspondant à celle du propriétaire.

## Fonctionnalités

### ✅ **Effet de Glow Principal**

-   **Émission du matériau** : Utilise l'émission du shader pour créer un effet lumineux
-   **Couleur dynamique** : La couleur correspond à celle du propriétaire de l'unité
-   **Intensité configurable** : Paramètre ajustable dans l'inspecteur

### ✅ **Effet de Glow sur le Texte**

-   **Texte lumineux** : Le nombre d'unités a aussi un effet de glow
-   **Couleur cohérente** : Même couleur que l'unité
-   **Intensité séparée** : Paramètre distinct pour le texte

### ✅ **Effet de Glow Alternatif**

-   **Sprite enfant** : Crée un sprite plus grand et semi-transparent
-   **Positionnement automatique** : Se place derrière l'unité
-   **Échelle configurable** : Taille relative à l'unité

### ✅ **Animation de Pulsation (Optionnelle)**

-   **Pulsation continue** : L'effet de glow pulse doucement
-   **Paramètres ajustables** : Vitesse, échelle min/max
-   **Activation optionnelle** : Peut être désactivée

### ✅ **Intégration avec le Fog of War**

-   **Masquage automatique** : L'effet de glow est masqué hors champ de vision
-   **Cohérence visuelle** : Respecte les règles de visibilité du jeu

## Configuration

### 1. **Ajouter le composant UnitGlowSettings**

-   Créez un GameObject vide dans votre scène
-   Ajoutez le composant `UnitGlowSettings`
-   Configurez les paramètres dans l'inspecteur

### 2. **Paramètres disponibles**

#### **Glow Effect Settings**

-   `Enable Glow` : Active/désactive l'effet de glow
-   `Glow Intensity` : Intensité de l'émission (défaut: 2.0)
-   `Glow Scale` : Échelle de l'effet de glow alternatif (défaut: 1.5)
-   `Glow Transparency` : Transparence de l'effet (défaut: 0.6)
-   `Enable Text Glow` : Active/désactive le glow du texte
-   `Text Glow Intensity` : Intensité du glow du texte (défaut: 1.5)

#### **Animation Settings**

-   `Enable Pulse Animation` : Active l'animation de pulsation
-   `Pulse Speed` : Vitesse de la pulsation (défaut: 2.0)
-   `Pulse Min Scale` : Échelle minimale (défaut: 1.3)
-   `Pulse Max Scale` : Échelle maximale (défaut: 1.7)

## Utilisation

### **Automatique**

-   L'effet se déclenche automatiquement quand des unités sont envoyées
-   La couleur est automatiquement celle du propriétaire
-   L'effet respecte le fog of war

### **Personnalisation**

-   Modifiez les paramètres dans `UnitGlowSettings`
-   Les changements s'appliquent aux nouvelles unités
-   Redémarrez le jeu pour voir les changements

## Exemples de Configuration

### **Effet Subtil**

```
Glow Intensity: 1.0
Glow Scale: 1.2
Glow Transparency: 0.4
Enable Pulse Animation: false
```

### **Effet Dramatique**

```
Glow Intensity: 3.0
Glow Scale: 2.0
Glow Transparency: 0.8
Enable Pulse Animation: true
Pulse Speed: 3.0
```

### **Effet Désactivé**

```
Enable Glow: false
Enable Text Glow: false
```

## Compatibilité

-   ✅ **Unity 2022.3+**
-   ✅ **Fog of War** : Intégration complète
-   ✅ **Performance** : Optimisé pour de nombreuses unités
-   ✅ **Shaders** : Compatible avec les shaders standards

## Dépannage

### **L'effet ne s'affiche pas**

1. Vérifiez que `UnitGlowSettings` est présent dans la scène
2. Vérifiez que `Enable Glow` est activé
3. Vérifiez que le shader supporte l'émission

### **Performance dégradée**

1. Réduisez `Glow Intensity`
2. Désactivez `Enable Pulse Animation`
3. Réduisez `Glow Scale`

### **Couleur incorrecte**

1. Vérifiez que le propriétaire de l'unité a une couleur définie
2. Vérifiez que `Player.Color` est correctement assigné
