---@type number
max_level = 6
---@type number
image_id = 628

---@type string
name = "Transfert"
---@type number
pa_cost = 3

---@type ChainMode
chain_mode = ChainMode.StartOrMiddleChain

---@type string
description = "Transfère le niveau de cette carte à une autre"

--fonction qui recupere toute les cartes de sa main sauf celle ci est lui demande d'en selectionné une
---@param aCard Card
local function cardFilter(aCard)
	return EffectOwner.Hand.Contains(aCard)
			and This ~= aCard and not aCard.IsMaxLevel
end

---@type Target[]
targets = {
	CreateTarget("Transferer niveau", TargetTypes.Card, false, cardFilter),
}

function precondition()
	local tailleMain = #EffectOwner.Hand
	for i = 1, tailleMain do
		if ((EffectOwner.Hand[i-1].IsMaxLevel) ~= true 
		) then
			return EffectOwner.Hand.Count > 0
		
		end
	end
	return true
end

function do_effect()
	
	local card = --[[---@type Card]] AskForTarget(1)--la carte
	local NiveauTransfert = This.CurrentLevel.Value
	if(card.CurrentLevel.Value + NiveauTransfert > card.MaxLevel) then --Verifie si l'ajout du niveau ne dépasse pas le niveau maximal
		card.CurrentLevel.TryChangeValue(card.MaxLevel)
	else
		card.CurrentLevel.TryChangeValue(card.CurrentLevel.Value + NiveauTransfert)
	end
end

