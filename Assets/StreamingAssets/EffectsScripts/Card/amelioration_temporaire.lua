---@type number
max_level = 2
---@type number
image_id = 526

---@type string
name = "Amélioration temporaire"
---@type number
pa_cost = 2

---@type string
description = "La carte à améliorer temporairement"

---@param aCard Card
local function card_filter(aCard)
	-- carte choisis aleatoirement depuis ton deck
	
	return EffectOwner.Hand.Contains(aCard)
			and This ~= aCard and not aCard.IsMaxLevel and aCard.CanBePlayed(EffectOwner)
end

---@type Target[]
targets = {
	CreateTarget("la premiere carte à améliorer temporairement", TargetTypes.Card, false, card_filter),
	CreateTarget("la deuxieme carte à améliorer temporairement", TargetTypes.Card, false, card_filter),
}

--- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
	local tailleMain = #EffectOwner.Hand
	for i = 1, tailleMain do
		if ((EffectOwner.Hand[i - 1].IsMaxLevel) ~= true) then
			return EffectOwner.Hand.Count >= 1 and TargetsExists({1})
		end
	end
end

function do_effect()
	local DiscardPile = EffectOwner.Discard
	local Hand = EffectOwner.Hand
	local card1 = (--[[---@type Card]] AskForTarget(1))
	
	card1.CurrentLevel.TryChangeValue(card1.CurrentLevel.Value + 1)
	Game.PlayCard(EffectOwner, card1, Hand, DiscardPile)
	card1.CurrentLevel.TryChangeValue(card1.CurrentLevel.Value - 1)
	
	if (This.CurrentLevel.Value == max_level) then
		-- lvl2
		--application des 2 effets cartes lvl2
		local card2 = (--[[---@type Card]] AskForTarget(2))
		
		card2.CurrentLevel.TryChangeValue(card2.CurrentLevel.Value + 1)
		Game.PlayCard(EffectOwner, card2, Hand, DiscardPile)
		card2.CurrentLevel.TryChangeValue(card2.CurrentLevel.Value - 1)
	
	end
end

---@param _ number
---@param new number
function on_level_change(_, new)
	if new == 2 then
		This.Description.TryChangeValue("Les 2 cartes à améliorer temporairement")
	end
end 