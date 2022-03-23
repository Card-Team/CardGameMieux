---@type number
max_level = 1
---@type number
image_id = 613

---@type string
name = "Extracteur"
---@type number
pa_cost = 1
--TODO a definir le nombre de cout

---@type ChainMode
chain_mode = ChainMode.StartOrMiddleChain

---@type string
description = "Gagner les points d'action d'une carte de ta main"

--fonction qui recupere toute les cartes de sa main sauf celle ci est lui demande d'en selectionné une
local function cardFilter(aCard)
	return EffectOwner.Hand.Contains(aCard)
			and This ~= aCard
end

---@type Target[]
targets = {
	CreateTarget("gagner ses points d'action", TargetTypes.Card, false, cardFilter),
}

--Extracteur  : Carte qui permet au joueur de choisir parmi une de ses cartes en main et d'en gagner le cout en points d'action.
--La carte choisit retourne en bas du deck et devient lourde jusqu à prochaine pioche. Lv1 -> Cout max de la carte : 2, Lv2-> Cout max de la carte : 3, lv3 -> Cout max de la carte : 4 "

function precondition()
    return TargetsExists({ 1 })
end

function do_effect()
	--on recupere les points d'action de la carte choisis et on modifie grace a TryChangeValue son cout actuelle
	local carte = --[[---@type Card]] AskForTarget(1)
	local coutCard = carte.Cost.Value                     --cout de la carte 
	local pointAction = EffectOwner.ActionPoints.Value               --point d'action du joueur
	EffectOwner.ActionPoints.TryChangeValue(pointAction + coutCard)  --ajout du cout de la carte sur les points d'action
	EffectOwner.Hand.MoveTo(EffectOwner.Discard, carte, 0)
end