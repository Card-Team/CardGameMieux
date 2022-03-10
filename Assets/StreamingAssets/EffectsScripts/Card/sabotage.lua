---@type number
max_level = 1
---@type number
image_id = 529

---@type string
name = "Sabotage"
---@type number
pa_cost = 3

---@type string
description = "La carte défausser pour saboter votre ennemi"

local function card_filter(aCard)
	-- carte choisis aleatoirement depuis ton deck
	return EffectOwner.Hand.Contains(aCard)
			and This ~= aCard
end

local function card_filter2(aCard)
	-- carte choisis aleatoirement depuis ton deck
	
	return EffectOwner.OtherPlayer.Discard.Contains(aCard) 

end

---@type Target[]
targets = {
	CreateTarget("La carte que l'on veut défausser", TargetTypes.Card, false, card_filter),
	CreateTarget("la carte de l'ennemie dont on veut enlever le marquage", TargetTypes.Card, false, card_filter2),
}

--- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition() 
	return EffectOwner.Hand.Count > 0 and EffectOwner.OtherPlayer.Discard.Count > 0 and TargetsExists({1,2})
end

function do_effect()
	local DiscardPile = EffectOwner.OtherPlayer.Discard
	local Hand = EffectOwner.Hand
	local card1 = (--[[---@type Card]] AskForTarget(1))
	local card2 = (--[[---@type Card]] AskForTarget(2))
	
	if (card1.CurrentLevel == card2.CurrentLevel) then
		-- lvl2
		Hand.MoveTo(DiscardPile, card1, 1) --On défausse la carte card1
		DiscardPile.UnMarkForUpgrade(card2) -- On enlever le marquage de la carte ennemi
	
    end

end 