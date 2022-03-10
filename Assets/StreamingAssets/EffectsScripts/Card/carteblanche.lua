---@type number
max_level = 1

---@type number
image_id = 517

---@type string
name = "Carte Blanche"
---@type number
pa_cost = 2

local base_description = "Cette carte jouer l'effet d'une carte de ta main"
description = base_description

---@param a_card Card
local function card_filter(a_card)
	return EffectOwner.Hand.Contains(a_card)
			and a_card.Name.Value ~= This.Name.Value
			and not string.match(a_card.EffectId,"^_")
end

---@type Target[]
targets = {
	CreateTarget("jouer un effet d'une carte de ta main", TargetTypes.Card, false, card_filter),
}

---@type Card
carte_copie = --[[---@type Card]] nil

-- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
	local targ = TargetsExists({ 1 })
	if carte_copie == nil then
		return targ
	else
		return carte_copie.CanBePlayed(EffectOwner)
	end
end

function do_effect()
	if (carte_copie == nil) then
		local carte = --[[---@type Card]] AskForTarget(1)
		
		---@type Card
		carte_copie = carte.Virtual(This)         --creer une copie virtuel de la carte ciblé
		
		This.Cost.TryChangeValue(carte_copie.Cost.Value)
		This.Description.TryChangeValue("Carte Blanche : " .. carte_copie.Description.Value)
		This.Name.TryChangeValue(carte_copie.Name.Value)
		This.ImageId.TryChangeValue(carte_copie.ImageId.Value)
		return false    --pour pas carteblanche se fasse defaussé (A REVOIR !!)
	else
		Game.PlayCardEffect(EffectOwner, carte_copie)
		This.Description.TryChangeValue(base_description)
		This.Name.TryChangeValue(name)
		This.Cost.TryChangeValue(pa_cost)
		This.ImageId.TryChangeValue(image_id)
		---@type Card
		carte_copie = --[[---@type Card]] nil
	end
end

