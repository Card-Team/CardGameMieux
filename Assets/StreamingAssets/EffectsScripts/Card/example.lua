---@type number
max_level = 2
---@type number
image_id = 500

---@type string
name = "Nom"
---@type number
pa_cost = 2

---@type string
description = "une description de la carte qui peut changer"

---@param a_card Card
local function card_filter(a_card)
	-- permet uniquement le ciblage de carte ayant comme nom 'Exemple'
	return a_card.Name == "Exemple"
end

---@type Target[]
targets = {
	-- Nom, Type, Automatique ou non,Fonction de filtre des cibles potentielles
	CreateTarget("Une cible carte", TargetTypes.Card, false, card_filter),
	CreateTarget("Un joueur", TargetTypes.Player, true),
}


-- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
    -- la carte peut être jouée sans aucun critère spécifiques
    return true
end

function do_effect()
    -- le code de l'effet de la carte
end

function on_level_change(oldLevel, newLevel)
    -- fonction appelée quand la carte change de niveau (OPTIONNEL)
end