using System.Collections.Generic;
using UnityEngine;

/* 
Ce script permet de générer des noms d'étoiles de manière 
procédurale en combinant des préfixes et des suffixes prédéfinis.
La méthode GenerateStarName choisit aléatoirement un préfixe et 
un suffixe dans leurs listes respectives, puis les combine pour créer un nom 
unique. Cette fonctionnalité est utile pour créer des noms variés et 
intéressants pour les étoiles dans un jeu, ajoutant ainsi de la diversité et de l'immersion.
*/

public class StarNameGenerator : MonoBehaviour
{
    private List<string> prefixes = new List<string>
{
    "Acamar", "Achemar", "Achird", "Acrux", "Acubens", "Adhafera", "Adhil", "Ain", "Al Athfar",
    "Al Bali", "Al Dhanab", "Al Gieba", "Al Giedi", "Al Haud", "Al Na'ir", "Al Nair", "Al Niyat",
    "Al Reschia", "Al Thalimain", "Aladfar", "Albireo", "Alchibah", "Alcor", "Alcyone", "Aldebaran",
    "Alderamin", "Alfirk", "Algenib", "Algol", "Algorab", "Alioth", "Alkaid", "Alkalurops", "Alkes",
    "Almach", "Almeisan", "Alnilam", "Alnitak", "Alphard", "Alpheratz", "Alsafi", "Alshain", "Altair",
    "Alterf", "Aludra", "Alula Australis", "Alula Borealis", "Alya", "Ancha", "Andromeda", "Angetenar",
    "Ankaa", "Antares", "Antlia", "Apus", "Aquarius", "Aquila", "Ara", "Arcturus", "Aries", "Arkab",
    "Arneb", "Arrakis", "Ascella", "Asellus Astralius", "Asellus Borealis", "Asmidiske", "Asterope",
    "Atik", "Atlas", "Atria", "Auriga", "Auva", "Avior", "Azelfafage", "Azha", "Baham", "Baten Kaitos",
    "Beid", "Bellatrix", "Betelgeuse", "Bootes", "Botein", "Caelum", "Camelopardalis", "Cancer",
    "Canes Venatici", "Canis Major", "Canis Minor", "Canopus", "Capella", "Caph", "Capricornus", "Carina",
    "Cassiopeia", "Castor", "Celaeno", "Centaurus", "Cepheus", "Cetus", "Chamaeleon", "Chara", "Cheleb",
    "Choo or Qu", "Circinus", "Columba", "Coma Berenices", "Cor Caroli", "Corona Australis",
    "Corona Borealis", "Corvus", "Coxa", "Crater", "Crux", "Cujam", "Cursa", "Cygnus", "Dabih", "Delphinus",
    "Deneb", "Deneb Algiedi", "Deneb or Al Dhanab al Dulfim", "Denebola", "Diphda", "Dorado", "Draco",
    "Dschubba", "Dubhe", "Dziban", "Edasich", "El Nasl", "El Nath", "Electra", "Eltanin", "Enif",
    "Equuleus", "Er Rai", "Eridanus", "Fomalhaut", "Fornax", "Fum al Samakah", "Furud", "Gacrux",
    "Gemini", "Gemma", "Giauzar", "Gienah", "Gomeisa", "Graffias", "Grumium", "Grus", "Hadar", "Hamal",
    "Heka", "Hercules", "Heze", "Homam", "Horologium", "Hydra", "Hydrus", "Indus", "Jabbah",
    "Jabhat al Akrab", "Kabdhilinan", "Kaffaljidhmah", "Kaus Australis", "Kaus Borealis", "Kaus Media",
    "Ke Kuan", "Keid", "Kerhah", "Kitalpha", "Kochab", "Kornephoros", "Kraz", "Kuma", "Lacerta", "Leo",
    "Leo Minor", "Lepus", "Lesath", "Libra", "Lupus", "Lynx", "Lyra", "Maaz", "Maia", "Marfik", "Markab",
    "Massim", "Matar", "Mebsuta", "Megrez", "Mekbuda", "Men", "Menkalinan", "Menkar", "Menkent",
    "Menkib", "Mensa", "Merak", "Merope", "Mesarthim", "Miaplacidus", "Microscopium", "Mimosa",
    "Minhar al Shuja", "Minkar", "Mintaka", "Mira", "Mirach", "Mirak", "Mirfak", "Mirzam", "Mizar",
    "Monoceros", "Mothallah", "Muhlifain", "Muliphein", "Mulu-lizi", "Muphrid", "Musca", "Muscida",
    "Na'ir al Saif", "Naos", "Nashira", "Nekkar", "Nihal", "Nodus Secundus", "Norma", "Nunki", "Nusakan",
    "Octans", "Ophiuchus", "Orion", "Pavo", "Pegasus", "Perseus", "Phact", "Phad", "Pherkab", "Phoenix",
    "Pictor", "Pisces", "Piscis Austrinus", "Pleadies", "Pleione", "Polaris", "Pollux", "Porrima",
    "Praecipula", "Prijipati", "Primus Hyadum", "Procyon", "Propus", "Puppis", "Pyxis", "Rana",
    "Ras Algethi", "Ras Alhague", "Ras Elased", "Rasalas", "Rastaban", "Regulus", "Reticulum", "Rigel",
    "Rigel Kentaurus", "Rotanev", "Ruchba", "Ruchbah", "Rukbat", "Sabik", "Sadachbia", "Sadal Melik",
    "Sadal Suud", "Sadalbari", "Sadatoni", "Sadr", "Sagitta", "Sagittarius", "Sargas", "Sarin", "Scheat",
    "Schedar", "Schemali", "Scorpius", "Sculptor", "Scutulum", "Scutum", "Segin", "Serpens", "Sextans",
    "Shaula", "Sheliak or Shelyak", "Sheratan", "Signus", "Sirius", "Situla", "Skat", "Spica", "Sualocin",
    "Suhail", "Sulafat", "Syrma", "Talitha Australis", "Talitha Borealis", "Tania Australis",
    "Tania Borealis", "Tarazed", "Tarf", "Taurus", "Taygeta", "Tegmine", "Tejat", "Telescopium",
    "Thabit", "The Garnet Star", "The Peacock Star", "Thuban", "Tian Ke", "Triangulum",
    "Triangulum Australe", "Tsih", "Tucana", "Tyl", "Ukdah", "Unuk Al Hay", "Ursa Major", "Ursa Minor",
    "Vega", "Vela", "Vindemiatrix", "Virgo", "Volans", "Vulpecula", "Wasat", "Wezen", "Wezn", "X-1",
    "Yed Posterior", "Yed Prior", "Yildun", "Zaniah", "Zaurak", "Zavijava", "Zhou", "Zi", "Zi Ceng",
    "Zozma", "Zuben El Genubi", "Zuben Eschamali", "Zubenhakrabi"
};
    private List<string> suffixes = new List<string> { "ari", "os", "ion", "a", "or", "us", "ius", "ix" };

    public string GenerateStarName()
    {
        string prefix = prefixes[Random.Range(0, prefixes.Count)];
        string suffix = suffixes[Random.Range(0, suffixes.Count)];
        return prefix/*  + suffix */;
    }
}
