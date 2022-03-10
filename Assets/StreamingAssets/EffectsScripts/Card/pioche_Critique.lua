---@type number
max_level = 3

---@type number
image_id = 564

---@type string
name = "Pioche Critique"
---@type number
pa_cost = 2

local base_description = "Cette carte te permet d'en piocher une de ton deck"
description = base_description

local function card_filter()
	local Deck = EffectOwner.Deck
	return Deck[0]
end

---@type Target[]
targets = {
	CreateTarget("La carte piochée", TargetTypes.Card, true, card_filter),
}

-- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
	return EffectOwner.Deck.Count >0
end

---@param CardEvent CardPlayEvent
---@param ecouteurs IEventHandler
local function baisserCout(CardEvent, ecouteurs)
	local CoutCard = CardEvent.Card.Cost.Value                   --point d'action de l'adversaire
	CardEvent.Card.Cost.TryChangeValue(math.max(1, CoutCard
														   ))--commence l'evenement au prochain tour : il baissera ses points d'action apres le prochain tour
	UnsubscribeTo(ecouteurs)
end

function do_effect()
	local carte = --[[---@type Card]] AskForTarget(1)
	
	if (This.CurrentLevel == max_level) then
		local desc = "La carte est jouable et son cout est réduit de 1 "
		carte.Cost.TryChangeValue(carte.Cost.Value - 1)
		EffectOwner.Deck.MoveTo(EffectOwner.Hand,carte,1)
		SubscribeTo(CardPlayEvent, baisserCout, false,
					true)
	end
	if (This.CurrentLevel == 2) then
		local desc = "La carte est jouable avec son cout initial "
		EffectOwner.Deck.MoveTo(EffectOwner.Hand,carte,1)
		SubscribeTo(CardPlayEvent, baisserCout, false,
					true)
	
	else
		local desc = "La carte est jouable avec un cout augmenté de 1"
		carte.Cost.TryChangeValue(carte.Cost.Value + 1)
		EffectOwner.Deck.MoveTo(EffectOwner.Hand,carte,1)
		SubscribeTo(CardPlayEvent, baisserCout, false,
					true)
	end
end