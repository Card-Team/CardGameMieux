--immutable data
---@type number
max_level = 3
---@type number
image_id = 512

--mutable data
---@type string
name = "Montgolfière"
---@type number
pa_cost = 2

---@type ChainMode
chain_mode = ChainMode.StartOrMiddleChain

local base_description = "Remonte une carte de 1 place dans le deck."
description = base_description

local function card_filter(a_card)
	return EffectOwner.Deck.Contains(a_card)
			and
			(
					This.CurrentLevel.Value == max_level
							or
							EffectOwner.Deck.IndexOf(a_card) > 0
			)
end

---@type Target[]
targets = {
	CreateTarget("La carte à remonter", TargetTypes.Card, false, card_filter)
}
---@type number[]
local nb_to_move = { 1, 2, 3 }

function precondition()
	return TargetsExists({ 1 })
end

function do_effect()
	local theCard = --[[---@type Card]] AskForTarget(1)
	
	local currentPos = EffectOwner.Deck.IndexOf(theCard)
	
	local newPos = currentPos - nb_to_move[This.CurrentLevel.Value]
	if (newPos < 0 and This.CurrentLevel.Value == max_level) then
		EffectOwner.Deck.MoveTo(EffectOwner.Hand, theCard, 0)
	else
		EffectOwner.Deck.MoveInternal(theCard, math.max(0, newPos))
	end
end

---@param old number
---@param new number
function on_level_change(old, new)
	if (new == 1) then
		This.Description.TryChangeValue(base_description)
	else
		local desc = "Remonte une carte de " .. nb_to_move[new] .. " places dans le deck."
		if (new == max_level) then
			desc = desc .. "\nSi la carte doit dépasser le haut du deck, vous la piochez"
		end
		This.Description.TryChangeValue(desc)
	end
end 