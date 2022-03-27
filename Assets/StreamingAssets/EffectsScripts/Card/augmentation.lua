---@type number
max_level = 4
---@type number
image_id = 3

---@type ChainMode
chain_mode = ChainMode.StartOrMiddleChain

---@type string
name = "Augmentation"
---@type number
pa_cost = 2

local base_description = "(bloqué avant level 4) Augmentation du nombre de PA max de 2 pendant 2 tours (niveau réinitialisé à l'utilisation)"
---@type string
description = base_description

local function card_filter()
	-- carte choisis aleatoirement depuis ton deck
	return EffectOwner
end

---@type Target[]
targets = {
	CreateTarget("le joueur dont son nombre de PA va diminuer", TargetTypes.Player, true, card_filter),
}

--- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
	return This.CurrentLevel.Value > max_level - 1
end

---@return fun(evt:EndTurnEvent,handler:IEventHandler)
function getEventSubscriber()
	local theEffectOwner = EffectOwner
	local counter = 0
	return function (endTurnevt,handler)
		if endTurnevt.Player == theEffectOwner then
			counter = counter + 1
		end
		if counter == 2 then
			local visibleCard = This.Virtual()
			Game.RevealCard(theEffectOwner, visibleCard)
			theEffectOwner.MaxActionPoints.TryChangeValue(math.max(0,theEffectOwner.MaxActionPoints.Value - 2))
			UnsubscribeTo(handler)
		end
	end
end

function do_effect()
	local Player = --[[---@type Player]] AskForTarget(1)                                        
	local max_Action_Point = Player.MaxActionPoints.Value                --PA max du joueur
	Player.MaxActionPoints.TryChangeValue(max_Action_Point + 2)        --on enleve au cout d'action le cout de la carte
	This.CurrentLevel.TryChangeValue(1)
	SubscribeTo(T_EndTurnEvent,getEventSubscriber(),false,true)
end

---@param _ number
---@param new number
function on_level_change(_, new)
	if new == 4 then
		This.Description.TryChangeValue("Augmentation du nombre de PA max de 1")
	else
		This.Description.TryChangeValue(base_description)
	end
end 