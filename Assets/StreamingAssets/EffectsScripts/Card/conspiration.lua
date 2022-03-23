---@type number
max_level = 2
---@type number
image_id = 530

---@type ChainMode
chain_mode = ChainMode.MiddleChain

---@type string
name = "Conspiration"
---@type number
pa_cost = 3

---@type string
description = "Annule l'amélioration de la dernière carte adverse "

local function cardFilter()
	for card in --[[---@type fun:Card]]EffectOwner.OtherPlayer.Discard do
		if EffectOwner.OtherPlayer.Discard.IsMarkedForUpgrade(card) then
			return card
		end
	end
end

---@type Target[]
targets = {
	CreateTarget("la carte pour laquelle il faut annuler l'amélioration",TargetTypes.Card,true,cardFilter)
}

--- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
	for card in EffectOwner.OtherPlayer.Discard do
		if EffectOwner.OtherPlayer.Discard.IsMarkedForUpgrade(card) then
			return true
		end
	end
	return false
end

function do_effect()
	local carte = --[[---@type Card]] AskForTarget(1)
	EffectOwner.OtherPlayer.Discard.UnMarkForUpgrade(carte)
end

