---@type number
max_level = 1
---@type number
image_id = 523

---@type ChainMode
chain_mode = ChainMode.NoChain


---@type string
name = "Amélioration Débug"
---@type number
pa_cost = 0

---@type string
description = "Augmente le niveau d'une carte de 5"

---@param a_card Card
local function card_filter(a_card)
	return not a_card.IsMaxLevel
end

---@type Target[]
targets = {
	CreateTarget("Carte à augmenter", TargetTypes.Card, false, card_filter),
}

--- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
	return TargetsExists({ 1 })
end

function do_effect()
	
	
	local up = Game.MakeVirtual("Augmentation", "augmentation 5")
	local down = Game.MakeVirtual("Diminution", "diminution 5")
	
	local chosen = Game.ChooseBetween(EffectOwner, { up, down })
	
	Game.PlayCardEffect(EffectOwner, chosen)
	
	local toUp = --[[---@type Card]] AskForTarget(1)
	local newLevel = toUp.CurrentLevel.Value
	if chosen == up then
		newLevel = math.min(newLevel + 5, toUp.MaxLevel)
	else
		newLevel = math.max(newLevel - 5, 0)
	end
	if (newLevel ~= toUp.CurrentLevel.Value) then
		toUp.CurrentLevel.TryChangeValue(newLevel)
	end
	return false
end