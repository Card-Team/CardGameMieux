---@type number
max_level = 1
---@type number
image_id = 518

---@type string
name = "Echange inversé"
---@type number
pa_cost = 2

---@type ChainMode
chain_mode = ChainMode.StartChain

---@type string
description = "Echange cette carte avec une carte aléatoire dans votre défausse."

local function card_filter()
	-- Verifier quand pas de carte dans la défausse
	local DiscardPile = EffectOwner.Discard
	local random = GetRandomNumber(0,DiscardPile.Count)
	return DiscardPile[random]
end

---@type Target[]
targets = {
	CreateTarget("la carte avec laquelle elle s'échange dans la défausse", TargetTypes.Card, true, card_filter),
}

--- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
	return EffectOwner.Discard.Count > 0
end

function do_effect()
	local theCard = --[[---@type Card]] AskForTarget(1)

	local posFirst = EffectOwner.Discard.IndexOf(theCard)
	local posSec = EffectOwner.Hand.IndexOf(This)
	EffectOwner.Discard.MoveTo(EffectOwner.Hand, theCard, posSec)
	EffectOwner.Hand.MoveTo(EffectOwner.Discard, This, posFirst)
end


