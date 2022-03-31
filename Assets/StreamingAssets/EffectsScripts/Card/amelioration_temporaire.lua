---@type number
max_level = 2
---@type number
image_id = 17

---@type ChainMode
chain_mode = ChainMode.NoChain

---@type string
name = "Amélioration temporaire"
---@type number
pa_cost = 2

local base_description ="Au niveau 1, joue une carte avec un niveau supplémentaire\nAu niveau 2, joue deux cartes un niveau supplémentaire."
---@type string
description = base_description

---@param aCard Card
local function card_filter(aCard)
	-- carte choisis aleatoirement depuis ton deck
	local premPart = EffectOwner.Hand.Contains(aCard)
			and This ~= aCard and not aCard.IsMaxLevel
			and aCard.EffectId ~= "joker" -- sinon boucle
			and aCard.EffectId ~= "amelioration_temporaire" -- sinon boucle
	print("premier : " .. tostring(premPart))
	if not premPart then
		return false
	end
	local secPart = aCard.CanBePlayed(EffectOwner)
	print("deuxieme : " .. tostring(secPart))
	return premPart and secPart
end

---@type Target[]
targets = {
	CreateTarget("la première carte à améliorer temporairement", TargetTypes.Card, false, card_filter),
	CreateTarget("la deuxième carte à améliorer temporairement", TargetTypes.Card, false, card_filter),
}

--- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
	local tailleMain = #EffectOwner.Hand
	for i = 1, tailleMain do
		if ((EffectOwner.Hand[i - 1].IsMaxLevel) ~= true) then
			return EffectOwner.Hand.Count >= 1 and TargetsExists({1})
		end
	end
	return false
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
	else
		This.Description.TryChangeValue(base_description)
	end
end 