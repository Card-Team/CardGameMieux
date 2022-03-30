---@type number
max_level = 1
---@type number
image_id = 19

---@type string
name = "Annulation"
---@type number
pa_cost = 3

---@type ChainMode
chain_mode = ChainMode.MiddleChain

local base_description = "Annule la dernière carte jouée (ne peut être que chainée)"
---@type string
description = base_description

---@type Target[]
targets = {
}

--- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
	return EventStack.Count > 2
end

function do_effect()
	local a = EventStack.truc
	print("EventStack count : " .. EventStack.Count)
	for i = 3, EventStack.Count - 1 do
		print("Doing " .. EventStack.Count - i)
		local cur = EventStack.Count - i
		local ev = EventStack[cur]
		if ev.GetType() == T_CardEffectPlayEvent then
			--cancel l'effet 
			local ev = --[[---@type CardEffectPlayEvent ]] ev
			print("Event " .. i .. " is " .. ev.ToString())
			ev.Cancelled = true
		end
	end
end

-- idée 
-- niv 2 elle défausse en plus d'annuler
-- et le changement de mode de chainage niv 3
function on_level_change(old, new)
	if (new == max_level) then
		This.Description.TryChangeValue(base_description + "\n Termine la chaine")
		This.ChainMode.TryChangeValue(ChainMode.EndChain)
	else
		This.Description.TryChangeValue(base_description)
	end
end 
