---@type number
max_level = 4
---@type number
image_id = 523

---@type string
name = "Augmentation"
---@type number
pa_cost = 2

---@type string
description = "(bloqué avant level 4) Augmentation du nombre de PA max de 1"

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

function do_effect()
	local Player = --[[---@type Player]] AskForTarget(1)                                        --cout de la carte 
	local max_Action_Point = Player.MaxActionPoints.Value                --PA max du joueur
	Player.MaxActionPoints.TryChangeValue(max_Action_Point + 1)        --on enleve au cout d'action le cout de la carte
end

---@param _ number
---@param new number
function on_level_change(_, new)
	if new == 4 then
		This.Description.TryChangeValue("Augmentation du nombre de PA max de 1")
	end
end 