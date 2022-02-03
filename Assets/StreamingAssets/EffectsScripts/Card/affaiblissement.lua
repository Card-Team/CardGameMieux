---@type number
---@type number
max_level = 1
---@type number
image_id = 523

---@type string
name = "affaiblissement"
---@type number
pa_cost = 3

---@type string
description = "Fait baisser le coût d'action de l'adversaire avec le cout d'une carte aleatoire de ton deck"

local function card_filter()
	-- carte choisis aleatoirement depuis ton deck
	local CardPile = EffectOwner.Deck
	local random = math.random(0, CardPile.Count - 1)
	return CardPile[random]
end

---@type Target[]
targets = {
	CreateTarget("points d'action baisser aleatoirement", TargetTypes.Card, true, card_filter),
}

--- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
	return EffectOwner.Deck.Count > 0
end

local coutCard = 0

---@param startEvent StartTurnEvent
---@param ecouteurs IEventHandler
local function baisserPointAction(startEvent, ecouteurs)
	local pointActionAdv = startEvent.Player.ActionPoints.Value                   --point d'action de l'adversaire
	startEvent.Player.ActionPoints.TryChangeValue(math.max(pointActionAdv - coutCard,
														   0))--commence l'evenement au prochain tour : il baissera ses points d'action apres le prochain tour
	UnsubscribeTo(ecouteurs)
end

function do_effect()
	--prends le cout de CardPile est enleve le nombre de pints d'action a l'edversaire
	local carte = --[[---@type Card]] AskForTarget(1)
	coutCard = carte.Cost.Value                                         --cout de la carte
	SubscribeTo(StartTurnEvent, baisserPointAction, false,
				true)                      --s'abonne a l'evenement (debut de tour,la fonction execute une fois que l'evenement est la,es qu'on ecpute une evenement anulé,es que tu t'abone apres)
end