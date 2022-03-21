---@type number
max_level = 2
---@type number
image_id = 636

---@type string
name = "Visionnaire"
---@type number
pa_cost = 3
--TODO a definir le nombre de cout

---@type ChainMode
chain_mode = ChainMode.StartChain

local base_description = "Cette carte vous permet de voir (précisément) les 2(->3) premieres cartes de votre deck"
description = base_description

local function cardFilter1()
	return EffectOwner.Deck[0]
end
local function cardFilter2()
	return EffectOwner.Deck[1]
end
local function cardFilter3()
	return EffectOwner.Deck[2]
end--[[---@type Card]]
---@type Target[]
targets = {
	CreateTarget("la premiere carte du deck", TargetTypes.Card, true, cardFilter1),
	CreateTarget("la deuxieme carte du deck", TargetTypes.Card, true, cardFilter2),
	CreateTarget("la troisieme carte du deck", TargetTypes.Card, true, cardFilter3)
}

--Visionnaire

function precondition()
	--si la carte est au niveau 1 ou 2
	if This.CurrentLevel.Value == 1 then
		return EffectOwner.Deck.Count >= 2
	else
		return EffectOwner.Deck.Count >= 3
	end
end

function do_effect()
	--voir les dernieres cartes
	if This.CurrentLevel.Value == 1 then
		local card1 = --[[---@type Card]] AskForTarget(1)            --la carte 1 Deck
		local card2 = --[[---@type Card]] AskForTarget(2)            --la carte 2
		Game.RevealCard(EffectOwner, card1)       -- fait voir les carte 1
		Game.RevealCard(EffectOwner, card2)       -- fait voir les carte 2
	else
		local card1 = --[[---@type Card]] AskForTarget(1)            --la carte 1 du Deck
		local card2 = --[[---@type Card]] AskForTarget(2)            --la carte 2 du Deck
		local card3 = --[[---@type Card]] AskForTarget(3)            --la carte 3
		Game.RevealCard(EffectOwner, card1)       -- fait voir les carte selectionner
		Game.RevealCard(EffectOwner, card2)
		Game.RevealCard(EffectOwner, card3)
	end
end

---@param _ number
---@param new number
function on_level_change(_, new)
	if (new == 1) then
		This.Description.TryChangeValue(base_description)
	else
		This.Description.TryChangeValue("Cette carte vous permet de voir (précisément) les 3 premiers cartes du deck")
	end
end