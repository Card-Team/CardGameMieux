---@type number
max_level = 1

---@type number
image_id = 528

---@type string
name = "Sacrifice"
---@type number
pa_cost = 2

---@type ChainMode
chain_mode = ChainMode.StartChain


local base_description = "Enlever le marquage d'évolution d'une de vos cartes et réduisez le cout d'une carte de votre main"
description = base_description

local function card_filter(aCard)
		return EffectOwner.Discard.Contains(aCard)
				and This ~= aCard
end

---@type Target[]
targets = {
	CreateTarget("la carte dont on veut enlever le marquage", TargetTypes.Card, false, card_filter),
}

-- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
	
	local tailleDefausse = #EffectOwner.Discard
	for i = 1, tailleDefausse do
		if ( EffectOwner.Discard.IsMarkedForUpgrade(EffectOwner.Discard[i-1]) ~= false) then
			return EffectOwner.Discard.Count > 0 and EffectOwner.Hand.Count > 0
			
		end
	end 
	return false
end

function do_effect()
		
		local card = --[[---@type Card]] AskForTarget(1)
		local taille = #EffectOwner.Hand
		EffectOwner.Discard.UnMarkForUpgrade(card)
	
	for i = 1, taille do
		
		EffectOwner.Hand[i-1].Cost.TryChangeValue(EffectOwner.Hand[i-1].Cost.Value -1)
		print("Le cout est réduit de "..EffectOwner.Hand[i-1].Cost.Value -1)
		local desc = base_description.."\nLe cout a été réduit de "..i
		This.Description.TryChangeValue(desc)
		
	end

end

