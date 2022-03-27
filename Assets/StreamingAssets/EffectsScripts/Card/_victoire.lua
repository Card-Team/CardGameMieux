---@type number
max_level = 6 --TODO-- A définir
---@type number
image_id = 1

---@type ChainMode
chain_mode = ChainMode.NoChain


---@type string
name = "Victoire"
---@type number
pa_cost = 5

---@type string
local base_description = "Améliore la carte jusqu'au niveau " .. max_level .. " pour gagner."
description = base_description

---@type Target[]
targets = {}

-- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
    return This.CurrentLevel.Value == This.MaxLevel
end

function do_effect()
    Game.MakeWin(EffectOwner)
    return false
end

function on_level_change(old, new)
    if (new < 3) then
        This.Description.TryChangeValue(base_description)
    elseif (new == max_level) then
        This.Description.TryChangeValue("Allez-y !")
    elseif (max_level - new < 5) then
        This.Description.TryChangeValue("Plus que ".. (max_level - new) .." améliorations pour gagner la partie.")
    else
        This.Description.TryChangeValue("La carte est au niveau : ".. new)
    end
end