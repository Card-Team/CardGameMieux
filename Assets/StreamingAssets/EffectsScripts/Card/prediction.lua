---@type number
max_level = 1
---@type number
image_id = 15

---@type ChainMode
chain_mode = ChainMode.NoChain

---@type string
name = "Prédiction"
---@type number
pa_cost = 2

---@type string
description = "Devine une carte de la main du joueur (parmis toutes ses cartes) si c'est la bonne elle va dans sa défausse"


local function cardFilter()
	local handCount = EffectOwner.OtherPlayer.Hand.Count
	local index = GetRandomNumber(0,handCount)
	return EffectOwner.OtherPlayer.Hand[index]
end

---@type Target[]
targets = {
	--cible avec la precondition qui renvoie faux s'il y'a aucune carte dans sa main
	CreateTarget("Devine une carte de la main du joueur", TargetTypes.Card, true, cardFilter),
}

function precondition()
	return EffectOwner.OtherPlayer.Hand.Count > 0
end

function do_effect()
	--la carte qui doit etre devinée
	local carte = --[[---@type Card]] AskForTarget(1)                                                      --carte
	
	print("Choisie : ")
	print(carte)
	
	--construction de la liste de toutes les cartes de l'adversaire
	---@type Card[]
	local cartes = {}
	---@type table<string,boolean>
	local effectIds = {}
	for card in --[[---@type fun:Card]] EffectOwner.OtherPlayer.Cards do
		print("loop card is ")
		print(card)
		if effectIds[card.EffectId] == nil then
			effectIds[card.EffectId] = true
			cartes[#cartes+1] = card.Virtual()
		end
	end
	print(#effectIds)
	print(#cartes)
	
	local chosen = Game.ChooseBetween(EffectOwner,cartes)

	if chosen.EffectId == carte.EffectId then
		print("good")
		EffectOwner.OtherPlayer.Hand.MoveTo(EffectOwner.OtherPlayer.Discard, carte,
				0)     --prends la carte de l'adversaire depuis la main et la met dans sa defausse
	else
		print("bad")
	end
end